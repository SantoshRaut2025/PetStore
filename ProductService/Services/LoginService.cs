using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ProductService.Models;

namespace ProductService.Services
{
    public class LoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public LoginService(UserManager<User> userManager,IJwtService jwtService) {
            _userManager = userManager;
            _jwtService = jwtService;

        }

        public async Task<AuthResult> Login(User user)
        {
            var loggedInUser = await _userManager.FindByNameAsync(user.UserName);

            if (loggedInUser == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Invalid username or password" }
                };
            }
            // Create Access Token and Refresh Token
            var accessToken = _jwtService.GenerateToken(loggedInUser);

            var refreshToken = Guid.NewGuid().ToString();
          //  user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);// read from config or constant        
            // ...
            loggedInUser.RefreshToken = refreshToken;
            loggedInUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(loggedInUser);

          //  await _userManager.UpdateAsync(user);

            return new AuthResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = loggedInUser.RefreshTokenExpiryTime
            };

        }
    }
}
