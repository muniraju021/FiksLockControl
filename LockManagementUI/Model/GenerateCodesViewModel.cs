﻿using LockServices.Lib.Cache;
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

namespace LockManagementUI.Model
{
    public class GenerateCodesViewModel : NotificationObject
    {
        private readonly ILockActionServices _lockActionServices;
        private readonly ICacheService _cacheService;
        
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

      
        private ObservableCollection<LockInformationObject> _listLockInfoColl = new ObservableCollection<LockInformationObject>();
        public ObservableCollection<LockInformationObject> ListLockInfoColl
        {
            get {
                return _listLockInfoColl;
            }
            set {
                _listLockInfoColl = value;
                RaisePropertyChanged("ListLockInfoColl");
            }
        }


        public GenerateCodesViewModel(ILockActionServices lockActionServices, ICacheService cacheService)
        {
            _lockActionServices = lockActionServices;
            _cacheService = cacheService;
            IsBusyIndicator = true;    
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
                throw ex;
            }
            finally
            {
                IsBusyIndicator = false;
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
                throw;
            }
            finally
            {
                IsBusyIndicator = false;
            }
            
            return default(ApiResponseMessage);
            
        }

        public async Task<List<LockStatusDO>> GetLockHistory(string vehicleNo)
        {
            var userInfo = _cacheService.GetUserCredentials();
            if(userInfo != null)
            {
                return await _lockActionServices.GetLockHistory(userInfo.EmailId, vehicleNo);
            }
            return default(List<LockStatusDO>);
        }

        public void OpenLock(string code,string lockNo,ref ApiResponseMessage objApiRespMessage)
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

    }

}
