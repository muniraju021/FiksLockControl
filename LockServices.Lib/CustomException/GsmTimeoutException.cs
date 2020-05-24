using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.CustomException
{
    public class GsmTimeoutException : Exception
    {
        public GsmTimeoutException(string message) : base(message)
        {

        }
        public string Code { get; set; }
    }
}
