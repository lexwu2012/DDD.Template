using System;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 修改预订日期
    /// </summary>
    [AutoMapTo(typeof(TOHeader))]
    public class OTAModifyPlandateInput: ValidateParams
    {
        /// <summary>
        /// 第三方订单号
        /// </summary> 
        public string AgentOrderId { get; set; }


        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }


        /// <summary>
        /// 预订日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }
    }
}
