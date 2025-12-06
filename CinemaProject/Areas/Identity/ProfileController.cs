
using CinemaProject.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CinemaProject.Areas.Identity
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<PersonInfo> _PersonInfo;
        public ProfileController(UserManager<ApplicationUser> userManager, IRepository<PersonInfo> personInfo)
        {
            _userManager = userManager;
            _PersonInfo = personInfo;
        }

        public async Task<IActionResult> Index()
        {
            // to get the data from the coockie "User"
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();
            // to send all the user information in a single code using Mapster
            return View(user.Adapt<ApplicationUserVM>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM , IFormFile Img)
        {

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();
            if (applicationUserVM.ImgFile != null)
            {
                var uploadFolder = Path.Combine("wwwroot/images/User_images");

                if (!string.IsNullOrEmpty(user.Img)) 
                {
                    var oldPath = Path.Combine(uploadFolder, user.Img);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var newFileName = Guid.NewGuid() + Path.GetExtension(applicationUserVM.ImgFile.FileName);
                var newFilePath = Path.Combine(uploadFolder, newFileName);
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await applicationUserVM.ImgFile.CopyToAsync(fileStream);
                }
                user.Img = newFileName;
            }
            user.Name = applicationUserVM.Name;
            user.UserName = applicationUserVM.UserName;
            user.Email = applicationUserVM.Email;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Index", applicationUserVM); 
            }
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.OldPassword, applicationUserVM.NewPassword);

            if (!result.Succeeded)
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                    return View(applicationUserVM);
                }

            return RedirectToAction(nameof(Index));
        }
    }
}
