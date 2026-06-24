using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class AddSectorViewModel : ObservableObject
    {
        private readonly ISectorService _sectorService;

        [ObservableProperty] private string _sectorName;
        [ObservableProperty] private string _managerFullName;
        public Action CloseAction { get; set; }

        public AddSectorViewModel(ISectorService sectorService) { _sectorService = sectorService; }

        [RelayCommand]
        private void Save()
        {

            if (string.IsNullOrWhiteSpace(SectorName))
            {
                MessageBox.Show("Поле 'Название участка' не может быть пустым!", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }


            
            if (string.IsNullOrWhiteSpace(ManagerFullName))
            {
                MessageBox.Show("Поле 'ФИО Начальника' не может быть пустым!", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                _sectorService.AddSector(new Sector { SectorName = SectorName, ManagerFullName = ManagerFullName });
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}