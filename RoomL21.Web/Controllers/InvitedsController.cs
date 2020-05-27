using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Web.Controllers
{
    public class InvitedsController : Controller
    {
        private readonly DataContext _context;

        public InvitedsController(DataContext context)
        {
            _context = context;
        }

        // GET: Inviteds
        public async Task<IActionResult> Index()
        {
            return View(await _context.Inviteds.ToListAsync());
        }

        // GET: Inviteds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invited = await _context.Inviteds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invited == null)
            {
                return NotFound();
            }

            return View(invited);
        }

        // GET: Inviteds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inviteds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] Invited invited)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invited);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(invited);
        }

        // GET: Inviteds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invited = await _context.Inviteds.FindAsync(id);
            if (invited == null)
            {
                return NotFound();
            }
            return View(invited);
        }

        // POST: Inviteds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Invited invited)
        {
            if (id != invited.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invited);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvitedExists(invited.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(invited);
        }

        // GET: Inviteds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invited = await _context.Inviteds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invited == null)
            {
                return NotFound();
            }

            return View(invited);
        }

        // POST: Inviteds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invited = await _context.Inviteds.FindAsync(id);
            _context.Inviteds.Remove(invited);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvitedExists(int id)
        {
            return _context.Inviteds.Any(e => e.Id == id);
        }
    }
}
