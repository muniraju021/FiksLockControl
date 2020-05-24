using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.DataObjects
{
    public class ApiResponseMessage
    {
        public int HttpStatusCode { get; set; }
        public string LockCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }

        public Exception exception { get; set; }
    }
}
