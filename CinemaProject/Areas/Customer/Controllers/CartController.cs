using CinemaProject.Models;
using CinemaProject.Repositories.IRepositories;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Linq.Expressions;

namespace CinemaProject.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IRepository<Cinema> _cinemaRepository;

        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cinema> cinemaRepository,IRepository<Movie> movieRepository, IRepository<Cart> cartRepository , IRepository<Promotion> promotionRepository, UserManager<ApplicationUser> userManager)
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _cartRepository = cartRepository;  
            _promotionRepository = promotionRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> AddToCart(int movieId, int count)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _movieRepository.GetOneAsync(e => e.Id == movieId);

            if (movie is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (cartInDb is not null)
            {
                cartInDb.Count += count;
            }
            else
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    MovieId = movieId,
                    Count = count,
                    MoviePrice = movie.Price - (movie.Price * (movie.Discount / 100m))
                });
            }

            await _cartRepository.CommitAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            if (code is not null)
            {
                var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code && e.IsValid && e.ValidTo > DateTime.UtcNow && e.MaxUsage > 0);

                if (promotion is null)
                    TempData["error-notification"] = "Invalid Code";
                else
                {
                    bool founded = false;

                    foreach (var item in cartInDb)
                    {
                        if (item.MovieId == promotion.MovieId)
                        {
                            item.MoviePrice -= (item.MoviePrice * (promotion.Discount / 100));
                            promotion.MaxUsage -= 1;
                            await _cartRepository.CommitAsync();
                            TempData["success-notification"] = "Apply Code Successfully";
                            founded = true;
                            break;
                        }
                    }

                    if (!founded)
                        TempData["error-notification"] = "Invalid Code";
                }
            }

            return View(cartInDb);
        }

        public async Task<IActionResult> IncrementCount(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (cartInDb is null)
                return NotFound();

            cartInDb.Count += 1;
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementCount(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (cartInDb is null)
                return NotFound();

            if (cartInDb.Count > 1)
            {
                cartInDb.Count -= 1;
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (cartInDb is null)
                return NotFound();

            _cartRepository.Delete(cartInDb);
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cartInDb = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            foreach (var item in cartInDb)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)item.MoviePrice * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
