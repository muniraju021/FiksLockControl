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
using log4net;

namespace FiksLockControl.Model
{
    public class GenericMessageBoxViewModel : BaseViewModel
    {
        
        public GenericMessageBoxViewModel(ICacheService cacheService, ILog logger) 
            : base(cacheService,logger)
        {
            TitleName = "Info";
            TitleIcon = "Information";
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

        private string _titleName;
        public string TitleName
        {
            get { return _titleName; }
            set
            {
                _titleName = value;
                RaisePropertyChanged("TitleName");
            }
        }

        private string _titleIcon;
        public string TitleIcon
        {
            get { return _titleIcon; }
            set
            {
                _titleIcon = value;
                RaisePropertyChanged("TitleIcon");
            }
        }
       
    }
}
