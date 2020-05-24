using LockServices.Lib.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Cache
{
    public class CacheManagement : ICacheService
    {
        private static Dictionary<string, UserInfo> _userDetails = new Dictionary<string, UserInfo>();
        private static Dictionary<string, LockInformationObject> _lstLockInfo = new Dictionary<string, LockInformationObject>();


        #region Getter
        public UserInfo GetUserCredentials()
        {
            UserInfo objUserInfo = null;
            _userDetails.TryGetValue(CacheConstants.UserLoginDetails, out objUserInfo);
            return objUserInfo;
        }

        public string GetSessionToken()
        {
            var userInfo = GetUserCredentials();
            return userInfo?.JwtToken;
        }

        public List<LockInformationObject> GetLockInformationLstByVehicleNo(List<string> vehicleNo)
        {
            var lst = _lstLockInfo.Where(i => vehicleNo.Exists(j => j == i.Key)).Select(k => k.Value).ToList();
            return lst;
        }

        public LockInformationObject GetLockInformationByVehicleNo(string vehicleNo)
        {
            if(_lstLockInfo.ContainsKey(vehicleNo))
            {
                return _lstLockInfo[vehicleNo];
            }
            return default(LockInformationObject);
        }

        public LockInformationObject GetLockInformationByPhoneNo(string phoneNo)
        {
            var lst = _lstLockInfo.Values;
            return lst.Where(i => i.LockPhNo == phoneNo).FirstOrDefault();
        }

        #endregion

        #region Setter
        public void LoadUserCredentails(UserInfo userInfo)
        {
            if (_userDetails.ContainsKey(CacheConstants.UserLoginDetails))
            {
                _userDetails.Remove(CacheConstants.UserLoginDetails);
            }
            _userDetails.Add(CacheConstants.UserLoginDetails, userInfo);
        }

        public void LoadLockInfoDetails(List<LockInformationObject> lstLockInfo)
        {
            if(lstLockInfo != null)
            {
                var res = lstLockInfo.GroupBy(i => i.VehicleNumber).ToDictionary(g => g.Key, g => g.First());
                _lstLockInfo.Clear();
                _lstLockInfo = res;
            }            
        }

        
        #endregion
    }

    public interface ICacheService
    {
        UserInfo GetUserCredentials();
        string GetSessionToken();
        void LoadUserCredentails(UserInfo userInfo);
        void LoadLockInfoDetails(List<LockInformationObject> lstLockInfo);
        List<LockInformationObject> GetLockInformationLstByVehicleNo(List<string> vehicleNo);
        LockInformationObject GetLockInformationByVehicleNo(string vehicleNo);
        LockInformationObject GetLockInformationByPhoneNo(string phoneNo);
    }

    public class CacheConstants
    {
        public const string UserLoginDetails = "UserLoginDetails";
    }
}
