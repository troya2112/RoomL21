using System.Collections.Generic;

namespace RoomL21.Web.Data.Entities
{
    public class Organizer
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
