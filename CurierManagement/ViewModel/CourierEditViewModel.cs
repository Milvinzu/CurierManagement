using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurierManagement.DataBase.Data_Service;
using CurierManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CurierManagement.ViewModel
{
    public partial class CourierEditViewModel : ObservableObject
    {
        private readonly CourierRepository _courierRepository;

        public CourierEditViewModel(CourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
            Couriers = new ObservableCollection<Courier>();
            LoadCouriers();
        }

        [ObservableProperty]
        private ObservableCollection<Courier> couriers;

        [ObservableProperty]
        private Courier? selectedCourier;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private decimal hourlyRate;

        [ObservableProperty]
        private decimal ratePerKm;

        [ObservableProperty]
        private decimal ratePerOrder;

        [ObservableProperty]
        private bool isEditing = false;

        partial void OnSelectedCourierChanged(Courier? value)
        {
            if (value != null)
            {
                Name = value.Name;
                HourlyRate = value.HourlyRate;
                RatePerKm = value.RatePerKm;
                RatePerOrder = value.RatePerOrder;
                IsEditing = true;
            }
        }

        [RelayCommand]
        private async Task AddOrUpdateCourier()
        {
            

            try
            {
                if (IsEditing && SelectedCourier != null)
                {
                    SelectedCourier.Name = Name;
                    SelectedCourier.HourlyRate = HourlyRate;
                    SelectedCourier.RatePerKm = RatePerKm;
                    SelectedCourier.RatePerOrder = RatePerOrder;

                    await _courierRepository.UpdateCourier(SelectedCourier);
                    MessageBox.Show("Кур'єра успішно оновлено!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newCourier = new Courier
                    {
                        Name = Name,
                        HourlyRate = HourlyRate,
                        RatePerKm = RatePerKm,
                        RatePerOrder = RatePerOrder
                    };

                    await _courierRepository.AddCourier(newCourier);
                    MessageBox.Show("Кур'єра успішно додано!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ClearForm();
                await LoadCouriers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні кур'єра: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteCourier()
        {
            if (SelectedCourier == null)
            {
                MessageBox.Show("Оберіть кур'єра для видалення", "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Ви впевнені, що хочете видалити кур'єра '{SelectedCourier.Name}'?",
                "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _courierRepository.DeleteCourier(SelectedCourier.Id);
                    MessageBox.Show("Кур'єра успішно видалено!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearForm();
                    await LoadCouriers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні кур'єра: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void ClearForm()
        {
            Name = string.Empty;
            HourlyRate = 0;
            RatePerKm = 0;
            RatePerOrder = 0;
            SelectedCourier = null;
            IsEditing = false;
        }

        [RelayCommand]
        private async Task RefreshCouriers()
        {
            await LoadCouriers();
        }

        private async Task LoadCouriers()
        {
            try
            {
                var couriers = await _courierRepository.GetAllWithPackagesAsync();
                Couriers = new(couriers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні кур'єрів: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
