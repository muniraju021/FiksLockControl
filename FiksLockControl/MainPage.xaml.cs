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
using FiksLockControl.Views;
using FiksLockControl.Utilities;
using LockServices.Lib.GsmMessages;

namespace FiksLockControl
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        ApplicationViewModel _applicationViewModel;
        DashboardViewModel _dashboardViewModel;
        ILog _logger;

        public MainPage()
        {
            InitializeComponent();
            _applicationViewModel = DataContext as ApplicationViewModel;
            _logger = log4net.LogManager.GetLogger(typeof(Startup));
            _applicationViewModel.Initialize();
            ListViewMenu.SelectedIndex = 0;
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
                    contentCtrl.DataContext = _dashboardViewModel;
                    //DataContext = _dashboardViewModel;
                    break;
                case "GetCodes":
                    //var lst = new List<string>();
                    //lst.Add("+CMGR: \"REC UNREAD\",\"+919739402306\",\"\",\"20/12/08,19:24:34+22\"");
                    //lst.Add("POWER ON 20/12/08,19:23:59");
                    //var obj = _applicationViewModel.Kernel.Get<ISmsMessageService>();
                    //obj.ProcessActualMessages(lst).Wait();
                    _dashboardViewModel?.Dispose();
                    contentCtrl.DataContext = new GenerateCodesViewModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>(), _applicationViewModel.Kernel.Get<ILog>());
                    break;
                case "Reports":
                    _dashboardViewModel?.Dispose();
                    contentCtrl.DataContext = new ReportViewModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>(), _applicationViewModel.Kernel.Get<ILog>());
                    break;
                case "Settings":
                    _dashboardViewModel?.Dispose();
                    contentCtrl.DataContext = new AdminSettingsModel(_applicationViewModel.Kernel.Get<ILockActionServices>(), _applicationViewModel.Kernel.Get<ICacheService>(), _applicationViewModel.Kernel.Get<ILog>());
                    break;
                default:
                    break;
            }
        }

        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GenericMessageBoxTemplate();
            var model = dialog.DataContext as GenericMessageBoxViewModel;
            model.TitleName = MessageBoxTitles.Info.ToString();
            model.MessageContent = $"Are you sure you want to exit";
            var res = await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "DialogHostCtrl");

            if (Convert.ToBoolean(res))
            {
                this.Close();
            }
        }
    }
}
