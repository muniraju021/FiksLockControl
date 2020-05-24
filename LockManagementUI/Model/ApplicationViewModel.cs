using LockServices.Lib.Cache;
using LockServices.Lib.GsmMessages;
using LockServices.Lib.Services;
using Ninject;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockManagementUI.Model
{
    public class ApplicationViewModel : NotificationObject
    {
        IKernel _kernel;
        IReceiveSmsMessage _receiveSmsMessage;
        ICacheService _cacheService;

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
        public ApplicationViewModel(IKernel kernel, IReceiveSmsMessage recieveSmsMsg, ICacheService cacheService)
        {
            _kernel = kernel;
            _receiveSmsMessage = recieveSmsMsg;
            _cacheService = cacheService;
            PopulateUserEmail();
            //UserEmail = "muniraju021@gmail.com";
        }

        public IKernel Kernel { get { return _kernel; } }

        private void PopulateUserEmail()
        {
            var userInfo = _cacheService.GetUserCredentials();
            UserEmail = userInfo?.EmailId;
        }

    }
}
