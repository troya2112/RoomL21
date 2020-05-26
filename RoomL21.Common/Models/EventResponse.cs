using System;
using System.Collections.Generic;
using System.Text;

namespace RoomL21.Common.Models
{
    public class EventResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int InvitedsNumber { get; set; }

        public string EventType { get; set; }

        public int EventTypeId { get; set; }

        public string Remarks { get; set; }

        public DateTime Date { get; set; }

        public string Organizer { get; set; }

        public string RoomAddress { get; set; }

        public int RoomCapacity { get; set; }
    }
}
