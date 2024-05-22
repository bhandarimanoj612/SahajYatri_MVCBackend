using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Models.Dto;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase

    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private static Dictionary<string, List<string>> _userMessages = new Dictionary<string, List<string>>();

        public MessageController(ApplicationDBContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;

        }
        [HttpGet("get/{username}")]
        public async Task<IActionResult> GetProfileImageByUsername(string username)
        {
            try
            {
                // Retrieve the profile image URL based on the user's username
                var user = await _userManager.FindByNameAsync(username);
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

        // Endpoint to get all usernames for making chat between this user 
        [HttpGet("usernames")]
        public async Task<IActionResult> GetAllUsernames()
        {
            try
            {
                var usernames = await _db.Users.Select(u => u.UserName).ToListAsync();
                return Ok(usernames);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("userprofiles")]
        public async Task<IActionResult> GetUserProfiles()
        {
            try
            {
                var profiles = await _db.Users.Select(u => new { UserName = u.UserName, ProfileImg = u.ProfileImg }).ToListAsync();
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        //// GET api/message/{sender}/{recipient}
        //[HttpGet("{sender}/{recipient}")]
        //public ActionResult<IEnumerable<string>> GetMessages(string sender, string recipient)
        //{
        //    string key = $"{sender}_{recipient}";
        //    if (!_userMessages.ContainsKey(key))
        //    {
        //        return NotFound();
        //    }

        //    var messages = _userMessages[key];
        //    return Ok(messages);
        //}
        // GET api/message/{sender}/{recipient}
        [HttpGet("{sender}/{recipient}")]
        public ActionResult<IEnumerable<string>> GetMessages(string sender, string recipient)
        {
            string senderRecipientKey = $"{sender}_{recipient}";
            string recipientSenderKey = $"{recipient}_{sender}";

            List<string> senderRecipientMessages = _userMessages.ContainsKey(senderRecipientKey) ? _userMessages[senderRecipientKey] : new List<string>();
            List<string> recipientSenderMessages = _userMessages.ContainsKey(recipientSenderKey) ? _userMessages[recipientSenderKey] : new List<string>();

            return Ok(new { senderRecipientMessages, recipientSenderMessages });
        }


        // POST api/message
        [HttpPost]
        public IActionResult SendMessage([FromBody] ClasicMessageDto message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Sender) || string.IsNullOrWhiteSpace(message.Recipient) || string.IsNullOrWhiteSpace(message.Content))
            {
                return BadRequest("Invalid message format");
            }

            string key = $"{message.Sender}_{message.Recipient}";
            if (!_userMessages.ContainsKey(key))
            {
                _userMessages.Add(key, new List<string>());
            }

            _userMessages[key].Add($"{message.Sender}  -  {message.Content}");

            return Ok();
        }

    }

}