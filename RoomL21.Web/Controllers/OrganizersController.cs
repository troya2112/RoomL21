using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using RoomL21.Web.Models;

namespace RoomL21.Web.Controllers
{
    [Authorize(Roles = "Admin,Organizer")]
    public class OrganizersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public OrganizersController(
            DataContext context,
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Organizers
                .Include(o => o.User));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organizer = await _dataContext.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Organizer == null)
            {
                return NotFound();
            }

            return View(Organizer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
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

                var response = await _userHelper.AddUserAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDB, "Organizer");

                    var Organizer = new Organizer
                    {
                        User = userInDB
                    };

                    _dataContext.Organizers.Add(Organizer);

                    try
                    {
                        await _dataContext.SaveChangesAsync();

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        var tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme);

                        _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                            $"To allow the user, " +
                            $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organizer = await _dataContext.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (Organizer == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = Organizer.User.Address,
                Document = Organizer.User.Document,
                FirstName = Organizer.User.FirstName,
                Id = Organizer.Id,
                LastName = Organizer.User.LastName,
                PhoneNumber = Organizer.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Organizer = await _dataContext.Organizers
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                Organizer.User.Document = model.Document;
                Organizer.User.FirstName = model.FirstName;
                Organizer.User.LastName = model.LastName;
                Organizer.User.Address = model.Address;
                Organizer.User.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(Organizer.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organizer = await _dataContext.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (Organizer == null)
            {
                return NotFound();
            }

            await _userHelper.DeleteUserAsync(Organizer.User.Email);
            _dataContext.Organizers.Remove(Organizer);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
