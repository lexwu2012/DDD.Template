using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 基础票种新增输入
    /// </summary>
    [AutoMapTo(typeof(TicketClass))]
    public class AddTicketClassInput
    {
        /// <summary>
        /// 基础票种Id
        /// </summary>
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 全价票 儿童票 长者票 亲子游 合家欢
        /// </summary>    
        [Required(ErrorMessage = "必填字段")]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 门票类型方式
        /// </summary>
        [Required(ErrorMessage = "请选择门票类型")]
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// 标准价格
        /// </summary>
        [Required]
        public decimal StandardPrice { get; set; }

        /// <summary>
        /// 入园规则ID
        /// </summary>    
        [Required]
        public int InParkRuleId { get; set; }

        /// <summary>
        /// 可入公园过滤条件（多园设置）
        /// </summary>
        //public string InParkIdFilter { get; set; }//where 1=1 where parkid in (11,19)
        public string[] InParkIdFilter { get; set; }

        /// <summary>
        /// Remark
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 套票新增公园
        /// </summary>
        public List<AddMultiTicketClassParkInput> AddMultiTicketClassParkInputs { get; set; }
    }
}

