using NFChoes.HostedServices;
using NFChoes.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddSignalR();
builder.Services.AddTransient<NfcProxyHub>();
builder.Services.AddTransient<NfcStoreProxyHub>();
builder.Services.AddHostedService<MQTTBroker>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NfcHub>("/userhub");
    endpoints.MapHub<NfcStoreHub>("/storehub");
});

app.Run();
