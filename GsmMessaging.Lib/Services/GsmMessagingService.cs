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

        public GsmMessagingService(ISerialPortService serialPortService,ILog logger)
        {
            _serialPortService = serialPortService;
            _logger = logger;
        }

        public void InitializeSerialConnection(Action<string> processMsgReceiver = null)
        {
            try
            {
                var lstPorts = _serialPortService.GetSerialComPorts();
                _logger.InfoFormat($"GsmMessagingService: InitializeSerialConnection - Serial Ports Active - {string.Join(",", lstPorts)}");
                if (lstPorts != null && lstPorts.Count > 0)
                {
                    var comPort = !string.IsNullOrWhiteSpace(serialComPort) ? serialComPort : lstPorts[0];
                    _serialPortService.InitializeSerialPortComm(comPort, callBackOnDataReceived: processMsgReceiver);
                    _logger.InfoFormat($"GsmMessagingService: InitializeSerialConnection - Complete on port No - {comPort}");
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
            if(_serialPortService != null)
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
