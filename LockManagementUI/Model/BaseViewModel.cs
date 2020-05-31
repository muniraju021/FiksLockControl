﻿using LockServices.Lib.Cache;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockManagementUI.Model
{
    public class BaseViewModel : NotificationObject
    {
        protected readonly ICacheService _cacheService;

        public BaseViewModel(ICacheService cacheService)
        {
            _cacheService = cacheService;
            PopulateUserEmail();
        }

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                RaisePropertyChanged("UserEmail");
            }
        }

        private void PopulateUserEmail()
        {
            var userInfo = _cacheService.GetUserCredentials();
            UserEmail = userInfo?.EmailId;
        }
    }
}
