using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LockServices.Lib.DataObjects;
using LockServices.Lib.WebClientApi;
using LockServices.Lib.Cache;
using GsmMessaging.Lib.Services;
using LockServices.Lib.GsmMessages;
using System.Threading;
using log4net;
using LockServices.Lib.CustomException;

namespace LockServices.Lib.Services
{
    public class LockActionServices : ILockActionServices
    {
        private readonly IFiksApi _iFiksApi;
        private readonly ICacheService _iCacheService;
        private readonly ISendSmsMessages _sendSmsMessages;
        private ILog _logger;

        public LockActionServices(IFiksApi iFiksApi,ICacheService cacheService, ISendSmsMessages sendSmsMessages,ILog logger)
        {
            _iFiksApi = iFiksApi;
            _iCacheService = cacheService;
            _sendSmsMessages = sendSmsMessages;
            _logger = logger;
        }

        public async Task<List<LockInformationObject>> GetLockDetailsByEmailId(string emailId)
        {
            _logger.InfoFormat($"LockActionServices: GetLockDetailsByEmailId - emailId:{emailId} - Started");
            var lstLockInfo = await _iFiksApi.GetLockDetails(emailId);
            if(lstLockInfo != null && lstLockInfo.Count > 0)
            {
                _iCacheService.LoadLockInfoDetails(lstLockInfo);
            }
            _logger.InfoFormat($"LockActionServices: GetLockDetailsByEmailId - emailId:{emailId} - Finished: Result:{lstLockInfo.Count}");
            return lstLockInfo;
        }

        public async Task<bool> Login(string emailId, string password)
        {
            var userInfo = await _iFiksApi.LoginApi(emailId, password);
            if(userInfo != null)
            {
                _iCacheService.LoadUserCredentails(userInfo);
                await this.GetLockDetailsByEmailId(emailId);
                return true;
            }
            return false;
        }

        public async Task<List<LockInformationObject>> GetVehiclesTagged(string emailId)
        {
            var lst = await _iFiksApi.GetVehicleTaggedList(emailId);

            #region comment
            //if (lst != null && lst.Count > 0)
            //{
            //    await GetLockDetailsByEmailId(emailId);
            //    var vehicleLst = new List<string>();
            //    foreach (var item in lst)
            //    {
            //        if (!string.IsNullOrWhiteSpace(item))
            //        {
            //            var vehiceNo = item.IndexOf(" ") != -1 ? item.Substring(0,item.IndexOf(" ") + 1) : item;
            //            vehicleLst.Add(vehiceNo.Trim());
            //        }
            //    }
            //    return _iCacheService.GetLockInformationLstByVehicleNo(vehicleLst);
            //}
            #endregion  

            return lst;
        }

        public async Task<ApiResponseMessage> GenerateAndGetCode(string emailId,string vehicleNo)
        {
            ApiResponseMessage apiResponseResp = null;
            try
            {
                var result = await _iFiksApi.GetCodeApi(emailId, vehicleNo);
                int code = 0;
                if (!string.IsNullOrWhiteSpace(result) && int.TryParse(result, out code))
                {
                    apiResponseResp = new ApiResponseMessage { LockCode = result, HttpStatusCode = 200 };
                }
                else
                {
                    apiResponseResp = new ApiResponseMessage { ErrorMessage = result != null ? result : "Invalid Code Generated" };
                }
                return apiResponseResp;
            }
            catch (Exception ex)
            {
                _logger.Error($"LockActionServices: GenerateAndGetCode - Error:{ex}");
                apiResponseResp.ErrorMessage = ex.Message;
                return apiResponseResp;
            }
                        
        }
        
        public async Task<List<LockStatusDO>> GetLockHistory(string emailId, string vehicleNo)
        {
            var lst = await _iFiksApi.GetLockHistory(emailId,vehicleNo);
            return lst;
        }
        
        public async Task<string> UpdateLockStatus(string lockPhoneNo, string status)
        {
            var lstObj = _iCacheService.GetLockInformationByPhoneNo(lockPhoneNo);
            var userDetails = _iCacheService.GetUserCredentials();
            if(lstObj != null)
            {
                var res = await _iFiksApi.UpdateLockStatus(userDetails.EmailId, lstObj.LockId, status);
                return res;
            }
            return default(string);
        }

        private void ProcessIncomingGsmMessage(string message)
        {
            Console.WriteLine($"MessageReceived: {message}");
        }
        
        public void OpenLock(string code,string phoneNo, ref ApiResponseMessage objApiRespMessage)
        {
            try
            {
                _logger.Info($"LockActionServices: OpenLock - Code:{code} - PhoneNo:{phoneNo}");
                _sendSmsMessages.SendLockCodeMessage(code.ToString(), phoneNo, ref objApiRespMessage);
            }
            catch (Exception ex)
            {
                _logger.Error($"LockActionServices: OpenLock - Exception - {ex}");
                if (string.IsNullOrWhiteSpace(objApiRespMessage.ErrorMessage))
                    objApiRespMessage.ErrorMessage = "SMS Sending Failed. Check With Support";
            }
        }
    }
}
