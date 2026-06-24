using System;
using System.Windows;
using PracticeApp.ViewModels;

namespace PracticeApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            if (viewModel.CloseAction == null)
            {
                viewModel.CloseAction = new Action(this.Close);
            }
        }
    }
}