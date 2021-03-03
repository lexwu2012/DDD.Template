using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OTA.DTO
{


    /// <summary>
    /// 检查结果
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class CheckPidDto
    {
        /// <summary>
        /// 身份证
        /// </summary>
        [MapFrom(nameof(TOBody.Customer), nameof(Customer.Pid))]
        public string Pid { get; set; }

        /// <summary>
        /// 有效订单数
        /// </summary>
        public int Qty { get; set; }
    }

}
