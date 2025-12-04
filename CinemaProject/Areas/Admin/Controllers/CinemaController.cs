using CinemaProject.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _cinemaRepository;

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        // ================= Index =================
        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetAsync(tracked: false);
            return View(cinemas);
        }

        // ================= Create =================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile Img)
        {
            if (Img != null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinema_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Img.CopyToAsync(stream);
                }

                cinema.Img = fileName;
            }

            await _cinemaRepository.CreateAsync(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? Img)
        {
            var cinemaInDb = await _cinemaRepository.GetOneAsync(e => e.Id == cinema.Id, tracked: false);
            if (cinemaInDb == null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            if (Img != null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinema_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Img.CopyToAsync(stream);
                }

                // Delete old image
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinema_images", cinemaInDb.Img);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                cinema.Img = fileName;
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            // Delete image
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinema_images", cinema.Img);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}