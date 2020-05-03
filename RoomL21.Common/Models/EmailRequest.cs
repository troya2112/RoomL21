using System.ComponentModel.DataAnnotations;

namespace RoomL21.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
