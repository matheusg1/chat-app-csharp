using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Chat.App.Hubs
{
    public class ChatHub : Hub
    {
        public static ConcurrentDictionary<string, string> Users { get; set; } = new ConcurrentDictionary<string, string>();

        //Envia mensagem geral
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            //await Clients.User("Usuario").SendAsync("ReceiveMessage", user, message);
        }

        //Envia mensagem p/usuário com cópia para o admin
        public async Task SendToUserWithAdmin(string senderName, string receiverName, string adminName, string message)
        {
            // Adicione o usuário e o administrador a um grupo     
            if (Users.TryGetValue(senderName, out var senderConnection) &&
                Users.TryGetValue(receiverName, out var receiverConnection) &&
                Users.TryGetValue(adminName, out var adminConnection))
            {
                await Groups.AddToGroupAsync(receiverConnection, senderConnection);
                await Groups.AddToGroupAsync(adminConnection, senderConnection);
                await Groups.AddToGroupAsync(senderConnection, senderConnection);

                // Envie a mensagem para o grupo
                await Clients.Group(senderConnection).SendAsync("ReceiveMessage", senderName, message);

                // Remova o usuário e o administrador do grupo
                await Groups.RemoveFromGroupAsync(receiverConnection, senderConnection);
                await Groups.RemoveFromGroupAsync(adminConnection, senderConnection);
                await Groups.RemoveFromGroupAsync(senderConnection, senderConnection);
            }

        }

        public void AddUser(string user, string connectionId)
        {            
            Users.AddOrUpdate(user, connectionId, (key, existingValue) => connectionId);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}