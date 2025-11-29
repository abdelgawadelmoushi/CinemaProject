using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;

        public ActorController(
            IRepository<Actor> actorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository)
        {
            _actorRepository = actorRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
        }

        // ================= Index =================
        public async Task<IActionResult> Index(ActorFilterVM actorFilterVM)
        {
            var actors = await _actorRepository.GetAsync(
    expression: null,
    includes: new Expression<Func<Actor, object>>[]
    {
        e => e.ActorCategories,
        e => e.ActorCinema
    },
    tracked: false,
    cancellationToken: default
);


            var query = actors.AsQueryable();

            if (!string.IsNullOrEmpty(actorFilterVM.ActorName))
            {
                var trimmedName = actorFilterVM.ActorName.Trim();
                query = query.Where(e => e.Name.Contains(trimmedName));
                ViewBag.ActorName = actorFilterVM.ActorName;
            }

            if (actorFilterVM.ActorId.HasValue)
            {
                query = query.Where(e => e.Id == actorFilterVM.ActorId.Value);
                ViewBag.ActorId = actorFilterVM.ActorId;
            }

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.Actors = actors;

            var totalPages = Math.Ceiling(query.Count() / 8.0);
            ViewBag.totalNumberOfPages = totalPages;
            ViewBag.currentPage = actorFilterVM.page;

            query = query.Skip((actorFilterVM.page - 1) * 8).Take(8);

            return View(query);
        }

        // ================= Create =================
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile Img)
        {
            if (Img != null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actor_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Img.CopyToAsync(stream);
                }

                actor.Img = fileName;
            }

            await _actorRepository.CreateAsync(actor);
            await _actorRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (actor == null) return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? Img)
        {
            var actorInDb = await _actorRepository.GetOneAsync(e => e.Id == actor.Id, tracked: false);
            if (actorInDb == null) return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            if (Img != null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actor_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Img.CopyToAsync(stream);
                }

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actor_images", actorInDb.Img);
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);

                actor.Img = fileName;
            }
            else
            {
                actor.Img = actorInDb.Img;
            }

            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _actorRepository.GetOneAsync(a => a.Id == id);
            if (actor == null) return NotFound();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actor_images", actor.Img);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
