using LockServices.Lib.Cache;
using log4net;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiksLockControl.Model
{
    public class BaseViewModel : NotificationObject
    {
        protected readonly ICacheService _cacheService;
        protected readonly ILog Logger;

        public BaseViewModel(ICacheService cacheService,ILog logger)
        {
            _cacheService = cacheService;
            Logger = logger;
            PopulateUserEmail();
        }

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                RaisePropertyChanged("UserEmail");
            }
        }

        private bool _isDialogHostOpen;
        public bool IsDialogHostOpen
        {
            get { return _isDialogHostOpen; }
            set
            {
                _isDialogHostOpen = value;
                RaisePropertyChanged("IsDialogHostOpen");
            }
        }

        private void PopulateUserEmail()
        {
            var userInfo = _cacheService.GetUserCredentials();
            UserEmail = userInfo?.EmailId;
        }
    }
}
