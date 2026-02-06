using MovieBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBooking.Models.Interfaces
{
    public interface IBookingRepository
    {
        Task<int> AddBookingAsync(Booking booking);
        Task<Booking> GetBookingByIdAsync(int id);
        Task<List<Booking>> GetBookingsByUserAsync(string userId);
        Task CancelBookingAsync(int bookingId);
        Task<List<Booking>> GetAllBookingsAsync();
    }
}
