using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Data.Entities
{
    public class Room
    {
        public int Id { get; set; }
        
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Total Capacity")]
        [Range(1, 300, ErrorMessage = "The number of invites, must be between 1 and 300")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public int Capacity { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters")]
        public string Address { get; set; }

        public ICollection<Agenda> Agendas { get; set; }
        public ICollection<Event> Events { get; set; }
        public Owner Owner { get; set; }
        public string Remarks { get; set; }

        //TODO replace the correct URL for the image and make a carousel for the images
        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl)
         ? "https://https://RoomL21.azurewebsites.net/images/Rooms/noimage.png"
         : $"https://RoomL21.azurewebsites.net{ImageUrl.Substring(1)}";
    }
}
