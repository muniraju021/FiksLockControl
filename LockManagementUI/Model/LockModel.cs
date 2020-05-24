using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockManagementUI.Model
{
    public class LockModel : NotificationObject
    {
        private string _vehicleNo;
        private string _lockStatus;
        private string _powerStatus;
        private string _lockOwner;

        public string VehicleNo { get { return _vehicleNo; } set { _vehicleNo = value; } }
        public string LockStatus { get { return _lockStatus; } set { _lockStatus = value; } }
        public string PowerStatus { get { return _powerStatus; } set { _powerStatus = value; } }
        public string LockOwner { get { return _lockOwner; } set { _lockOwner = value; } }

        public string RowCount => "3";
        public string ColumnCount => "3";

    }
}
