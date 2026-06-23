using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Business_Logic.Services;
using Data_Logic.Entities;

namespace PracticeApp.ViewModels 
{
    
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDetailService detailService;

      
        [ObservableProperty]
        private ObservableCollection<Detail> details;

        
        public MainViewModel(IDetailService detailService)
        {
            detailService = detailService;
            LoadDetails();
        }

        private void LoadDetails()
        {
            
            var dataFromDb = detailService.GetAllDetails();
            Details = new ObservableCollection<Detail>(dataFromDb);
        }
    }
}