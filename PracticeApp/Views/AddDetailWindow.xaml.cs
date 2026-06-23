using PracticeApp.ViewModels;
using System.Windows;
using System;
using System.Windows;
using PracticeApp.ViewModels;

namespace PracticeApp.Views
{
    public partial class AddDetailWindow : Window
    {
        public AddDetailWindow(AddDetailViewModel viewModel)
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