using Microsoft.AspNetCore.Mvc;

namespace CarGoCarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Get user profile by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            // TODO: Implement user retrieval
            return Ok(new { id, name = "User Name", email = "user@example.com" });
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            // TODO: Implement getting all users with admin check
            return Ok(new[] { new { id = 1, name = "User 1" } });
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            // TODO: Implement user update
            return Ok(new { message = "User updated successfully" });
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            // TODO: Implement user deletion
            return Ok(new { message = "User deleted successfully" });
        }

        /// <summary>
        /// Disable user account (Admin only)
        /// </summary>
        [HttpPost("{id}/disable")]
        public IActionResult DisableUser(int id)
        {
            // TODO: Implement user disable
            return Ok(new { message = "User disabled successfully" });
        }
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
