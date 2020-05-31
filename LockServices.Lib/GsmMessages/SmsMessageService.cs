using GsmMessaging.Lib.Services;
using LockServices.Lib.Services;
using LockServices.Lib.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockServices.Lib.GsmMessages
{
    public interface ISmsMessageService
    {
        //Read Sms Messages
        string GetMessageAtIndexCommand(string index);
        //Get All Message From Inbox
        string GetMessageAllCommand();
        //Delete Sms Messages
        string DeleteMsgAtIndexCommand(string index);
        //Processing
        void ProcessReceivedMessages();
        //Select Storage Command
        string GetSelectStorageCommand();
    }
    public class SmsMessageService : ISmsMessageService
    {
        private readonly IGsmMessagingService _gsmMessagingService;
        private static ILog _logger;
        private ILockActionServices _lockActionService;

        public SmsMessageService(IGsmMessagingService gsmMessagingService, ILog logger, ILockActionServices lockActionService)
        {
            _gsmMessagingService = gsmMessagingService;
            _lockActionService = lockActionService;
            _logger = logger;
            ProcessReceivedMessages();
        }

        public string GetMessageAllCommand()
        {
            return string.Format(SmsMessageConstants.ReadMessageAll);
        }

        public string GetMessageAtIndexCommand(string index)
        {
            return string.Format(SmsMessageConstants.ReadMessageAtIndex, index);
        }

        public string DeleteMsgAtIndexCommand(string index)
        {
            return string.Format(SmsMessageConstants.DeleteMsgAtIndex, index);
        }

        public string GetSelectStorageCommand()
        {
            return string.Format(SmsMessageConstants.SelectStorageCommand);
        }

        public void ProcessReceivedMessages()
        {
            if (_gsmMessagingService.IsSmsConnectionActive())
            {
                _logger.Warn($"SendSmsMessages: SendLockCodeMessage - Initalizing SerialPort Connection");
                _gsmMessagingService.InitializeSerialConnection(ReceiveSmsMessage.ProcessSerialPortMessages);
            }

            Task.Factory.StartNew(() =>
            {
                List<string> partialMsg = new List<string>();
                while (true)
                {
                    try
                    {
                        string msg;
                        while (ReceiveSmsMessage.ReceivedMessagesColl.TryDequeue(out msg))
                        {
                            _logger.Info($"ReceivedSmsMessage: ProcessReceivedMessages - ConsolidatedMessage:[{msg}]");

                            //Receive Msg At Index
                            if (msg.StartsWith(SmsMessageConstants.ReceiveMsgAlert))
                            {
                                var msgIndex = msg.Substring(msg.IndexOf(",") + 1);
                                _gsmMessagingService.SendMessage(this.GetMessageAtIndexCommand(msgIndex));
                                Thread.Sleep(1000);
                                _gsmMessagingService.SendMessage(this.DeleteMsgAtIndexCommand(msgIndex));
                            }
                            //Receive Actual Message
                            else if (msg.StartsWith(SmsMessageConstants.ReceiveActualMsgAlert) || msg.StartsWith(SmsMessageConstants.ReceiveActualMsgAllAlert))
                            {
                                partialMsg.Clear();
                                partialMsg.Add(msg);
                            }
                            else if (partialMsg.Count > 0)
                            {
                                partialMsg.Add(msg);
                                ProcessActualMessages(partialMsg);
                                partialMsg.Clear();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"ReceivedSmsMessage: ProcessReceivedMessages - Error:{ex}");
                    }
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async void ProcessActualMessages(List<string> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Message:[{messages[i]}]");
                var msg = messages[i];

                //Receive Msg At Index
                if (msg.Contains(SmsMessageConstants.ReceiveActualMsgAlert))
                {
                    var lineContent = msg.Split(',');
                    var senderPhNo = lineContent[1].Replace("\"", string.Empty).Replace("+91", string.Empty);
                    DateTime msgSentTime;
                    DateTime.TryParse(lineContent[3], out msgSentTime);

                    _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Message:[{messages[++i]}]");
                    var lockStatus = messages[i];
                    var result = await _lockActionService.UpdateLockStatus(senderPhNo, lockStatus);
                }
                else if(msg.Contains(SmsMessageConstants.ReceiveActualMsgAllAlert)) //Receive All Messages
                {
                    var lineContent = msg.Split(',');
                    var msgIndex = lineContent[0].Substring(lineContent[0].IndexOf(":") + 1).Trim();
                    _gsmMessagingService.SendMessage(this.DeleteMsgAtIndexCommand(msgIndex));

                    var senderPhNo = lineContent[2].Replace("\"", string.Empty).Replace("+91", string.Empty);
                    DateTime msgSentTime;
                    DateTime.TryParse(lineContent[4], out msgSentTime);

                    _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Message:[{messages[++i]}]");
                    var lockStatus = messages[i];
                    var result = await _lockActionService.UpdateLockStatus(senderPhNo, lockStatus);
                }
            }
        }

        
    }
}
