using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using RoomL21.Web.Models;

namespace RoomL21.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AdminsController(
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
            return View(_dataContext.Admins
                .Include(o => o.User));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _dataContext.Admins
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
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
                    await _userHelper.AddUserToRoleAsync(userInDB, "Admin");

                    var admin = new Admin
                    {
                        User = userInDB
                    };

                    _dataContext.Admins.Add(admin);

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

            var admin = await _dataContext.Admins
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (admin == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = admin.User.Address,
                Document = admin.User.Document,
                FirstName = admin.User.FirstName,
                Id = admin.Id,
                LastName = admin.User.LastName,
                PhoneNumber = admin.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
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

            var admin = await _dataContext.Admins
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            await _userHelper.DeleteUserAsync(admin.User.Email);
            _dataContext.Admins.Remove(admin);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
