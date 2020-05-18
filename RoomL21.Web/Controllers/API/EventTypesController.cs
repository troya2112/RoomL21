using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventTypesController : ControllerBase
    {
        private readonly DataContext _context;
        public EventTypesController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetEventTypes")]
        public IEnumerable<EventType> GetEventTypes()
        {
            return _context.EventTypes.OrderBy(pt => pt.Name);
        }
    }
}
