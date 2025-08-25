using Microsoft.AspNetCore.SignalR;
using appacd.Models;

namespace appacd.Hubs
{
    public class NotifikasiHub : Hub
    {
        public async Task KirimNotifikasi(string pesan)
        {
            await Clients.All.SendAsync("TerimaNotifikasi", pesan);
        }

        public async Task StatusTrackingOrder(StatusRequest request)
        {
            // sekarang bisa kirim ke email (UserId dari CustomUserIdProvider)
            await Clients.User(request.userId).SendAsync("UpdateStatusTrackingOrder", request.Pesan);
        }

        

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("=== SignalR Connected ===");
            Console.WriteLine($"ConnId: {Context.ConnectionId}");
            Console.WriteLine($"UserIdentifier: {Context.UserIdentifier}");

            foreach (var claim in Context.User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"❌ Disconnected: ConnId={Context.ConnectionId}, UserId={Context.UserIdentifier}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
