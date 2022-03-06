using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using NFChoes.Dto;

namespace NFChoes.Hubs
{
    public class NfcStoreHub: Hub
    {
        private readonly ILogger<NfcHub> Logger;
        private const string storeId = "storeId";
        private readonly IMemoryCache _memoryCache;

        public NfcStoreHub(ILogger<NfcHub> logger, IMemoryCache memoryCache)
        {
            Logger = logger;
            _memoryCache = memoryCache;
        }

        public override async Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            var idStore = Context.GetHttpContext()?.Request.Query[storeId];

            if (!idStore.HasValue)
            {
                Context.Abort();
                return;
            }

            await PushClientToGroup(id, idStore.Value, Context.ConnectionAborted);

            await base.OnConnectedAsync();

            Logger.LogInformation("Store connected : {id}", idStore);

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.ConnectionId;

            await RemoveClientFromGroups(id);

            await base.OnDisconnectedAsync(exception);

            Logger.LogInformation($"Store disconnected : {id}.");
        }

        public List<NFCHistory> GetBaseList()
        {
            var idEquipment = GetIdUserOfContext();

            if (_memoryCache.TryGetValue(idEquipment + "-store", out List<NFCHistory> storeList))
            {
                return storeList.Where(user => user.OutTimestamp == null).ToList();
            }

            return new List<NFCHistory>();
        }

        private async Task PushClientToGroup(string idClient, string groupname, CancellationToken token)
        {

            await Groups.AddToGroupAsync(idClient, groupname, token);

            Context.Items.Add(storeId, groupname);

            Logger.LogInformation($"Client {idClient} added to group {groupname}.");
        }

        private async Task RemoveClientFromGroups(string storeId)
        {
            var idEquipment = GetIdUserOfContext();

            await Groups.RemoveFromGroupAsync(storeId, idEquipment, Context.ConnectionAborted);

            Logger.LogInformation($"Store {storeId} removed from group {idEquipment}.");
        }

        private string GetIdUserOfContext()
        {
            Context.Items.TryGetValue(storeId, out object? idEquipment);
            if (idEquipment != null && idEquipment is string)
                return idEquipment as string;
            else
                return string.Empty;
        }
    }
}
