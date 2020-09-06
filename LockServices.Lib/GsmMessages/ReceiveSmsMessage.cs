using GsmMessaging.Lib.Services;
using LockServices.Lib.Utilities;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockServices.Lib.GsmMessages
{
    public interface IReceiveSmsMessage
    {
        void InitializeSerilaComm();
    }

    public class ReceiveSmsMessage : IReceiveSmsMessage
    {
        private readonly IGsmMessagingService _gsmMessagingService;
        public static ConcurrentQueue<string> ReceivedMessagesColl = new ConcurrentQueue<string>();
        public static StringBuilder ReceivedMessagesStr = new StringBuilder();
        private static ILog _logger;
        private readonly ISmsMessageService _smsMessageService;

        public ReceiveSmsMessage(IGsmMessagingService gsmMessagingService,ILog logger, ISmsMessageService smsMsgService)
        {
            _gsmMessagingService = gsmMessagingService;
            _logger = logger;
            _smsMessageService = smsMsgService;
            InitializeSerilaComm(); 
        }

        public void InitializeSerilaComm()
        {
            try
            {
                _logger.Info($"ReceiveSmsMessage: InitializeSerilaComm - Started..");
                _gsmMessagingService.InitializeSerialConnection(ProcessSerialPortMessages);
                _gsmMessagingService.SendMessage(_smsMessageService.GetSelectStorageCommand());
                _gsmMessagingService.SendMessage(_smsMessageService.GetMessageAllCommand());
                _logger.Info($"ReceiveSmsMessage: InitializeSerilaComm - Completed..");
            }
            catch (Exception ex)
            {
                _logger.Error($"ReceiveSmsMessage: InitializeSerilaComm - Error:{ex}");
            }
            
        }

        public static void ProcessSerialPortMessages(string message)
        {
            try
            {
                //_logger.Info($"ReceiveSmsMessage: ProcessSerialPortMessages - Message:[{message}]");
                if (message.Trim().Equals(">"))
                {
                    SendSmsMessages.ReceivedMessages.Enqueue(message);
                }
                else
                {                    
                    ReceivedMessagesStr.Append(message);
                    if(message.EndsWith("\n"))
                    {
                        var lstMessages = ReceivedMessagesStr.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var msg in lstMessages)
                        {
                            ReceivedMessagesColl.Enqueue(msg);
                        }
                        ReceivedMessagesStr.Length = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ReceiveSmsMessage: ProcessReceivedMessages - Mesage:{message} - Error:{ex}");
            }
            
        }
        
    }
}
