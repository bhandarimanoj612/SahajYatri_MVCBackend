using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;
using Sahaj_Yatri.Models.Dto.Auth;
using Sahaj_Yatri.Models.Dto.Role;
using Sahaj_Yatri.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;


namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly ApiResponse _response;
        private readonly string _secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int ExpirationMinutes = 30;
        private readonly IEmailService _emailService;
        public AuthController(ApplicationDBContext db, IConfiguration configuration
             , UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _db = db;
            _secretKey = configuration.GetValue<string>("JWT:Secret");
            _response = new ApiResponse();
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
        }
        [HttpPost("assign-role")]
        //[Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDTO model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    return BadRequest("Role does not exist.");
                }

                if (model.AssignRole)
                {
                    // Remove user from all existing roles
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);

                    // Assign the new role to the user
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return Ok("Role assigned successfully.");
                }
                else
                {
                    // Remove the specified role from the user
                    await _userManager.RemoveFromRoleAsync(user, model.Role);
                    return Ok("Role removed successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        //[Authorize(Roles = Role.Hotel + "," + Role.Admin+","+Role.Travel+","+Role.Vehicle)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Username or password cannot be empty.");
            }

            ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == model.UserName.ToLower());
            if (userFromDb == null || !await _userManager.CheckPasswordAsync(userFromDb, model.Password))
            {
                return BadRequest("Username or password is incorrect.");
            }



            // Check if the user's email is verified
            if (!userFromDb.EmailConfirmed)
            {
                // Generate a new verification code
                var newVerificationCode = GenerateRandomCode();

                // Update the user's verification code
                userFromDb.VerificationCode = newVerificationCode;
                await _userManager.UpdateAsync(userFromDb);

                // Send the verification code to the user's email
                var message = new Message(new string[] { userFromDb.Email }, "Verification Code", $"Your verification code is: {newVerificationCode}");
                _emailService.SendEmail(message);

                return BadRequest("Email is not verified. A new verification code has been sent to your email. Please verify your email before logging in.");
            }
            // Proceed with login if the user's email is verified
            var roles = await _userManager.GetRolesAsync(userFromDb);

            // Check if the user has any of the allowed roles
            if (!roles.Any(role => role == Role.Admin || role == Role.Hotel || role == Role.Travel || role == Role.Vehicle))
            {
                return BadRequest("User does not have the necessary roles to log in.");
            }


          
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("fullName", userFromDb.Name),
            new Claim("id", userFromDb.Id.ToString()),
            new Claim(ClaimTypes.Email, userFromDb.UserName != null ? userFromDb.UserName.ToString() : string.Empty),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.Now.AddDays(ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return Ok(new LoginResponseDTO
            {
                Email = userFromDb.Email,
                Token = tokenString,
                Name = userFromDb.Name, // Include the user's name in the response
                Role = roles.FirstOrDefault() // Include the user's role in the response
            });
        }

        [HttpPost("LoginCustomer")]
        //[Authorize(Roles = Role.Customer )]
        public async Task<IActionResult> LoginCustomer([FromBody] LoginRequestDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Username or password cannot be empty.");
            }

            ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == model.UserName.ToLower());
            if (userFromDb == null || !await _userManager.CheckPasswordAsync(userFromDb, model.Password))
            {
                return BadRequest("Username or password is incorrect.");
            }

            // Check if the user exists and is in the 'Customer' role
            if (userFromDb == null || !await _userManager.IsInRoleAsync(userFromDb, Role.Customer))
            {
                return BadRequest("Invalid username or password.");
            }
            // Check if the user's email is verified
            if (!userFromDb.EmailConfirmed)
            {
                // Generate a new verification code
                var newVerificationCode = GenerateRandomCode();

                // Update the user's verification code
                userFromDb.VerificationCode = newVerificationCode;
                await _userManager.UpdateAsync(userFromDb);

                // Send the verification code to the user's email
                var message = new Message(new string[] { userFromDb.Email }, "Verification Code", $"Your verification code is: {newVerificationCode}");
                _emailService.SendEmail(message);

                return BadRequest("Email is not verified. A new verification code has been sent to your email. Please verify your email before logging in.");
            }

            // Proceed with login if the user's email is verified
            var roles = await _userManager.GetRolesAsync(userFromDb);
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("fullName", userFromDb.Name),
            new Claim("id", userFromDb.Id.ToString()),
            new Claim(ClaimTypes.Email, userFromDb.UserName != null ? userFromDb.UserName.ToString() : string.Empty),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.Now.AddDays(ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return Ok(new LoginResponseDTO
            {
                Email = userFromDb.Email,
                Token = tokenString,
                Name = userFromDb.Name, // Include the user's name in the response
                Role = roles.FirstOrDefault() // Include the user's role in the response
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            try
            {
                if (model.Role.ToLower() == Role.Admin.ToLower())
                {
                    // Check if there are any existing admin users
                    var existingAdmins = await _userManager.GetUsersInRoleAsync(Role.Admin);
                    if (existingAdmins != null && existingAdmins.Any())
                    {
                        return BadRequest("An admin already exists. Registration of admin is not allowed.");
                    }
                }

                // Check if the username already exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    return BadRequest("Username already exists.");
                }

                var newUser = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.UserName
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
                    return BadRequest($"Error while registering: {errorMessage}");
                }
                // Ensure that the role exists
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }

                // Add the user to the role
                await _userManager.AddToRoleAsync(newUser, model.Role);

                // Generate and store verification code
                var verificationCode = GenerateRandomCode(); // Implement this method to generate a random code
                newUser.VerificationCode = verificationCode;
                await _userManager.UpdateAsync(newUser);

                // Send verification code via email
                var message = new Message(new string[] { newUser.Email }, "Email Verification Code", $"Your verification code is: {verificationCode}");
                _emailService.SendEmail(message);

                return Ok($"Please check your email for verification. {model.Role} registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (user.VerificationCode != model.VerificationCode)
            {
                return BadRequest("Invalid verification code.");
            }

            // Mark the user's email as verified
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Ok("Email verified successfully.");
        }

        // Utility method to generate a random code
        private string GenerateRandomCode()
        {
            // Implement your random code generation logic here
            // Example: Generate a random 6-digit numeric code
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        //forget password and reset password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var otp = GenerateRandomCode(); // Generate OTP
                user.VerificationCode = otp; // Store OTP in user entity
                await _userManager.UpdateAsync(user);

                var message = new Message(new string[] { user.Email }, "Reset Password OTP", $"Your OTP for password reset is: {otp}");
                _emailService.SendEmail(message);

                return Ok($"An OTP has been sent to your email: {user.Email}. Please check your inbox.");
            }
            return BadRequest("User not found.");
        }

        
