using System.Diagnostics;
using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Cinema> _cinemaRepository;

        public int Id { get; private set; }

        public HomeController(
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

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Welcome()
        {
            return View();
        }

        public ViewResult cinemaAdded()
        {
            return View();
        }

        public IActionResult viewAllcinemas()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Addcinemas()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Addcinemas(Cinema cinema)
        {
            await _cinemaRepository.CreateAsync(cinema);
            await _cinemaRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}