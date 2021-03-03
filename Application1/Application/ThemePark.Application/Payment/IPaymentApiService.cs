using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Castle.Core.Logging;
using Newtonsoft.Json;
using ThemePark.Application.Payment.Dto;
using ThemePark.Common;
using ThemePark.Core.Settings;

namespace ThemePark.Application.Payment
{
    /// <summary>
    /// 收银台Api服务
    /// </summary>
    public class PaymentApiService : IPaymentApiService, ITransientDependency
    {
        #region 构造函数、私有字段

        private readonly ISettingManager _settingManager;
        public ILogger Logger { get; set; }

        public PaymentApiService(ISettingManager settingManager)
        {
            Logger = NullLogger.Instance;
            _settingManager = settingManager;
        }

        #endregion

        /// <summary>
        /// 统一下单接口
        /// </summary>
        public Task<PaymentResult<PaymentUnifiedOrderResponse>> UnifiedOrderAsync(PaymentPartner partner, PaymentUnifedOrderData data)
        {
            return SendAsync<PaymentUnifiedOrderResponse>(partner,"Pay/UnifiedOrder", data);
        }

        /// <summary>
        /// 商户扫码支付接口
        /// </summary>
        public Task<PaymentResult<PaymentOrderQueryResponse>> PartnerScanCodePayAsync(PaymentPartner partner, PaymentPartnerScanCodePayData data)
        {
            return SendAsync<PaymentOrderQueryResponse>(partner, "Pay/PartnerScanCode", data);
        }


        private async Task<PaymentResult<TData>> SendAsync<TData>(PaymentPartner partner, string api, object data)
        {
            PaymentResult<TData> response = null;
            string responseBody = null;
            string requestBody = null;

            try
            {
                var serverRoot = await _settingManager.GetSettingValueForApplicationAsync(PaymentSetting.ServerRoot);
                var outerPublickKey = await _settingManager.GetSettingValueForApplicationAsync(PaymentSetting.OuterRsaRublickKey);

                if(string.IsNullOrEmpty(serverRoot))
                    throw new Exception("未配置收银台接口 ServerRoot");
                if(string.IsNullOrEmpty(outerPublickKey))
                    throw new Exception("未配置收银台接口 OuterRsaRublickKey");

                if(string.IsNullOrEmpty(partner?.PartnerCode))
                    throw new Exception("未配置收银台接口 PartnerCode");
                if(string.IsNullOrEmpty(partner?.InnerRsaPrivateKey))
                    throw new Exception("未配置收银台接口 InnerRsaPrivateKey");

                var request = new PaymentRequest();
                request.SetJsonValue(data, "Data");
                request.Partner = partner.PartnerCode;
                request.TimeStamp = DateTime.Now;
                request.Sign = request.MakeRsaSign(partner.InnerRsaPrivateKey);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverRoot);

                    var resquestContent = new StringContent(request.ToJson(), Encoding.UTF8, "application/json");
                    requestBody = await resquestContent.ReadAsStringAsync();
                    var responseMessage = await client.PostAsync(api, resquestContent);
                    responseBody = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.EnsureSuccessStatusCode();
                }

                if (!string.IsNullOrEmpty(responseBody))
                {
                    response = JsonConvert.DeserializeObject<PaymentResult<TData>>(responseBody);
                    if (response != null && (response.IsOk() || !string.IsNullOrEmpty(response.Sign)))
                    {
                        if (!response.CheckRsaSign(outerPublickKey))
                        {
                            Logger.Error($"返回结果签名验证失败:\r\nSignData:{response.GetSignData()}\r\nSign:{response.Sign}\r\nPublicKey:{outerPublickKey}");
                            throw new Exception("返回结果签名验证失败");
                        }
                    }
                }

                if (response == null)
                    throw new Exception("返回结果解析失败");
            }
            catch (Exception ex)
            {
                response = new PaymentResult<TData>()
                {
                    Code = PaymentResultCode.Fail,
                    Message = ex.Message,
                };
                Logger.Error($"支付异常：{api}\r\nRequest:{requestBody}\r\nResponse:{responseBody}",
                    ex.GetOriginalException());

            }
            return response;
        }
    }

    /// <summary>
    /// 收银台Api服务
    /// </summary>
    public interface IPaymentApiService
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        Task<PaymentResult<PaymentUnifiedOrderResponse>> UnifiedOrderAsync(PaymentPartner partner, PaymentUnifedOrderData data);

        /// <summary>
        /// 商户扫码支付接口
        /// </summary>
        Task<PaymentResult<PaymentOrderQueryResponse>> PartnerScanCodePayAsync(PaymentPartner partner, PaymentPartnerScanCodePayData data);
    }
}
