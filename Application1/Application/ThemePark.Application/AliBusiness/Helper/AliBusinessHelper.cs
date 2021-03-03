using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using ThemePark.AliPartner.Constants;
using ThemePark.AliPartner.Model;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Common;
using ThemePark.Core.AliPartner;
using ThemePark.Infrastructure.Application;
using ThemePark.AliPartner.TopSdk;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Core.Settings;
using Top.Api.Response;

namespace ThemePark.Application.AliBusiness.Helper
{
    /// <summary>
    /// 业务帮助类
    /// </summary>
    public class AliBusinessHelper
    {
        //天猫旗舰店与中心会话token
        private static string _tmallToken = string.Empty;
        private static DateTime _tmallTokenEndTime = DateTime.Now.AddHours(-1);
        //淘宝与中心会话token
        private static string _taobaoToken = string.Empty;
        private static DateTime _taobaoTokenEndTime = DateTime.Now.AddHours(-1);
        private static readonly object TaobaoLockObj = new object();
        private static readonly object TmallLockObj = new object();

        /// <summary>
        /// 设置淘宝token过期
        /// </summary>
        /// <param name="dateTime"></param>
        public static void SetFangteThemeParkTokenEndTime(DateTime dateTime)
        {
            lock (TaobaoLockObj)
            {
                _taobaoTokenEndTime = dateTime;
            }
        }

        /// <summary>
        /// 设置天猫token过期
        /// </summary>
        /// <param name="dateTime"></param>
        public static void SetFangteTravelTokenEndTime(DateTime dateTime)
        {
            lock (TmallLockObj)
            {
                _tmallTokenEndTime = dateTime;
            }
        }

        ///// <summary>
        ///// 向天猫发码
        ///// </summary>
        ///// <param name="ticketCode">票务平台返回的取票码</param>
        ///// <param name="idcard"></param>
        ///// <param name="method"></param>
        ///// <param name="outerId"></param>
        ///// <param name="amount"></param>
        ///// <param name="orderToken"></param>
        ///// <param name="accessToken"></param>
        ///// <returns></returns>
        //public static Result SendECode(string ticketCode, string idcard, string method, string outerId, int amount, string orderToken, string accessToken)
        //{
        //    var ticketcode = string.IsNullOrEmpty(ticketCode) ? idcard : ticketCode;

        //    Result result;

        //    switch (method)
        //    {
        //        case AliBusinessNotificationMethod.Send:

        //            result = TopSdkHelper.MaSendRequest(ticketcode, outerId, amount, orderToken, AliAppSetingsModel.TaobaoAccessToken);
        //            break;
        //        case AliBusinessNotificationMethod.ReSend:
        //            result = TopSdkHelper.MaResendRequest(ticketcode, outerId, amount, orderToken, AliAppSetingsModel.TaobaoAccessToken);
        //            break;
        //        default:
        //            result = Result.FromError("不是send和resend方法");
        //            break;
        //    }
        //    return result;
        //}

        /// <summary>
        /// 向天猫发码
        /// </summary>
        /// <param name="ticketCode">票务平台返回的取票码</param>
        /// <param name="idcard"></param>
        /// <param name="method"></param>
        /// <param name="outerId"></param>
        /// <param name="amount"></param>
        /// <param name="orderToken"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static EticketMerchantMaSendResponse SendECode(string ticketCode, string idcard, string outerId, int amount, string orderToken, string accessToken)
        {
            var ticketcode = string.IsNullOrEmpty(ticketCode) ? idcard : ticketCode;

            return TopSdkHelper.MaSendRequest(ticketcode, outerId, amount, orderToken, AliAppSetingsModel.TaobaoAccessToken);
        }

        /// <summary>
        /// 向天猫发码
        /// </summary>
        /// <param name="ticketCode">票务平台返回的取票码</param>
        /// <param name="idcard"></param>
        /// <param name="outerId"></param>
        /// <param name="amount"></param>
        /// <param name="orderToken"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static EticketMerchantMaResendResponse ResendECode(string ticketCode, string idcard, string outerId, int amount, string orderToken, string accessToken)
        {
            var ticketcode = string.IsNullOrEmpty(ticketCode) ? idcard : ticketCode;

            return TopSdkHelper.MaResendRequest(ticketcode, outerId, amount, orderToken, AliAppSetingsModel.TaobaoAccessToken);

        }

