using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using Microsoft.Extensions.DependencyInjection;
using PracticeApp.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;

namespace PracticeApp.ViewModels 
{
    
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDetailService _detailService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkerService _workerService;
        private readonly ISectorService _sectorService;
        private readonly IAuthService _authService;
        private readonly IShiftService _shiftService;

        [ObservableProperty]
        private ObservableCollection<Detail> details;

        
        [ObservableProperty]
        private Detail _selectedDetail;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private bool _isEditor;

        [ObservableProperty]
        private ObservableCollection<Worker> _workers;

        [ObservableProperty]
        private ObservableCollection<Sector> _sectors;

        [ObservableProperty]
        private ObservableCollection<Shift> _shifts;

        [ObservableProperty]
        private Worker _selectedWorker;

        [ObservableProperty]
        private int _selectedTableIndex;

        [ObservableProperty]
        private Sector _selectedSector;

        [ObservableProperty]
        private Shift _selectedShift;


        public void SetupAccessRights(int roleId)
        {

            IsAdmin = roleId == 1;

            IsEditor = roleId == 1 || roleId == 3;
        }


        private ICollectionView _detailsView;
        private ICollectionView _workersView;
        private ICollectionView _sectorsView;
        private ICollectionView _shiftsView;


        public MainViewModel(IDetailService detailService, IAuthService authService, IWorkerService workerService, ISectorService sectorService, IShiftService shiftService, IServiceProvider serviceProvider)
        {
            _detailService = detailService;
            _authService = authService;
            _workerService = workerService;
            _sectorService = sectorService;
            _serviceProvider = serviceProvider;
            _shiftService = shiftService;

            LoadDetails();
            LoadDictionaries(); 
        }

        private void LoadDetails()
        {
            var dataFromDb = _detailService.GetAllDetails();
            Details = new ObservableCollection<Detail>(dataFromDb);

           
            _detailsView = CollectionViewSource.GetDefaultView(Details);

            
            _detailsView.Filter = FilterDetails;
        }

        [RelayCommand]
        private void Refresh()
        {
            
            LoadDetails();
        }

        [RelayCommand]
        private void GlobalAdd()
        {
            switch (SelectedTableIndex)
            {
                case 0:
                    var addWindow = _serviceProvider.GetRequiredService<AddDetailWindow>();
                    addWindow.ShowDialog();
                    LoadDetails();
                    break;

                case 1:
                    var addWorkerWindow = _serviceProvider.GetRequiredService<AddWorkerWindow>();
                    addWorkerWindow.ShowDialog();
                    LoadWorkers();
                    break;

                case 2:
                    var addSectorWindow = _serviceProvider.GetRequiredService<AddSectorWindow>();
                    addSectorWindow.ShowDialog();
                    LoadSectors(); 
                    break;

                case 3:
                    var addShiftWindow = _serviceProvider.GetRequiredService<AddShiftWindow>();
                    addShiftWindow.ShowDialog();
                    LoadShifts();
                    break;
            }
        }

