using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieBooking.Hubs;
using MovieBooking.Models;
using MovieBooking.Models.Interfaces;
using System.Threading.Tasks;

namespace MovieBooking.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IHubContext<MovieHub> _hubContext;

        public AdminController(
            IMovieRepository movieRepo,
            IBookingRepository bookingRepo,
            IHubContext<MovieHub> hubContext)
        {
            _movieRepo = movieRepo;
            _bookingRepo = bookingRepo;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(string title, string genre, decimal price, string posterUrl, int ticketsAvailable)
        {
            var movie = new Movie
            {
                Title = title,
                Genre = genre,
                Price = price,
                PosterUrl = posterUrl,
                TicketsAvailable = ticketsAvailable
            };

            await _movieRepo.AddAsync(movie);

            // SignalR: notify clients
            await _hubContext.Clients.All.SendAsync(
                "ReceiveMovieAdded",
                movie.MovieId,
                movie.Title,
                movie.Genre,
                movie.Price,
                movie.PosterUrl,
                movie.TicketsAvailable
            );

            TempData["Success"] = "Movie added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
        {
            var movie = await _movieRepo.GetByIdAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(int movieId, string title, string genre, decimal price, string posterUrl, int ticketsAvailable)
        {
            var updatedMovie = new Movie
            {
                MovieId = movieId,
                Title = title,
                Genre = genre,
                Price = price,
                PosterUrl = posterUrl,
                TicketsAvailable = ticketsAvailable
            };

            await _movieRepo.UpdateAsync(updatedMovie);

            // SignalR: notify clients about price/ticket changes
            await _hubContext.Clients.All.SendAsync(
                "ReceiveMovieUpdated",
                updatedMovie.MovieId,
                updatedMovie.Price,
                updatedMovie.TicketsAvailable
            );

            TempData["Success"] = "Movie updated successfully!";
            return RedirectToAction("AllMovies");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _movieRepo.GetByIdAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovieConfirmed(int movieId)
        {
            await _movieRepo.DeleteAsync(movieId);

            // SignalR notify clients
            await _hubContext.Clients.All.SendAsync("ReceiveMovieDeleted", movieId);

            TempData["Success"] = "Movie deleted successfully!";

            return RedirectToAction("AllMovies", "Admin");
        }
        

        public async Task<IActionResult> AllMovies()
        {
            var movies = await _movieRepo.GetAllAsync();
            return View(movies);
        }

        public async Task<IActionResult> AllBookings()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            return View(bookings);
        }
    }
}
