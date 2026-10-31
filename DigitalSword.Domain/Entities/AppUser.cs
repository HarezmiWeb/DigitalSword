using Microsoft.AspNetCore.Identity;

namespace DigitalSword.Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public AppUser() { Id = Guid.NewGuid().ToString(); }
        public string FullName { get; set; } = string.Empty;
    }

    public class AppRole : IdentityRole<string>
    {
        public AppRole() { Id = Guid.NewGuid().ToString(); }
    }
}
