using GsmMessaging.Lib.Services;
using LockServices.Lib.DataObjects;
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
    public interface ISendSmsMessages
    {
        void SendLockCodeMessage(string code, string phoneNo, ref ApiResponseMessage obApiResponseMsg);
        //void ProcessReceivedMessage(string message);
    }

    public class SendSmsMessages : ISendSmsMessages,IDisposable
    {
        private Dictionary<string, string> _lstCommands = new Dictionary<string, string>
        {
            { "AT{0}","OK" },
            { "AT+CMGF=1{0}","OK" },
            { "AT+CMGS=\"{0}\", 129{1}",">"}
        };
        private int _commandIndex = -1;
        private IGsmMessagingService _gsmMessagingService;
        private string _lockCode = string.Empty;
        private string _lockPhoneNo = string.Empty;
        private char CtrlZ = (char)26;
        private char CR = (char)13;

        public static ConcurrentQueue<string> ReceivedMessages = new ConcurrentQueue<string>();
        private static long _gsmMessageReceiveTimeoutInMs = 20000;
        private readonly ILog _logger;

        public SendSmsMessages(IGsmMessagingService gsmMessagingService,ILog logger)
        {
            _gsmMessagingService = gsmMessagingService;
            _logger = logger;
        }

        public void SendLockCodeMessage(string code, string phoneNo, ref ApiResponseMessage obApiResponseMsg)
        {
            _lockCode = code;
            _lockPhoneNo = phoneNo;
            SendLockCodeMessage(ref obApiResponseMsg);
        }
        
        private void SendLockCodeMessage(ref ApiResponseMessage obApiResponseMsg)
        {
            if(_gsmMessagingService.IsSmsConnectionActive())
            {
                _logger.Warn($"SendSmsMessages: SendLockCodeMessage - Initalizing SerialPort Connection");
                _gsmMessagingService.InitializeSerialConnection(ReceiveSmsMessage.ProcessSerialPortMessages);
            }

            foreach (var cmd in _lstCommands)
            {
                if(!cmd.Key.Contains("{1}"))
                    _gsmMessagingService.SendMessage(string.Format(cmd.Key, CR));
                else
                    _gsmMessagingService.SendMessage(string.Format(cmd.Key, _lockPhoneNo, CR));
                Thread.Sleep(1000);
            }

            string result = string.Empty;
            long elapsedTime = 0;
            while (string.IsNullOrWhiteSpace(result) && elapsedTime <= _gsmMessageReceiveTimeoutInMs)
            {
                if (ReceivedMessages.TryDequeue(out result))
                {
                    _logger.Info($"SendSmsMessages: SendLockCodeMessage - Message Receive:{result}");
                    var key = _lstCommands.Keys.ToList()[_lstCommands.Count - 1];
                    if (result.Trim() == _lstCommands[key])
                    {
                        _gsmMessagingService.SendMessage($"{_lockCode}{CtrlZ}{CR}");
                        obApiResponseMsg.StatusMessage = "SMS Sent";
                    }
                    break;
                }
                Thread.Sleep(1000);
                elapsedTime += 1000;
                _logger.Info($"SendSmsMessages: SendLockCodeMessage - ElapsedTime:{elapsedTime}");
            }
            if(string.IsNullOrWhiteSpace(obApiResponseMsg.StatusMessage))
                obApiResponseMsg.ErrorMessage = "SMS Sending Timeout";

        }

        //public void ProcessReceivedMessage(string message)
        //{
        //    if(!string.IsNullOrWhiteSpace(message))
        //    {
        //        message = message.Trim();
        //        if (_commandIndex != -1)
        //        {
        //            var key = _lstCommands.Keys.ToList()[_commandIndex];
        //            var expectedRes = _lstCommands[key];
        //            if (message.Contains(expectedRes.Trim()))
        //            {
        //                SendLockCodeMessage();
        //            }
        //        }
        //    }
        //}

        public void Dispose()
        {
            if (_gsmMessagingService != null)
                _gsmMessagingService.Dispose();
        }
    }
}
