using LockServices.Lib.Cache;
using LockServices.Lib.DataObjects;
using LockServices.Lib.Services;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using FiksLockControl.Extensions;
using System.Windows;
using MaterialDesignThemes.Wpf;
using FiksLockControl.Views;
using log4net;

namespace FiksLockControl.Model
{
    public class GenerateCodesViewModel : BaseViewModel
    {
        private readonly ILockActionServices _lockActionServices;

        public string RowCount { get; set; }
        public double ColumnCount => 2;
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

        public ICommand ShowDialogCommand { get; }

        private ObservableCollection<LockInformationObject> _listLockInfoColl = new ObservableCollection<LockInformationObject>();
        public ObservableCollection<LockInformationObject> ListLockInfoColl
        {
            get
            {
                return _listLockInfoColl;
            }
            set
            {
                _listLockInfoColl = value;
                RaisePropertyChanged("ListLockInfoColl");
            }
        }


        public GenerateCodesViewModel(ILockActionServices lockActionServices, ICacheService cacheService, ILog logger) : base(cacheService, logger)
        {
            _lockActionServices = lockActionServices;
            ShowDialogCommand = new RelayCommand(OnShowDialog);
        }


        public async void GetVehiclesTagged()
        {
            try
            {
                List<string> lstVehicles = new List<string>();
                List<LockInformationObject> lstLockInfo = new List<LockInformationObject>();
                var userInfo = _cacheService.GetUserCredentials();
                if (userInfo != null)
                {
                    lstLockInfo = await _lockActionServices.GetVehiclesTagged(userInfo.EmailId);
                    var lstLockInfoFromDash = await _lockActionServices.GetLockDetailsByEmailId(userInfo.EmailId);

                    if (lstLockInfo != null && lstLockInfo.Count > 0)
                    {
                        var obsColl = new ObservableCollection<LockInformationObject>();
                        foreach (var item in lstLockInfo.OrderBy(i => i.VehicleNumber))
                        {
                            if (lstLockInfoFromDash.Any(i => i.VehicleNumber == item.VehicleNumber))
                            {
                                item.CodeList = lstLockInfoFromDash.Where(i => i.VehicleNumber == item.VehicleNumber).FirstOrDefault().CodeList;
                                obsColl.Add(item);
                            }
                        }
                        ListLockInfoColl = obsColl;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetVehiclesTagged - ", ex);
                throw;
            }
            finally
            {

            }

        }

        public async Task<ApiResponseMessage> GenerateCode(string vehicleNo)
        {
            try
            {
                IsBusyIndicator = true;
                var userInfo = _cacheService.GetUserCredentials();
                if (userInfo != null)
                {
                    var result = await _lockActionServices.GenerateAndGetCode(userInfo.EmailId, vehicleNo);
                    if (result != null && !string.IsNullOrWhiteSpace(result.LockCode))
                    {
                        var lst = await _lockActionServices.GetLockDetailsByEmailId(userInfo.EmailId);
                        var obsColl = new ObservableCollection<LockInformationObject>();
                        foreach (var item in lst.OrderBy(i => i.VehicleNumber))
                        {
                            obsColl.Add(item);
                        }
                        ListLockInfoColl = obsColl;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GenerateCode - ", ex);
                throw;
            }
            finally
            {
                IsBusyIndicator = false;
            }

            return default(ApiResponseMessage);

        }

        public async Task<List<LockStatusDO>> GetLockHistory(string vehicleNo, string lockId)
        {
            try
            {
                var userInfo = _cacheService.GetUserCredentials();
                if (userInfo != null)
                {
                    return await _lockActionServices.GetLockHistory(userInfo.EmailId, vehicleNo, lockId);
                }
                return default(List<LockStatusDO>);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetLockHistory - ", ex);
                throw;
            }

        }

        public void OpenLock(string code, string lockNo, ref ApiResponseMessage objApiRespMessage)
        {
            try
            {
                IsBusyIndicator = true;
                _lockActionServices.OpenLock(code, lockNo, ref objApiRespMessage);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in OpenLock - ", ex);
                throw;
            }
            finally
            {
                IsBusyIndicator = false;
            }
        }

        private void OnShowDialog(object obj)
        {
            try
            {
                IsDialogHostOpen = true;
                if (obj is LockInformationObject)
                {
                    var msgBox = new MessageBoxTemplate();
                    var model = msgBox.DataContext as MessageBoxViewModel;
                    var lockObj = obj as LockInformationObject;
                    model.MessageContent = $"Code Generation Successfull...";
                    model.MessageBoxTitle = "Open Lock - GetCodes";
                    model.LockCode = lockObj.LatestLockCode;
                    model.LockPhoneNo = lockObj.LockPhNo;
                    DialogHost.Show(msgBox);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in OnShowDialog - ", ex);
                throw;
            }


        }

    }

}
