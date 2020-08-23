using FiksLockControl.Model;
using LockServices.Lib.DataObjects;
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
        }

        private void btnOpenLock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objApiResponse = new ApiResponseMessage();
                _viewModel.OpenLock(_viewModel.LockCode, _viewModel.LockPhoneNo, ref objApiResponse);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Opening Lock. Contact Admin");
            }
        }
    }
}
