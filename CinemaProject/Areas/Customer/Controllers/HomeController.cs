using System.Diagnostics;
using CinemaProject.Models;
using CinemaProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace CinemaProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        Repository<Movie> _movieRepository = new();

        public int Id { get; private set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        ApplicationDbContext _context = new();

        public async Task<IActionResult> Index()
        {
         var movie = await _movieRepository.GetAsync();
          
            return View(movie.AsEnumerable());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Welcome () {

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

        public ViewResult PersonInfo()
        {
            List<PersonInfo> persons = [
                new PersonInfo() {
      Id = 1, Name = "Ahmed", Age = 25 ,Skills = ["C#" , "CSS"]
        },
                    new PersonInfo() {
      Id = 1, Name = "Amr", Age = 35 ,Skills = ["C#" , ".net"]
        }

                ];
                ;
            return View(persons);

        }
        [HttpGet]
        public IActionResult Addmovies()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Addmovies(Movie movie)
        {

            await _movieRepository.CreateAsync(movie);
            await _movieRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
