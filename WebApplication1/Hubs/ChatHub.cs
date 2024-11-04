using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebApplication1.Hubs
{
/*    [Authorize]
*/    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> _userConnectionMapping = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            _userConnectionMapping[username] = Context.ConnectionId;
            await SetUpName(Context.ConnectionId);
            await base.OnConnectedAsync();
        }


        public async Task SendMessageToUser(string receiver, string message)
        {
            try
            {
                var sender = Context.User.Identity.Name;
                var receiverConnectionId = _userConnectionMapping.GetValueOrDefault(receiver);

                if (!string.IsNullOrEmpty(receiverConnectionId))
                {
                    string groupName = GetGroupName(sender, receiver);

                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    await Groups.AddToGroupAsync(receiverConnectionId, groupName);

                    await Clients.Client(receiverConnectionId).SendAsync("CreateChat", receiver, groupName, sender);


                    await Clients.Group(groupName).SendAsync("ReceiveMessage", sender, message);
                }
                else
                {
                    Console.WriteLine($"Receiver connection ID not found for user: {receiver}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.Identity.Name;
            _userConnectionMapping.TryRemove(username, out _);
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string user1, string user2)
        {
            var stringCompare = string.CompareOrdinal(user1, user2) < 0;
            return stringCompare ? $"{user1}-{user2}" : $"{user2}-{user1}";
        }

        // Новый метод для получения идентификатора текущего пользователя
        public Task<string> GetCurrentUserId()
        {
            return Task.FromResult(Context.User.Identity.Name);
        }

        // Новый метод для получения идентификатора текущего пользователя
        public async Task SetUpName(string receiverConnectionId)
        {
            await Clients.Client(receiverConnectionId).SendAsync("SetUpName", Context.User.Identity.Name);
        }
    }




}
