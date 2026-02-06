using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieBooking.Hubs;
using MovieBooking.Models;
using MovieBooking.Models.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieBooking.Controllers
{

    [Authorize(Policy = "User")]
    public class BookingController : Controller
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IHubContext<MovieHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(
            IMovieRepository movieRepo,
            IBookingRepository bookingRepo,
            IHubContext<MovieHub> hubContext,
            UserManager<ApplicationUser> userManager)
        {
            _movieRepo = movieRepo;
            _bookingRepo = bookingRepo;
            _hubContext = hubContext;
            _userManager = userManager;
        }

 
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepo.GetAllAsync();

            var nowShowing = movies
                .Where(m => m.ReleaseDate <= DateTime.Today)
                .ToList();

            return View(nowShowing);
        }

        public async Task<IActionResult> ToBook(int id)
        {
            var movie = await _movieRepo.GetByIdAsync(id);
            if (movie == null) return NotFound();

            ViewBag.Movie = movie;
            return View();
        }

      
        [HttpPost]
        public async Task<IActionResult> ToBook(int movieId, DateTime showDate, int ticketsCount)
        {
            var movie = await _movieRepo.GetByIdAsync(movieId);

            if (movie == null || ticketsCount <= 0 || showDate < DateTime.Today)
            {
                TempData["Error"] = "Invalid ticket count or show date.";
                return RedirectToAction("ToBook", new { id = movieId });
            }

            if (movie.TicketsAvailable < ticketsCount)
            {
                TempData["Error"] = "Not enough tickets available.";
                return RedirectToAction("ToBook", new { id = movieId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = new Booking
            {
                MovieId = movieId,
                UserId = userId,
                ShowDate = showDate,
                TicketsCount = ticketsCount,
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                TotalPrice = (movie.Price ?? 0) * ticketsCount
            };

            int bookingId = await _bookingRepo.AddBookingAsync(booking);

            movie.TicketsAvailable -= ticketsCount;
            await _movieRepo.UpdateAsync(movie);

            await _hubContext.Clients.All.SendAsync(
                "ReceiveTicketUpdate",
                movie.MovieId,
                movie.TicketsAvailable
            );

            return RedirectToAction("Confirmation", new { id = bookingId });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Unauthorized();

            return View(booking);
        }


        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _bookingRepo.GetBookingsByUserAsync(userId);
            return View(bookings);
        }

      
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (booking == null || booking.UserId != userId || booking.Status != "Confirmed")
                return Unauthorized();

            await _bookingRepo.CancelBookingAsync(id);

            var movie = await _movieRepo.GetByIdAsync(booking.MovieId);
            movie.TicketsAvailable += booking.TicketsCount;
            await _movieRepo.UpdateAsync(movie);

            await _hubContext.Clients.All.SendAsync(
                "ReceiveTicketUpdate",
                movie.MovieId,
                movie.TicketsAvailable
            );

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("List");
        }
    }
}
