using System;
using System.Windows;
using PracticeApp.ViewModels;

namespace PracticeApp.Views
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow(UserManagementViewModel viewModel)
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