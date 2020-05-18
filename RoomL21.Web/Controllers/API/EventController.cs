using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public EventController(DataContext dataContext, IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        [Route("AddEvent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostEvent([FromBody] EventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var organizer = await _dataContext.Organizers.FindAsync(request.OrganizerId);
            if (organizer == null)
            {
                return BadRequest("Not valid organizer.");
            }

            var eventType = await _dataContext.EventTypes.FindAsync(request.EventTypeId);
            if (eventType == null)
            {
                return BadRequest("Not valid event type.");
            }

            var eventVar = new Event
            {
                Name = request.Name,
                Organizer = organizer,
                EventType = eventType,
                InvitesNumber = request.InvitedsNumber,
                Remarks = request.Remarks
            };

            _dataContext.Events.Add(eventVar);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToEventResponse(eventVar));
        }

        //[HttpPut("{id}")]
        [Route("EditEvent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutEvent([FromBody] EventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (id != request.Id)
            //{
            //    return BadRequest("Bad request !!!");
            //}

            var oldEvent = await _dataContext.Events.FindAsync(request.Id);
            if (oldEvent == null)
            {
                return BadRequest("Event doesn't exists.");
            }

            var eventType = await _dataContext.EventTypes.FindAsync(request.EventTypeId);
            if (eventType == null)
            {
                return BadRequest("Not valid event type.");
            }

            if (request.RoomId != 0)
            {
                var room = await _dataContext.Rooms.FindAsync(request.RoomId);
                oldEvent.Room = room;
            }

            oldEvent.Name = request.Name;
            oldEvent.EventType = eventType;
            oldEvent.InvitesNumber = request.InvitedsNumber;
            oldEvent.Remarks = request.Remarks;

            _dataContext.Events.Update(oldEvent);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToEventResponse(oldEvent));
        }
    }
}
