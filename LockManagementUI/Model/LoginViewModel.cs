using LockServices.Lib.Services;
using Squirrel;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using LockManagementUI.Extensions;
using log4net;

namespace LockManagementUI.Model
{
    public class LoginViewModel : NotificationObject
    {
        private readonly ILockActionServices _lockActionServices;
        private readonly IFtpService _ftpService;
        private readonly IVersionCheck _versionCheck;
        private readonly ILog _logger;
        private readonly string FtpUserName = ConfigurationManager.AppSettings["FtpUserName"];
        private readonly string FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
        private readonly string FtpUrl = ConfigurationManager.AppSettings["FtpUrl"];
        private readonly string FtpDirectoryPath = ConfigurationManager.AppSettings["FtpDirectoryPath"];
        private readonly string ReleaseUpdateDestinationPath = ConfigurationManager.AppSettings["ReleaseUpdateDestinationPath"];
        private readonly bool IUpdateAvailable = Convert.ToBoolean(ConfigurationManager.AppSettings["IUpdateAvailable"]);

        public LoginViewModel(ILockActionServices lockActionServices, IFtpService ftpService, IVersionCheck versionCheck,ILog logger)
        {
            _lockActionServices = lockActionServices;
            _ftpService = ftpService;
            _versionCheck = versionCheck;
            _logger = logger;
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
            //_ftpService.DowloadFtpDirectoryContents(FtpUrl, FtpDirectoryPath, FtpUserName, FtpPassword, ReleaseUpdateDestinationPath);

            var latestVersion = await _versionCheck.GetAppVersion();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            if (!latestVersion.Equals(version))
            {
                _logger.Info($"LoginViewModel: New Application updates available version:{latestVersion}");

                var fileName = Path.Combine(ReleaseUpdateDestinationPath, $"Download{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                await _versionCheck.DownloadLatestAppVersion(latestVersion, fileName);

                using (ZipArchive archive = ZipFile.OpenRead(fileName))
                {
                    archive.ExtractToDirectory(ReleaseUpdateDestinationPath, true);
                }
                File.Delete(fileName);

                using (var manager = new UpdateManager(ReleaseUpdateDestinationPath))
                {
                    await manager.UpdateApp();
                }
            }
            else
            {
                _logger.Info($"LoginViewModel: No Application updates available");
            }


        }

        public async Task<bool> Login(string userName, string password)
        {
            bool isLoggedIn = false;
            try
            {
                IsBusyIndicator = true;
                isLoggedIn = await _lockActionServices.Login(userName, password);

                Task.Factory.StartNew(() =>
                {
                    if (IUpdateAvailable)
                        CheckForUpdates();
                });
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
