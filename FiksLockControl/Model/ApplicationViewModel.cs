using LockServices.Lib.Cache;
using LockServices.Lib.GsmMessages;
using LockServices.Lib.Services;
using log4net;
using Ninject;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO.Compression;
using Squirrel;
using FiksLockControl.Extensions;
using System.Configuration;

namespace FiksLockControl.Model
{
    public class ApplicationViewModel : BaseViewModel
    {
        IKernel _kernel;
        IReceiveSmsMessage _receiveSmsMessage;
        private readonly IVersionCheck _versionCheck;
        private readonly string ReleaseUpdateDestinationPath = ConfigurationManager.AppSettings["ReleaseUpdateDestinationPath"];
        private readonly bool IUpdateAvailable = Convert.ToBoolean(ConfigurationManager.AppSettings["IUpdateAvailable"]);
        private readonly ILog _logger;

        public ApplicationViewModel(IKernel kernel, ICacheService cacheService, IVersionCheck versionCheck, ILog logger)
            : base(cacheService, logger)
        {
            _kernel = kernel;
            //_receiveSmsMessage = recieveSmsMsg;
            _versionCheck = versionCheck;
            _logger = logger;
            //UserEmail = "muniraju021@gmail.com";
        }

        public IKernel Kernel { get { return _kernel; } }

        private string _newVersionMsg;
        public string NewVersionMsg
        {
            get { return _newVersionMsg; }
            set
            {
                _newVersionMsg = value;
                RaisePropertyChanged("NewVersionMsg");
            }
        }

        private string _currentVersion;
        public string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = value;
                RaisePropertyChanged("CurrentVersion");
            }
        }

        public Task Initialize()
        {
            NewVersionMsg = "Checking for Updates..";
            Task.Factory.StartNew(async () =>
            {
                await CheckForUpdates();
            }).Wait();
            return default(Task);
        }


        private async Task CheckForUpdates()
        {
            //_ftpService.DowloadFtpDirectoryContents(FtpUrl, FtpDirectoryPath, FtpUserName, FtpPassword, ReleaseUpdateDestinationPath);

            var latestVersion = await _versionCheck.GetAppVersion();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            CurrentVersion = version;
            if (!latestVersion.Equals(version))
            {
                _logger.Info($"LoginViewModel: New Application updates available version:{latestVersion}");
                NewVersionMsg = $"Downloading New Version - {latestVersion}";

                var fileName = Path.Combine(ReleaseUpdateDestinationPath, $"Download{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                await _versionCheck.DownloadLatestAppVersion(latestVersion, fileName);

                using (ZipArchive archive = ZipFile.OpenRead(fileName))
                {
                    archive.ExtractToDirectory(ReleaseUpdateDestinationPath, true);
                }
                File.Delete(fileName);
                NewVersionMsg = $"New Version Available - {latestVersion}. Restart Application";

                using (var manager = new UpdateManager(ReleaseUpdateDestinationPath))
                {
                    await manager.UpdateApp();
                    NewVersionMsg = $"New Version Available - {latestVersion}. Restart Application";
                }
            }
            else
            {
                NewVersionMsg = string.Empty;
                _logger.Info($"LoginViewModel: No Application updates available");
            }
        }

    }
}
