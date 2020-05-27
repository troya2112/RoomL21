using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvitedsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public InvitedsController(DataContext context)
        {
            _dataContext = context;
        }

        [HttpPost]
        [Route("GetInvitedByEmail")]
        public async Task<IActionResult> GetInvited(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var invited = await _dataContext.Inviteds
                .Include(o => o.User)
                .Include(o => o.Event)
                .ThenInclude(e => e.EventType)
                .Include(o => o.Event)
                .ThenInclude(i => i.Organizer)
                .Include(e => e.Event)
                .ThenInclude(e => e.Room)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == emailRequest.Email.ToLower());

            var response = new InvitedResponse
            {
                Id = invited.Id,
                FirstName = invited.User.FirstName,
                LastName = invited.User.LastName,
                Address = invited.User.Address,
                Document = invited.User.Document,
                Email = invited.User.Email,
                PhoneNumber = invited.User.PhoneNumber,
                EventName = invited.Event.Name,
                EventInvitesNumber = invited.Event.InvitesNumber,
                EventType = invited.Event.EventType.Name,
                EventRemarks = invited.Event.Remarks,
                //RoomAddress = invited.Event.Room.Address
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetInviteds")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<InvitedResponse> GetInviteds()
        {
            return _dataContext.Inviteds.Select(pt => new InvitedResponse
            {
                Id = pt.Id,
                FirstName = pt.User.FirstName,
                LastName = pt.User.LastName,
                Document = pt.User.Document,
                Address = pt.User.Address,
                PhoneNumber = pt.User.PhoneNumber,
                Email = pt.User.Email,
                EventId = pt.Event.Id,
                EventName = pt.Event.Name,
                EventInvitesNumber = pt.Event.InvitesNumber,
                EventType = pt.Event.EventType.Name,
                EventRemarks = pt.Event.Remarks,
            });
        }

    }
}