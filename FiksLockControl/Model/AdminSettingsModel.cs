using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LockServices.Lib.Cache;
using log4net;
using LockServices.Lib.Services;

namespace FiksLockControl.Model
{
    public class AdminSettingsModel : BaseViewModel
    {
        private readonly ILockActionServices _iLockActionService;

        public AdminSettingsModel(ILockActionServices lockActionService, ICacheService cacheService, ILog logger) : base(cacheService, logger)
        {
            _iLockActionService = lockActionService;
        }

        public void ReadAllMessages()
        {
            try
            {
                Logger.Info($"AdminSettingsModel: ReadAllMessagesTriggered");
                _iLockActionService.ReadAllMessages();
            }
            catch (Exception ex)
            {
                Logger.Error($"AdminSettingsModel: Error in ReadAllMessages",ex);
                throw;
            }
           
        }


    }
}
