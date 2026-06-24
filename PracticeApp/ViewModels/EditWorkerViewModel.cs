using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;

namespace PracticeApp.ViewModels
{
    public partial class EditWorkerViewModel : ObservableObject
    {
        private readonly IWorkerService _workerService;

        
        private int _idWorker;

        [ObservableProperty]
        private string _fullName;

        [ObservableProperty]
        private string _specialty;

        [ObservableProperty]
        private int _grade;

        [ObservableProperty]
        private decimal _tariffRate;

        public Action CloseAction { get; set; }

        public EditWorkerViewModel(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        
        public void Initialize(Worker selectedWorker)
        {
            _idWorker = selectedWorker.IdWorker;
            FullName = selectedWorker.FullName;
            Specialty = selectedWorker.Specialty;
            Grade = selectedWorker.Grade;
            TariffRate = selectedWorker.TariffRate;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                var updatedWorker = new Worker
                {
                    IdWorker = _idWorker, 
                    FullName = FullName,
                    Specialty = Specialty,
                    Grade = Grade,
                    TariffRate = TariffRate
                };

               
                _workerService.UpdateWorker(updatedWorker);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}