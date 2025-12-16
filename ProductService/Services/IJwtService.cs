using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace ProductService.Services;

public interface IJwtService
    {
        string GenerateToken(string userId, string username, string email, List<string>? roles = null);

        string GenerateToken(IdentityUser user);

        ClaimsPrincipal? ValidateToken(string token);
    }
