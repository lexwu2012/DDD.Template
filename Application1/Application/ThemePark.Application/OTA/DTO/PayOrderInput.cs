using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// OTA支付确认服务Input
    /// </summary>
    [AutoMapTo(typeof(TOHeader))]
    public class PayOrderInput:ValidateParams
    {
        /// <summary>
        /// 主订单ID
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 二维码存储服务器地址
        /// </summary>
        public string EmailServer { get; set; }

        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }




    }
}
