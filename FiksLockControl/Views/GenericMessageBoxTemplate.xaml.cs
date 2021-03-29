using FiksLockControl.Model;
using LockServices.Lib.DataObjects;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FiksLockControl.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxTemplate.xaml
    /// </summary>
    public partial class GenericMessageBoxTemplate : UserControl
    {

        private readonly GenericMessageBoxViewModel _viewModel;

        public GenericMessageBoxTemplate()
        {
            InitializeComponent();
            _viewModel = DataContext as GenericMessageBoxViewModel;
        }
        

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Custom MessageBox");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogHost.CloseDialogCommand.Execute(false, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Opening Lock. Contact Admin");
            }
        }
    }
}
