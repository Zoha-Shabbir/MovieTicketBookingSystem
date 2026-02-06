using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using MovieBooking.Controllers;
using MovieBooking.Hubs;
using MovieBooking.Models;
using MovieBooking.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
namespace MovieBooking.Tests.Controller
{
    public class BookingControllerTests
    {
        private readonly Mock<IMovieRepository> _movieRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IHubContext<MovieHub>> _hubMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public BookingControllerTests()
        {
            _movieRepoMock = new Mock<IMovieRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _hubMock = new Mock<IHubContext<MovieHub>>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task Index_ReturnsViewWithMovies()
        {
            var controller = new BookingController(
                _movieRepoMock.Object,
                _bookingRepoMock.Object,
                _hubMock.Object,
                _userManagerMock.Object
            );

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ToBook_InvalidMovie_ReturnsNotFound()
        {
            _movieRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Movie)null);

            var controller = new BookingController(
                _movieRepoMock.Object,
                _bookingRepoMock.Object,
                _hubMock.Object,
                _userManagerMock.Object
            );

            var result = await controller.ToBook(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}