using Microsoft.Extensions.Caching.Memory;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using Newtonsoft.Json;
using NFChoes.Dto;
using NFChoes.Hubs;

namespace NFChoes.HostedServices
{
    public class MQTTBroker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IMqttServer _mqttServer;
        private readonly NfcProxyHub _proxy;
        private readonly NfcStoreProxyHub _storeProxy;
        //Actions
        private readonly Dictionary<string, Action<MqttApplicationMessageReceivedEventArgs>> _topicActions;

        private readonly IMemoryCache _memoryCache;

        private readonly object _lockObj = new();


        public MQTTBroker(IMemoryCache memoryCache, ILogger<MQTTBroker> logger, NfcProxyHub proxy, NfcStoreProxyHub storeProxy)
        {
            _mqttServer = new MqttFactory().CreateMqttServer();
            _logger = logger;
            _topicActions = new Dictionary<string, Action<MqttApplicationMessageReceivedEventArgs>>();
            _proxy = proxy;
            _storeProxy = storeProxy;
            _memoryCache = memoryCache;
        }

        private void InitHandler()
        {
            _mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e => OnReceiveMessage(e));
            _topicActions.Add("userevent", HandlerUserEvent);
        }

        private async void HandlerUserEvent(MqttApplicationMessageReceivedEventArgs eventMQTT)
        {
            string obj = eventMQTT.ApplicationMessage.ConvertPayloadToString();
            try
            {
                var data = JsonConvert.DeserializeObject<NFCMessage>(obj);
                data!.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                NFCHistory? history;


                lock (_lockObj)
                {
                    if (!_memoryCache.TryGetValue(data.StoreId, out List<NFCHistory> lists))
                    {
                        lists = new List<NFCHistory>();
                        _memoryCache.Set(data.StoreId, lists);
                    }

                   history = lists.SingleOrDefault(user => user.UserId.Equals(data.UserId) && user.OutTimestamp == null);

                    if(history == null)
                    {
                        history = new NFCHistory()
                        {
                            UserId = data.UserId,
                            StoreId = data.StoreId,
                            InTimestamp = data.Timestamp
                        };
                        lists.Add(history);
                    }
                    else
                    {
                        history.OutTimestamp = data.Timestamp;
                    }
                }

                if(history != null) 
                    await _proxy.OnReceivedMessage(history);

                if (!_memoryCache.TryGetValue(data.StoreId, out List<NFCHistory> storeList))
                {
                    List<NFCHistory> insideStoreusers = storeList.Where(user => user.OutTimestamp == null).ToList();
                    await _storeProxy.OnReceivedMessage(insideStoreusers, history.StoreId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return;
            }
            
            _logger.LogInformation(obj);
        }

        public void OnReceiveMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            _logger.LogInformation("I have received a message.");
            string topic = e.ApplicationMessage.Topic;

            _topicActions[topic]?.Invoke(e);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_mqttServer.IsStarted)
                return Task.CompletedTask;

            InitHandler();

            var options = new MqttServerOptionsBuilder()
               .WithConnectionBacklog(100)
               .WithDefaultEndpointPort(9000);

            _logger.LogInformation("MQTT broker startup.");

            return _mqttServer.StartAsync(options.Build());
        }
    }
}
