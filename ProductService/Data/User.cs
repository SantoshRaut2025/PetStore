using Microsoft.AspNetCore.Identity;

namespace ProductService.Data
{
    // Ensure User inherits from IdentityUser
    public class User : IdentityUser
    {  
        public  int ID  { get; set; }
        public  string Name  { get; set; } = null!;
        public string RefreshToken { get; internal set; }
        public DateTime RefreshTokenExpiryTime { get; internal set; }
    }
}
