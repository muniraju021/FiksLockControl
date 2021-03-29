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
    public partial class MessageBoxTemplate : UserControl
    {

        private readonly MessageBoxViewModel _viewModel;

        public MessageBoxTemplate()
        {
            InitializeComponent();

            _viewModel = DataContext as MessageBoxViewModel;
            _viewModel.OpenLockButtonStatus = true;
        }

        private void btnOpenLock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objApiResponse = new ApiResponseMessage();
                _viewModel.OpenLockButtonStatus = false;
                _viewModel.IsBusyIndicator = true;
                Task.Factory.StartNew(async() =>
                {
                    await _viewModel.OpenLock(_viewModel.LockCode, _viewModel.LockPhoneNo, objApiResponse);
                });                                             
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Opening Lock. Contact Admin");
            }
            finally
            {
                //btnOpenLock.IsEnabled = true;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogHost.CloseDialogCommand.Execute(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Opening Lock. Contact Admin");
            }
        }
    }
}
