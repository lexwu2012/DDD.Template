using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Configuration;
using Castle.Core.Logging;
using Newtonsoft.Json;
using ThemePark.Common;
using ThemePark.Core.Settings;

namespace ThemePark.Application.OTA.V1
{
    /// <summary>
    /// V1版本OTA对接接口
    /// </summary>
    public interface IV1OTAApiService : IApplicationService
    {

        /// <summary>
        /// 从旧版OTA接口获取Token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TDto> GenerateAsync<TDto>(object input);

        /// <summary>
        /// 从旧版OTA家口获取票类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        Task<TDto> GetTicketTypeAsync<TDto>(string token);

        /// <summary>
        /// 从旧版接口验证身份证
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TDto> CheckIdNum<TDto>(object input);

        /// <summary>
        /// 从旧版接口查询订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TDto> QueryOrderByPhone<TDto>(object input);

        /// <summary>
        /// 从旧版借口找订单详情
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TDto> GetOrderDetail<TDto>(object input);

        /// <summary>
        /// 转发到老系统
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<TDto> Transmit<TDto>(string url, Dictionary<string, string> data);

   
    }


    /// <summary>
    /// V1版本OTA对接接口
    /// </summary>
    public class V1OTAApiService : IV1OTAApiService
    {
        private readonly ISettingManager _settingManager;
        public ILogger Logger { get; set; }

        public V1OTAApiService(ISettingManager settingManager)
        {
            Logger = NullLogger.Instance;
            _settingManager = settingManager;
        }

        /// <summary>
        /// 从旧版OTA接口获取Token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TDto> GenerateAsync<TDto>(object input)
        {
            //转换为键值对
            var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
            return await SendAsync<TDto>("/ticketserver/token/generate", content);
        }


        /// <summary>
        /// 从旧版OTA家口获取票类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public async Task<TDto> GetTicketTypeAsync<TDto>(string token)
        {
            var content = new Dictionary<string, string> {{"token", token}};
            return await SendAsync<TDto>("/ticketserver/ValidTicketType/get", content);
        }


        /// <summary>
        /// 从旧版接口验证身份证
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TDto> CheckIdNum<TDto>(object input)
        {
            var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
            return await SendAsync<TDto>("/ticketserver/checkidnumV2/check", content);
        }

        /// <summary>
        /// 从旧版接口查询订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TDto> QueryOrderByPhone<TDto>(object input)
        {
            var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
            return await SendAsync<TDto>("/ticketserver/QueryOTAOrder/get", content);
        }

        /// <summary>
        /// 从旧版借口查询订单详情
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TDto> GetOrderDetail<TDto>(object input)
        {
            var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
            return await SendAsync<TDto>("ticketserver/orderdetail/get", content);
        }


        /// <summary>
        /// 转发到老系统
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<TDto> Transmit<TDto>(string url,Dictionary<string, string> data)
        {
            return await SendAsync<TDto>(url,data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<TDto> SendAsync<TDto>(string api, object data)
        {
            string responseBody = null;
            string requestBody = null;
            TDto response = default(TDto);
            try
            {
                using (var client = new HttpClient())
                {
                    var otaUrl = await _settingManager.GetSettingValueAsync(V1OTASetting.ServerRoot);
                    client.BaseAddress = new Uri(otaUrl);
                    //表单数据提交
                    HttpContent requestContent = new FormUrlEncodedContent(data as Dictionary<string, string>);
                    requestBody = await requestContent.ReadAsStringAsync();
                    var responseMessage = await client.PostAsync(api, requestContent);
                    responseBody = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.EnsureSuccessStatusCode();
                }

                if (string.IsNullOrEmpty(responseBody))
                {
                    throw new Exception("数据获取失败");
                }
                response = JsonConvert.DeserializeObject<TDto>(responseBody);

            }
            catch (Exception ex)
            {
                Logger.Error($"旧版OTA接口调用异常：{api}\r\nRequest:{requestBody}\r\nResponse:{responseBody}",
                  ex.GetOriginalException());
            }
            return response;




        }
    }
}
