using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Common;

namespace ThemePark.ApplicationDto.BasicTicketType
{
    [AutoMap(typeof(TicketClass))]
    public class TicketClassDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 基础票种Id
        /// </summary>
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 标准价格
        /// </summary>
        public decimal StandardPrice { get; set; }

        /// <summary>
        /// 全价票 儿童票 长者票 亲子游 合家欢
        /// </summary>    
        public string TicketClassName { get; set; }

        /// <summary>
        /// Persons*n（此次数不做最终判断入园有效次数，结合规则表里面的次数一起使用） 如果规则表里面 次数限制 为true 则此处有效 否则就无次数限制
        /// </summary>    
        public int? InParkTimes { get; set; }


        /// <summary>
        /// 可入公园过滤条件（多园设置）
        /// </summary>
        public string InParkIdFilter { get; set; }//where 1=1 where parkid in (11,19)

        /// <summary>
        /// InParkRuleId
        /// </summary>    
        public int InParkRuleId { get; set; }

        /// <summary>
        /// 门票类型：常规票、多园多次套票、年卡、多园年卡
        /// </summary>    
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// Remark
        /// </summary>    
        public string Remark { get; set; }

        public OutTicketType OutTicketType { get; set; }

        public string OutTicketTypeName => OutTicketType.DisplayName();

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SerialNumber { get; set; }

        public  InParkRuleDto InParkRule { get; set; }

        public virtual TicketTypeNewDto TicketType { get; set; }
    }
}

