using Microsoft.AspNetCore.SignalR;
namespace MovieBooking.Hubs
{
    public class MovieHub:Hub
    {
        public async Task TicketUpdate(int movieId, int ticketsLeft)
        {
            await Clients.All.SendAsync("ReceiveTicketUpdate",movieId,ticketsLeft);
        }
        public async Task PriceUpdate(int movieId, decimal newPrice)
        {
            await Clients.All.SendAsync("ReceivePriceUpdate",movieId,newPrice);
        }
    }
}
