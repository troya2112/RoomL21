using System.ComponentModel.DataAnnotations;

namespace RoomL21.Common.Models
{
    public class UnAssignRequest
    {
        [Required]
        public int AgendaId { get; set; }
    }
}
