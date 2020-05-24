using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Services
{
    public class FtpService : IFtpService
    {
        private readonly ILog _logger;
        public FtpService(ILog logger)
        {
            _logger = logger;
        }

        public void DowloadFtpDirectoryContents(string url,string directoryPath,string ftpUserName,string ftpPassword,string destinationFolder)
        {
            _logger.Info($"FtpService: DowloadFtpDirectoryContents - Download Latest Updates Started..");
            try
            {
                NetworkCredential credential = new NetworkCredential(ftpUserName, ftpPassword);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + directoryPath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = credential;
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;

                List<string> lstLines = new List<string>();

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader listReader = new StreamReader(stream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lstLines.Add(listReader.ReadLine());
                    }
                }

                foreach (var line in lstLines)
                {
                    var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var file = tokens[tokens.Length - 1];

                    var fileUrl = url + directoryPath + file;
                    var localFilePath = Path.Combine(destinationFolder, file);
                    FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = credential;

                    using (FtpWebResponse downloadResponse =
                              (FtpWebResponse)downloadRequest.GetResponse())
                    using (Stream sourceStream = downloadResponse.GetResponseStream())
                    using (Stream targetStream = File.Create(localFilePath))
                    {
                        byte[] buffer = new byte[10240];
                        int read;
                        while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            targetStream.Write(buffer, 0, read);
                        }
                    }
                }
                
                _logger.Info($"FtpService: DowloadFtpDirectoryContents - Download Latest Updates Finished..");
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat($"FtpService: DowloadFtpDirectoryContents - Exception:{ex}");
            }
        }
    }
}
