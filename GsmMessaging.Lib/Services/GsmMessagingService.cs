using log4net;
using SerialPortComLib.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsmMessaging.Lib.Services
{
    public class GsmMessagingService : IGsmMessagingService
    {
        private readonly ISerialPortService _serialPortService;
        private readonly ILog _logger;
        private readonly string serialComPort = ConfigurationManager.AppSettings["SerialComPort"];
        private readonly bool useDynamicPort = false;

        public GsmMessagingService(ISerialPortService serialPortService, ILog logger)
        {
            _serialPortService = serialPortService;
            _logger = logger;
            useDynamicPort = ConfigurationManager.AppSettings["UseDynamicPort"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["UseDynamicPort"]) : false;
        }

        public void InitializeSerialConnection(Action<string> processMsgReceiver = null)
        {
            try
            {
                var lstPorts = new List<string>();
                var comPortVal = serialComPort;
                if (useDynamicPort)
                {
                    lstPorts = _serialPortService.GetSerialComPorts();
                    _logger.InfoFormat($"GsmMessagingService: InitializeSerialConnection - Serial Ports Active - {string.Join(",", lstPorts)}");
                    comPortVal = lstPorts?.Count > 0 ? lstPorts[0] : serialComPort;
                }
                if (!string.IsNullOrWhiteSpace(comPortVal))
                {
                    _logger.InfoFormat($"GsmMessagingService: InitializeSerialConnection - on Port - {comPortVal}");
                    _serialPortService.InitializeSerialPortComm(comPortVal, callBackOnDataReceived: processMsgReceiver);
                    _logger.InfoFormat($"GsmMessagingService: InitializeSerialConnection - Complete on port No - {comPortVal}");
                }
                else
                    throw new Exception("No Serial Ports Exists");
            }
            catch (Exception ex)
            {
                _logger.Error($"GsmMessagingService: InitializeSerialConnection - Error in Initializing Serial Comm - Exception:{ex}");
            }

        }

        public bool SendMessage(string message)
        {
            _logger.InfoFormat($"GsmMessagingService: SendingMessage - {message}");
            return _serialPortService.WriteToDevice<string>(message);
        }

        public bool IsSmsConnectionActive()
        {
            if (_serialPortService != null)
                return _serialPortService.IsSerialPortConnectionActive();
            return false;
        }

        public void Dispose()
        {
            _logger.InfoFormat($"GsmMessagingService: Dispose");
            _serialPortService?.CloseConnection();
        }
    }
}
