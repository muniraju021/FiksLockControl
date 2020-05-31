using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsmMessaging.Lib.Services
{
    public interface IGsmMessagingService : IDisposable
    {
        void InitializeSerialConnection(Action<string> processMsgReceiver = null);
        bool SendMessage(string message);
        bool IsSmsConnectionActive();
    }
}
