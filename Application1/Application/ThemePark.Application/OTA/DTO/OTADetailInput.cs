using AutoMapper.Configuration.Conventions;
using Castle.MicroKernel.Registration;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// OTA订单详情
    /// </summary>
    public class OTADetailInput: ValidateParams
    {
        /// <summary>
        /// 主订单ID
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string AgentOrderId { get; set; }


    }
}
