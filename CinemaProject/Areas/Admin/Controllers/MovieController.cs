using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MovieController : Controller
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IMovieSubImagesRepository _movieSubImagesRepository;

        public MovieController(
            IRepository<Actor> actorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IMovieSubImagesRepository movieSubImagesRepository,
            IRepository<Movie> movieRepository
        )
        {
            _actorRepository = actorRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieSubImagesRepository = movieSubImagesRepository;
            _movieRepository = movieRepository;
        }

        #region Index

        public async Task<IActionResult> Index(movieFilterVM movieFilterVM)
        {
            var movies = await _movieRepository.GetAsync(includes: [e => e.Category, e => e.Cinema], tracked: false);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(movieFilterVM.MovieName))
            {
                movies = movies.Where(e => e.Name.Contains(movieFilterVM.MovieName.Trim()));
                ViewBag.MovieName = movieFilterVM.MovieName;
            }

            if (movieFilterVM.minPrice.HasValue)
            {
                movies = movies.Where(e => e.Price >= movieFilterVM.minPrice.Value);
                ViewBag.MinPrice = movieFilterVM.minPrice;
            }

            if (movieFilterVM.maxPrice.HasValue)
            {
                movies = movies.Where(e => e.Price <= movieFilterVM.maxPrice.Value);
                ViewBag.MaxPrice = movieFilterVM.maxPrice;
            }

            if (movieFilterVM.lessQuantity)
            {
                movies = movies.OrderBy(e => e.Quantity);
                ViewBag.LessQuantity = movieFilterVM.lessQuantity;
            }

            if (movieFilterVM.status)
            {
                movies = movies.Where(e => e.Status);
                ViewBag.Status = movieFilterVM.status;
            }

            if (movieFilterVM.categoryId.HasValue)
            {
                movies = movies.Where(e => e.CategoryId == movieFilterVM.categoryId.Value);
                ViewBag.CategoryId = movieFilterVM.categoryId;
            }

            if (movieFilterVM.CinemaId.HasValue)
            {
                movies = movies.Where(e => e.CinemaId == movieFilterVM.CinemaId.Value);
                ViewBag.CinemaId = movieFilterVM.CinemaId;
            }

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);

            // Pagination
            var totalPages = Math.Ceiling(movies.Count() / 8.0);
            ViewBag.totalNumberOfPages = totalPages;
            ViewBag.currentPage = movieFilterVM.page;

            movies = movies.Skip((movieFilterVM.page - 1) * 8).Take(8);

            return View(movies);
        }
        #endregion

        #region Create
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ,{SD.Employee_Role} ")]

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.Actors = await _actorRepository.GetAsync(tracked: false); // جلب الممثلين

            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ,{SD.Employee_Role} ")]

        public async Task<IActionResult> Create(Movie movie, IFormFile Img, List<IFormFile> SubImgs)
        {
            // Validate Cinema & Category exist
            var cinemaExists = await _cinemaRepository.GetOneAsync(c => c.Id == movie.CinemaId) != null;
            var categoryExists = await _categoryRepository.GetOneAsync(c => c.Id == movie.CategoryId) != null;

            if (!cinemaExists || !categoryExists)
            {
                ModelState.AddModelError("", "Selected Cinema or Category does not exist.");
                ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
                ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
                return View(movie);
            }

            // Main Image
            if (Img != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine("wwwroot/images/movie_images", fileName);

                using var stream = System.IO.File.Create(filePath);
                Img.CopyTo(stream);
                movie.MainImg = fileName;
            }

            await _movieRepository.CreateAsync(movie);
            await _movieRepository.CommitAsync();

            // Sub Images
            if (SubImgs != null && SubImgs.Count > 0)
            {
                var newSubImgs = new List<MovieSubImages>();
                foreach (var file in SubImgs)
                {
                    var subFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var subFilePath = Path.Combine("wwwroot/images/movie_images/movie_sub_images", subFileName);

                    using var stream = System.IO.File.Create(subFilePath);
                    file.CopyTo(stream);

                    newSubImgs.Add(new MovieSubImages
                    {
                        Img = subFileName,
                        MovieId = movie.Id
                    });
                }

                await _movieSubImagesRepository.AddRangeAsync(newSubImgs);
                await _movieSubImagesRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (movie == null) return NotFound();

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.MovieSubImages = await _movieSubImagesRepository.GetAsync(e => e.MovieId == id);

            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

        public async Task<IActionResult> Edit(Movie model, IFormFile MainImg, List<IFormFile> SubImgs)
        {
            var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == model.Id, tracked: false);
            if (movieInDb == null) return NotFound();

            movieInDb.Name = model.Name;
            movieInDb.Description = model.Description;
            movieInDb.Price = model.Price;
            movieInDb.Discount = model.Discount;
            movieInDb.Quantity = model.Quantity;
            movieInDb.Status = model.Status;
            movieInDb.CategoryId = model.CategoryId;
            movieInDb.CinemaId = model.CinemaId;

            if (MainImg != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(MainImg.FileName);
                var filePath = Path.Combine("wwwroot/images/movie_images", fileName);

                using var stream = System.IO.File.Create(filePath);
                MainImg.CopyTo(stream);

                var oldPath = Path.Combine("wwwroot/images/movie_images", movieInDb.MainImg);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                movieInDb.MainImg = fileName;
            }

            if (SubImgs != null && SubImgs.Count > 0)
            {
                var oldSubImgs = await _movieSubImagesRepository.GetAsync(e => e.MovieId == movieInDb.Id);
                foreach (var sub in oldSubImgs)
                {
                    var oldSubPath = Path.Combine("wwwroot/images/movie_images/movie_sub_images", sub.Img);
                    if (System.IO.File.Exists(oldSubPath))
                        System.IO.File.Delete(oldSubPath);
                }

                _movieSubImagesRepository.RemoveRange(oldSubImgs.ToList());
                await _movieSubImagesRepository.CommitAsync();

                List<MovieSubImages> newSubImgs = new();
                foreach (var item in SubImgs)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine("wwwroot/images/movie_images/movie_sub_images", fileName);

                    using var stream = System.IO.File.Create(filePath);
                    item.CopyTo(stream);

                    newSubImgs.Add(new MovieSubImages
                    {
                        Img = fileName,
                        MovieId = movieInDb.Id
                    });
                }

                await _movieSubImagesRepository.AddRangeAsync(newSubImgs);
                await _movieSubImagesRepository.CommitAsync();
            }

            _movieRepository.Update(movieInDb);

            await _movieRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpGet]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} , {SD.Admin_Role} ")]

        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie == null) return NotFound();

            // Delete Main Image
            var mainPath = Path.Combine("wwwroot/images/movie_images", movie.MainImg);
            if (System.IO.File.Exists(mainPath)) System.IO.File.Delete(mainPath);

            // Delete Sub Images
            var subImgs = await _movieSubImagesRepository.GetAsync(e => e.MovieId == movie.Id);
            foreach (var sub in subImgs)
            {
                var subPath = Path.Combine("wwwroot/images/movie_images/movie_sub_images", sub.Img);
                if (System.IO.File.Exists(subPath)) System.IO.File.Delete(subPath);
            }

            _movieSubImagesRepository.RemoveRange(subImgs.ToList());
            await _movieSubImagesRepository.CommitAsync();

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