        ///// <summary>
        ///// 获取方特系统会话令牌
        ///// </summary>
        ///// <param name="organizerNick"></param>
        ///// <returns></returns>
        //public static FtTokenEntity GetFtToken(string organizerNick)
        //{
        //    FtTokenEntity tokenEntity = new FtTokenEntity();
        //    lock (Obj)
        //    {
        //        if (string.IsNullOrWhiteSpace(organizerNick)) return tokenEntity;
        //        string apiPassword;
        //        string apiName;
        //        switch (organizerNick)
        //        {
        //            //天猫
        //            case AliApplicationSetting.FangteTravel:
        //            {
        //                //判断token是否过期
        //                if (DateTime.Now < _tmallTokenEndTime)
        //                {
        //                    return new FtTokenEntity { Data = new TokenEntity() { Token = _tmallToken }, ResultStatus = 0 };
        //                }

        //                apiName = AliAppSetingsModel.TmallTicketServerApiUserName;
        //                apiPassword = AliAppSetingsModel.TmallTicketServerApiPwd;
        //                var strtokenurl = AliAppSetingsModel.TicketServerUrl + "token/generate";

        //                var encrypttoken = Md5Helper.ComputeHash(apiPassword, Encoding.Default);

        //                var rettoken = HttpHelper.HttpPost(strtokenurl,
        //                    "ft_api_name=" + apiName + "&ft_api_password=" + encrypttoken);

        //                tokenEntity = JsonConvert.DeserializeObject<FtTokenEntity>(rettoken);
        //                if (tokenEntity.ResultStatus == 0)
        //                {
        //                    _tmallToken = tokenEntity.Data.Token;
        //                    _tmallTokenEndTime = DateTime.Now.AddHours(1);
        //                }
        //                //apiPassword = "QgN7WVK+gJo=";
        //                //apiName = "tb";
        //                //apiName = "lvmama";
        //                break;
        //            }
        //            //淘宝
        //            case AliApplicationSetting.FangteThemePark:
        //            {
        //                //判断token是否过期
        //                if (DateTime.Now < _taobaoTokenEndTime)
        //                {
        //                    return new FtTokenEntity { Data = new TokenEntity() { Token = _taobaoToken }, ResultStatus = 0 };
        //                }

        //                apiName = AliAppSetingsModel.TaobaoTicketServerApiUserName;
        //                apiPassword = AliAppSetingsModel.TaobaoTicketServerApiPwd;
        //                var strtokenurl = AliAppSetingsModel.TicketServerUrl + "token/generate";

        //                var encrypttoken = Md5Helper.ComputeHash(apiPassword, Encoding.Default);

        //                var rettoken = HttpHelper.HttpPost(strtokenurl,
        //                    "ft_api_name=" + apiName + "&ft_api_password=" + encrypttoken);

        //                tokenEntity = JsonConvert.DeserializeObject<FtTokenEntity>(rettoken);
        //                if (tokenEntity.ResultStatus == 0)
        //                {
        //                    _taobaoToken = tokenEntity.Data.Token;
        //                    _taobaoTokenEndTime = DateTime.Now.AddHours(1);
        //                }
        //                //apiPassword = "QgN7WVK+gJo=";
        //                //apiName = "leyou";
        //                break;
        //            }
        //        }
        //    }
        //    return tokenEntity;
        //}

        /// <summary>
        /// 天猫旗舰店获取与票务系统的会话
        /// </summary>
        /// <returns></returns>
        public static FtTokenEntity GetFtToken4Tmall()
        {
            FtTokenEntity tokenEntity;

            //判断token是否过期
            if (DateTime.Now < _tmallTokenEndTime)
            {
                return new FtTokenEntity { Data = new TokenEntity() { Token = _tmallToken }, ResultStatus = 0 };
            }
            lock (TmallLockObj)
            {
                var apiName = AliAppSetingsModel.TmallTicketServerApiUserName;
                var apiPassword = AliAppSetingsModel.TmallTicketServerApiPwd;
                var strtokenurl = AliAppSetingsModel.TicketServerUrl + "token/generate";

                var encrypttoken = Md5Helper.ComputeHash(apiPassword, Encoding.Default);

                var rettoken = HttpHelper.HttpPost(strtokenurl,
                    "ft_api_name=" + apiName + "&ft_api_password=" + encrypttoken);

                tokenEntity = JsonConvert.DeserializeObject<FtTokenEntity>(rettoken);
                if (tokenEntity.ResultStatus == 0)
                {
                    _tmallToken = tokenEntity.Data.Token;
                    _tmallTokenEndTime = DateTime.Now.AddHours(1);
                }
            }
            return tokenEntity;
        }

