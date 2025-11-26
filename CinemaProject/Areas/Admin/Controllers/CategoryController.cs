using CinemaProject.Repositories;
using CinemaProject.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new();
        Repository<Category> _categoryRepository = new();


        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync(tracked: false);

            // Add Filter

            return View(categories.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }

            //CategoryValidator validationRules = new CategoryValidator();
            //var result = validationRules.Validate(category);

            //if (!result.IsValid)
            //    return View(category);

            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("success-notification", "Create Category Successfully", new()
            //{
            //    Expires = DateTime.UtcNow.AddMinutes(30),
            //});
            //HttpContext.Session.SetString("success-notification", "Create Category Successfully");

            TempData["success-notification"] = "Create Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

            if (category is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("success-notification", "Update Category Successfully");
            TempData["success-notification"] = "Update Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

            if (category is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("success-notification", "Delete Category Successfully");
            TempData["success-notification"] = "Delete Category Successfully";


            return RedirectToAction(nameof(Index));
        }
    }
}
