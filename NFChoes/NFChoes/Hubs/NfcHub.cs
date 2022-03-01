using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using NFChoes.Dto;

namespace NFChoes.Hubs
{
    public class NfcHub: Hub
    {
        private readonly ILogger<NfcHub> Logger;
        private readonly IMemoryCache _cache;
        private const string IdUserKey = "idUser";

        public NfcHub(IMemoryCache cache, ILogger<NfcHub> logger)
        {
            Logger = logger;
            _cache = cache;
        }

        public override async Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            var idUser = Context.GetHttpContext()?.Request.Query[IdUserKey];

            if (!idUser.HasValue)
            {
                Context.Abort();
                return;
            }

            await PushClientToGroup(id, idUser.Value, Context.ConnectionAborted);

            await base.OnConnectedAsync();

            Logger.LogInformation("Client connected : {id}", idUser);

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.ConnectionId;

            await RemoveClientFromGroups(id);

            await base.OnDisconnectedAsync(exception);

            Logger.LogInformation($"Client disconnected : {id}.");
        }

        public List<NFCHistory> MyHistory()
        {
            var idEquipment = GetIdUserOfContext();

            if(idEquipment == null || !_cache.TryGetValue(idEquipment, out List<NFCHistory> userlists))
                return new List<NFCHistory>();

            return userlists;
        }

        private async Task PushClientToGroup(string idClient, string groupname, CancellationToken token)
        {

            await Groups.AddToGroupAsync(idClient, groupname, token);

            Context.Items.Add(IdUserKey, groupname);

            Logger.LogInformation($"Client {idClient} added to group {groupname}.");
        }

        private async Task RemoveClientFromGroups(string idClient)
        {
            var idEquipment = GetIdUserOfContext();

            await Groups.RemoveFromGroupAsync(idClient, idEquipment, Context.ConnectionAborted);

            Logger.LogInformation($"Client {idClient} removed from group {idEquipment}.");
        }

        private string GetIdUserOfContext()
        {
            Context.Items.TryGetValue(IdUserKey, out object? idEquipment);
            if (idEquipment != null && idEquipment is string)
                return idEquipment as string;
            else
                return string.Empty;
        }
    }
}
