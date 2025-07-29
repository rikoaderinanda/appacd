using Microsoft.AspNetCore.SignalR;

namespace appacd.Hubs
{
    public class NotifikasiHub : Hub
    {
        public async Task KirimNotifikasi(string pesan)
        {
            await Clients.All.SendAsync("TerimaNotifikasi", pesan);
        }
    }
}