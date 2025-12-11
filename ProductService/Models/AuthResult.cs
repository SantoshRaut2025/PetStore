
namespace ProductService.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string[]? Errors { get; set; }
    }
}

