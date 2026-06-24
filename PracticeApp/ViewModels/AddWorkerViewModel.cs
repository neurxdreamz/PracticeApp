using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class AddWorkerViewModel : ObservableObject
    {
        private readonly IWorkerService _workerService;

      
        [ObservableProperty]
        private string _fullName;

        [ObservableProperty]
        private string _specialty;

        [ObservableProperty]
        private int _rank;

        [ObservableProperty]
        private decimal _tariffRate;

        
        public Action CloseAction { get; set; }

        public AddWorkerViewModel(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                var newWorker = new Worker
                {
                    FullName = FullName,
                    Specialty = Specialty,
                    Grade = Rank,
                    TariffRate = TariffRate
                };

               
                _workerService.AddWorker(newWorker);

               
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}