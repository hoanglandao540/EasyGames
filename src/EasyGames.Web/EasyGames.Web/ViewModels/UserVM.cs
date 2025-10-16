using System.ComponentModel.DataAnnotations;

namespace EasyGames.Web.ViewModels
{
    // VM for Create/Edit forms 
    public class UserVM
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string FullName { get; set; } = "";

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; } = "";

        [Required, StringLength(30)]
        public string Role { get; set; } = "Staff";

        public bool IsActive { get; set; } = true;
    }
}


