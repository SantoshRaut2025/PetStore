using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;
        private readonly LoginService _loginService;
        private readonly UserManager<User> _userManager;

        public AuthController(IJwtService jwtService, ILogger<AuthController> logger, LoginService loginService)
        {
            _jwtService = jwtService;
            _logger = logger;
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // TODO: Validate credentials against your database
                // For now, using hardcoded credentials for demo
                if (request.Username == "admin" && request.Password == "admin123")
                {
                    var token = _jwtService.GenerateToken(
                        userId: "1",
                        username: request.Username,
                        email: "admin@example.com",
                        roles: new List<string> { "Admin", "User" }
                    );

                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Username = request.Username,
                        ExpiresIn = 3600 // 60 minutes in seconds
                    });
                }
                else if (request.Username == "user" && request.Password == "user123")
                {
                    var token = _jwtService.GenerateToken(
                        userId: "2",
                        username: request.Username,
                        email: "user@example.com",
                        roles: new List<string> { "User" }
                    );

                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Username = request.Username,
                        ExpiresIn = 3600
                    });
                }
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in");
                return BadRequest(new { message = "Invalid credentials" });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // TODO: Implement user registration logic
            // 1. Validate input
            // 2. Check if user exists
            // 3. Hash password
            // 4. Save to database
            // 5. Generate token

            return Ok(new { message = "Registration endpoint - implement your logic here" });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {

            var refreshToken = request.RefreshToken;

            // 1. Find the user associated with the refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.RefreshToken == refreshToken &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow);

            if (user == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            // 2. Generate a NEW access token (and optionally a new refresh token)
            var newAccessToken = _jwtService.GenerateToken(user);

            // Optional: Rotate the refresh token for better security
            //var newRefreshToken = GenerateRefreshToken();
            //user.RefreshToken = newRefreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            //await _userManager.UpdateAsync(user);

            // 3. Return the new tokens
            return Ok(new AuthResult
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshToken

            });


        }
    }
}