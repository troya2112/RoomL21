using System;
using System.Collections.Generic;
using System.Text;

namespace RoomL21.Common.Models
{
    public class OrganizerResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Document { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public ICollection<EventResponse> Events { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
