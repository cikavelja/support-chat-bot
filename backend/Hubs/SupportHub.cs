using Microsoft.AspNetCore.SignalR;

public class SupportHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}