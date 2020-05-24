using LockServices.Lib.Services;
using Squirrel;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockManagementUI.Model
{
    public class LoginViewModel : NotificationObject
    {
        private readonly ILockActionServices _lockActionServices;
        private readonly IFtpService _ftpService;
        private readonly string FtpUserName = ConfigurationManager.AppSettings["FtpUserName"];
        private readonly string FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
        private readonly string FtpUrl = ConfigurationManager.AppSettings["FtpUrl"];
        private readonly string FtpDirectoryPath = ConfigurationManager.AppSettings["FtpDirectoryPath"];
        private readonly string ReleaseUpdateDestinationPath = ConfigurationManager.AppSettings["ReleaseUpdateDestinationPath"];
        private readonly bool IUpdateAvailable = Convert.ToBoolean(ConfigurationManager.AppSettings["IUpdateAvailable"]);

        public LoginViewModel(ILockActionServices lockActionServices, IFtpService ftpService)
        {
            _lockActionServices = lockActionServices;
            _ftpService = ftpService;
            if(IUpdateAvailable)
                CheckForUpdates();
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
        
        private async Task CheckForUpdates()
        {
            _ftpService.DowloadFtpDirectoryContents(FtpUrl, FtpDirectoryPath, FtpUserName, FtpPassword, ReleaseUpdateDestinationPath);

            using (var manager = new UpdateManager(ReleaseUpdateDestinationPath))
            {
                await manager.UpdateApp();
            }
        }

        public async Task<bool> Login(string userName, string password)
        {
            bool isLoggedIn = false;
            try
            {
                IsBusyIndicator = true;
                isLoggedIn = await _lockActionServices.Login(userName, password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsBusyIndicator = false;
            }
            return isLoggedIn;
        }

    }
}
