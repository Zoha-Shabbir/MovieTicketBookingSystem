using System.ComponentModel.DataAnnotations;

namespace MovieBooking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string? UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ShowDate { get; set; }

        public int TicketsCount { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Status { get; set; }
        public Movie? Movie { get; set; }   

        public ApplicationUser? User { get; set; }
    }
}
