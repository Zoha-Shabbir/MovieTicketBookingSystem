using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

namespace MovieBooking.Tests.Controller
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_Returns_View()
        {
            var movieRepo = new Mock<IMovieRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var hub = new Mock<IHubContext<MovieHub>>();

            var controller = new AdminController(
                movieRepo.Object,
                bookingRepo.Object,
                hub.Object
            );

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddMovie_Redirects_To_Index()
        {
            var movieRepo = new Mock<IMovieRepository>();
            var bookingRepo = new Mock<IBookingRepository>();

            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    default))
                .Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<MovieHub>>();
            hub.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new AdminController(
                movieRepo.Object,
                bookingRepo.Object,
                hub.Object
            );

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AddMovie("Avatar", "Sci-Fi", 500, "url", 10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            mockClientProxy.Verify(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default), Times.Once);
        }
    
        [Fact]
        public async Task AddMovie_InvalidData_Returns_ViewWithError()
        {
            var movieRepo = new Mock<IMovieRepository>();
            var bookingRepo = new Mock<IBookingRepository>();

            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                           .Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<MovieHub>>();
            hub.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new AdminController(movieRepo.Object, bookingRepo.Object, hub.Object);
            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AddMovie("", "Genre", 100, "url", 5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async Task AddMovie_RepositoryThrows_ExceptionHandled()
        {
            var movieRepo = new Mock<IMovieRepository>();
            movieRepo.Setup(m => m.AddAsync(It.IsAny<Movie>()))
                     .ThrowsAsync(new Exception("Database error"));

            var bookingRepo = new Mock<IBookingRepository>();

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<MovieHub>>();
            hub.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new AdminController(movieRepo.Object, bookingRepo.Object, hub.Object);
            controller.TempData = new Mock<ITempDataDictionary>().Object;

            await Assert.ThrowsAsync<Exception>(() => controller.AddMovie("Avatar", "Sci-Fi", 500, "url", 10));
        }


    }

}
