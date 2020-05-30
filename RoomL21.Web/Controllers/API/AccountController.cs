using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoomL21.Common.Models;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(DataContext dataContext, IUserHelper userHelper, IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad Request"
                });
            }
            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "This email is already registered."
                });
            }
            user = new User
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email
            };
            var result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest("Error Jonathan!!");
            }
            var userNew = await _userHelper.GetUserByEmailAsync(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, request.Role);

            if (request.Role == "Organizer")
            {
                _dataContext.Organizers.Add(new Organizer { User = userNew });
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken,
                }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(request.Email, "RoomL21 Email confirmation", $"<h1>Email Confirmation</h1>" +
                $"Hi {request.FirstName}" +
                $"Your register as Event Organizer was successfully" +
                $"Please click on this link:<br><br><a href = \"{tokenLink}\">Confirm Email</a>");

                return Ok(new Response<object>
                {
                    IsSuccess = true,
                    Message = "A confirmation email was sent. Please confirm your account and login into the App."
                });
            }
            else
            {
                var eventVar = _dataContext.Events.FirstOrDefault(e => e.Id == request.EventId);
                _dataContext.Inviteds.Add(new Invited { User = userNew, Event = eventVar });
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken,
                }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(request.Email, "RoomL21 Email confirmation", $"<h1>Email Confirmation</h1>" +
                $"Hi {request.FirstName} <br>" +
                $"You got an invitation for a <b>{eventVar.Name}</b> Event from RoomL21 Events.<br>" +
                $" Remeber your password it's your document.<br>" +
                $"Please confirm your asistence on this link:<br><br><a href = \"{tokenLink}\">Confirm Email</a>");

                return Ok(new Response<object>
                {
                    IsSuccess = true,
                    Message = "A confirmation email was sent to the Invited."
                });
            }

        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad Request"
                });
            }
            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "The email is not assigned to any user."
                });
            }
            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new
            {
                token = myToken
            }, protocol: HttpContext.Request.Scheme);
            _mailHelper.SendMail(request.Email, "RoomL21 Password reset", $"<h1>Recover Password</h1> " +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

            return Ok(new Response<object>
            {
                IsSuccess = true,
                Message = "An email with instructions to change the password was sent."
            });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("PutUser")]
        public async Task<IActionResult> PutUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userEntity = await _userHelper.GetUserByEmailAsync(request.Email);
            if (userEntity == null)
            {
                return BadRequest("User not found");
            }
            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.Address = request.Address;
            userEntity.PhoneNumber = request.Phone;
            userEntity.Document = request.Document;

            var response = await _userHelper.UpdateUserAsync(userEntity);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors.FirstOrDefault().Description);
            }
            var updateUser = await _userHelper.GetUserByEmailAsync(request.Email);
            return Ok(updateUser);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad Request"
                });
            }
            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "This email is not assigned to any user"
                });
            }
            var result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault().Description
                });
            }
            return Ok(new Response<object>
            {
                IsSuccess = true,
                Message = "The Password has been changed successfully"
            });
        }
    }
}