using CinemaProject.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;
namespace CinemaProject.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        // to check eligiblity of signin
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded) foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                    return View(registerVM);
                }
            // for sending confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action(nameof(Confirm), "Account",
                new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);


         await   _emailSender.SendEmailAsync(registerVM.Email, "CinemaProject - Confirm Your Email",
                $"<h1> Please Confirm your Email by clicking  <a href=' {Link} '>Here</a></h1>"
                );

            TempData["success-notification"] = " Account Added Successfully , please Confirm your Email";

            return RedirectToAction("Login");


        }

        public async Task<IActionResult> Confirm(string token, string userId)
        {
            // to find the user in DB
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            //validate the  Email
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["error-notification"] = string.Join(", ", result.Errors.Select(e => e.Code));

                return RedirectToAction(nameof(Login));
            }
            else
            {
                TempData["success-notification"] = "Account Confirmed Successfully";
                return RedirectToAction(nameof(Login));

            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailOrUserName);
            if (user is null)

            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Email Or Password");

                TempData["error-notification"] = "Invalid UserName / Email Or Password";
                return View(loginVM);
            }

            // to check the password validity
            //    var password = await _userManager.CheckPasswordAsync(user , loginVM.Password);
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, lockoutOnFailure: true, isPersistent: loginVM.RememberMe);


            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)

                    ModelState.AddModelError(string.Empty, "Invalid UserName / Email Or Password");


                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too Many Attempts , please try again later");
                    TempData["error-notification"] = " Too Many Attempts , please try again later";
                }
                return View(loginVM);


            }

            TempData["sucess-notification"] = "Successfully logged in";

            return RedirectToAction("index", "Admin", new { area = "Cinema" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid) return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);
            if (user is null)

            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Emai");

                TempData["error-notification"] = "Invalid UserName / Email ";
                return View(resendEmailConfirmationVM);
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Your Account Already Confirmed");

                TempData["error-notification"] = "Your Account Already Confirmed ";
                return View(resendEmailConfirmationVM);

            }

            // for sending confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action(nameof(Confirm), "Account",
                new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);


          await  _emailSender.SendEmailAsync(user.Email!, "CinemaProject - Resend Your Email",
                $"<h1> Please Confirm your Email by clicking  <a href=' {Link} '>Here</a></h1>"
                );

            TempData["success-notification"] = " Email sent Successfully , please Confirm your Email";

            return RedirectToAction("index", "Admin", new { area = "Cinema" });


        }

    }

    }
