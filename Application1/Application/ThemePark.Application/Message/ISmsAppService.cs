using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Message
{
    /// <summary>
    /// 短信服务
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService" />
    public interface ISmsAppService : IApplicationService
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="message">消息内容</param>
        /// <param name="ext">扩展子号</param>
        /// <param name="stime">定时时间</param>
        /// <param name="rrid">唯一标识,全数字</param>
        Task<string> SendMessage(IEnumerable<string> phones, string message);
        
        /// <summary>
        /// OTA下单发送短信
        /// </summary>
        /// <param name="order"></param>
        /// <param name="imagePath"></param>
        Task SendOTAMessage(TOHeader order,string imagePath);

        /// <summary>
        /// 旅行社确认发送短信
        /// </summary>
        /// <param name="id"></param>
        Task<Result> SendTravelMessage(TOHeader toheader);
    }
}
