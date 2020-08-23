using FiksLockControl.Views;
using LockServices.Lib.Cache;
using LockServices.Lib.DataObjects;
using LockServices.Lib.Services;
using log4net;
using MaterialDesignThemes.Wpf;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiksLockControl.Model
{
    public class DashboardViewModel : BaseViewModel
    {
        private ObservableCollection<LockInformationObject> _lstLockModels = new ObservableCollection<LockInformationObject>();
        private readonly ILockActionServices _lockActionServices;
        private readonly ILog _logger;

        public string RowCount { get; set; }

        public double ColumnCount => 3;

        public ICommand ShowDialogCommand { get; }

        private bool _isBusyIndicator;
        public bool IsBusyIndicator
        {
            get
            {
                return _isBusyIndicator;
            }
            set
            {
                _isBusyIndicator = value;
                RaisePropertyChanged("IsBusyIndicator");
            }
        }

        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set
            {
                _isDialogOpen = value;
                RaisePropertyChanged("IsDialogOpen");
            }
        }

        public ObservableCollection<LockInformationObject> LstLockModels
        {
            get
            {
                return _lstLockModels;
            }
            set
            {
                _lstLockModels = value;
                RaisePropertyChanged("LstLockModels");
            }
        }

        public DashboardViewModel(ILockActionServices lockActionServices, ICacheService cacheService, ILog logger) : base(cacheService)
        {
            _lockActionServices = lockActionServices;
            IsBusyIndicator = true;
            _logger = logger;
            ShowDialogCommand = new Extensions.RelayCommand(OnShowDialog);
        }

        public void Initialize()
        {
            PopulateLockModels();
        }

        public void PopulateLockModels()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        var userInfo = _cacheService.GetUserCredentials();
                        if (userInfo != null)
                        {
                            List<LockInformationObject> lstLocks = new List<LockInformationObject>();
                            lstLocks = await _lockActionServices.GetLockDetailsByEmailId(userInfo.EmailId);
                            if (lstLocks != null)
                            {
                                var obColl = new ObservableCollection<LockInformationObject>();
                                foreach (var item in lstLocks)
                                {
                                    obColl.Add(item);
                                }
                                LstLockModels = obColl;
                                RowCount = Convert.ToString(Math.Ceiling(LstLockModels.Count / ColumnCount));
                            }
                        }
                        Thread.Sleep(10000);
                    }
                }, TaskCreationOptions.LongRunning);

            }
            catch (Exception ex)
            {
                _logger.Error($"DashboardViewModel: Exception in PopulateLockModels {ex}");
            }
            finally
            {
                IsBusyIndicator = false;
            }

        }

        public void OpenLock(string code, string lockNo, ref ApiResponseMessage objApiRespMessage)
        {
            try
            {
                IsBusyIndicator = true;
                _lockActionServices.OpenLock(code, lockNo, ref objApiRespMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusyIndicator = false;
            }
        }

        private async void OnShowDialog(object obj)
        {
            IsDialogHostOpen = true;
            var objLockHistoryView = new LockHistoryView();
            var viewModel = objLockHistoryView.DataContext as LockHistoryViewModel;
            var lockObj = obj as LockInformationObject;
            viewModel.GetLockHistory(lockObj.EmailId, lockObj.VehicleNumber);
            viewModel.LastLockCode = lockObj.LatestLockCode;            
            await DialogHost.Show(objLockHistoryView);
        }


    }
}
