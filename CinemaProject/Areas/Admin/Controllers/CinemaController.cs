using CinemaProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //ApplicationDbContext _context = new();
        Repository<Cinema> _cinemaRepository = new();

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetAsync(tracked: false);

            // Add Filter

            return View(cinemas.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", fileName);

                using(var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Save Img in Db
                cinema.Img = fileName;
            }

            await _cinemaRepository.CreateAsync(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);

            if (cinema is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? Img)
        {
            var cinemaInDb = await _cinemaRepository.GetOneAsync(e => e.Id == cinema.Id, tracked: false);

            if (cinemaInDb is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Delete Old Img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", cinemaInDb.Img);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save Img in Db
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

        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);

            if (cinema is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            // Delete Old Img from wwwroot
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", cinema.Img);

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
