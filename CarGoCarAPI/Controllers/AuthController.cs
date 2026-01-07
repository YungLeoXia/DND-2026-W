using Microsoft.AspNetCore.Mvc;

namespace CarGoCarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Register a new user account
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // TODO: Implement registration logic
            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Implement login logic
            return Ok(new { token = "jwt_token_here" });
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // TODO: Implement logout logic
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Verify email address
        /// </summary>
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            // TODO: Implement email verification
            return Ok(new { message = "Email verified successfully" });
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class VerifyEmailRequest
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
