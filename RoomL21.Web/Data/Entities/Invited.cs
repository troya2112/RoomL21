namespace RoomL21.Web.Data.Entities
{
    public class Invited
    {
        //TODO the invited can have more than one event
        public int Id { get; set; }
        public User User { get; set; }
        public Event Event { get; set; }
    }
}
