using FiksLockControl.Model;
using FiksLockControl.Utilities;
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
    /// Interaction logic for Troubleshoot.xaml
    /// </summary>
    public partial class Troubleshoot : UserControl
    {
        private readonly AdminSettingsModel _viewModel;

        public Troubleshoot()
        {
            InitializeComponent();
            _viewModel = (DataContext as AdminSettingsModel);
        }

        private async void btnReadAllMessages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.ReadAllMessages();
                var dialog = new GenericMessageBoxInfoTemplate();
                var model = dialog.DataContext as GenericMessageBoxViewModel;
                model.TitleName = MessageBoxTitles.Info.ToString();
                model.MessageContent = $"Reading All Messages from Device triggered";
                var res = await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "DialogHostCtrl");
               
            }
            catch (Exception ex)
            {
                var dialog = new GenericMessageBoxInfoTemplate();
                var model = dialog.DataContext as GenericMessageBoxViewModel;
                model.TitleName = MessageBoxTitles.Error.ToString();
                model.TitleIcon = "ErrorOutline";
                model.MessageContent = $"Error in reading All Messages";
                var res = await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "DialogHostCtrl");
            }
        }
    }
}
