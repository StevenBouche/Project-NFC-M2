// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

string Id = "cringe";
HubConnection connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:8080/userhub?idUser={Id}")
                .Build();

connection.Closed += async (error) => {
    Console.WriteLine("errooooooooooor");
    await Task.Delay(new Random().Next(0, 5) * 1000);
    await connection.StartAsync();
};

connection.On<UserDTO>("ReceivedMessage", (user) => {
    Console.WriteLine(JsonConvert.SerializeObject(user));
});

connection.StartAsync();

class UserDTO
{
    public string UserId { get; set; } = "";
    public string StoreId { get; set; } = "";
    public long Timestamp { get; set; }
}

