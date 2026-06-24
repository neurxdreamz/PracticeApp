using Business_Logic.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data_Logic.Entities;
using Microsoft.Extensions.DependencyInjection;
using PracticeApp.Views;
using System.Windows;
using System.Windows.Controls;
using PracticeApp.Views; 

namespace PracticeApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _username;

        public Action CloseAction { get; set; }

        public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private void Login(object parameter)
        {
            
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;

            string rawPassword = passwordBox.Password;

            try
            {
                
                User loggedInUser = _authService.AuthenticateUser(Username, rawPassword);

                
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

               
                var mainViewModel = (MainViewModel)mainWindow.DataContext;
                mainViewModel.SetupAccessRights(loggedInUser.IdRole);

               
                mainWindow.Show();
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordBox.Clear(); 
            }
        }
    }
}