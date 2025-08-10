using CurierManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace CurierManagement.Service
{
    public interface ISignalRConnection
    {
        event Action<string, string, string> MessageReceived;
        Task SendMessageAsync(string type, string user, string message);
        ValueTask DisposeAsync();
    }

    // Простий SignalR сервер на TCP
    public class SimpleSignalRServer : ISignalRConnection
    {
        private TcpListener _listener;
        private readonly List<TcpClient> _clients = new();
        private readonly string _ip;
        private readonly int _port;
        private CancellationTokenSource _cancellationTokenSource;

        public event Action<string, string, string> MessageReceived;

        public SimpleSignalRServer(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Parse(_ip), _port);
            _listener.Start();

            _cancellationTokenSource = new CancellationTokenSource();

            // Приймаємо клієнтів в окремому потоці
            _ = Task.Run(async () => await AcceptClientsAsync(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _listener?.Stop();

            foreach (var client in _clients.ToList())
            {
                client?.Close();
            }
            _clients.Clear();
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _clients.Add(client);

                    // Обробляємо повідомлення від клієнта
                    _ = Task.Run(async () => await HandleClientAsync(client, cancellationToken));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested && client.Connected)
            {
                try
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        var messageData = JsonSerializer.Deserialize<MessageData>(message);

                        // Розсилаємо повідомлення всім клієнтам
                        await BroadcastMessageAsync(messageData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                    break;
                }
            }

            _clients.Remove(client);
            client.Close();
        }

        public async Task BroadcastMessageAsync(MessageData messageData)
        {
            var json = JsonSerializer.Serialize(messageData);
            var data = Encoding.UTF8.GetBytes(json);

            var clientsToRemove = new List<TcpClient>();

            foreach (var client in _clients.ToList())
            {
                try
                {
                    if (client.Connected)
                    {
                        await client.GetStream().WriteAsync(data, 0, data.Length);
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }
                catch
                {
                    clientsToRemove.Add(client);
                }
            }

            // Видаляємо відключених клієнтів
            foreach (var client in clientsToRemove)
            {
                _clients.Remove(client);
                client.Close();
            }

            // Викликаємо подію для сервера (щоб він також отримав повідомлення)
            MessageReceived?.Invoke(messageData.Type, messageData.User, messageData.Message);
        }

        public async Task SendMessageAsync(string type, string user, string message)
        {
            var messageData = new MessageData
            {
                Type = type,
                User = user,
                Message = message,
                Time = DateTime.Now.ToString("HH:mm:ss")
            };

            await BroadcastMessageAsync(messageData);
        }

        public ValueTask DisposeAsync()
        {
            Stop();
            return ValueTask.CompletedTask;
        }
    }
    
}
