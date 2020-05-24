using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.DataObjects
{
    public class UserInfo
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }
    }
}
