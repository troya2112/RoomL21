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
    [Authorize(Roles = "Admin,Owner")]
    public class OwnersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public OwnersController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Rooms));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Rooms)
                .ThenInclude(p => p.Events)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
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
                    await _userHelper.AddUserToRoleAsync(userInDB, "Owner");

                    var owner = new Owner
                    {
                        Rooms = new List<Room>(),
                        User = userInDB
                    };

                    _dataContext.Owners.Add(owner);

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

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = owner.User.Address,
                Document = owner.User.Document,
                FirstName = owner.User.FirstName,
                Id = owner.Id,
                LastName = owner.User.LastName,
                PhoneNumber = owner.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
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

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Rooms)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            if (owner.Rooms.Count > 0)
            {
                //TODO: Message
                return RedirectToAction(nameof(Index));
            }

            await _userHelper.DeleteUserAsync(owner.User.Email);
            _dataContext.Owners.Remove(owner);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                OwnerId = owner.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var room = await _converterHelper.ToRoomAsync(model, path, true);
                _dataContext.Rooms.Add(room);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }

            return View(model);
        }

        public async Task<IActionResult> EditRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _dataContext.Rooms
                .Include(p => p.Owner)
                .Include(p => p.Events)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToRoomViewModel(room));
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var room = await _converterHelper.ToRoomAsync(model, path, false);
                _dataContext.Rooms.Update(room);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DetailsRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _dataContext.Rooms
                .Include(p => p.Owner)
                .ThenInclude(o => o.User)
                .Include(p => p.Events)
                .ThenInclude(e => e.Agenda)
                .Include(p => p.Events)
                .ThenInclude(e => e.EventType)
                .Include(p => p.Events)
                .ThenInclude(e => e.Organizer)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        public async Task<IActionResult> AddEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _dataContext.Rooms.FindAsync(id.Value);
            if (room == null)
            {
                return NotFound();
            }

            var model = new EventViewModel
            {
                Date = DateTime.Now,
                RoomId = room.Id,
                EventTypes = _combosHelper.GetComboEventTypes(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Agenda agenda = await _dataContext.Agendas.Where(x => x.Date > model.Date).OrderBy(x => x.Date).FirstOrDefaultAsync();
                model.Agenda = agenda;
                var Event = await _converterHelper.ToEventAsync(model, true);
                _dataContext.Events.Add(Event);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsRoom)}/{model.RoomId}");
            }

            model.EventTypes = _combosHelper.GetComboEventTypes();
            return View(model);
        }

        public async Task<IActionResult> EditEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Event = await _dataContext.Events
                .Include(e => e.Room)
                .Include(e => e.EventType)
                .Include(e => e.Agenda)
                .Include(e => e.Organizer)
                .Include(e => e.Inviteds)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (Event == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToEventViewModel(Event));
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Event = await _converterHelper.ToEventAsync(model, false);
                _dataContext.Events.Update(Event);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsRoom)}/{model.RoomId}");
            }

            model.EventTypes = _combosHelper.GetComboEventTypes();
            return View(model);
        }

        public async Task<IActionResult> DeleteEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Event = await _dataContext.Events
                .Include(e => e.Room)
                .FirstOrDefaultAsync(h => h.Id == id.Value);
            if (Event == null)
            {
                return NotFound();
            }

            _dataContext.Events.Remove(Event);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsRoom)}/{Event.Room.Id}");
        }

        public async Task<IActionResult> DeleteRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rooms = await _dataContext.Rooms
                .Include(r => r.Owner)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (rooms == null)
            {
                return NotFound();
            }

            if (rooms.Events.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The room can't be deleted because it has related records.");
                return RedirectToAction($"{nameof(Details)}/{rooms.Owner.Id}");
            }

            _dataContext.Rooms.Remove(rooms);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{rooms.Owner.Id}");
        }
    }
}
