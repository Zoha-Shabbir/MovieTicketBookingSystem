using Microsoft.AspNetCore.Identity;
namespace MovieBooking.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
