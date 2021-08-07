using LockServices.Lib.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.WebClientApi
{
    public interface IFiksApi
    {
        Task<UserInfo> LoginApi(string emailId, string password);

        Task<List<LockInformationObject>> GetLockDetails(string emailId);

        Task<List<LockInformationObject>> GetVehicleTaggedList(string emailId); 

        Task<string> GetCodeApi(string emailId, string vehicleNo);

        Task<List<LockStatusDO>> GetLockHistory(string emailId, string vehicleNo,string lockId);

        Task<string> UpdateLockStatus(string emailId, string lockId, string status);

        Task<string> GetAppVersion();

        Task DownloadLatestAppVersion(string version, string downloadPath);

    }
}
