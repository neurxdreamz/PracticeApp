using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using Microsoft.Extensions.DependencyInjection;
using PracticeApp.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace PracticeApp.ViewModels 
{
    
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDetailService _detailService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<Detail> details;

        
        [ObservableProperty]
        private Detail _selectedDetail;


        public MainViewModel(IDetailService detailService, IServiceProvider serviceProvider)
        {
            _detailService = detailService;
            _serviceProvider = serviceProvider;
            LoadDetails();
        }

        private void LoadDetails()
        {
            
            var dataFromDb = _detailService.GetAllDetails();
            Details = new ObservableCollection<Detail>(dataFromDb);  
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
            // Проверяем, выделил ли пользователь строку в таблице
            if (SelectedDetail == null)
            {
                MessageBox.Show("Сначала выберите деталь для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Запрашиваем подтверждение
            var result = MessageBox.Show($"Вы уверены, что хотите удалить деталь '{SelectedDetail.DetailName}'?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Передаем ID на удаление в слой бизнес-логики
                    _detailService.DeleteDetail(SelectedDetail.IdRecord);

                    // Обновляем таблицу после успешного удаления
                    LoadDetails();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}