        /// <summary>
        /// 淘宝店获取与票务系统的会话
        /// </summary>
        /// <returns></returns>
        public static FtTokenEntity GetFtToken4Taobao()
        {
            FtTokenEntity tokenEntity;

            //判断token是否过期
            if (DateTime.Now < _taobaoTokenEndTime)
            {
                return new FtTokenEntity { Data = new TokenEntity() { Token = _taobaoToken }, ResultStatus = 0 };
            }
            lock (TaobaoLockObj)
            {
                var apiName = AliAppSetingsModel.TaobaoTicketServerApiUserName;
                var apiPassword = AliAppSetingsModel.TaobaoTicketServerApiPwd;
                var strtokenurl = AliAppSetingsModel.TicketServerUrl + "token/generate";

                var encrypttoken = Md5Helper.ComputeHash(apiPassword, Encoding.Default);

                var rettoken = HttpHelper.HttpPost(strtokenurl,
                    "ft_api_name=" + apiName + "&ft_api_password=" + encrypttoken);

                tokenEntity = JsonConvert.DeserializeObject<FtTokenEntity>(rettoken);
                if (tokenEntity.ResultStatus == 0)
                {
                    _taobaoToken = tokenEntity.Data.Token;
                    _taobaoTokenEndTime = DateTime.Now.AddHours(1);
                }
            }
            return tokenEntity;
        }


        /// <summary>
        /// 在票务系统更改订单状态,作废码
        /// </summary>
        /// <param name="organizerNick"></param>
        /// <param name="parkId"></param>
        /// <param name="outerId"></param>
        /// <returns></returns>
        public static ReturnResultEntity<string> OrderCancel(string outerId, string organizerNick, string parkId)
        {
            var tokenEntity = new FtTokenEntity();
            switch (organizerNick)
            {
                //天猫
                case AliApplicationSetting.FangteTravel:
                    tokenEntity = GetFtToken4Tmall();
                    break;
                //淘宝
                case AliApplicationSetting.FangteThemePark:
                    tokenEntity = GetFtToken4Taobao();
                    break;
            }
            if (tokenEntity.ResultStatus != 0)
            {
                return new ReturnResultEntity<string>
                {
                    Message = tokenEntity.Message
                };
            }

            string param = string.Empty;
            param += TakeParam("token", tokenEntity.Data.Token);
            param += TakeParam("outorderid", outerId);
            param += TakeParam("parkid", parkId);

            string strorderurl = AliAppSetingsModel.TicketServerUrl + "ordercancel/post";
            var strjson = HttpHelper.HttpPost(strorderurl, param.Substring(0, param.Length - 1));

            return JsonConvert.DeserializeObject<ReturnResultEntity<string>>(strjson);
        }

        /// <summary>
        /// 更改身份证号码
        /// </summary>
        /// <param name="modifyIdNotificationDto"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ReturnResultEntity<string> ChangeIdCardInFt(ModifyIdCardNotificationDto modifyIdNotificationDto, TmallOrderDetail detail)
        {
            var tokenEntity = new FtTokenEntity();
            switch (modifyIdNotificationDto.OrganizerNick)
            {
                //天猫
                case AliApplicationSetting.FangteTravel:
                    tokenEntity = GetFtToken4Tmall();
                    break;
                //淘宝
                case AliApplicationSetting.FangteThemePark:
                    tokenEntity = GetFtToken4Taobao();
                    break;
            }

            if (tokenEntity.ResultStatus != 0)
            {
                return new ReturnResultEntity<string>
                {
                    Message = tokenEntity.Message
                };
            }

            string param = string.Empty;
            param += TakeParam("token", tokenEntity.Data.Token);
            param += TakeParam("outorderid", detail.Id);
            param += TakeParam("parkid", detail.ParkId.ToString());
            param += TakeParam("newidnum", modifyIdNotificationDto.IdCard);
            param += TakeParam("oldidnum", detail.IdCard);

            string updateIdUrl = AliAppSetingsModel.TicketServerUrl + "SingleIdnum/Update";
            var strjson = HttpHelper.HttpPost(updateIdUrl, param.Substring(0, param.Length - 1));

            return JsonConvert.DeserializeObject<ReturnResultEntity<string>>(strjson);
        }

