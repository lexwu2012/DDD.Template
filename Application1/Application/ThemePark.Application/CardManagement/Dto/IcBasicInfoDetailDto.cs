using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.CardManagement.Dto
{
    /// <summary>
    /// IC卡详情
    /// </summary>
    [AutoMap(typeof(IcBasicInfo))]
    public class IcBasicInfoDetailDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// Ic卡内码
        /// </summary>    
        public string IcNo { get; set; }
        /// <summary>
        /// IC卡面编号
        /// </summary>    
        public string CardNo { get; set; }
        /// <summary>
        /// 类型编号
        /// </summary>    
        public int KindId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [MapFrom(nameof(IcBasicInfo.KindICKind), nameof(ICKind.KindName))]
        public string KindName { get; set; }

        /// <summary>
        /// 年卡Id
        /// </summary>
        public long VipCardId { get; set; }

        /// <summary>
        /// 基础票类Id
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 领用人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public System.DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public System.DateTime? ValidDateEnd { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime RequisitionTime { get; set; }
    }
}
