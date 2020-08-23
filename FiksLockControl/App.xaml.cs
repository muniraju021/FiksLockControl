using LockServices.Lib.Services;
using Ninject;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FiksLockControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Startup _objStartup;
        
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjA3MDgxQDMxMzcyZTM0MmUzMFBvWGtPYXh4UjRKd0ZnN0xyVEZjd1FqbS9UMDU1STB6czBPcm9ydk4yVUE9");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _objStartup = new Startup();            
            //CheckForUpdates();
            base.OnStartup(e);
        }

        //private async Task CheckForUpdates()
        //{
        //    using (var manager = new UpdateManager(@"D:\Apps\SquirrelReleases"))
        //    {
        //        await manager.UpdateApp();
        //    }
        //}

    }
}
