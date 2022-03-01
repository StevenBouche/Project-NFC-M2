using Microsoft.AspNetCore.SignalR;
using NFChoes.Dto;

namespace NFChoes.Hubs
{
    public class NfcStoreProxyHub
    {

        private readonly IHubContext<NfcStoreHub> _hub;

        public NfcStoreProxyHub(IHubContext<NfcStoreHub> hub)
        {
            _hub = hub;
        }

        public Task OnReceivedMessage(List<NFCHistory> message, string storeId)
        {
            return _hub.Clients.Group(storeId).SendAsync("ReceivedMessage", message);
        }
    }
}
