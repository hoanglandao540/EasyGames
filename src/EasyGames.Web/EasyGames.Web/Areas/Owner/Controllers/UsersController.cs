using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) { _db = db; }

        // GET: /Owner/Users
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .Select(u => new UserVM
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return View(list);
        }

        // GET: /Owner/Users/Create
        [HttpGet]
        public IActionResult Create() => View(new UserVM());

        // POST: /Owner/Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var entity = new AppUser
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Role = vm.Role,
                IsActive = vm.IsActive
            };

            _db.Users.Add(entity);
            await _db.SaveChangesAsync();
            TempData["msg"] = "User created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Owner/Users/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();

            var vm = new UserVM
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            };
            return View(vm);
        }

        // POST: /Owner/Users/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (u == null) return NotFound();

            u.FullName = vm.FullName;
            u.Email = vm.Email;
            u.Role = vm.Role;
            u.IsActive = vm.IsActive;

            await _db.SaveChangesAsync();
            TempData["msg"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Owner/Users/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u != null)
            {
                _db.Users.Remove(u);
                await _db.SaveChangesAsync();
                TempData["msg"] = "User deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


