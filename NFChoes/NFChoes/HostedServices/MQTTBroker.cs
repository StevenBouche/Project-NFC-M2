using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private IMqttServer _mqttServer;
        private readonly NfcProxyHub _proxy;
        //Actions
        private readonly Dictionary<string, Action<MqttApplicationMessageReceivedEventArgs>> _topicActions;


        public MQTTBroker(ILogger<MQTTBroker> logger, NfcProxyHub proxy)
        {
            _mqttServer = new MqttFactory().CreateMqttServer();
            _logger = logger;
            _topicActions = new Dictionary<string, Action<MqttApplicationMessageReceivedEventArgs>>();
            _proxy = proxy; 
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
                data.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                await _proxy.OnReceivedMessage(data);

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
