using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using RoomL21.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RoomL21.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;
        private readonly IMailHelper _mailHelper;
        private readonly ICombosHelper _combosHelper;

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration,
            DataContext dataContext,
            IMailHelper mailHelper,
            ICombosHelper combosHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _dataContext = dataContext;
            _mailHelper = mailHelper;
            _combosHelper = combosHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "User or password not valid.");
                model.Password = string.Empty;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password
                        );

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMonths(4),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            var model = new AddUserViewModel
            {
                UserTypes = _combosHelper.GetComboUserTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserTypeId == "0")
                {
                    ModelState.AddModelError(string.Empty, "You must select an account type");
                    model.UserTypes = _combosHelper.GetComboUserTypes();
                    return View(model);
                }
                var userType = model.UserTypeId;
                var user = await AddUserAsync(model, userType);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.UserTypes = _combosHelper.GetComboUserTypes();
                    return View(model);
                }

                if (model.UserTypeId.ToString() == "Organizer")
                {
                    var organizer = new Organizer
                    {
                        Events = new List<Event>(),
                        User = user,
                    };
                    _dataContext.Organizers.Add(organizer);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    var owner = new Owner
                    {
                        Rooms = new List<Room>(),
                        User = user,
                    };
                    _dataContext.Owners.Add(owner);
                    await _dataContext.SaveChangesAsync();
                }


                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Email confirmation",
                    $"<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                    $"  <tr>" +
                    $"    <td style = 'background-color: #34495e; text-align: center; padding: 0'>" +
                    $"       <a href = '' >" +
                    $"         <img width = '20%' style = 'display:block; margin: 1.5% 3%' src= ''>" +
                    $"       </a>" +
                    $"  </td>" +
                    $"  </tr>" +
                    $"  <tr>" +
                    $"  <td style = 'padding: 0'>" +
                    $"     <img style = 'padding: 0; display: block' src = '' width = '100%'>" +
                    $"  </td>" +
                    $"</tr>" +
                    $"<tr>" +
                    $" <td style = 'background-color: #ecf0f1'>" +
                    $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                    $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Hola </h1>" +
                    $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                    $"                      El mejor Hospital Veterinario Especializado de la Ciudad de Morelia enfocado a brindar servicios médicos y quirúrgicos<br>" +
                    $"                      aplicando las técnicas más actuales y equipo de vanguardia para diagnósticos precisos y tratamientos oportunos..<br>" +
                    $"                      Entre los servicios tenemos:</p>" +
                    $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                    $"        <li> Urgencias.</li>" +
                    $"        <li> Medicina Interna.</li>" +
                    $"        <li> Imagenologia.</li>" +
                    $"        <li> Pruebas de laboratorio y gabinete.</li>" +
                    $"        <li> Estetica canina.</li>" +
                    $"      </ul>" +
                    $"  <div style = 'width: 100%;margin:20px 0; display: inline-block;text-align: center'>" +
                    $"    <img style = 'padding: 0; width: 200px; margin: 5px' src = ''>" +
                    $"  </div>" +
                    $"  <div style = 'width: 100%; text-align: center'>" +
                    $"    <h2 style = 'color: #e67e22; margin: 0 0 7px' >Email Confirmation </h2>" +
                    $"    To allow the user,plase click in this link:</ br ></ br > " +
                    $"    <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Confirm Email</a>" +
                    $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0' > Nuskë Clinica Integral Veterinaria 2019 </p>" +
                    $"  </div>" +
                    $" </td >" +
                    $"</tr>" +
                    $"</table>");
                ViewBag.Message = "The instructions to allow your user has been sent to email.";
                model.UserTypes = _combosHelper.GetComboUserTypes();
                return View(model);
            }
            ModelState.AddModelError(string.Empty, "Error");
            model.UserTypes = _combosHelper.GetComboUserTypes();
            return View(model);
        }

        private async Task<User> AddUserAsync(AddUserViewModel model, string userType)
        {
            var user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(model.Username);
            await _userHelper.AddUserToRoleAsync(newUser, userType);
            return newUser;
        }

        public async Task<IActionResult> ChangeUser()
        {
            if (User.IsInRole("Owner"))
            {
                var owner = await _dataContext.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == User.Identity.Name.ToLower());
                if (owner == null)
                {
                    return NotFound();
                }
                var view = new EditUserViewModel
                {
                    Address = owner.User.Address,
                    Document = owner.User.Document,
                    FirstName = owner.User.FirstName,
                    Id = owner.Id,
                    LastName = owner.User.LastName,
                    PhoneNumber = owner.User.PhoneNumber
                };
                return View(view);
            }
            else if (User.IsInRole("Organizer"))
            {
                var organizer = await _dataContext.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == User.Identity.Name.ToLower());
                if (organizer == null)
                {
                    return NotFound();
                }
                var view = new EditUserViewModel
                {
                    Address = organizer.User.Address,
                    Document = organizer.User.Document,
                    FirstName = organizer.User.FirstName,
                    Id = organizer.Id,
                    LastName = organizer.User.LastName,
                    PhoneNumber = organizer.User.PhoneNumber
                };
                return View(view);
            }
            else if (User.IsInRole("Invited"))
            {
                var invited = await _dataContext.Inviteds
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == User.Identity.Name.ToLower());
                if (invited == null)
                {
                    return NotFound();
                }
                var view = new EditUserViewModel
                {
                    Address = invited.User.Address,
                    Document = invited.User.Document,
                    FirstName = invited.User.FirstName,
                    Id = invited.Id,
                    LastName = invited.User.LastName,
                    PhoneNumber = invited.User.PhoneNumber
                };
                return View(view);
            }
            else if (User.IsInRole("Admin"))
            {
                var admin = await _dataContext.Admins
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower() == User.Identity.Name.ToLower());
                if (admin == null)
                {
                    return NotFound();
                }
                var view = new EditUserViewModel
                {
                    Address = admin.User.Address,
                    Document = admin.User.Document,
                    FirstName = admin.User.FirstName,
                    Id = admin.Id,
                    LastName = admin.User.LastName,
                    PhoneNumber = admin.User.PhoneNumber
                };
                return View(view);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {

                if (model is null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                if (ModelState.IsValid)
                {
                    if (User.IsInRole("Owner"))
                    {
                        var owner = await _dataContext.Owners
                        .Include(o => o.User)
                        .FirstOrDefaultAsync(o => o.Id == model.Id);

                        owner.User.Document = model.Document;
                        owner.User.FirstName = model.FirstName;
                        owner.User.LastName = model.LastName;
                        owner.User.Address = model.Address;
                        owner.User.PhoneNumber = model.PhoneNumber;

                        await _userHelper.UpdateUserAsync(owner.User);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (User.IsInRole("Invited"))
                    {
                        var invited = await _dataContext.Inviteds
                       .Include(o => o.User)
                       .FirstOrDefaultAsync(o => o.Id == model.Id);

                        invited.User.Document = model.Document;
                        invited.User.FirstName = model.FirstName;
                        invited.User.LastName = model.LastName;
                        invited.User.Address = model.Address;
                        invited.User.PhoneNumber = model.PhoneNumber;

                        await _userHelper.UpdateUserAsync(invited.User);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (User.IsInRole("Organizer"))
                    {
                        var organizer = await _dataContext.Organizers
                        .Include(o => o.User)
                        .FirstOrDefaultAsync(o => o.Id == model.Id);

                        organizer.User.Document = model.Document;
                        organizer.User.FirstName = model.FirstName;
                        organizer.User.LastName = model.LastName;
                        organizer.User.Address = model.Address;
                        organizer.User.PhoneNumber = model.PhoneNumber;

                        await _userHelper.UpdateUserAsync(organizer.User);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (User.IsInRole("Admin"))
                    {
                        var admin = await _dataContext.Admins
                        .Include(o => o.User)
                        .FirstOrDefaultAsync(o => o.Id == model.Id);

                        admin.User.Document = model.Document;
                        admin.User.FirstName = model.FirstName;
                        admin.User.LastName = model.LastName;
                        admin.User.Address = model.Address;
                        admin.User.PhoneNumber = model.PhoneNumber;

                        await _userHelper.UpdateUserAsync(admin.User);
                        return RedirectToAction("Index", "Home");
                    }
                }

                return View(model);
            }

            return View(model);
        }

        public IActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePW(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        [HttpGet]
        [Route("RecoverPassword")]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "RoomL21 Password Reset", $"<h1>Password Reset</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");
                ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return View();

            }

            return View(model);
        }

        [HttpGet]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successfull.";
                    return View();
                }

                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }
    }
}