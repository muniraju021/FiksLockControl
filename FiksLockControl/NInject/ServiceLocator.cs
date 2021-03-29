using FiksLockControl.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiksLockControl.NInject
{
    public class ServiceLocator
    {
        private readonly IKernel _kernel;

        public ServiceLocator()
        {
            _kernel = new StandardKernel(new ServiceModule());
        }

        public GenerateCodesViewModel ObjGenerateCodesViewModel
        {
            get { return _kernel.Get<GenerateCodesViewModel>(); }
        }

        public DashboardViewModel ObjDashboardViewModel
        {
            get { return _kernel.Get<DashboardViewModel>(); }
        }

        public LoginViewModel ObjLoginViewModel
        {
            get { return _kernel.Get<LoginViewModel>(); }
        }

        public ApplicationViewModel ObjApplicationViewModel
        {
            get { return _kernel.Get<ApplicationViewModel>(); }
        }

        public LockHistoryViewModel ObjLockHistoryViewModel
        {
            get { return _kernel.Get<LockHistoryViewModel>(); }
        }

        public MessageBoxViewModel ObjMessageBoxViewModel
        {
            get { return _kernel.Get<MessageBoxViewModel>(); }
        }

        public GenericMessageBoxViewModel ObjGenericMessageBoxViewModel
        {
            get { return _kernel.Get<GenericMessageBoxViewModel>(); }
        }

        public ReportViewModel ObjReportViewModel
        {
            get { return _kernel.Get<ReportViewModel>(); }
        }

        public AdminSettingsModel ObjAdminSettingsModel
        {
            get { return _kernel.Get<AdminSettingsModel>(); }
        }
    }
}
