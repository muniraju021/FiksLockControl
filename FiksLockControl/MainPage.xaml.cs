using FiksLockControl.Model;
using LockServices.Lib.Services;
using Syncfusion.Windows.SampleLayout;
using Syncfusion.Windows.Shared;
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
using System.Windows.Shapes;
using Ninject;
using LockServices.Lib.Cache;
using log4net;

namespace FiksLockControl
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        ApplicationViewModel _applicationViewModel;
        DashboardViewModel _dashboardViewModel;

        public MainPage()
        {
            InitializeComponent();
            _applicationViewModel = DataContext as ApplicationViewModel;
        }

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            //DataContext = new DashboardViewModel();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Dashboard":
                    _dashboardViewModel = new DashboardViewModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>(), _applicationViewModel.Kernel.Get<ILog>());
                    DataContext = _dashboardViewModel;
                    break;
                case "GetCodes":
                    _dashboardViewModel?.Dispose();
                    DataContext = new GenerateCodesViewModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>());
                    break;
                case "Reports":
                    _dashboardViewModel?.Dispose();
                    DataContext = new ReportViewModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>(), _applicationViewModel.Kernel.Get<ILog>());
                    break;
                default:
                    break;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit", "Info", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                this.Close();
        }
    }
}
