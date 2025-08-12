using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurierManagement.View.Work;
using System.Windows.Controls;
using System.Windows;

namespace CurierManagement.ViewModel
{
    public partial class WorkViewModel : ObservableObject
    {
        [ObservableProperty]
        private Page? currentPage;

        [RelayCommand]
        private void OpenCourierEdit()
        {
            CurrentPage = new CourieEdit();
        }

        [RelayCommand]
        private void OpenOrderDistribution()
        {
            MessageBox.Show("Розподіл замовлень - в розробці", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void OpenOrderCreation()
        {
            MessageBox.Show("Створення замовлень - в розробці", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}