using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public string Name { get; set; }

        [Display(Name = "Number of invites")]
        [Range(1,300,ErrorMessage = "The number of invites, must be between 1 and 300")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        public int InvitesNumber { get; set; }

        public string Remarks { get; set; }

        public Organizer Organizer { get; set; }

        public Agenda Agenda { get; set; }
        public EventType EventType { get; set; }
        public Room Room { get; set; }

        public ICollection<Invited> Inviteds { get; set; }
    }
}
