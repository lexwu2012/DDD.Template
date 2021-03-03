using System.Collections.Generic;
using Abp.AutoMapper;
using AutoMapper.Configuration.Conventions;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 订单确认输入
    /// </summary>
    [AutoMapTo(typeof(TOHeader))]
    public class ConfirmOrderInput
    {
        /// <summary>
        /// 主订单Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 公园代理商团体类型Id（在保存的时候需要用到这个id验证代理商规则）
        /// </summary>
        public int ParkAgencyTypeGroupTypeId { get; set; }

        /// <summary>
        /// 子订单列表
        /// </summary>
        public List<CentreOrderEditInput> TOBodyPres { get; set; }

        /// <summary>
        /// 团体信息
        /// </summary>
        public GroupInfoConfirmDto GroupInfo { get; set; }

        /// <summary>
        /// 旅行社电话
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// 导游是否更改
        /// </summary>
        public bool IsGuideModify { get; set; }

        /// <summary>
        /// 司机是否更改
        /// </summary>
        public bool IsDriverModify { get; set; }       

        /// <summary>
        /// 订单备注
        /// </summary>
        [MapTo(nameof(TOHeader.Remark))]
        public string ToHeadRemark { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 购票总数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Amount { get; set; }

        public string SumAmount { get; set; }
    }
}
