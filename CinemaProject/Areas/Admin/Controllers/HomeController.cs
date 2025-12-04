using CinemaProject.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
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

        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}