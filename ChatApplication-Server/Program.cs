using ChatApplication_Server.Models.Context;
using ChatApplication_Server.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.WebSockets;
using System.Text;

public class Program
{
    // Entry point for the application
    static async Task Main(string[] args)
    {
        // Initialize the database context for chat history
        ChatDbContext dbContext = new ChatDbContext();

        // Create a web application builder and set the server URL
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://localhost:6000");
        var app = builder.Build();

        // Enable WebSocket support for the application
        app.UseWebSockets();

        // List to keep track of active WebSocket connections
        var connections = new List<WebSocket>();
        var connectedUsers = new List<string>();

        // Handle WebSocket connections
        app.Map("/ws", async context =>
        {
            // Check if the incoming request is a WebSocket request
            if (context.WebSockets.IsWebSocketRequest)
            {
                // Retrieve the username from the query string
                var userName = context.Request.Query["name"];

                // Accept the WebSocket connection from the client
                using var ws = await context.WebSockets.AcceptWebSocketAsync();

                // Add the user to the connected users list if not already present
                if (!connectedUsers.Contains(userName))
                {
                    connectedUsers.Add(userName);
                }

                // Add the WebSocket connection to the list of active connections
                connections.Add(ws);

                // Load and send previous chat history to the client
                await LoadChatHistory();

                // Broadcast a system message about the new user joining
                await Broadcast($"SystemDisplay(System){userName} joined the room.");
                await Broadcast($"SystemDisplay(System){connections.Count} users connected.");
                await Broadcast($"SystemDisplay(System)To go back, Type showmenu ");
                await Broadcast($"SystemDisplay(System)Type Exit or Press Ctrl+C to close the Application.");

                // Start receiving messages from the client
                await ReceivedMessage(ws, async (result, buffer) =>
                {
                    // Handle the received WebSocket message
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Convert the message to string and check for special commands
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        // If the message is a request for the list of connected users
                        if (message == "getConnectedUsers")
                        {
                            var usersList = string.Join(", ", connectedUsers);
                            var response = $"SystemDisplay(System)ConnectedUserList: {usersList}";
                            await ws.SendAsync(Encoding.UTF8.GetBytes(response), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            // If the message is a normal chat message, save and broadcast it
                            DateTime currentDateTime = DateTime.Now;
                            await SaveMessage(currentDateTime, userName, message);
                            await Broadcast($"{currentDateTime:MM/dd/yy hh:mm:ss tt} ({userName}): {message}");
                        }
                    }
                    // Handle WebSocket closure or aborted state
                    else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                    {
                        // Remove the user and connection from active lists and notify others
                        connections.Remove(ws);
                        connectedUsers.Remove(userName);
                        await Broadcast($"SystemDisplay(System){userName} left the room.");
                        await Broadcast($"SystemDisplay(System){connections.Count} users connected.");

                        // Close the WebSocket connection
                        await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    }
                });
            }
            else
            {
                // If the request is not a WebSocket request, return a BadRequest response
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        });

        // Method to save a chat message to the database
        async Task SaveMessage(DateTime dateTime, string userName, string message)
        {
            ChatHistory chatHistory = new ChatHistory
            {
                DatetimeCreated = dateTime,
                UserName = userName,
                Message = message
            };

            // Add the message to the database and save changes
            await dbContext.ChatHistories.AddAsync(chatHistory);
            await dbContext.SaveChangesAsync();
        }

        // Method to load and send chat history to all connected users
        async Task LoadChatHistory()
        {
            // Retrieve all chat history from the database ordered by timestamp
            List<ChatHistory> chatHistories = await dbContext.ChatHistories.OrderBy(x => x.DatetimeCreated).ToListAsync();

            // Send each message to all connected users
            foreach (ChatHistory chatHistory in chatHistories)
            {
                string datetime = chatHistory.DatetimeCreated.ToString("MM/dd/yy hh:mm:ss tt");
                string username = $" ({chatHistory.UserName}):";
                string message = chatHistory.Message;

                await Broadcast($"{datetime}{username}{message}");
            }
        }

        // Method to receive and process WebSocket messages
        async Task ReceivedMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[4096]; // Buffer for received data

            // Continuously receive messages while the WebSocket is open
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer); // Handle the received message
            }
        }

        // Method to broadcast a message to all connected WebSocket clients
        async Task Broadcast(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message); // Convert message to byte array

            // Send the message to each active WebSocket connection
            foreach (var socket in connections)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        // Run the web application asynchronously
        await app.RunAsync();
    }
}
