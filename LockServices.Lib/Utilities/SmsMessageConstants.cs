using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Utilities
{
    public class SmsMessageConstants
    {
        public const char CR = (char)13;

        public static readonly string ReadMessageAll = "AT+CMGL=\"ALL\"" + CR;
        public static readonly string ReadMessageAtIndex = "AT+CMGR={0}" + CR;

        public static readonly string ReceiveMsgAlert = "+CMTI: \"SM\",";
        public static readonly string ReceiveActualMsgAlert = "+CMGR";
        public static readonly string ReceiveActualMsgAllAlert = "+CMGL";

        public static readonly string DeleteMsgAtIndex = "AT+CMGD={0},0" + CR;

        public static readonly string SelectStorageCommand = "AT+CPMS=\"MT\"";
    }
    
}
