using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LockServices.Lib.DataObjects;
using Newtonsoft.Json.Linq;
using log4net;
using System.Configuration;
using LockServices.Lib.Cache;
using System.IO;

namespace LockServices.Lib.WebClientApi
{
    public class FiksApiClient : IFiksApi
    {
        HttpClient _httpClient = null;
        private readonly ILog _logger;
        private static string FiksBaseAddress = ConfigurationManager.AppSettings["FiksBaseAddress"];
        private readonly ICacheService _cachService;

        public FiksApiClient(ILog logger,ICacheService cacheService)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(FiksBaseAddress);
            _cachService = cacheService;
        }

        public async Task<string> GetCodeApi(string emailId, string vehicleNo)
        {
            _logger.Info($"FiksApiClient: GetCodeApi - emailId:{emailId} - vehicleNo:{vehicleNo}");

            if (string.IsNullOrWhiteSpace(_cachService.GetSessionToken()))
            {
                _logger.Error($"FiksApiClient: Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachService.GetSessionToken());
            var response = await _httpClient.GetAsync($@"api/v1/getCode/{emailId}/{vehicleNo}");
            if (response.IsSuccessStatusCode)
            {
                var jObject = await response.Content.ReadAsAsync<JObject>();
                var code = jObject.SelectToken("message", true).ToString();

                _logger.Info($"FiksApiClient: GetCodeApi - emailId:{emailId} - vehicleNo:{vehicleNo} - Result:{code}");

                return code;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.Error($"FiksApiClient: GetCodeApi - emailId:{emailId} - vehicleNo:{vehicleNo} - Error:{errorMessage}");
                return errorMessage;
            }
            
        }

        public async Task<List<LockInformationObject>> GetLockDetails(string emailId)
        {
            if (string.IsNullOrWhiteSpace(_cachService.GetSessionToken()))
            {
                _logger.Error($"FiksApiClient: GetLockDetails - Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachService.GetSessionToken());
            var response = await _httpClient.GetAsync($@"api/v1/{emailId}/getDashboardScreenValues");
            if(response.IsSuccessStatusCode)
            {
                var jObject = await response.Content.ReadAsAsync<JObject>();
                var message = jObject.SelectToken("message",true).ToString();

                //_logger.Info($"FiksApiClient: GetLockDetails - emailId:{emailId} - Result:{message}");
                _logger.Info($"FiksApiClient: GetLockDetails - emailId:{emailId} - Result:Retreived");


                var obj = JsonConvert.DeserializeObject<List<LockInformationObject>>(message, new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy HH:mm:ss" });
                return obj;
            }

            var error = await response.Content.ReadAsAsync<JObject>();
            _logger.Error($"FiksApiClient: GetLockDetails - emailId:{emailId} - Error:{error}");
            return default(List<LockInformationObject>);
        }

        public async Task<List<LockInformationObject>> GetVehicleTaggedList(string emailId)
        {
            _logger.Info($"FiksApiClient: GetVehicleTaggedList - emailId:{emailId}");

            if (string.IsNullOrWhiteSpace(_cachService.GetSessionToken()))
            {
                _logger.Error($"FiksApiClient: Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachService.GetSessionToken());
            var response = await _httpClient.GetAsync($@"api/v1/{emailId}/sync");
            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.Info($"FiksApiClient: GetVehicleTaggedList - emailId:{emailId} - Result:{message}");

                var obj = JsonConvert.DeserializeObject<List<LockInformationObject>>(message);
                return obj;
            }
            var error = await response.Content.ReadAsAsync<JObject>();
            _logger.Error($"FiksApiClient: GetVehicleTaggedList - emailId:{emailId} - Error:{error}");
            return default(List<LockInformationObject>);
        }

        public async Task<UserInfo> LoginApi(string emailId, string password)
        {
            _logger.Info($"FiksApiClient: LoginApi - emailId:{emailId}");

            var json = JsonConvert.SerializeObject(new
            {
                emailId = emailId,
                password = password
            });
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(@"login", stringContent);
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var userInfoObj = JsonConvert.DeserializeObject<UserInfo>(result);
                _logger.Info($"FiksApiClient: LoginApi - emailId:{emailId} - Result:{result}");
                return userInfoObj;
            }

            _logger.Error($"FiksApiClient: LoginApi - emailId:{emailId} - error:{response}");
            
            return default(UserInfo);

        }

        public async Task<List<LockStatusDO>> GetLockHistory(string emailId, string vehicleNo)
        {
            _logger.InfoFormat($"FiksApiClient: GetLockHistory - emailId:{emailId} - vehicleNo:{vehicleNo}");

            if (string.IsNullOrWhiteSpace(_cachService.GetSessionToken()))
            {
                _logger.Error($"FiksApiClient: GetLockDetails - Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachService.GetSessionToken());
            var response = await _httpClient.GetAsync($@"api/v1/{emailId}/{vehicleNo}/getDashboardHistory");
            if (response.IsSuccessStatusCode)
            {
                var jObject = await response.Content.ReadAsAsync<JObject>();
                var message = jObject.SelectToken("$..overallStatus", true)?.ToString();

                _logger.Info($"FiksApiClient: GetLockHistory - emailId:{emailId} - vehicleNo:{vehicleNo} - Result:{message}");

                var obj = JsonConvert.DeserializeObject<List<LockStatusDO>>(message, new JsonSerializerSettings { DateFormatString = "dd-MM-yyyy HH:mm:ss" });
                return obj;
            }

            var error = await response.Content.ReadAsAsync<JObject>();
            _logger.Error($"FiksApiClient: GetLockHistory - emailId:{emailId} - vehicleNo:{vehicleNo} - Error:{error}");
            return default(List<LockStatusDO>);
        }

        public async Task<string> UpdateLockStatus(string emailId, string lockId, string status)
        {
            _logger.InfoFormat($"FiksApiClient: UpdateLockStatus - emailId:{emailId} - lockId:{lockId} - status:{status}");

            if (string.IsNullOrWhiteSpace(_cachService.GetSessionToken()))
            {
                _logger.Error($"FiksApiClient: UpdateLockStatus - Session Expired");
                throw new Exception("Session Expired");
            }
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachService.GetSessionToken());

            var json = JsonConvert.SerializeObject(new
            {
                lockId = lockId,
                message = status
            });
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($@"api/v1/updateDashBoardByReadingSms/{emailId}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var jObject = await response.Content.ReadAsAsync<JObject>();
                var message = jObject.SelectToken("$.message", true).ToString();

                _logger.InfoFormat($"FiksApiClient: UpdateLockStatus - emailId:{emailId} - lockId:{lockId} - status:{status} - Result:{message}");
                
                return message;
            }

            var error = await response.Content.ReadAsAsync<JObject>();
            _logger.Error($"FiksApiClient: UpdateLockStatus - emailId:{emailId} - lockId:{lockId} - status:{status} - Error:{error}");

            return default(string);
        }

        public async Task<string> GetAppVersion()
        {
            _logger.InfoFormat($"FiksApiClient: GetAppVersion");

            var userDetails = _cachService.GetUserCredentials();

            if (userDetails == null)
            {
                _logger.Error($"FiksApiClient: GetAppVersion - Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userDetails.JwtToken);
            var response = await _httpClient.GetAsync($@"api/v1/{userDetails.EmailId}/0.0.0.0/checkDesktopAppVersion");
            if (response.IsSuccessStatusCode)
            {
                var jObject = await response.Content.ReadAsAsync<JObject>();
                var message = jObject.SelectToken("message", true).ToString();

                _logger.Info($"FiksApiClient: GetAppVersion - Result:{message}");
                               
                return message;
            }
                      
            return default(string);
        }

        public async Task DownloadLatestAppVersion(string version,string downloadPath)
        {
            _logger.InfoFormat($"FiksApiClient: DownloadLatestAppVersion Started");

            var userDetails = _cachService.GetUserCredentials();

            if (userDetails == null)
            {
                _logger.Error($"FiksApiClient: DownloadLatestAppVersion - Session Expired");
                throw new Exception("Session Expired");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userDetails.JwtToken);
            var response = await _httpClient.GetAsync($@"api/v1/admin/{version}/download");
            if (response.IsSuccessStatusCode)
            {
                using (var fs = new FileStream(downloadPath, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs);

                    _logger.InfoFormat($"FiksApiClient: DownloadLatestAppVersion Finished - {downloadPath}");

                }

            }
        }
    }
}
