using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoomsController : ControllerBase
    {
        private readonly DataContext _context;

        public RoomsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetRooms")]
        public IEnumerable<RoomResponse> GetRooms()
        {
            return _context.Rooms.Select(pt => new RoomResponse
            {
                Id = pt.Id,
                Owner = pt.Owner.User.FullName,
                ImageUrl = pt.ImageFullPath,
                Capacity = pt.Capacity,
                Address = pt.Address,
                Remarks = pt.Remarks
            });
        }
    }
}