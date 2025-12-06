using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =$"{SD.Super_Admin_Role}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser>userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
           var users = _userManager.Users.AsNoTracking().AsQueryable();
            return View(users.AsEnumerable());
        }
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            user.LockoutEnabled = false;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddDays(30);

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow; 

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

    }
}
