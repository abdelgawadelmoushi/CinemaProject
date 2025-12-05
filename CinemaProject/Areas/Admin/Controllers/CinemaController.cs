using CinemaProject.Areas.Customer.Controllers;
using CinemaProject.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
  
    public class CinemaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Cinema> _cinemaRepository;

        public int Id { get; private set; }

        public CinemaController(
            ILogger<HomeController> logger,
            IRepository<Movie> movieRepository,
            IRepository<Cinema> cinemaRepository,
            ApplicationDbContext context)
        {
            _logger = logger;
            _cinemaRepository = cinemaRepository;
            _cinemaRepository = cinemaRepository;
            _context = context;
        }
        public IActionResult Index(CinemaFilterVM filter)
        {
            var cinemas = _cinemaRepository
                .GetAsync(tracked: false)
                .Result
                .AsQueryable();

            // Filter by Name
            if (!string.IsNullOrWhiteSpace(filter.CinemaName))
            {
                var trimmed = filter.CinemaName.Trim();
                cinemas = cinemas.Where(c => c.Name.Contains(trimmed));
            }

            // Filter by Status
            if (filter.Status)
            {
                cinemas = cinemas.Where(c => c.Status == true);
            }

            // Filter by CinemaId
            if (filter.CinemaId.HasValue)
            {
                cinemas = cinemas.Where(c => c.Id == filter.CinemaId);
            }

            // Pagination
            int pageSize = 8;
            int totalItems = cinemas.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            cinemas = cinemas
                .Skip((filter.Page - 1) * pageSize)
                .Take(pageSize);

            // ViewBag values for filters
            ViewBag.CinemaName = filter.CinemaName;
            ViewBag.CinemaId = filter.CinemaId;
            ViewBag.Status = filter.Status;

            // Pagination ViewBag
            ViewBag.currentPage = filter.Page;
            ViewBag.totalNumberOfPages = totalPages;

            // Dropdown
            ViewBag.Cinemas = _cinemaRepository.GetAsync().Result;

            return View(cinemas);
        }
        // ================= Create =================
        [HttpGet]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ,{SD.Employee_Role} ")]

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ,{SD.Employee_Role} ")]

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
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            return View(cinema);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

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
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

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