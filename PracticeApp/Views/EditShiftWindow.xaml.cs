using System;
using System.Windows;
using PracticeApp.ViewModels;

namespace PracticeApp.Views
{
    public partial class EditShiftWindow : Window
    {
        public EditShiftWindow(EditShiftViewModel viewModel)
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