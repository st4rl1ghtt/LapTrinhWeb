using Microsoft.AspNetCore.Identity;

namespace KiemTraGiuaKy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
