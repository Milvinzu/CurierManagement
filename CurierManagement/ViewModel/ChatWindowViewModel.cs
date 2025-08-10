using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurierManagement.Model;
using CurierManagement.Service;
using Microsoft.AspNetCore.SignalR.Client;

namespace CurierManagement.ViewModels
{
    public partial class ChatWindowViewModel : ObservableObject
    {
        private readonly ISignalRConnection _connection;
        private readonly string _userName;
        private readonly bool _isPrimaryMode;

        [ObservableProperty]
        private string _messageText = "";

        [ObservableProperty]
        private ObservableCollection<ChatMessage> _messages = new();

        [ObservableProperty]
        private string _connectionStatus = "";

        [ObservableProperty]
        private Brush _statusColor = Brushes.Green;

        public string UserInfoText => $"Користувач: {_userName} | Режим: {(_isPrimaryMode ? "Головний ПК" : "Другорядний ПК")}";

        public ChatWindowViewModel(ISignalRConnection connection, string userName, bool isPrimaryMode)
        {
            _connection = connection;
            _userName = userName;
            _isPrimaryMode = isPrimaryMode;

            ConnectionStatus = "Підключено";
            StatusColor = Brushes.Green;

            InitializeHandlers();
            _ = JoinChatAsync();
        }

        private void InitializeHandlers()
        {
            if (_connection != null)
            {
                _connection.MessageReceived += (type, user, message) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        HandleMessage(type, user, message);
                    });
                };
            }
        }

        private void HandleMessage(string type, string user, string message)
        {
            switch (type)
            {
                case "ReceiveMessage":
                    var isCurrentUser = user == _userName;
                    Messages.Add(new ChatMessage
                    {
                        Text = message,
                        Header = $"{user} - {DateTime.Now.ToString("HH:mm:ss")}",
                        IsCurrentUser = isCurrentUser,
                        BackgroundColor = isCurrentUser ? "#E3F2FD" : "#F5F5F5",
                        Alignment = isCurrentUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
                    });
                    break;

                case "UserJoined":
                    if (user != _userName)
                    {
                        Messages.Add(new ChatMessage
                        {
                            Text = $"{user} приєднався до чату",
                            Header = $"Система - {DateTime.Now.ToString("HH:mm:ss")}",
                            IsCurrentUser = false,
                            BackgroundColor = "#E8F5E8",
                            Alignment = HorizontalAlignment.Center
                        });
                    }
                    break;

                case "UserLeft":
                    Messages.Add(new ChatMessage
                    {
                        Text = $"{user} покинув чат",
                        Header = $"Система - {DateTime.Now.ToString("HH:mm:ss")}",
                        IsCurrentUser = false,
                        BackgroundColor = "#FFEBEE",
                        Alignment = HorizontalAlignment.Center
                    });
                    break;
            }
        }

        [RelayCommand]
        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(MessageText))
                return;

            try
            {
                if (_connection != null)
                {
                    await _connection.SendMessageAsync("ReceiveMessage", _userName, MessageText);
                }

                MessageText = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправки повідомлення: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ClearChat()
        {
            Messages.Clear();
        }

        [RelayCommand]
        private async Task BackToSettingsAsync()
        {
            try
            {
                // Повідомляємо про вихід з чату
                if (_connection != null)
                {
                    await _connection.SendMessageAsync("UserLeft", _userName, "");
                }
            }
            catch { }

            // Повертаємося до головного вікна
            var mainWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.GetType().Name == "MainWindow");
            if (mainWindow != null)
            {
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                // Закриваємо чат вікно
                var chatWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.GetType().Name == "ChatWindow");
                chatWindow?.Close();
            }
        }

        private async Task JoinChatAsync()
        {
            try
            {
                if (_connection != null)
                {
                    await _connection.SendMessageAsync("UserJoined", _userName, "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка входу в чат: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}