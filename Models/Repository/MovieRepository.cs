using MovieBooking.Models.Interfaces;
using MovieBooking.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBooking.Models.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly string _connectionString;

        public MovieRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Movie>> GetAllAsync()
        {
            await using var connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Movies";
            var result = await connection.QueryAsync<Movie>(sql);
            return result.ToList();
        }

        public async Task<Movie?> GetByIdAsync(int movieId)
        {
            await using var connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Movies WHERE MovieId = @MovieId";
            return await connection.QueryFirstOrDefaultAsync<Movie>(sql, new { MovieId = movieId });
        }

        public async Task UpdateAsync(Movie movie)
        {
            await using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Movies 
                           SET Title = @Title, Genre = @Genre, Price = @Price, PosterUrl = @PosterUrl, TicketsAvailable = @TicketsAvailable
                           WHERE MovieId = @MovieId";
            await connection.ExecuteAsync(sql, movie);
        }

        public async Task AddAsync(Movie movie)
        {
            await using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Movies (Title, Genre, Price, PosterUrl, TicketsAvailable)
                           VALUES (@Title, @Genre, @Price, @PosterUrl, @TicketsAvailable);
                           SELECT CAST(SCOPE_IDENTITY() as int);";
            int id = await connection.QuerySingleAsync<int>(sql, movie);
            movie.MovieId = id;
        }

        public async Task DeleteAsync(int movieId)
        {
            await using var connection = new SqlConnection(_connectionString);
            string sql = "DELETE FROM Movies WHERE MovieId = @MovieId";
            await connection.ExecuteAsync(sql, new { MovieId = movieId });
        }
    }
}
