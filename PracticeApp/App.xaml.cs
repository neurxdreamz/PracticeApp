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

            // Если будем делать окно авторизации:
            // services.AddTransient<LoginViewModel>();
            // services.AddTransient<LoginWindow>();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
