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
        

        public ReportViewModel(ILockActionServices lockActionServices, ICacheService cacheService,ILog logger) : base(cacheService) 
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

        private ObservableCollection<string> _lstVehicleNo;
        public ObservableCollection<string> LstVehicleNo
        {
            get { return _lstVehicleNo; }
            set
            {
                _lstVehicleNo = value;
                RaisePropertyChanged("LstVehicleNo");
            }
        }

        public string SelectedVehNo { get; set; }

        public async void GetLockHistory()
        {
            if (!string.IsNullOrWhiteSpace(SelectedVehNo))
            {
                var lockStatusHistory = await _lockActionServices.GetLockHistory(UserEmail, SelectedVehNo);

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

        public async void GetVehicleNos()
        {
            var userInfo = _cacheService.GetUserCredentials();
            if (userInfo != null)
            {
                var lstLockInfo = await _lockActionServices.GetVehiclesTagged(userInfo.EmailId);
                var lstVehicles = lstLockInfo.Select(i => i.VehicleNumber).ToList();

                var lstVehicleNo = new ObservableCollection<string>();
                foreach (var item in lstVehicles)
                {
                    lstVehicleNo.Add(item);
                }
                LstVehicleNo = lstVehicleNo;
               
            }
        }
    }
}
