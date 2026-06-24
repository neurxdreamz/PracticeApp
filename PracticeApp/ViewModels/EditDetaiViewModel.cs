using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PracticeApp.ViewModels
{
    public partial class EditDetailViewModel : ObservableObject
    {
        private readonly IDetailService _detailService;
        private readonly ISectorService _sectorService;
        private readonly IWorkerService _workerService;
        private readonly IShiftService _shiftService;

        
        private int _idRecord;

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

        [ObservableProperty]
        private ObservableCollection<Sector> _sectors;

        [ObservableProperty]
        private ObservableCollection<Worker> _workers;

        [ObservableProperty]
        private ObservableCollection<Shift> _shifts;

        [ObservableProperty]
        private int _timeNorm;

        [ObservableProperty]
        private DateTime _manufactureDate;

        public Action CloseAction { get; set; }

        public EditDetailViewModel(
            IDetailService detailService,
            ISectorService sectorService,
            IWorkerService workerService,
            IShiftService shiftService)
        {
            _detailService = detailService;
            _sectorService = sectorService;
            _workerService = workerService;
            _shiftService = shiftService;

            LoadDropdownData();
        }

        private void LoadDropdownData()
        {
            Sectors = new ObservableCollection<Sector>(_sectorService.GetAllSectors());
            Workers = new ObservableCollection<Worker>(_workerService.GetAllWorkers());
            Shifts = new ObservableCollection<Shift>(_shiftService.GetAllShifts());
        }

        
        public void Initialize(Detail selectedDetail)
        {
            _idRecord = selectedDetail.IdRecord;
            DetailName = selectedDetail.DetailName;
            BatchVolume = selectedDetail.BatchVolume;
            SectorId = selectedDetail.SectorId;
            WorkerId = selectedDetail.WorkerId;
            ShiftId = selectedDetail.ShiftId;
            TimeNorm = selectedDetail.TimeNorm;
            ManufactureDate = selectedDetail.ManufactureDate;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                var updatedDetail = new Detail
                {
                    IdRecord = _idRecord, 
                    DetailName = DetailName,
                    BatchVolume = BatchVolume,
                    SectorId = SectorId,
                    WorkerId = WorkerId,
                    ShiftId = ShiftId,
                    TimeNorm = TimeNorm,
                    ManufactureDate = ManufactureDate
                };

              
                _detailService.UpdateDetail(updatedDetail);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}