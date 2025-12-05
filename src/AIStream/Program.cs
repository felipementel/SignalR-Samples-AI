using AIStreaming;
using AIStreaming.Hubs;

var builder = WebApplication.CreateBuilder(args);

var azureSignalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];

// Configure SignalR with Azure SignalR Service
if (!string.IsNullOrEmpty(azureSignalRConnectionString))
{
    // Usar Azure SignalR Service
    builder.Services.AddSignalR(configure =>
    {
        configure.EnableDetailedErrors = true;
        configure.HandshakeTimeout = TimeSpan.FromSeconds(5);
    })
    .AddAzureSignalR(options =>
    {
        options.ConnectionString = azureSignalRConnectionString;
    });
}
else
{
    // Usar SignalR local (desenvolvimento)
    builder.Services.AddSignalR(configure =>
    {
        configure.EnableDetailedErrors = true;
        configure.HandshakeTimeout = TimeSpan.FromSeconds(5);
    });
}

builder.Services
    .AddSingleton<GroupAccessor>()
    .AddSingleton<GroupHistoryStore>()
    //.AddOpenAI(builder.Configuration) // If you want to user OpenAI, uncomment here and comment the line below
    .AddAzureOpenAI(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<GroupChatHub>("/groupChat");

await app.RunAsync();
