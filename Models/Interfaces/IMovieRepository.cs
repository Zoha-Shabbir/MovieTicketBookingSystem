using MovieBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBooking.Models.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(int movieId);
        Task UpdateAsync(Movie movie);
        Task AddAsync(Movie movie);
        Task DeleteAsync(int movieId);
    }
}
