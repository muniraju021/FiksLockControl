using GsmMessaging.Lib.Services;
using LockServices.Lib.Services;
using LockServices.Lib.Utilities;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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

        string SentAtCommand();
        
    }
    public class SmsMessageService : ISmsMessageService
    {
        private readonly IGsmMessagingService _gsmMessagingService;
        private static ILog _logger;
        private IApiService _apiService;
        private static BlockingCollection<string> _deleteSmsMessageslst = new BlockingCollection<string>();

        public SmsMessageService(IGsmMessagingService gsmMessagingService, ILog logger, IApiService apiService)
        {
            _gsmMessagingService = gsmMessagingService;
            _apiService = apiService;
            _logger = logger;
            //ProcessReceivedMessages();
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

        public string SentAtCommand()
        {
            return string.Format(SmsMessageConstants.AtCommand);
        }

        private void DeleteMessageAtIndex(string msgIndex)
        {
            var cmdStr = this.DeleteMsgAtIndexCommand(msgIndex);
            _deleteSmsMessageslst.Add(cmdStr);
        }
        
        public void ProcessReceivedMessages()
        {
            if (!_gsmMessagingService.IsSmsConnectionActive())
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
                                //Thread.Sleep(1000);
                                //_gsmMessagingService.SendMessage(this.DeleteMsgAtIndexCommand(msgIndex));

                                this.DeleteMessageAtIndex(msgIndex);
                                if(msgIndex == "1")
                                    _gsmMessagingService.SendMessage(this.SentAtCommand());
                            }
                            //Receive Actual Message
                            else if (msg.StartsWith(SmsMessageConstants.ReceiveActualMsgAlert) || msg.StartsWith(SmsMessageConstants.ReceiveActualMsgAllAlert)
                                        || msg.StartsWith(SmsMessageConstants.ReceiveActualMsgAlert01))
                            {
                                partialMsg.Clear();
                                partialMsg.Add(msg);
                            }
                            else if (partialMsg.Count > 0)
                            {
                                partialMsg.Add(msg);
                                ProcessActualMessages(partialMsg).Wait();
                                partialMsg.Clear();
                            }
                            else if(msg.StartsWith(SmsMessageConstants.ReplyAtCommand))
                            {
                                var delStr = string.Empty;
                                if (_deleteSmsMessageslst.TryTake(out delStr))
                                {
                                    _logger.Info($"SmsMessageService: DeleteMsgAtIndexCommand Sending - [{delStr}]");
                                    _gsmMessagingService.SendMessage(delStr);
                                }
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

        private async Task ProcessActualMessages(List<string> messages)
        {
            try
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
                        _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - UpdateLockStatus Intiated Case 1 - LockStatus:{lockStatus}");
                        var result = await _apiService.UpdateLockStatus(senderPhNo, lockStatus);
                    }
                    //Receive Realtime Msg
                    if (msg.Contains(SmsMessageConstants.ReceiveActualMsgAlert01))
                    {
                        _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Before processing Message:[{messages[i]}]");

                        //var lineContent = msg.Split(',');
                        //var strPh = lineContent[0].Substring(lineContent[0].IndexOf("\"+"));
                        //var senderPhNo = strPh.Replace("\"", string.Empty).Replace("+91", string.Empty);
                        //DateTime msgSentTime;
                        //DateTime.TryParse(lineContent[3], out msgSentTime);

                        var lineContent = msg.SplitToCsv();
                        var index = lineContent[0].IndexOf("+91");
                        if (index != -1)
                        {
                            var senderPhNo = lineContent[0].Substring(index + 3);
                            DateTime msgSentTime;
                            DateTime.TryParseExact(lineContent[2].Replace("\"", string.Empty), "yy/MM/dd,HH:mm:ss+ff", CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None, out msgSentTime);
                            _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Message:[{messages[++i]}]");
                            var lockStatus = messages[i];
                            _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - UpdateLockStatus Intiated Case 2 - LockStatus:{lockStatus}");
                            var result = await _apiService.UpdateLockStatus(senderPhNo, lockStatus);
                        }
                        _logger.Warn($"ReceivedSmsMessage: Invalid Message - Message[msg]");
                    }
                    else if (msg.Contains(SmsMessageConstants.ReceiveActualMsgAllAlert)) //Receive All Messages
                    {
                        var lineContent = msg.Split(',');
                        var msgIndex = lineContent[0].Substring(lineContent[0].IndexOf(":") + 1).Trim();

                        this.DeleteMessageAtIndex(msgIndex);
                        if(msgIndex == "1")
                            _gsmMessagingService.SendMessage(this.SentAtCommand());
                        //_gsmMessagingService.SendMessage(this.DeleteMsgAtIndexCommand(msgIndex));

                        var senderPhNo = lineContent[2].Replace("\"", string.Empty).Replace("+91", string.Empty);
                        DateTime msgSentTime;
                        DateTime.TryParse(lineContent[4], out msgSentTime);

                        _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - Message:[{messages[++i]}]");
                        var lockStatus = messages[i];
                        _logger.Info($"ReceivedSmsMessage: ProcessActualMessages - UpdateLockStatus Intiated Case 3 - LockStatus:{lockStatus}");
                        var result = await _apiService.UpdateLockStatus(senderPhNo, lockStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat($"ProcessActualMessgaes: Exception thrown {ex}",ex);
            }            
        }
    }
}
