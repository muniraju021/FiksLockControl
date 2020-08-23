
using GsmMessaging.Lib.Services;
using LockServices.Lib.Cache;
using LockServices.Lib.GsmMessages;
using LockServices.Lib.Services;
using LockServices.Lib.WebClientApi;
using log4net;
using Ninject;
using Ninject.Modules;
using SerialPortComLib.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiksLockControl.NInject
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ConfigurationManager.AppSettings["log4netSource"]));
            ILog logger = log4net.LogManager.GetLogger(typeof(Startup));

            Bind<ILog>().ToConstant(logger);
            Bind<ISendSmsMessages>().To<SendSmsMessages>();
            Bind<ISerialPortService>().To<SerialPortService>().InSingletonScope();
            Bind<IGsmMessagingService>().To<GsmMessagingService>().InSingletonScope();
            Bind<IReceiveSmsMessage>().To<ReceiveSmsMessage>().InSingletonScope();
            Bind<ISmsMessageService>().To<SmsMessageService>().InSingletonScope();
            Bind<ILockActionServices>().To<LockActionServices>();
            Bind<IFiksApi>().To<FiksApiClient>().InSingletonScope();
            Bind<ICacheService>().To<CacheManagement>();
            Bind<IFtpService>().To<FtpService>();
            Bind<IVersionCheck>().To<VersionCheck>();
        }
    }
}
