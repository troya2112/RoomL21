using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomL21.Web.Data.Entities;

namespace RoomL21.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Invited> Inviteds { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
