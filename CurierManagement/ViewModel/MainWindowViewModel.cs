using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurierManagement.View.Work;
using CurierManagement.Service;

namespace CurierManagement.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private ISignalRConnection _connection;
        private SimpleSignalRServer _server;
        private SimpleSignalRClient _client;

        [ObservableProperty]
        private string _userName = "";

        [ObservableProperty]
        private bool _isPrimaryMode = true;

        [ObservableProperty]
        private bool _isSecondaryMode = false;

        [ObservableProperty]
        private string _serverIP = "";

        [ObservableProperty]
        private string _serverPort = "5000";

        [ObservableProperty]
        private string _targetServerIP = "";

        [ObservableProperty]
        private string _targetServerPort = "5000";

        [ObservableProperty]
        private string _connectionStatusText = "Не підключено";

        [ObservableProperty]
        private Brush _connectionStatusColor = Brushes.Red;

        [ObservableProperty]
        private bool _isConnected = false;

        [ObservableProperty]
        private bool _canGoToWork = false;

        public string ConnectionInfoHeader => IsPrimaryMode ? "Інформація сервера" : "Підключення до сервера";
        public Visibility PrimaryModeVisibility => IsPrimaryMode ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SecondaryModeVisibility => IsSecondaryMode ? Visibility.Visible : Visibility.Collapsed;
        public string StartButtonText => IsPrimaryMode ? "Запустити сервер" : "Підключитися";

        public MainWindowViewModel()
        {
            ServerIP = GetLocalIPAddress();

            // Підписка на зміни режиму
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsPrimaryMode))
            {
                IsSecondaryMode = !IsPrimaryMode;
                OnPropertyChanged(nameof(ConnectionInfoHeader));
                OnPropertyChanged(nameof(PrimaryModeVisibility));
                OnPropertyChanged(nameof(SecondaryModeVisibility));
                OnPropertyChanged(nameof(StartButtonText));
            }
            else if (e.PropertyName == nameof(IsSecondaryMode))
            {
                IsPrimaryMode = !IsSecondaryMode;
            }
        }

        [RelayCommand]
        private async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Будь ласка, введіть ваше ім'я", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsPrimaryMode)
                {
                    await StartServerAsync();
                }
                else
                {
                    await ConnectToServerAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка підключення", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectionStatusText = $"Помилка: {ex.Message}";
                ConnectionStatusColor = Brushes.Red;
            }
        }

        [RelayCommand]
        private async Task StopAsync()
        {
            try
            {
                if (_connection != null)
                {
                    await _connection.DisposeAsync();
                    _connection = null;
                }

                if (_server != null)
                {
                    _server.Stop();
                    _server = null;
                }

                _client = null;

                IsConnected = false;
                CanGoToWork = false;
                ConnectionStatusText = "Не підключено";
                ConnectionStatusColor = Brushes.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при зупинці: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void GoToWork()
        {
            var chatWindow = new ChatWindow();
            var chatViewModel = new ChatWindowViewModel(_connection, UserName, IsPrimaryMode);
            chatWindow.DataContext = chatViewModel;

            // Закриваємо поточне вікно і показуємо чат
            Application.Current.MainWindow = chatWindow;
            var currentWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.GetType().Name == "MainWindow");
            currentWindow?.Hide();
            chatWindow.Show();
        }

        [RelayCommand]
        private void CopyIP()
        {
            try
            {
                Clipboard.SetText(ServerIP);
                MessageBox.Show("IP адресу скопійовано в буфер обміну", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Не вдалося скопіювати IP адресу", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void CopyPort()
        {
            try
            {
                Clipboard.SetText(ServerPort);
                MessageBox.Show("Порт скопійовано в буфер обміну", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Не вдалося скопіювати порт", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task StartServerAsync()
        {
            ConnectionStatusText = "Запуск сервера...";
            ConnectionStatusColor = Brushes.Orange;

            try
            {
                _server = new SimpleSignalRServer(ServerIP, int.Parse(ServerPort));
                await _server.StartAsync();

                _connection = _server;

                IsConnected = true;
                CanGoToWork = true;
                ConnectionStatusText = $"Сервер запущено на {ServerIP}:{ServerPort}";
                ConnectionStatusColor = Brushes.Green;
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка запуску сервера: {ex.Message}");
            }
        }

        private async Task ConnectToServerAsync()
        {
            if (string.IsNullOrWhiteSpace(TargetServerIP) || string.IsNullOrWhiteSpace(TargetServerPort))
            {
                MessageBox.Show("Будь ласка, введіть IP адресу та порт сервера", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ConnectionStatusText = "Підключення до сервера...";
            ConnectionStatusColor = Brushes.Orange;

            try
            {
                _client = new SimpleSignalRClient(TargetServerIP, int.Parse(TargetServerPort));
                await _client.ConnectAsync();

                _connection = _client;

                IsConnected = true;
                CanGoToWork = true;
                ConnectionStatusText = $"Підключено до {TargetServerIP}:{TargetServerPort}";
                ConnectionStatusColor = Brushes.Green;
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка підключення: {ex.Message}");
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }

            return "127.0.0.1";
        }
    }
}