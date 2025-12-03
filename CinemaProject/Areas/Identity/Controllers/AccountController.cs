using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
namespace CinemaProject.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            ApplicationUser user = new()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                Name = registerVM.Name,
                Address = registerVM.Address,
            };

            //instead
            //ApplicationUser user = registerVM.Adapt<ApplicationUser>();
         var result =   await _userManager.CreateAsync(user , registerVM.Password);
            if (!result.Succeeded) foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                    return View(registerVM);
                }
            TempData["success-notification"] = " Account Added Successfully";

            return RedirectToAction("Login");

        }
    }
}
