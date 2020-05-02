using System.Collections.Generic;

namespace RoomL21.Web.Data.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
