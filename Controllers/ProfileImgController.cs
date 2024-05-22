using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Services;
using System;
using System.Threading.Tasks;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileImgController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileImgController(IFileService fileService, UserManager<ApplicationUser> userManager)
        {
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpPost("upload/{email}")]
        public async Task<IActionResult> UploadImage(string email, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Get the user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound("User not found.");

                // Handle file upload
                string imageUrl = await _fileService.WriteFile(file);

                // Update the user's profile image
                user.ProfileImg = imageUrl;
                user.IsProfileImgDeleted = false; // Set IsProfileImgDeleted to false
                await _userManager.UpdateAsync(user);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{email}")]
        public async Task<IActionResult> UpdateImage(string email, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Get the user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound("User not found.");

                // Handle file upload
                string imageUrl = await _fileService.WriteFile(file);

                // Update the user's profile image
                user.ProfileImg = imageUrl;
                user.IsProfileImgDeleted = false; // Set IsProfileImgDeleted to false
                await _userManager.UpdateAsync(user);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("soft-delete/{email}")]
        public async Task<IActionResult> SoftDeleteImage(string email)
        {
            try
            {
                // Get the user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound("User not found.");

                // Soft delete the profile image by marking it as deleted
                user.ProfileImg = null; // Optionally remove the image URL
                user.IsProfileImgDeleted = true; // Set IsProfileImgDeleted to true
                await _userManager.UpdateAsync(user);

                return Ok("Profile image soft deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get/{email}")]
        public async Task<IActionResult> GetProfileImage(string email)
        {
            try
            {
                // Retrieve the profile image URL based on the user's email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound("User not found.");

                if (user.IsProfileImgDeleted)
                    return NotFound("Profile image is soft deleted for the specified user.");

                string imageUrl = user.ProfileImg;

                if (string.IsNullOrEmpty(imageUrl))
                    return NotFound("Profile image not found for the specified user.");

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
