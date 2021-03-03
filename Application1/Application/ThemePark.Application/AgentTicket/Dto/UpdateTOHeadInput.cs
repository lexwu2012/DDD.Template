using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 编辑旅行社预订信息
    /// </summary>
    [AutoMap(typeof(TOHeaderDto))]
    public class UpdateTOHeadInput
    {
        /// <summary>
        /// 构造
        /// </summary>
        public UpdateTOHeadInput()
        {
            GroupInfo = new GroupInfo();
        }
        /// <summary>
        /// 代理商编号
        /// </summary>
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 团体编号
        /// </summary>   
        public int? GroupInfoId { get; set; }

        /// <summary>
        /// 第三方代理商交易号
        /// </summary>
        [StringLength(50)]
        public string AgentTradeNo { get; set; }

        /// <summary>
        /// 第三方代理商订单号
        /// </summary>    
        [StringLength(50)]
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 子订单信息
        /// </summary>
        [Required]
        public IList<TOBodyDto> TOBodies { get; set; }

        /// <summary>
        /// 团队人员信息
        /// </summary>
        public GroupInfo GroupInfo { get; set; }
    }
}
