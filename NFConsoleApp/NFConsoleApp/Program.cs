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
    var message = user.OutTimestamp == null ?
        $"I just walked into the store : {user.StoreId} at {DateTimeOffset.FromUnixTimeMilliseconds(user.InTimestamp)}" :
        $"I just left the store : {user.StoreId} at {DateTimeOffset.FromUnixTimeMilliseconds((long)user.OutTimestamp)}";
    Console.WriteLine(JsonConvert.SerializeObject(user));
    Console.WriteLine(message);
});

await connection.StartAsync();

Console.WriteLine("Enter help too give all commands.");

while (true)
{
    var line = ReadLine();
    Action? action = line switch
    {
        "help" => () => {
            Console.WriteLine("- history : Write history to get all your movements");
            Console.WriteLine("- exit : Stop command line");
        },
        "history" => async () =>
        {
            var list = await connection.InvokeAsync<List<UserDTO>>("MyHistory");
            list.ForEach(x => Console.WriteLine(JsonConvert.SerializeObject(x)));
        } ,
        "exit" => null,
        _ => () => { }
    };

    if (action == null)
        break;

    action?.Invoke();
}


await connection.StopAsync();

Environment.Exit(0);

static string? ReadLine()
{
    Console.Write(">");
    return Console.ReadLine();
}

class UserDTO
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;
    [JsonProperty("storeId")]
    public string StoreId { get; set; } = string.Empty;
    [JsonProperty("inTimestamp")]
    public long InTimestamp { get; set; }
    [JsonProperty("outTimestamp")]
    public long? OutTimestamp { get; set; } = null;
}

