using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurierManagement.DataBase.Data_Service;
using CurierManagement.Model;
using CurierManagement.Model.Basic_models;
using CurierManagement.View.Work;
using CurierManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CurierManagement.ViewModel
{
    public partial class CreateOrderViewModel : ObservableObject
    {
        private readonly OrderRepository _orderRepository;

        [ObservableProperty]
        private ObservableCollection<Order> orders = new();

        [ObservableProperty]
        private ObservableCollection<Order> filteredOrders = new();

        [ObservableProperty]
        private Order? selectedOrder;

        [ObservableProperty]
        private bool isEditMode;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string orderNumber = string.Empty;

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private string deliveryAddress = string.Empty;

        [ObservableProperty]
        private int entryway;

        [ObservableProperty]
        private DateTime orderDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan orderTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private decimal totalAmount;

        [ObservableProperty]
        private string createdBy = SessionData.UserName;

        [ObservableProperty]
        private OrderStatus status = OrderStatus.Pending;

        [ObservableProperty]
        private string webAddressInMap = string.Empty;

        [ObservableProperty]
        private bool hasMapLocation = false;

        [ObservableProperty]
        private ICommand orderDoubleClickCommand;

        public CreateOrderViewModel(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            Orders = new ObservableCollection<Order>();
            FilteredOrders = new ObservableCollection<Order>();
            OrderDoubleClickCommand = new RelayCommand<Order>(OnOrderDoubleClick);

            _ = LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetOrdersAsync();

                var sortedOrders = orders
                    .OrderByDescending(o => o.OrderDate.Date == DateTime.Today ? 1 : 0)
                    .ThenBy(o => o.Status == OrderStatus.Pending ? 0 : 1)
                    .ThenByDescending(o => o.OrderDate)
                    .ToList();

                Orders.Clear();
                foreach (var order in sortedOrders)
                {
                    Orders.Add(order);
                }

                FilterOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження замовлень: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void NewOrder()
        {
            ClearForm();
            IsEditMode = false;
        }

        [RelayCommand]
        private void EditOrder()
        {
            if (SelectedOrder != null)
            {
                LoadOrderToForm(SelectedOrder);
                IsEditMode = true;
            }
        }

        private void OnOrderDoubleClick(Order? order)
        {
            if (order != null)
            {
                LoadOrderToForm(order);
                IsEditMode = true;
                SelectedOrder = order;
            }
        }

        private void LoadOrderToForm(Order order)
        {
            OrderNumber = order.OrderNumber;
            CustomerName = order.CustomerName;
            DeliveryAddress = order.DeliveryAddress;
            Entryway = order.Entryway;
            OrderDate = order.OrderDate.Date;
            OrderTime = order.OrderDate.TimeOfDay;
            TotalAmount = order.TotalAmount;
            CreatedBy = order.CreatedBy;
            Status = order.Status;
            WebAddressInMap = order.WebAddressInMap ?? string.Empty;
            HasMapLocation = !string.IsNullOrEmpty(WebAddressInMap);
        }

        [RelayCommand]
        private async Task SaveOrderAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrderNumber))
                {
                    MessageBox.Show("Будь ласка, введіть номер замовлення", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    MessageBox.Show("Будь ласка, заповніть обов'язкові поля (Номер замовлення, Ім'я клієнта та Адреса доставки)", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (IsEditMode && SelectedOrder != null)
                {
                    SelectedOrder.OrderNumber = OrderNumber;
                    SelectedOrder.CustomerName = CustomerName;
                    SelectedOrder.DeliveryAddress = DeliveryAddress;
                    SelectedOrder.Entryway = Entryway;
                    SelectedOrder.OrderDate = OrderDate.Date.Add(OrderTime);
                    SelectedOrder.TotalAmount = TotalAmount;
                    SelectedOrder.CreatedBy = CreatedBy;
                    SelectedOrder.Status = Status;
                    SelectedOrder.WebAddressInMap = WebAddressInMap;

                    await _orderRepository.UpdateOrder(SelectedOrder);
                    MessageBox.Show("Замовлення успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newOrder = new Order
                    {
                        OrderNumber = OrderNumber,
                        CustomerName = CustomerName,
                        DeliveryAddress = DeliveryAddress,
                        Entryway = Entryway,
                        OrderDate = OrderDate.Date.Add(OrderTime),
                        TotalAmount = TotalAmount,
                        Tips = 0,
                        CreatedBy = CreatedBy,
                        Status = Status,
                        WebAddressInMap = WebAddressInMap
                    };

                    await _orderRepository.AddOrder(newOrder);
                    MessageBox.Show("Замовлення успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ClearForm();
                await LoadOrdersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження замовлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteOrderAsync()
        {
            if (SelectedOrder != null)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити замовлення №{SelectedOrder.OrderNumber}?",
                    "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _orderRepository.DeleteOrder(SelectedOrder.Id);
                        MessageBox.Show("Замовлення успішно видалено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadOrdersAsync();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка видалення замовлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        [RelayCommand]
        private void ShowMap()
        {
            var mapWindow = new MapWindow(WebAddressInMap);
            mapWindow.Owner = Application.Current.MainWindow;

            mapWindow.MapUrlSelected += OnMapUrlSelected;

            mapWindow.ShowDialog();
        }

        [RelayCommand]
        private void OpenMapLocation()
        {
            if (!string.IsNullOrEmpty(WebAddressInMap))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = WebAddressInMap,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка відкриття карти: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Локація на карті не обрана", "Увага",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ClearForm();
        }

        private void OnMapUrlSelected(object? sender, string mapUrl)
        {
            if (!string.IsNullOrEmpty(mapUrl))
            {
                WebAddressInMap = mapUrl;
                HasMapLocation = true;

                MessageBox.Show("Локація успішно обрана!", "Успіх",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterOrders();
        }

        private void FilterOrders()
        {
            FilteredOrders.Clear();

            var filtered = Orders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(o =>
                    o.OrderNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    o.DeliveryAddress.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var order in filtered)
            {
                FilteredOrders.Add(order);
            }
        }

        private void ClearForm()
        {
            OrderNumber = string.Empty;
            CustomerName = string.Empty;
            DeliveryAddress = string.Empty;
            Entryway = 0;
            OrderDate = DateTime.Now.Date;
            TotalAmount = 0;
            Status = OrderStatus.Pending;
            WebAddressInMap = string.Empty;
            HasMapLocation = false;
            IsEditMode = false;
            SelectedOrder = null;
        }

        public bool IsOrderFromToday(Order order)
        {
            return order.OrderDate.Date == DateTime.Today;
        }

        public bool IsPendingOrder(Order order)
        {
            return order.Status == OrderStatus.Pending;
        }
    }
}