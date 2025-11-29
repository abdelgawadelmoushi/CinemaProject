using CinemaProject.Data;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        public readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var moviesCount = await _context.Movies.CountAsync();
            var actorsCount = await _context.Actors.CountAsync();
            var categoriesCount = await _context.Categories.CountAsync();
            var CinemaCount = await _context.Cinemas.CountAsync();

            var VM = new StatisticsVM
            {
                MoviesCount = moviesCount,
                ActorsCount = actorsCount,
                CategoriesCount = categoriesCount,
                CinemasCount = CinemaCount
            };

            return View(VM);
        }
    }
}
