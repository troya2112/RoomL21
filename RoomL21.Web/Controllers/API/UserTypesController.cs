using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoomL21.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public UserTypesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetUserTypes")]
        public IEnumerable<SelectListItem> GetUserTypes()
        {
            return _context.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Name
            }).ToList().Where(u => u.Text == "Invited" || u.Text == "Organizer").ToList();
        }
    }
}