        [RelayCommand]
        private void GlobalEdit()
        {
            switch (SelectedTableIndex)
            {
                case 0:
                    if (SelectedDetail == null) { MessageBox.Show("Сначала выберите деталь для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    var editWindow = _serviceProvider.GetRequiredService<EditDetailWindow>();
                    var editViewModel = (EditDetailViewModel)editWindow.DataContext;
                    editViewModel.Initialize(SelectedDetail);
                    editWindow.ShowDialog();
                    LoadDetails();
                    break;

                case 1:
                    if (SelectedWorker == null) { MessageBox.Show("Сначала выберите рабочего для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    var editWorkerWindow = _serviceProvider.GetRequiredService<EditWorkerWindow>();
                    var editWorkerViewModel = (EditWorkerViewModel)editWorkerWindow.DataContext;
                    editWorkerViewModel.Initialize(SelectedWorker);
                    editWorkerWindow.ShowDialog();
                    LoadWorkers(); 
                    break;

                case 2:
                    if (SelectedSector == null) { MessageBox.Show("Выберите участок."); return; }
                    var editSectorWindow = _serviceProvider.GetRequiredService<EditSectorWindow>();
                    var editSectorVM = (EditSectorViewModel)editSectorWindow.DataContext;
                    editSectorVM.Initialize(SelectedSector);
                    editSectorWindow.ShowDialog();
                    LoadSectors(); 
                    break;

                case 3:
                    if (SelectedShift == null) { MessageBox.Show("Выберите смену."); return; }
                    var editShiftWindow = _serviceProvider.GetRequiredService<EditShiftWindow>();
                    var editShiftVM = (EditShiftViewModel)editShiftWindow.DataContext;
                    editShiftVM.Initialize(SelectedShift);
                    editShiftWindow.ShowDialog();
                    LoadShifts(); 
                    break;
            }
        }

        [RelayCommand]
        private void GlobalDelete()
        {
            switch (SelectedTableIndex)
            {
                case 0:
                    if (SelectedDetail == null) { MessageBox.Show("Выберите деталь для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    if (MessageBox.Show($"Удалить деталь '{SelectedDetail.DetailName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _detailService.DeleteDetail(SelectedDetail.IdRecord);
                        LoadDetails();
                    }
                    break;

                case 1:
                    if (SelectedWorker == null) { MessageBox.Show("Выберите рабочего для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    if (MessageBox.Show($"Удалить рабочего '{SelectedWorker.FullName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _workerService.DeleteWorker(SelectedWorker.IdWorker);
                        LoadWorkers(); 
                    }
                    break;

                case 2:
                    if (SelectedSector == null) { MessageBox.Show("Выберите участок для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    if (MessageBox.Show($"Удалить участок '{SelectedSector.SectorName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _sectorService.DeleteSector(SelectedSector.IdSector);
                        LoadSectors();
                    }
                    break;

                case 3:
                    if (SelectedShift == null) { MessageBox.Show("Выберите смену для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                    if (MessageBox.Show($"Удалить смену №{SelectedShift.ShiftNumber}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _shiftService.DeleteShift(SelectedShift.IdShift);
                        LoadShifts(); 
                    }
                    break;
            }
        }

        private bool FilterDetails(object obj)
        {
            
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is Detail detail)
            {
                string search = SearchText.Trim();

              
                bool containsInName = (detail.DetailName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool containsInWorker = (detail.WorkerFullName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool containsInSector = (detail.SectorName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

               
                return containsInName || containsInWorker || containsInSector;
            }

            return false;
        }

        private bool FilterWorkers(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (obj is Worker worker)
            {
                string search = SearchText.Trim();
                bool containsInName = (worker.FullName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool containsInSpecialty = (worker.Specialty ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                return containsInName || containsInSpecialty;
            }
            return false;
        }

        private bool FilterSectors(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (obj is Sector sector)
            {
                string search = SearchText.Trim();
                bool containsInName = (sector.SectorName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool containsInManagerFullName = (sector.ManagerFullName ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                return containsInName || containsInManagerFullName;
            }
            return false;
        }

        private bool FilterShifts(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (obj is Shift shift)
            {
                string search = SearchText.Trim();
               
                bool containsInNumber = shift.ShiftNumber.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool containsInForeman = (shift.Foreman ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                return containsInNumber || containsInForeman;
            }
            return false;
        }


        partial void OnSearchTextChanged(string value)
        {
            _detailsView?.Refresh();
            _workersView?.Refresh();
            _sectorsView?.Refresh();
            _shiftsView?.Refresh();
        }

        private void LoadDictionaries()
        {
            LoadWorkers();
            LoadSectors();
            LoadShifts();
        }

        private void LoadWorkers()
        {
            Workers = new ObservableCollection<Worker>(_workerService.GetAllWorkers());
            _workersView = CollectionViewSource.GetDefaultView(Workers);
            _workersView.Filter = FilterWorkers;
        }

        private void LoadSectors()
        {
            Sectors = new ObservableCollection<Sector>(_sectorService.GetAllSectors());
            _sectorsView = CollectionViewSource.GetDefaultView(Sectors);
            _sectorsView.Filter = FilterSectors;
        }

        private void LoadShifts()
        {
            Shifts = new ObservableCollection<Shift>(_shiftService.GetAllShifts());
            _shiftsView = CollectionViewSource.GetDefaultView(Shifts);
            _shiftsView.Filter = FilterShifts;
        }
    }
}