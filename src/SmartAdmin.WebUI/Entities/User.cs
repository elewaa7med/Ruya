using Microsoft.AspNetCore.Identity;

namespace SmartAdmin.WebUI.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
