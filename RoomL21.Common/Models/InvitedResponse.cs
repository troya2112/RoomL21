using System;
using System.Collections.Generic;
using System.Text;

namespace RoomL21.Common.Models
{
    public class InvitedResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Document { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public int EventInvitesNumber { get; set; }

        public string EventType { get; set; }

        public string EventRemarks { get; set; }

        public string RoomAddress { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
