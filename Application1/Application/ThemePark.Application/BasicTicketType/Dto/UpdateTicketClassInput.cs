using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 基础票类更新输入
    /// </summary>
    [AutoMapTo(typeof(TicketClass))]
    public class UpdateTicketClassInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 全价票 儿童票 长者票 亲子游 合家欢
        /// </summary>    
        [Required(ErrorMessage = "票类名称是必填字段")]
        [StringLength(20)]
        [DisplayName("票类名称")]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 标准价格
        /// </summary>
        [Required]
        public decimal StandardPrice { get; set; }

        /// <summary>
        /// 入园规则ID
        /// </summary>    
        [Required(ErrorMessage = "入园规则是必填字段")]
        public int InParkRuleId { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        [Range(0, int.MaxValue)]
        [DisplayName("默认排序")]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 门票类型：常规票、多园套票、年卡、多园年卡
        /// </summary>    
        [Required(ErrorMessage = "请选择门票类型")]
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// Remark
        /// </summary>  
        [StringLength(50)]  
        public string Remark { get; set; }

        /// <summary>
        /// 套票配置（多园设置）
        /// </summary>
        public string InParkIdFilter { get; set; }//where 1=1 where parkid in (11,19)

        /// <summary>
        /// 可入公园过滤条件（多园设置），页面select2控件多选专用
        /// </summary>
        public string[] InParkIdFilterArray { get; set; }
    }
}

