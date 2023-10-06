using Microsoft.AspNetCore.SignalR;

namespace Chat.App.Hubs
{
    public class ChatHub : Hub
    {
        //Envia mensagem geral
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            //await Clients.User("Usuario").SendAsync("ReceiveMessage", user, message);
        }

        //Envia mensagem p/usuário com cópia para o admin
        public async Task SendToUserWithAdmin(string user, string receiverConnectionId, string adminConnectionId, string message)
        {
            // Adicione o usuário e o administrador a um grupo
            await Groups.AddToGroupAsync(receiverConnectionId, user);
            await Groups.AddToGroupAsync(adminConnectionId, user);
            await Groups.AddToGroupAsync(user, user);

            // Envie a mensagem para o grupo
            await Clients.Group(user).SendAsync("ReceiveMessage", user, message);

            // Remova o usuário e o administrador do grupo
            await Groups.RemoveFromGroupAsync(receiverConnectionId, user);
            await Groups.RemoveFromGroupAsync(adminConnectionId, user);
            await Groups.RemoveFromGroupAsync(user, user);
        }

        //public string DefineUserName(string username)
        //{
        //    return username;
        //}
        
        public string GetConnectionId() => Context.ConnectionId;
    }
}