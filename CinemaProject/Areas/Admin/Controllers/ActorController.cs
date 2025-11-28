using CinemaProject.Models;
using CinemaProject.Repositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        ApplicationDbContext _context = new();
        //Repository<Actor> _actorMovieRepository = new();
        Repository<Category> _categoryRepository = new();
        Repository<Cinema> _cinemaRepository = new();
        Repository<Actor> _actorRepository = new();

        public async Task<IActionResult> Index(ActorFilterVM actorFilterVM)
        {
            var actor = await _actorRepository.GetAsync(includes: [
           e => e.ActorCategories,
           e => e.ActorCinema], tracked: false);

            // Fix: Define actor as a strongly-typed variable, not object
            var Actor = actor.AsQueryable();

            // Add Filter
            if (actorFilterVM.ActorName is not null)
            {
                var ActorNameTrimmed = actorFilterVM.ActorName.Trim();

                actor = actor.Where(e => e.Name.Contains(ActorNameTrimmed));
                ViewBag.ActorName = actorFilterVM.ActorName;
            }


            if (actorFilterVM.ActorId is not null)
            {
                actor = actor.Where(e => e.Id == actorFilterVM.ActorId);
                ViewBag.ActorId = actorFilterVM.ActorId;
            }

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.Actors = await _actorRepository.GetAsync(tracked: false);

            // Add Pagination
            var totalNumberOfPages = Math.Ceiling(actor.Count() / 8.0);
            ViewBag.totalNumberOfPages = totalNumberOfPages;
            ViewBag.currentPage = actorFilterVM.page;

            actor = actor.Skip((actorFilterVM.page - 1) * 8).Take(8);


            return View(actor);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.Actors = await _actorRepository.GetAsync(tracked: false);

            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile Img, List<IFormFile> SubImgs)
        {
            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Save Img in Db
                actor.Img = fileName;
            }

            await _actorRepository.CreateAsync(actor);
            await _actorRepository.CommitAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, tracked: false);

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.Actors = await _actorRepository.GetAsync(tracked: false);

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? Img, List<IFormFile>? SubImgs)
        {
            var actorInDb = await _actorRepository.GetOneAsync(e => e.Id == actor.Id, tracked: false);

            if (actorInDb is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Delete Old Img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor_images", actorInDb.Img);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save Img in Db
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

