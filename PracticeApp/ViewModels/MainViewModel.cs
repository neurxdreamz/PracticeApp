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


        public void SetupAccessRights(int roleId)
        {

            IsAdmin = roleId == 1;

            IsEditor = roleId == 1 || roleId == 3;
        }


        private ICollectionView _detailsView;


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
        private void OpenAddWindow()
        {
           
            var addWindow = _serviceProvider.GetRequiredService<AddDetailWindow>();

           
            addWindow.ShowDialog();


            LoadDetails();

           
        }

        [RelayCommand]
        private void DeleteDetail()
        {
            
            if (SelectedDetail == null)
            {
                MessageBox.Show("Сначала выберите деталь для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

       
            var result = MessageBox.Show($"Вы уверены, что хотите удалить деталь '{SelectedDetail.DetailName}'?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                   
                    _detailService.DeleteDetail(SelectedDetail.IdRecord);

                   
                    LoadDetails();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        [RelayCommand]
        private void OpenEditWindow()
        {
           
            if (SelectedDetail == null)
            {
                MessageBox.Show("Сначала выберите деталь для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

           
            var editWindow = _serviceProvider.GetRequiredService<EditDetailWindow>();

           
            var editViewModel = (EditDetailViewModel)editWindow.DataContext;
            editViewModel.Initialize(SelectedDetail); 

          
            editWindow.ShowDialog();

           
            LoadDetails();
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

     
        partial void OnSearchTextChanged(string value)
        {
            
            _detailsView?.Refresh();
        }

        private void LoadDictionaries()
        {
            Workers = new ObservableCollection<Worker>(_workerService.GetAllWorkers());
            Sectors = new ObservableCollection<Sector>(_sectorService.GetAllSectors());
            Shifts = new ObservableCollection<Shift>(_shiftService.GetAllShifts()); 
        }
    }
}