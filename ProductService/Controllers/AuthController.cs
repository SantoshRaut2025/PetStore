using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Services;
using ProductService.Models;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
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
        
    }
}