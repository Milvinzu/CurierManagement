using CurierManagement.Model;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurierManagement.Service
{

    //public class SimpleSignalRClient : ISignalRConnection
    //{
    //    private TcpClient _client;
    //    private NetworkStream _stream;
    //    private readonly string _ip;
    //    private readonly int _port;
    //    private CancellationTokenSource _cancellationTokenSource;

    //    public event Action<string, string, string> MessageReceived;

    //    public SimpleSignalRClient(string ip, int port)
    //    {
    //        _ip = ip;
    //        _port = port;
    //    }

    //    public async Task ConnectAsync()
    //    {
    //        _client = new TcpClient();
    //        await _client.ConnectAsync(_ip, _port);
    //        _stream = _client.GetStream();

    //        _cancellationTokenSource = new CancellationTokenSource();

    //        _ = Task.Run(async () => await ListenForMessagesAsync(_cancellationTokenSource.Token));
    //    }

    //    public async Task SendMessageAsync(string type, string user, string message)
    //    {
    //        var messageData = new MessageData
    //        {
    //            Type = type,
    //            User = user,
    //            Message = message,
    //            Time = DateTime.Now.ToString("HH:mm:ss")
    //        };

    //        var json = JsonSerializer.Serialize(messageData);
    //        var data = Encoding.UTF8.GetBytes(json);

    //        if (_stream != null && _client.Connected)
    //        {
    //            await _stream.WriteAsync(data, 0, data.Length);
    //        }
    //    }

    //    private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
    //    {
    //        var buffer = new byte[4096];

    //        while (!cancellationToken.IsCancellationRequested && _client?.Connected == true)
    //        {
    //            try
    //            {
    //                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
    //                if (bytesRead > 0)
    //                {
    //                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //                    var messageData = JsonSerializer.Deserialize<MessageData>(message);

    //                    MessageReceived?.Invoke(messageData.Type, messageData.User, messageData.Message);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine($"Error listening for messages: {ex.Message}");
    //                break;
    //            }
    //        }
    //    }

    //    public ValueTask DisposeAsync()
    //    {
    //        _cancellationTokenSource?.Cancel();
    //        _stream?.Close();
    //        _client?.Close();
    //        return ValueTask.CompletedTask;
    //    }
    //}
}
