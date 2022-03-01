using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;
using NFChoes.HostedServices;
using NFChoes.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(
                    o =>
                    {
                        o.ListenAnyIP(9000, l => l.UseMqtt()); // MQTT pipeline
                        o.ListenAnyIP(5000);
                        o.ListenAnyIP(8080);
                    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddSignalR();
builder.Services.AddTransient<NfcProxyHub>();
//builder.Services.AddHostedService<MQTTBroker>();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<MQTTBroker>();
builder.Services.AddHostedMqttServerWithServices(options => {
    var s = options.ServiceProvider.GetRequiredService<MQTTBroker>();
    s.ConfigureMqttServerOptions(options);
});
builder.Services.AddMqttConnectionHandler();
builder.Services.AddMqttWebSocketServerAdapter();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "all", builder => {
        builder.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("all");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NfcHub>("/userhub");
    endpoints.MapHub<NfcStoreHub>("/storehub");
    endpoints.MapConnectionHandler<MqttConnectionHandler>(
                "/mqtt",
                httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                                                       protocolList =>
                                                           protocolList.FirstOrDefault() ?? string.Empty);
});

app.UseMqttServer(server => app.Services.GetRequiredService<MQTTBroker>().ConfigureMqttServer(server));

app.Run();
