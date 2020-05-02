using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Document")]
        [MaxLength(20, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public string Document { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters")]
        public string Address { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string FullNameWithDocument => $"{FirstName}{LastName}-{Document}";
    }
}
