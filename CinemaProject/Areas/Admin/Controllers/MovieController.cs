using CinemaProject.Models;
using CinemaProject.Repositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //ApplicationDbContext _context = new();
        Repository<Movie> _movieRepository = new();
        MovieSubImagesRepository _MovieSubImagesRepository = new();
        Repository<Category> _categoryRepository = new();
        Repository<Cinema> _cinemaRepository = new();

        public async Task<IActionResult> Index(movieFilterVM movieFilterVM)
        {
            var movie = await _movieRepository.GetAsync(includes: [e => e.Category, e => e.Cinema], tracked: false);

            // Add Filter
            if(movieFilterVM.MovieName is not null)
            {
                var MovieNameTrimmed = movieFilterVM.MovieName.Trim();

                movie = movie.Where(e=>e.Name.Contains(MovieNameTrimmed));
                ViewBag.MovieName = movieFilterVM.MovieName;
            }

            if(movieFilterVM.minPrice is not null)
            {
                movie = movie.Where(e => e.Price > movieFilterVM.minPrice);
                ViewBag.MinPrice = movieFilterVM.minPrice;
            }

            if (movieFilterVM.maxPrice is not null)
            {
                movie = movie.Where(e => e.Price < movieFilterVM.maxPrice);
                ViewBag.MaxPrice = movieFilterVM.maxPrice;
            }

            if (movieFilterVM.lessQuantity)
            {
                movie = movie.OrderBy(e => e.Quantity);
                ViewBag.LessQuantity = movieFilterVM.lessQuantity;
            }

            if (movieFilterVM.status)
            {
                movie = movie.Where(e=>e.Status);
                ViewBag.Status = movieFilterVM.status;
            }

            if (movieFilterVM.categoryId is not null)
            {
                movie = movie.Where(e => e.CategoryId == movieFilterVM.categoryId);
                ViewBag.CategoryId = movieFilterVM.categoryId;
            }

            if (movieFilterVM.CinemaId is not null)
            {
                movie = movie.Where(e => e.CinemaId == movieFilterVM.CinemaId);
                ViewBag.CinemaId = movieFilterVM.CinemaId;
            }

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);

            // Add Pagination
            var totalNumberOfPages = Math.Ceiling(movie.Count() / 8.0);
            ViewBag.totalNumberOfPages = totalNumberOfPages;
            ViewBag.currentPage = movieFilterVM.page;

            movie = movie.Skip((movieFilterVM.page - 1) * 8).Take(8);


            return View(movie);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile Img, List<IFormFile> SubImgs)
        {
            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Save Img in Db
                movie.MainImg = fileName;
            }

            await _movieRepository.CreateAsync(movie);
            await _movieRepository.CommitAsync();

            if (SubImgs is not null && SubImgs.Count > 0)
            {
                foreach (var item in SubImgs)
                {
                    // Save Img in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\movie_sub_images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    // Save Img in Db
                    await _MovieSubImagesRepository.CreateAsync(new()
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }

                await _MovieSubImagesRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, tracked: false);

            ViewBag.Categories = await _categoryRepository.GetAsync(tracked: false);
            ViewBag.Cinemas = await _cinemaRepository.GetAsync(tracked: false);
            ViewBag.MovieSubImages = await _MovieSubImagesRepository.GetAsync(e => e.MovieId == id, tracked: false);

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? Img, List<IFormFile>? SubImgs)
        {
            var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == movie.Id, tracked: false);

            if (movieInDb is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            if (Img is not null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                // Save Img in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                // Delete Old Img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", movieInDb.MainImg);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save Img in Db
                movie.MainImg = fileName;
            }
            else
            {
                movie.MainImg = movieInDb.MainImg;
            }

            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync();

            if (SubImgs is not null && SubImgs.Count > 0)
            {
                // Delete Old sub imgs from wwwroot & Db
                var MovieSubImages = await _MovieSubImagesRepository.GetAsync(e => e.MovieId == movie.Id);

                List<MovieSubImages> listOfMovieSubImages = [];
                foreach (var item in MovieSubImages)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema_images", item.Img);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    listOfMovieSubImages.Add(item);
                }

                _MovieSubImagesRepository.RemoveRange(listOfMovieSubImages);
                await _MovieSubImagesRepository.CommitAsync();

                // Create & Save New sub imgs
                List<MovieSubImages> listOfNewMovieSubImages = [];
                foreach (var item in SubImgs)
                {
                    // Save Img in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\movie_sub_images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    // Save Img in Db
                    listOfNewMovieSubImages.Add(new()
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }

                await _MovieSubImagesRepository.AddRangeAsync(listOfNewMovieSubImages);
                await _MovieSubImagesRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);

            if (movie is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), "Home");

            // Delete Old Img from wwwroot
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", movie.MainImg);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Delete Old sub imgs from wwwroot & Db
            var MovieSubImages = await _MovieSubImagesRepository.GetAsync(e => e.MovieId == movie.Id);

            List<MovieSubImages> listOfMovieSubImages = [];
            foreach (var item in MovieSubImages)
            {
                var oldSubImgFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\movie_sub_images", item.Img);

                if (System.IO.File.Exists(oldSubImgFilePath))
                {
                    System.IO.File.Delete(oldSubImgFilePath);
                }

                listOfMovieSubImages.Add(item);
            }

            _MovieSubImagesRepository.RemoveRange(listOfMovieSubImages);
            await _MovieSubImagesRepository.CommitAsync();

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
