using LockServices.Lib.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public interface ILockActionServices
    {
        Task<bool> Login(string emailId, string password);

        Task<List<LockInformationObject>> GetVehiclesTagged(string emailId);
        
        Task<List<LockInformationObject>> GetLockDetailsByEmailId(string emailId);

        Task<ApiResponseMessage> GenerateAndGetCode(string emailId, string vehicleNo);

        Task<List<LockStatusDO>> GetLockHistory(string emailId, string vehicleNo);

        Task<string> UpdateLockStatus(string vehicleNo, string status);
        
        void OpenLock(string code, string phoneNo, ref ApiResponseMessage objApiRespMessage);

        void ReadAllMessages();
    }
}
