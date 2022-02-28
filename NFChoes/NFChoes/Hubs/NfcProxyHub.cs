using Microsoft.AspNetCore.SignalR;
using NFChoes.Dto;

namespace NFChoes.Hubs
{
    public class NfcProxyHub
    {

        private readonly IHubContext<NfcHub> _hub;

        public NfcProxyHub(IHubContext<NfcHub> hub)
        {
            _hub = hub;
        }

        public Task OnReceivedMessage(NFCMessage message)
        {
            return _hub.Clients.Group(message.UserId).SendAsync("ReceivedMessage", message);
        }

        

    }
}
