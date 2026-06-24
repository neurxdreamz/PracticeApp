using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class EditShiftViewModel : ObservableObject
    {
        private readonly IShiftService _shiftService;

       
        private int _idShift;

        [ObservableProperty]
        private int _shiftNumber;

        [ObservableProperty]
        private string _foreman;

        public Action CloseAction { get; set; }

        public EditShiftViewModel(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

       
        public void Initialize(Shift selectedShift)
        {
            _idShift = selectedShift.IdShift;
            ShiftNumber = selectedShift.ShiftNumber;
            Foreman = selectedShift.Foreman;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                var updatedShift = new Shift
                {
                    IdShift = _idShift, 
                    ShiftNumber = ShiftNumber,
                    Foreman = Foreman
                };

                _shiftService.UpdateShift(updatedShift);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}