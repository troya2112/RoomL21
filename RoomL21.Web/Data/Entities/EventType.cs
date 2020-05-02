using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Data.Entities
{
    public class EventType
    {
        public int Id { get; set; }

        [Display(Name = "Event Type")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public string Name { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
