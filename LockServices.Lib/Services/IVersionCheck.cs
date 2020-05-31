using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public interface IVersionCheck
    {
        Task<string> GetAppVersion();

        Task DownloadLatestAppVersion(string version, string downloadPath);
    }
}
