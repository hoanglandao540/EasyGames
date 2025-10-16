using System.ComponentModel.DataAnnotations;

namespace EasyGames.Web.Models
{
    // Simple user for Owner management 
    public class AppUser
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string FullName { get; set; } = "";

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; } = "";

        [Required, StringLength(30)]
        public string Role { get; set; } = "Staff"; // Staff, Manager, Owner

        public bool IsActive { get; set; } = true;
    }
}


