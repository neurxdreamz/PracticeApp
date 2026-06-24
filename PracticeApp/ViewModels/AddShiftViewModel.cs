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
            try
            {
                _shiftService.AddShift(new Shift { ShiftNumber = ShiftNumber, Foreman = Foreman });
                CloseAction?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }
}