        /// <summary>
        /// 在票务系统获取订单详情
        /// </summary>
        /// <param name="organizerNick"></param>
        /// <param name="parkId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static OrderTicketDetailEntity GetOrderDetailFromFt(string organizerNick, string parkId, string orderId)
        {
            var tokenEntity = new FtTokenEntity();
            switch (organizerNick)
            {
                //天猫
                case AliApplicationSetting.FangteTravel:
                    tokenEntity = GetFtToken4Tmall();
                    break;
                //淘宝
                case AliApplicationSetting.FangteThemePark:
                    tokenEntity = GetFtToken4Taobao();
                    break;
            }

            if (tokenEntity?.ResultStatus != 0)
            {
                return new OrderTicketDetailEntity
                {
                    Message = tokenEntity?.Message ?? "获取token为空"
                };
            }

            //票务系统订单状态查询
            string param = string.Empty;
            param += TakeParam("token", tokenEntity.Data.Token);
            param += TakeParam("parkid", parkId);
            param += TakeParam("orderid", orderId);

            string orderDetailUrl = AliAppSetingsModel.TicketServerUrl + "orderdetail/get";
            var orderDetail = HttpHelper.HttpPost(orderDetailUrl, param.Substring(0, param.Length - 1));

            return JsonConvert.DeserializeObject<OrderTicketDetailEntity>(orderDetail);
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SetResult(string code, string message)
        {
            return JsonConvert.SerializeObject(new { code, msg = message });
        }

        /// <summary>
        /// 设置返回的HttpResponseMessage
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static HttpResponseMessage SendResponseMessage(string result, string message)
        {
            return new HttpResponseMessage
            {
                Content =
                    new StringContent(SetResult(result, message), Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// 获取公园Id和票类Id
        /// </summary>
        /// <param name="outerIdSku"></param>
        /// <returns></returns>
        public static string[] GetParkIdAndTicketTypeId(string outerIdSku)
        {
            return outerIdSku.Split('_');
        }

        /// <summary>
        /// 转换为键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TakeParam(string key, string value)
        {
            return key + "=" + value + "&";
        }

        /// <summary>
        /// 模型转换
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TmallOrder TradeTransfer(SendAndResendNotificationDto model)
        {
            BizExtend bizExtend = null;
            if (!string.IsNullOrWhiteSpace(model.BizExtend))
            {
                //去掉最后一个字符
                var biz = model.BizExtend.Substring(0, model.BizExtend.Length - 1);
                //去掉第一个字符
                biz = biz.Substring(1);
                bizExtend = JsonConvert.DeserializeObject<BizExtend>(biz);
            }

            TmallOrder tmallOrder = new TmallOrder
            {
                //交易主订单id，若为主子一体订单，则与order_id一致。否则，order_id是mainOrderId的子订单
                Id = model.MainOrderId,
                BuyerNick = bizExtend == null ? string.Empty : bizExtend.BuyerNick,
                Mobile = model.Mobile,
                Payment = decimal.Parse(model.TotalFee ?? "0"),
                Status = TradeStatus.WAIT_SELLER_SEND_GOODS.ToString(),
                Type = "eticket",
                ValidStart = Convert.ToDateTime(model.ValidStart),
                ValidEnd = Convert.ToDateTime(model.ValidEnd),
                OrganizerNick = model.OrganizerNick,
                Name = bizExtend == null ? string.Empty : bizExtend.Name
            };
            return tmallOrder;
        }
    }
}
