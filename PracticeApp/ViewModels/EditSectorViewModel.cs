using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class EditSectorViewModel : ObservableObject
    {
        private readonly ISectorService _sectorService;
        private int _idSector;

        [ObservableProperty] private string _sectorName;
        [ObservableProperty] private string _managerFullName;
        public Action CloseAction { get; set; }

        public EditSectorViewModel(ISectorService sectorService) { _sectorService = sectorService; }

        public void Initialize(Sector sector)
        {
            _idSector = sector.IdSector;
            SectorName = sector.SectorName;
            ManagerFullName = sector.ManagerFullName;
        }

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
                _sectorService.UpdateSector(new Sector { IdSector = _idSector, SectorName = SectorName, ManagerFullName = ManagerFullName });
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}