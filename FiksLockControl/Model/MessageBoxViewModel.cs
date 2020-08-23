using FiksLockControl.Model;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LockServices.Lib.Cache;
using LockServices.Lib.Services;
using LockServices.Lib.DataObjects;

namespace FiksLockControl.Model
{
    public class MessageBoxViewModel : BaseViewModel
    {
        private ILockActionServices _lockActionServices;

        public MessageBoxViewModel(ICacheService cacheService, ILockActionServices lockActionServices) : base(cacheService)
        {
            _lockActionServices = lockActionServices;
            ProgressStatus = "Ready";
        }

        private string _messageContent;
        public string MessageContent
        {
            get { return _messageContent; }
            set
            {
                _messageContent = value;
                RaisePropertyChanged("MessageContent");
            }
        }

        private string _lockCode;
        public string LockCode
        {
            get { return _lockCode; }
            set
            {
                _lockCode = value;
                RaisePropertyChanged("LockCode");
            }
        }

        private string _lockPhoneNo;
        public string LockPhoneNo
        {
            get { return _lockPhoneNo; }
            set
            {
                _lockPhoneNo = value;
                RaisePropertyChanged("LockPhoneNo");
            }
        }

        private string _progressStatus;
        public string ProgressStatus
        {
            get { return _progressStatus; }
            set
            {
                _progressStatus = value;
                RaisePropertyChanged("ProgressStatus");
            }
        }

        public void OpenLock(string code, string lockNo, ref ApiResponseMessage objApiRespMessage)
        {
            try
            {
                //IsBusyIndicator = true;
                ProgressStatus = "In Progress..";
                _lockActionServices.OpenLock(code, lockNo, ref objApiRespMessage);
                if (string.IsNullOrEmpty(objApiRespMessage.ErrorMessage))
                {
                    ProgressStatus = "SMS Sent Status: SENT";
                }
                else
                {
                    ProgressStatus = objApiRespMessage.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                ProgressStatus = $"Error - {ex.Message}";
                throw;
            }
            finally
            {
                //IsBusyIndicator = false;
            }
        }
    }
}
