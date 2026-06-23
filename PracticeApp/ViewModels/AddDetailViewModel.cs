using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PracticeApp.ViewModels
{
    public partial class AddDetailViewModel : ObservableObject
    {
        private readonly IDetailService _detailService;

        
        [ObservableProperty]
        private string _detailName;

        [ObservableProperty]
        private int _batchVolume;

      
        [ObservableProperty]
        private int _sectorId;

        [ObservableProperty]
        private int _workerId;

        [ObservableProperty]
        private int _shiftId;

     
        public Action CloseAction { get; set; }

        public AddDetailViewModel(IDetailService detailService)
        {
            _detailService = detailService;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                var newDetail = new Detail
                {
                    DetailName = DetailName,
                    BatchVolume = BatchVolume,
                    SectorId = SectorId,
                    WorkerId = WorkerId,
                    ShiftId = ShiftId
                };

              
                _detailService.AddDetail(newDetail);

            
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
               
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}