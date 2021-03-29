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
using System.Threading;
using log4net;

namespace FiksLockControl.Model
{
    public class MessageBoxViewModel : BaseViewModel
    {
        private ILockActionServices _lockActionServices;

        public MessageBoxViewModel(ICacheService cacheService, ILockActionServices lockActionServices, ILog logger) 
            : base(cacheService,logger)
        {
            _lockActionServices = lockActionServices;
            ProgressStatus = "Ready";
        }

        private bool _isBusyIndicator;
        public bool IsBusyIndicator
        {
            get
            {
                return _isBusyIndicator;
            }
            set
            {
                _isBusyIndicator = value;
                RaisePropertyChanged("IsBusyIndicator");
            }
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

        private string _messageBoxTitle;
        public string MessageBoxTitle
        {
            get { return _messageBoxTitle; }
            set
            {
                _messageBoxTitle = value;
                RaisePropertyChanged("MessageBoxTitle");
            }
        }

        private bool _openLockButtonStatus;
        public bool OpenLockButtonStatus
        {
            get { return _openLockButtonStatus; }
            set
            {
                _openLockButtonStatus = value;
                RaisePropertyChanged("OpenLockButtonStatus");
            }
        }

        public async Task<ApiResponseMessage> OpenLock(string code, string lockNo, ApiResponseMessage objApiRespMessage)
        {
            try
            {
                IsBusyIndicator = true;
                ProgressStatus = "SMS Sending..";
                _lockActionServices.OpenLock(code, lockNo, ref objApiRespMessage);
                await Task.Delay(TimeSpan.FromSeconds(5));
                if (string.IsNullOrEmpty(objApiRespMessage.ErrorMessage))
                {
                    ProgressStatus = "SMS Sent Successfully";
                }
                else
                {
                    ProgressStatus = objApiRespMessage.ErrorMessage;
                    OpenLockButtonStatus = true;
                }

            }
            catch (Exception ex)
            {
                Thread.Sleep(10000);
                ProgressStatus = $"Error - {ex.Message}";
                Logger.Error($"Error in OpenLock - ", ex);
                OpenLockButtonStatus = true;
                throw;
            }
            finally
            {
                IsBusyIndicator = false;
            }
            return objApiRespMessage;
        }
    }
}
