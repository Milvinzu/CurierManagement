using CurierManagement.ViewModel;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CurierManagement.View.Work
{
    public partial class MapWindow : Window
    {
        private readonly MapWindowViewModel _viewModel;
        public event EventHandler<string>? MapUrlSelected;

        public MapWindow(string existingUrl = "")
        {
            InitializeComponent();
            _viewModel = new MapWindowViewModel(existingUrl);
            DataContext = _viewModel;
            _viewModel.MapUrlSelected += OnMapUrlSelected;
            Loaded += SimpleMapWindow_Loaded;
        }

        private async void SimpleMapWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await MapWebView.EnsureCoreWebView2Async();
                MapWebView.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                MapWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

                if (!string.IsNullOrEmpty(_viewModel.MapUrl) && _viewModel.MapUrl != "https://www.google.com/maps/place/Kharkiv,+Kharkiv+Oblast,+Ukraine/@49.9935,36.2304,13z")
                {
                    MapWebView.CoreWebView2.Navigate(_viewModel.MapUrl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації WebView: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _viewModel.MapUrl = MapWebView.CoreWebView2.Source;
            }
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            if (MapWebView.CoreWebView2 != null)
            {
                _viewModel.MapUrl = MapWebView.CoreWebView2.Source;
            }
        }

        private void OnMapUrlSelected(object? sender, string mapUrl)
        {
            MapUrlSelected?.Invoke(this, mapUrl);
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.MapUrlSelected -= OnMapUrlSelected;
            base.OnClosed(e);
        }
    }
}