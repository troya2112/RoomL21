using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvitedNumbersController : ControllerBase
    {
        private readonly DataContext _context;
        public InvitedNumbersController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetInvitedNumbers")]
        public IEnumerable<InvitedNumberResponse> GetInvitedNumbers()
        {
            List<InvitedNumberResponse> invitedNumbers = new List<InvitedNumberResponse>();

            invitedNumbers.Add(new InvitedNumberResponse { Id = 1, Range = "0-50", Value = 50 });
            invitedNumbers.Add(new InvitedNumberResponse { Id = 2, Range = "51-100", Value = 100 });
            invitedNumbers.Add(new InvitedNumberResponse { Id = 3, Range = "101-150", Value = 150 });
            invitedNumbers.Add(new InvitedNumberResponse { Id = 4, Range = "151-200", Value = 200 });
            invitedNumbers.Add(new InvitedNumberResponse { Id = 5, Range = "201-250", Value = 250 });
            invitedNumbers.Add(new InvitedNumberResponse { Id = 6, Range = "251-300", Value = 300 });

            return invitedNumbers;
        }

    }
}
