using LockServices.Lib.Cache;
using LockServices.Lib.WebClientApi;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public interface IApiService
    {
        Task<string> UpdateLockStatus(string lockPhoneNo, string status);
    }

    public class ApiService : IApiService
    {
        private readonly IFiksApi _iFiksApi;
        private readonly ICacheService _iCacheService;
        private ILog _logger;

        public ApiService(IFiksApi iFiksApi, ICacheService cacheService, ILog logger)
        {
            _iFiksApi = iFiksApi;
            _iCacheService = cacheService;
            _logger = logger;
        }

        public async Task<string> UpdateLockStatus(string lockPhoneNo, string status)
        {
            //var lstObj = _iCacheService.GetLockInformationByPhoneNo(lockPhoneNo);
            var userInfo = _iCacheService.GetUserCredentials();
            var lstLocks = await _iFiksApi.GetLockDetails(userInfo.EmailId);
            var lstObj = lstLocks.Where(i => i.LockPhNo == lockPhoneNo).FirstOrDefault();
            var userDetails = _iCacheService.GetUserCredentials();
            if (lstObj != null)
            {
                var res = await _iFiksApi.UpdateLockStatus(userDetails.EmailId, lstObj?.LockId, lockPhoneNo, status);
                return res;
            }
            _logger.Error($"UpdateLockStatus Failed [PhoneNo Not Found] - LockPhNo:{lockPhoneNo};Status:{status}");
            return default(string);
        }
    }
}
