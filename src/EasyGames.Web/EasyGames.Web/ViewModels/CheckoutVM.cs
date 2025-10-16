using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGames.Web.ViewModels
{
    // We reuse CartRowVM lines so code stays DRY and student-friendly.
    public class CheckoutVM
    {
        [Required, StringLength(80)]
        public string CustomerName { get; set; } = "";

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; } = "";

        [Phone, StringLength(24)]
        public string? Phone { get; set; }

        [Required, StringLength(160)]
        public string Address { get; set; } = "";

        public List<CartRowVM> Lines { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal GrandTotal { get; set; }
    }
}



