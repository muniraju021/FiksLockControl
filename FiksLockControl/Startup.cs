using LockServices.Lib.Services;
using LockServices.Lib.WebClientApi;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FiksLockControl
{
    public class Startup
    {
        //public static IUnityContainer Container;
        private ILog _logger;
        private static string BaseAddress = ConfigurationManager.AppSettings["FiksBaseAddress"];

    }
}
