using Business_Logic.Services;
using Data_Logic.Repositories;
using Microsoft.Extensions.DependencyInjection;
using PracticeApp;
using PracticeApp.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using PracticeApp.Views;

namespace PracticeApp
{
    public partial class App : Application
    {
        
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
            services.AddSingleton<ISectorRepository, SectorRepository>();
            services.AddSingleton<IShiftRepository, ShiftRepository>();
            services.AddSingleton<IDetailRepository, DetailRepository>();


            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IWorkerService, WorkerService>();
            services.AddSingleton<ISectorService, SectorService>();
            services.AddSingleton<IShiftService, ShiftService>();
            services.AddSingleton<IDetailService, DetailService>();


            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            services.AddTransient<AddDetailViewModel>();
            services.AddTransient<AddDetailWindow>();

            services.AddTransient<EditDetailViewModel>();
            services.AddTransient<EditDetailWindow>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>();

            services.AddTransient<AddWorkerViewModel>();
            services.AddTransient<AddWorkerWindow>();

            services.AddTransient<EditWorkerViewModel>();
            services.AddTransient<EditWorkerWindow>();

            services.AddTransient<AddSectorViewModel>();
            services.AddTransient<AddSectorWindow>();
            services.AddTransient<EditSectorViewModel>();
            services.AddTransient<EditSectorWindow>();

            
            services.AddTransient<AddShiftViewModel>();
            services.AddTransient<AddShiftWindow>();
            services.AddTransient<EditShiftViewModel>();
            services.AddTransient<EditShiftWindow>();

        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

           
            var authService = ServiceProvider.GetRequiredService<IAuthService>();

            if (!authService.IsUserExist("admin"))
            {
                try
                {
   
                    authService.RegisterNewUser("admin", 1, "1234");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании администратора: {ex.Message}");
                }
            }

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
