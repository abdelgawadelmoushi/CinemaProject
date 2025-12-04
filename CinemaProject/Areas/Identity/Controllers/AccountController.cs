using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        // to check eligiblity of signin
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender , IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
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
                {
                    ModelState.AddModelError(string.Empty, "Invalid UserName / Email Or Password");
                    TempData["error-notification"] = "Invalid UserName / Email Or Password";
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too Many Attempts , please try again later");
                    TempData["error-notification"] = "Too Many Attempts , please try again later";
                }
                else
                {
                    // أي فشل آخر
                    ModelState.AddModelError(string.Empty, "Invalid UserName / Email Or Password");
                    TempData["error-notification"] = "Invalid UserName / Email Or Password";
                }

                return View(loginVM);
            }

            TempData["success-notification"] = "Successfully logged in";

            return RedirectToAction("index", "Cinema", new { area = "admin" });

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

            return RedirectToAction("index", "Cinema", new { area = "admin" });


        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {

            if (!ModelState.IsValid) return View(forgetPasswordVM);

               var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);
            if (user is null)

            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Emai");

                TempData["error-notification"] = "Invalid UserName / Email ";
                return View(forgetPasswordVM);
            }

                // for sending confirmation email

          var otp =   new Random().Next(1000, 9999);

         var userOTPs= await _applicationUserOTPRepository.GetAsync(e=>e.ApplicationUserId==user.Id &&
         e.CreatedAt <DateTime.UtcNow.AddHours(-24));

            if (userOTPs.ToList().Count>5)
            {
                ModelState.AddModelError(string.Empty, "Too Many Attempts , please try again Later");

                return View(forgetPasswordVM);

            }

            // create OTP in DB
            await _applicationUserOTPRepository.CreateAsync(new()
                {
               OTP = otp.ToString(),
               ApplicationUserId =user.Id,
            } );
            await _applicationUserOTPRepository.CommitAsync();
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action(nameof(Confirm), "Account",
                new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);


            await _emailSender.SendEmailAsync(user.Email!, "CinemaProject - Reset Your Password",
                  $"<h1> Please reset your password using this OTP {otp} " +
                  $"please do not share your OTP with any One'></h1>"
                  );

            TempData["success-notification"] = " Email sent Successfully , please Check your Email";

            return RedirectToAction("ValidateOTP", new {userId=user.Id});
        }

        [HttpGet]
        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM ()
            {
                UserId= userId,
            });
        }



        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {

            if (!ModelState.IsValid) return View(validateOTPVM);

            var user = await _userManager.FindByIdAsync(validateOTPVM.UserId);
            if (user is null) return NotFound();
            var valideOTPs = await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id && e.isValid &&
            e.Validto>DateTime.UtcNow);

            var resul = valideOTPs.Any(e=>e.OTP == validateOTPVM.OTP);

            if (!resul)
            {
                ModelState.AddModelError(string.Empty, "Invalid OR Expired OTP");
                return View(validateOTPVM);

            }

            TempData["success-notification"] = " Valid OTP";


            return RedirectToAction("ResetPassword" , new { userId = user.Id});

        }
        public IActionResult ResetPassword(string userId)
        {
            return View(new ResetPasswordVM
            {
                UserId = userId,
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid) return View(resetPasswordVM);

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user is null) return NotFound();

     var dummyToen =    await _userManager.GeneratePasswordResetTokenAsync(user);
      var result =  await _userManager.ResetPasswordAsync(user , dummyToen, resetPasswordVM.Password);

            if (!result.Succeeded)
            {
                TempData["error-notification"] = string.Join(", ", result.Errors.Select(e => e.Code));

                return RedirectToAction(nameof(Login));
            }
            else
            {
                TempData["success-notification"] = "Password has been changed Suceessfully";
                return RedirectToAction(nameof(Login));

            }


        }
    }

}




