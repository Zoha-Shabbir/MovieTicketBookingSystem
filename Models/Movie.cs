using System.ComponentModel.DataAnnotations;

namespace MovieBooking.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public decimal? Price { get; set; }
        public string? PosterUrl { get; set; }
        public int TicketsAvailable { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
