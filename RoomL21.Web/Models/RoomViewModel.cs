using Microsoft.AspNetCore.Http;
using RoomL21.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Models
{
    public class RoomViewModel : Room
    {
        public int OwnerId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
