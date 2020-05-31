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
    public class ApplicationViewModel : BaseViewModel
    {
        IKernel _kernel;
        IReceiveSmsMessage _receiveSmsMessage;
                
        public ApplicationViewModel(IKernel kernel, IReceiveSmsMessage recieveSmsMsg, ICacheService cacheService) : base(cacheService)
        {
            _kernel = kernel;
            _receiveSmsMessage = recieveSmsMsg;
            //UserEmail = "muniraju021@gmail.com";
        }

        public IKernel Kernel { get { return _kernel; } }

        

    }
}
