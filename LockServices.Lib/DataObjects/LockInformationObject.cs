using LockServices.Lib.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LockServices.Lib.DataObjects
{
    public class LockInformationObject
    {
        [JsonProperty("lockId")]
        public string LockId { get; set; }

        [JsonProperty("vehicleNumber")]
        public string VehicleNumber { get; set; }

        [JsonProperty("vehicleName")]
        public string VehicleName { get; set; }

        [JsonProperty("codeCounter")]
        public int CodeCounter { get; set; }

        [JsonProperty("lockRank")]
        public string LockRank { get; set; }

        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("tagDate")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? TagDate { get; set; }

        [JsonProperty("lockEndDate")]
        //[JsonConverter(typeof(CustomDateConverter))]
        //public DateTime? LockEndDate { get; set; }
        public string LockEndDate { get; set; }

        [JsonProperty("lockPhNo")]
        public string LockPhNo { get; set; }

        [JsonProperty("lockPhoneNumber")]
        private string LockPhoneNumber { set { LockPhNo = value; } }

        [JsonProperty("dealerID")]
        public string DealerID { get; set; }

        [JsonProperty("dealerName")]
        public string DealerName { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        public string Address { get; set; }

        [JsonProperty("codeList")]
        public List<LockCodeList> CodeList { get; set; }

        [JsonProperty("overallStatus")]
        public List<LockStatusDO> OverallStatus { get; set; }

        public string LatestLockCode
        {
            get
            {
                if (CodeList != null && CodeList.Count > 0)
                {
                    return CodeList[0].Code;
                }
                return string.Empty;
            }
        }

        public string LatestLastUpdatedTime
        {
            get
            {
                if(OverallStatus != null && OverallStatus.Count > 0)
                {
                    int lastStatusIndex = CodeList[0].Status.Count - 1;
                    return $"{OverallStatus[0].LockStatus} {OverallStatus[0].LastUpdatedTime?.ToString("yyyy-MM-dd HH:mm:ss")}";

                }
                return string.Empty;
            }
        }

        public string LockStatusIcon
        {
            get
            {
                if (LatestLastUpdatedTime != null && LatestLastUpdatedTime.Contains("LOCK_OPEN"))
                    return "LockOpen";
                else
                    return "Lock";
            }
        }

        public string LockStatus
        {
            get
            {
                if (OverallStatus != null && OverallStatus.Count > 0)
                    return OverallStatus[0].LockStatus;
                else
                    return string.Empty;
            }
        }

        public string LockStatusUpdateDateTime
        {
            get
            {
                if (OverallStatus != null && OverallStatus.Count > 0 && OverallStatus[0].LastUpdatedTime != null)
                    return OverallStatus[0].LastUpdatedTime?.ToString("dd-MM-yyyy HH:mm:ss");
                else
                    return string.Empty;
            }
        }

        public DialogHostType DialogHostTypeInstance; 
    }

    public class LockCodeList
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("status")]
        public List<LockStatusDO> Status { get; set; }
    }

    public class LockStatusDO
    {
        [JsonProperty("lockStatus")]
        public string LockStatus { get; set; }

        [JsonProperty("lastUpdatedTime")]
        public DateTime? LastUpdatedTime { get; set; }

        public string Status { get { return LockStatus + " " + LastUpdatedTime?.ToString("yyyy-MM-dd HH:mm:ss"); } }
    }

    public enum DialogHostType
    {
        OpenLock,
        GenericeMessageYesOrNo,
        LockHistory
    }
}
