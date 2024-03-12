using Microsoft.AspNetCore.Identity;

namespace HPVTesting.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
