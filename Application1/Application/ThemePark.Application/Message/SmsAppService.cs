using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.Settings;
using EnumerableExtensions = ThemePark.Common.EnumerableExtensions;
using Newtonsoft.Json;
using ThemePark.Application.DataSync;
using ThemePark.Application.Message.Dto;
using System.Text;
using Abp.BackgroundJobs;
using ThemePark.Application.OTA;
using ThemePark.Common;
using ThemePark.Core.AgentTicket.Repositories;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Message
{
    /// <summary>
    /// 短信服务
    /// </summary>
    /// <seealso cref="ThemePark.Application.Message.ISmsAppService" />
    [DisableAuditing]
    public class SmsAppService : ISmsAppService
    {
        #region Fields
        private readonly ISettingManager _settingManager;
        private readonly IRepository<GuideCustomer> _guideCustomerRepository;
        private readonly ITOHeaderRepository _tOHeaderRepository;
        private readonly IRepository<Park> _parkRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        #endregion

        #region cotr

        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="settingManager"></param>
        /// <param name="guideCustomerRepository"></param>
        /// <param name="tOHeaderRepository"></param>
        /// <param name="parkRepository"></param>
        /// <param name="backgroundJobManager"></param>
        public SmsAppService(ISettingManager settingManager, IRepository<GuideCustomer> guideCustomerRepository, 
            ITOHeaderRepository tOHeaderRepository, IRepository<Park> parkRepository, IBackgroundJobManager backgroundJobManager)
        {
            _settingManager = settingManager;
            _guideCustomerRepository = guideCustomerRepository;
            _tOHeaderRepository = tOHeaderRepository;
            _parkRepository = parkRepository;
            _backgroundJobManager = backgroundJobManager;
        }
        #endregion


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="message">消息内容</param>
        public async Task<string> SendMessage(IEnumerable<string> phones, string message)
        {
            var strSn = await _settingManager.GetSettingValueForApplicationAsync(SendMessageSetting.SDK_SN);
            var strPwd = Common.Md5Helper.ComputeHash(await _settingManager.GetSettingValueForApplicationAsync(SendMessageSetting.SDK_PWD), Encoding.UTF8);
            var serviceURL = await _settingManager.GetSettingValueForApplicationAsync(SendMessageSetting.MessageServiceUrl);//短信服务域名
            var tokenURL = await _settingManager.GetSettingValueForApplicationAsync(SendMessageSetting.ApiTokenUrl);//登录接口
            var sendURL = await _settingManager.GetSettingValueForApplicationAsync(SendMessageSetting.DhstSendSmsUrl);//发送接口

            string tokenContent = JsonConvert.SerializeObject(new { userName = strSn, password = strPwd });
            string tokenResult = await HttpHelper.PostAsync(serviceURL, tokenURL, tokenContent);
            MessageTokenDto tokenModel = JsonConvert.DeserializeObject<MessageTokenDto>(tokenResult);//获取token

            if (tokenModel.ResultStatus == 1)//登录成功
            {
                MessageSendDto sendModel = new MessageSendDto()//短信内容
                {
                    ApiToken = tokenModel.Data.ApiToken,
                    PhoneNumber = phones.JoinAsString(","),
                    SmsContent = message,
                    SmsType = SmsType.Notice,
                    SignType = SignType.FangteTravel
                };
                string sendContent = JsonConvert.SerializeObject(sendModel);
                string sendResult = await HttpHelper.PostAsync(serviceURL, sendURL, sendContent);//发送短信
                return JsonConvert.DeserializeObject<MessageReceiveDto>(sendResult).Message;
            }
            else
            {
                return tokenModel.Message;
            }
        }

        /// <summary>
        /// 发送OTA短信
        /// </summary>
        /// <param name="order"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task SendOTAMessage(TOHeader order, string imagePath)
        {
            //短信模板
            //eg:确认码：{0}，您已成功通过“{1}”预订{2}：{3}，{4}当日有效。入园方式：刷身份证或二维码入园；凭身份证取票入园。查看二维码：{5};
            var messageTemplate = await _settingManager.GetSettingValueAsync(V1OTASetting.MessageTemplate);

            //0:订单号
            //格式转换 每四位字符加上一个空格 易读取
            //eg 1000 1000 1000 1000
            var orderId = Regex.Replace(order.Id, @"(\w{4}(?=[^$]))", "$1 ");

            //1:代理商名称
            //eg:驴妈妈
            var agencyName = order.Agency.AgencyName;

            //2:公园名称
            //eg:济南方特东方神画
            var parkName = order.TOBodies.First().Park.ParkName;

            //3:订单详情 
            //eg:全价票2张、儿童票1张
            var orderDetail = "";
            EnumerableExtensions.ForEach(order.TOBodies, p => orderDetail += $"{p.AgencySaleTicketClass.AgencySaleTicketClassName}{p.Qty}张、");
            orderDetail = orderDetail.Substring(0, orderDetail.Length - 1);

            //4:预订日期
            //eg：2016-05-16
            var validStartDate = order.ValidStartDate.Date;

            //5:二维码路径
            var message = string.Format(messageTemplate, orderId, agencyName, parkName, orderDetail, validStartDate,
                imagePath);

            //接口一单只预留一个电话
            var phone = order.TOBodies.First().Customer.PhoneNumber;

            //发送短信
            await SendMessage(new List<string>() { phone }, message);
        }


        /// <summary>
        /// 旅行社发送短信
        /// </summary>
        /// <param name="tohead"></param>
        /// <returns></returns>
        public async Task<Result> SendTravelMessage(TOHeader tohead)
        {
            //var tohead = await _tOHeaderRepository.GetAllIncluding(p => p.TOBodies.Select(m => m.AgencySaleTicketClass), p => p.GroupType, p => p.GroupInfo, p => p.Agency).FirstAsync(p => p.Id == id);

            //短信模板
            //eg:"【{0}】 团队预订信息：{1}，{2}，订单号：{3}，票种：{4}，有效期：{5}。
            //    当日凭短信至【团体接待中心】购票。{6}
            //var messageTemplate = await _settingManager.GetSettingValueAsync(TravelSetting.MessageTemplate);
            var messageTemplate = await _settingManager.GetSettingValueForTenantAsync(TravelSetting.MessageTemplate, tohead.TOBodies.First().ParkId)
                ?? await _settingManager.GetSettingValueForApplicationAsync(TravelSetting.MessageTemplate);

            if (null == messageTemplate)
                return Result.FromError<TOHeader>("没配置短信模板");

            //0:代理商名称
            //eg:驴妈妈
            var agencyName = tohead.Agency.AgencyName;

            //1:公园名称
            //eg:厦门方特东方神画
            //var parkName = order.TOBodies.First().Park.ParkName;
            var park = await _parkRepository.GetAsync(tohead.TOBodies.First().ParkId);

            //2:团体类型
            //eg:标准团
            var groupType = tohead.GroupType.TypeName;

            //3:订单号
            //格式转换 每四位字符加上一个空格 易读取
            //eg 1000 1000 1000 1000
            var orderId = Regex.Replace(tohead.Id, @"(\w{4}(?=[^$]))", "$1 ");

            //4:订单详情 
            //eg:全价票2张、儿童票1张
            var orderDetail = "";
            tohead.TOBodies.ForEach(p => orderDetail += $"{p.AgencySaleTicketClass.AgencySaleTicketClassName}{p.Qty}张、");
            orderDetail = orderDetail.Substring(0, orderDetail.Length - 1);

            //5:有效日期
            //eg：2016-05-16
            var validStartDate = tohead.ValidStartDate.Date.ToString("yyyy-MM-dd");

            //6:最小团体人数
            var remark = tohead.Remark;

            //导游电话
            if (string.IsNullOrEmpty(tohead.GroupInfo.GuideIds))
                return Result.FromError<TOHeader>("导游电话为空");
            var guideIds = tohead.GroupInfo.GuideIds.Split(',').ToList();
            var phone = await _guideCustomerRepository.GetAll().Where(p => guideIds.Contains(p.Id.ToString())).Select(p => p.Phone).ToListAsync();

            //var personQty = tohead.Qty;

            //发送数据
            var message = string.Format(messageTemplate, agencyName, park.ParkName, groupType, orderId, orderDetail,
                validStartDate, remark);

            var phoneStirng = string.Join("|", phone);

            _backgroundJobManager.Enqueue<SendMessageJob, SendMessageJobArgs>(new SendMessageJobArgs()
            {
                TOMessage = ToMessageRecord(tohead, message, phoneStirng),
                Phones = phone,
                Message = message
            });

            return Result.Ok();
        }

        #region MyRegion

        /// <summary>
        /// 增加短信发送记录
        /// </summary>
        /// <returns></returns>
        private TOMessage ToMessageRecord(TOHeader order, string message, string phone)
        {
            var entity = new TOMessage()
            {
                TOHeaderId = order.Id,
                AgencyId = order.AgencyId,
                Message = message,
                SendTo = phone,
                CustomerId = order.TOBodies.First().CustomerId,
                SendTime = DateTime.Now,
            };
            return entity;

        }

        #endregion

    }
}