//for verifing otp

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.VerificationCode == model.OTP) // Check if OTP matches
                    {
                        // Clear OTP after successful verification
                        user.VerificationCode = null;
                        await _userManager.UpdateAsync(user);

                        // Redirect the user to the reset password screen
                        return Ok(new { message = "OTP verified successfully", redirectTo = "ResetPasswordScreen" });
                    }
                    else
                    {
                        return BadRequest("Invalid OTP.");
                    }
                }
                return BadRequest("User not found.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception occurred: {ex}");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                if (user != null)
                {
                    // Proceed with the password reset process
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, resetPassword.Password);

                    if (result.Succeeded)
                    {
                        // Clear OTP after successful password reset
                        user.VerificationCode = null;
                        await _userManager.UpdateAsync(user);

                        return Ok("Password has been reset successfully.");
                    }
                    else
                    {
                        return BadRequest("Failed to reset password.");
                    }
                }
                return BadRequest("User not found.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception occurred: {ex}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameModel model)
        {
            try
            {
                // Get the current user
                var user = await _userManager.FindByNameAsync(model.CurrentUsername);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                // Check if the new username is available
                var existingUser = await _userManager.FindByNameAsync(model.NewUsername);
                if (existingUser != null)
                {
                    return BadRequest("Username already exists.");
                }

                // Update the username
                user.UserName = model.NewUsername;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
                    return BadRequest($"Error while updating username: {errorMessage}");
                }

                return Ok("Username updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //showing  list of users from the tables 
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = new List<object>();
                foreach (var user in await _userManager.Users.ToListAsync())
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    users.Add(new
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Role = roles.FirstOrDefault()
                    });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //deleting specific user
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return BadRequest("Failed to delete user.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
