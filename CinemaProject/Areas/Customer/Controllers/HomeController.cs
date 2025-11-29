using System.Diagnostics;
using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
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
            _movieRepository = movieRepository;
            _cinemaRepository = cinemaRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetAsync(tracked: false);
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

        public ViewResult movieAdded()
        {
            return View();
        }

        public IActionResult viewAllmovies()
        {
            return View();
        }

     
        [HttpGet]
        public IActionResult Addmovies()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Addmovies(Movie movie)
        {
            await _movieRepository.CreateAsync(movie);
            await _movieRepository.CommitAsync();

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
