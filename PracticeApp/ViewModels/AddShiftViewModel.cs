using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class AddShiftViewModel : ObservableObject
    {
        private readonly IShiftService _shiftService;

        [ObservableProperty] private int _shiftNumber;
        [ObservableProperty] private string _foreman;
        public Action CloseAction { get; set; }

        public AddShiftViewModel(IShiftService shiftService) { _shiftService = shiftService; }

        [RelayCommand]
        private void Save()
        {

            if (ShiftNumber <= 0)
            {
                MessageBox.Show("Номер смены должен быть положительным числом (1, 2, 3 и т.д.)!", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            if (string.IsNullOrWhiteSpace(Foreman))
            {
                MessageBox.Show("Поле 'ФИО Бригадира' не может быть пустым!", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newShift = new Shift
                {
                    ShiftNumber = ShiftNumber,
                    Foreman = Foreman
                };

                _shiftService.AddShift(newShift);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}