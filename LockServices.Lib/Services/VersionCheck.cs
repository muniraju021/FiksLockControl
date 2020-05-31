using LockServices.Lib.WebClientApi;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public class VersionCheck : IVersionCheck
    {
        private readonly IFiksApi _fiksApi;
        private readonly ILog _logger;

        public VersionCheck(IFiksApi fiksApi, ILog logger)
        {
            _fiksApi = fiksApi;
            _logger = logger;
        }

        public async Task DownloadLatestAppVersion(string version, string downloadPath)
        {
            try
            {
                await _fiksApi.DownloadLatestAppVersion(version, downloadPath);
            }
            catch (Exception ex)
            {

                _logger.Error($"VersionCheck - Error - {ex}");
            }
            
        }

        public async Task<string> GetAppVersion()
        {
            return await _fiksApi.GetAppVersion();
        }
    }
}
