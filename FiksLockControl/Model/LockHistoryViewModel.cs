using LockServices.Lib.Cache;
using LockServices.Lib.DataObjects;
using LockServices.Lib.Services;
using log4net;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiksLockControl.Model
{
    public class LockHistoryViewModel : BaseViewModel
    {
        private readonly ILockActionServices _lockActionServices;
        private readonly ILog _logger;
        private string _vehicleNumber;
        private string _lastLockCode;

        private ObservableCollection<LockStatusDO> _overrallStatus = new ObservableCollection<LockStatusDO>();
        public string VehicleNumber
        {
            get { return _vehicleNumber; }
            set
            {
                _vehicleNumber = value;
                RaisePropertyChanged("VehicleNumber");
            }
        }
        public string LockId { get; set; }

        public ObservableCollection<LockStatusDO> LockStatusLst {
            get { return _overrallStatus; }
            set {
                _overrallStatus = value;
                RaisePropertyChanged("LockStatusLst");
            }
        }

        public string LastLockCode
        {
            get { return _lastLockCode; }
            set { _lastLockCode = value;
                RaisePropertyChanged("LastLockCode");
            }
        }

        public LockHistoryViewModel(ILockActionServices lockActionServices, ICacheService cacheService, ILog logger) 
            : base(cacheService,logger)
        {
            _lockActionServices = lockActionServices;
            _logger = logger;
        }

        public async void GetLockHistory(string emailId,string vehicleNumber,string lockId)
        {
            try
            {
                VehicleNumber = vehicleNumber;
                var lockStatusHistory = await _lockActionServices.GetLockHistory(emailId, vehicleNumber, lockId);

                if (lockStatusHistory != null && lockStatusHistory.Count > 0)
                {
                    var overrallStatus = new ObservableCollection<LockStatusDO>();
                    foreach (var item in lockStatusHistory)
                    {
                        overrallStatus.Add(item);
                    }
                    LockStatusLst = overrallStatus;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetLockHistory - ", ex);
                throw;
            }
            

        }
    }
}
