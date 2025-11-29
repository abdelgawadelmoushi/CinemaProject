using CinemaProject.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // ================= Index =================
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync(tracked: false);
            return View(categories);
        }

        // ================= Create =================
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

            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();

            TempData["success-notification"] = "Category Created Successfully";

            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category == null)
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

            TempData["success-notification"] = "Category Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category == null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();

            TempData["success-notification"] = "Category Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
