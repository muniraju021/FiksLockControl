﻿using FiksLockControl.Model;
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
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly DashboardViewModel _viewModel;
        public DashboardView()
        {
            InitializeComponent();
            _viewModel = (DataContext as DashboardViewModel);
            _viewModel.Initialize();
        }

        private async void btnGetLockHistory_Click(object sender, RoutedEventArgs e)
        {
            //var btn = e.Source as Button;
            //var obj = btn.DataContext as LockInformationObject;
            //Window.GetWindow(this.VisualParent).Opacity = 0.5;

            //LockHistoryView objLockHist = new LockHistoryView();
            //objLockHist.Closed += (objLockHistSender, e1) =>
            //{
            //    Window.GetWindow(this.VisualParent).Opacity = 1;
            //};
            //var viewModel = (objLockHist.DataContext as LockHistoryViewModel);
            //viewModel.GetLockHistory(obj.EmailId, obj.VehicleNumber);
            //viewModel.LastLockCode = obj.LatestLockCode;
            //objLockHist.ShowDialog();           

            var btn = e.Source as Button;
            var obj = btn.DataContext as LockInformationObject;
            _viewModel.ShowDialogCommand.Execute(obj);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IsDialogOpen = true;
        }
    }
}