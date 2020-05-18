using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrganizersController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public OrganizersController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: api/Organizers/5
        [HttpPost]
        [Route("GetOrganizerByEmail")]
        public async Task<IActionResult> GetOrganizer(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var organizer = await _dataContext.Organizers
                .Include(o => o.User)
                .Include(o => o.Events)
                .ThenInclude(e => e.EventType)
                .Include(o => o.Events)
                .ThenInclude(i => i.Inviteds)
                .Include(e => e.Events)
                .ThenInclude(e => e.Room)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == emailRequest.Email.ToLower());

            var response = new OrganizerResponse
            {
                Id = organizer.Id,
                FirstName = organizer.User.FirstName,
                LastName = organizer.User.LastName,
                Address = organizer.User.Address,
                Document = organizer.User.Document,
                Email = organizer.User.Email,
                PhoneNumber = organizer.User.PhoneNumber,
                Events = organizer.Events.Select(p => new EventResponse
                {
                    InvitedsNumber = p.InvitesNumber,
                    Id = p.Id,
                    EventType = p.EventType.Name,
                    EventTypeId = p.EventType.Id,
                    Name = p.Name,
                    Remarks = p.Remarks,
                    //TODO -> Add the room data
                }).ToList()
            };

            return Ok(response);
        }

    }
}
