using Microsoft.AspNetCore.SignalR;

namespace Chat.App
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            //await Clients.User("Usuario").SendAsync("ReceiveMessage", user, message);
        }
    }
}
