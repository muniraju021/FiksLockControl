using FiksLockControl.Model;
using FiksLockControl.Utilities;
using LockServices.Lib.CustomException;
using LockServices.Lib.DataObjects;
using LockServices.Lib.Services;
using log4net;
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
    /// Interaction logic for GetCodesControl.xaml
    /// </summary>
    public partial class GetCodesControl : UserControl
    {
        private readonly GenerateCodesViewModel _viewModel;
        private static string CodeGenerationStatus = $"Code Generation Status: {0} {Environment.NewLine} SMS Send Status:{1}";

        public GetCodesControl()
        {
            InitializeComponent();
            _viewModel = (DataContext as GenerateCodesViewModel);
            //_viewModel.IsBusyIndicator = true;
            Initialize();
        }

        public void Initialize()
        {
            _viewModel.GetVehiclesTagged();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            try
            {
                var obj = btn.DataContext as LockInformationObject;
                btn.IsEnabled = false;

                var currLockStatus = await _viewModel.GetLockHistory(obj.VehicleNumber, obj.LockId);
                if (currLockStatus != null && currLockStatus.Count > 0)
                {
                    int code;
                    if (currLockStatus[0].LockStatus.Contains("LOCK_OPEN") || int.TryParse(currLockStatus[0].LockStatus, out code))
                    {
                        var dialog = new GenericMessageBoxTemplate();
                        var model = dialog.DataContext as GenericMessageBoxViewModel;
                        model.TitleName = MessageBoxTitles.Warning.ToString();
                        model.MessageContent = $"Code is already Generated or Lock is in Open State. Please confirm if you want to generate Code again";
                        var res = await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "DialogHostCtrl");

                        if (Convert.ToBoolean(res))
                        {
                            var result = await _viewModel.GenerateCode(obj.VehicleNumber);

                            if (result != null)
                            {
                                if (!string.IsNullOrWhiteSpace(result.LockCode))
                                {
                                    obj.CodeList[0].Code = result.LockCode;
                                    _viewModel.ShowDialogCommand.Execute(obj);
                                }
                                else
                                {
                                    var lockStatus = $"Code Generation: UnSuccessful - {result.ErrorMessage}";
                                    MessageBox.Show(lockStatus, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        var result = await _viewModel.GenerateCode(obj.VehicleNumber);

                        if (result != null)
                        {
                            if (!string.IsNullOrWhiteSpace(result.LockCode))
                            {
                                obj.CodeList[0].Code = result.LockCode;
                                _viewModel.ShowDialogCommand.Execute(obj);
                            }
                            else
                            {
                                var lockStatus = $"Code Generation: UnSuccessful - {result.ErrorMessage}";
                                MessageBox.Show(lockStatus, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }

            }
            catch (GsmTimeoutException ex)
            {
                MessageBox.Show($"Code Generated:{ex.Code} - Exception:{ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error - {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }

        private void btnOpenLock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                var obj = btn.DataContext as LockInformationObject;
                var objApiResponse = new ApiResponseMessage();
                _viewModel.OpenLock(obj.LatestLockCode, obj.LockPhNo, ref objApiResponse);
                if (string.IsNullOrEmpty(objApiResponse.ErrorMessage))
                {
                    var lockStatus = "SMS Sent Status: SENT";
                    MessageBox.Show(lockStatus, "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var lockStatus = objApiResponse.ErrorMessage;
                    MessageBox.Show(lockStatus, "Status", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error - {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
