using LockServices.Lib.Cache;
using LockServices.Lib.DataObjects;
using LockServices.Lib.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiksLockControl.Model
{
    public class ReportViewModel : BaseViewModel
    {
        private readonly ILog _logger;
        private readonly ILockActionServices _lockActionServices;

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

        public ReportViewModel(ILockActionServices lockActionServices, ICacheService cacheService,ILog logger)
            : base(cacheService,logger) 
        {
            _logger = logger;
            _lockActionServices = lockActionServices;
            //GetVehicleNos();
        }

        private ObservableCollection<LockStatusDO> _lockHistoryDetails;
        public ObservableCollection<LockStatusDO> LockHistoryDetails
        {
            get { return _lockHistoryDetails; }
            set
            {
                _lockHistoryDetails = value;
                RaisePropertyChanged("LockHistoryDetails");
            }
        }

        private ObservableCollection<LockInformationObject> _lstVehicleNo;
        public ObservableCollection<LockInformationObject> LstVehicleNo
        {
            get { return _lstVehicleNo; }
            set
            {
                _lstVehicleNo = value;
                RaisePropertyChanged("LstVehicleNo");
            }
        }

        public LockInformationObject SelectedVehNo { get; set; }

        public async void GetLockHistory()
        {
            IsBusyIndicator = true;
            try
            {
                if (SelectedVehNo != null)
                {
                    var lockStatusHistory = await _lockActionServices.GetLockHistory(UserEmail, SelectedVehNo.VehicleNumber,SelectedVehNo.LockId);

                    if (lockStatusHistory != null && lockStatusHistory.Count > 0)
                    {
                        var lockHistoryDetails = new ObservableCollection<LockStatusDO>();
                        foreach (var item in lockStatusHistory)
                        {
                            lockHistoryDetails.Add(item);
                        }
                        LockHistoryDetails = lockHistoryDetails;
                    }
                    else
                    {
                        _logger.ErrorFormat($"ReportViewModel: GetLockHistory - Invalid VehicleNo Selected - {SelectedVehNo}");
                    }
                }
                else
                {
                    _logger.ErrorFormat($"ReportViewModel: GetLockHistory - Invalid VehicleNo Selected - {SelectedVehNo}");
                    throw new Exception("Invalid Vehicle No Selected");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetLockHistory - ", ex);
                throw ex;
            }
            finally
            {
                IsBusyIndicator = false;
            }

        }

        public async void GetVehicleNos()
        {
            try
            {
                var userInfo = _cacheService.GetUserCredentials();
                if (userInfo != null)
                {
                    var lstLockInfo = await _lockActionServices.GetVehiclesTagged(userInfo.EmailId);
                    //var lstVehicles = lstLockInfo.Select(i => i.VehicleNumber).ToList();

                    var lstVehicles = new ObservableCollection<LockInformationObject>();
                    foreach (var item in lstLockInfo)
                    {
                        lstVehicles.Add(item);
                    }
                    LstVehicleNo = lstVehicles;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetVehicleNos - ", ex);
                throw;
            }
            
        }
    }
}
