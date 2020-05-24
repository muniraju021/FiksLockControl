using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public interface IFtpService
    {
        void DowloadFtpDirectoryContents(string url, string directoryPath, string ftpUserName, string ftpPassword, string destinationFolder);
    }
}
