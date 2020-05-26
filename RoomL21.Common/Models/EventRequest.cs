using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoomL21.Common.Models
{
    public class EventRequest
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int InvitedsNumber { get; set; }

        public int OrganizerId { get; set; }

        public int EventTypeId { get; set; }

        public int RoomId { get; set; }

        public string Remarks { get; set; }
    }
}
