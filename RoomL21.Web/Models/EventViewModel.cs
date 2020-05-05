using Microsoft.AspNetCore.Mvc.Rendering;
using RoomL21.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomL21.Web.Models
{
    public class EventViewModel : Event
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Service Type")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a event type.")]
        public int EventTypeId { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "The field {0} is mandatory")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public IEnumerable<SelectListItem> EventTypes { get; set; }
    }
}
