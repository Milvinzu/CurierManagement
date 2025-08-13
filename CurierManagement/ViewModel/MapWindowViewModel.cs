using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CurierManagement.ViewModel
{
    public partial class MapWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string mapUrl = string.Empty;

        [ObservableProperty]
        private bool hasValidUrl = false;

        public event EventHandler<string>? MapUrlSelected;

        public MapWindowViewModel(string existingUrl = "")
        {
            if (!string.IsNullOrEmpty(existingUrl))
            {
                MapUrl = existingUrl;
            }
            else
            {
                MapUrl = "https://www.google.com/maps/place/Kharkiv,+Kharkiv+Oblast,+Ukraine/@49.9935,36.2304,13z";
            }
        }

        partial void OnMapUrlChanged(string value)
        {
            HasValidUrl = !string.IsNullOrWhiteSpace(value) &&
                         (value.Contains("google.com/maps") || value.Contains("maps.google.com"));
        }

        [RelayCommand]
        private void SaveMapUrl()
        {
            if (HasValidUrl)
            {
                MapUrlSelected?.Invoke(this, MapUrl);
                CloseWindow();
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректне посилання на Google Maps", "Увага",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}