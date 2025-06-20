using Microsoft.AspNetCore.Mvc;
using AuthAPI.Models;
using AuthAPI.Services;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.Username == "admin" && user.Password == "123")
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _tokenService.InvalidateToken(token);
            return Ok(new { message = "Token invalidated" });
        }

        [HttpGet("verify-token")]
        public IActionResult VerifyToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { error = "Token query parameter is required" });
            }

            var isInvalid = _tokenService.IsTokenInvalidated(token);
            return Ok(new
            {
                token,
                isInvalid,
                status = isInvalid ? "Invalidated" : "Active"
            });
        }
    }
}
