using MovieBooking.Data;
using MovieBooking.Models;
using MovieBooking.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace MovieBooking.Models.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddBookingAsync(Booking booking)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
                "SELECT * FROM Movies WHERE MovieId = @MovieId",
                new { booking.MovieId },
                transaction
            );

            if (movie == null || movie.TicketsAvailable < booking.TicketsCount)
            {
                await transaction.RollbackAsync();
                return 0;
            }

            int remainingTickets = movie.TicketsAvailable - booking.TicketsCount;

            await connection.ExecuteAsync(
                "UPDATE Movies SET TicketsAvailable = @Remaining WHERE MovieId = @MovieId",
                new { Remaining = remainingTickets, booking.MovieId },
                transaction
            );

            int bookingId = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Bookings
                  (MovieId, UserId, TicketsCount, TotalPrice, Status, BookingDate, ShowDate)
                  VALUES (@MovieId, @UserId, @TicketsCount, @TotalPrice, @Status, @BookingDate, @ShowDate);
                  SELECT CAST(SCOPE_IDENTITY() as int);",
                new
                {
                    booking.MovieId,
                    booking.UserId,
                    booking.TicketsCount,
                    booking.TotalPrice,
                    booking.Status,
                    booking.BookingDate,
                    booking.ShowDate
                },
                transaction
            );

            booking.BookingId = bookingId;
            await transaction.CommitAsync();

            return bookingId;
        }



        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT b.*, m.*, u.* FROM Bookings b 
                JOIN Movies m ON b.MovieId = m.MovieId 
                JOIN AspNetUsers u ON b.UserId = u.Id
                WHERE b.BookingId = @Id";

            var result = await connection.QueryAsync<Booking, Movie, ApplicationUser, Booking>(
                sql,
                (booking, movie, user) =>
                {
                    booking.Movie = movie;
                    booking.User = user;
                    return booking;
                },
                new { Id = id },
                splitOn: "MovieId,Id"
            );

            return result.FirstOrDefault();
        }
        public async Task<List<Booking>> GetBookingsByUserAsync(string userId)
        {
            await using var connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT b.*, m.*
                FROM Bookings b
                JOIN Movies m ON b.MovieId = m.MovieId
                WHERE b.UserId = @UserId
                ORDER BY b.BookingDate DESC";

            var result = await connection.QueryAsync<Booking, Movie, Booking>(
                sql,
                (booking, movie) =>
                {
                    booking.Movie = movie;
                    return booking;
                },
                new { UserId = userId },
                splitOn: "MovieId"
            );

            return result.ToList();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            var booking = await connection.QueryFirstOrDefaultAsync<Booking>(
                @"SELECT b.*, m.TicketsAvailable 
                  FROM Bookings b 
                  JOIN Movies m ON b.MovieId = m.MovieId
                  WHERE b.BookingId = @Id",
                new { Id = bookingId },
                transaction
            );

            if (booking != null && booking.Status == "Confirmed")
            {
                await connection.ExecuteAsync(
                    "UPDATE Bookings SET Status = 'Cancelled' WHERE BookingId = @Id",
                    new { Id = bookingId },
                    transaction
                );

                await connection.ExecuteAsync(
                    @"UPDATE Movies 
                      SET TicketsAvailable = TicketsAvailable + @Tickets 
                      WHERE MovieId = @MovieId",
                    new { Tickets = booking.TicketsCount, booking.MovieId },
                    transaction
                );

                await transaction.CommitAsync();
            }
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            await using var connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT b.*, m.*, u.*
                FROM Bookings b
                JOIN Movies m ON b.MovieId = m.MovieId
                JOIN AspNetUsers u ON b.UserId = u.Id
                ORDER BY b.BookingDate DESC";

            var result = await connection.QueryAsync<Booking, Movie, ApplicationUser, Booking>(
                sql,
                (booking, movie, user) =>
                {
                    booking.Movie = movie;
                    booking.User = user;
                    return booking;
                },
                splitOn: "MovieId,Id"
            );

            return result.ToList();
        }
    }
}


