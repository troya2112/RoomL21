using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
