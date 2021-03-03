using System;
using System.ComponentModel;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMap(typeof(InParkRule))]
    public class InParkRuleInput
    {
        [Required(ErrorMessage = "入园规则名称是必填字段")]
        [DisplayName("入园规则名称"), StringLength(50)]
        public string InParkRuleName { get; set; }

        [Required]
        [DisplayName("是否验证指纹")]
        public bool CheckFinger { get; set; }

        [Required]
        [DisplayName("是否验证照片")]
        //[DefaultValue(true)]
        public bool CheckPhoto { get; set; }

        [Required]
        [DisplayName("是否有节假日限制")]
        public bool HolidayLimit { get; set; }


        /// <summary>
        /// 是否指定日期
        /// </summary>
        [Required]
        [DisplayName("是否指定日期")]
        public bool AppointDate { get; set; }

        

        /// <summary>
        /// 门票类型：常规票、多园多次套票、年卡、多园年卡
        /// </summary>    
        [Required(ErrorMessage = "请选择门票类型")]
        public TicketClassMode TicketClassMode { get; set; }

        [DisplayName("备注")]
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否有日期限制（如果为true，有效日期由具体票、卡决定）
        /// </summary> 
        [Required]
        [DisplayName("是否限制门票有效期")]
        public bool DateLimit { get; set; }

        /// <summary>
        /// 入园开始时间
        /// </summary>  
        [Required]
        [DisplayName("入园开始时间")]
        [DataType(DataType.Time)]
        public DateTime InParkTimeBegin { get; set; }

        /// <summary>
        /// 入园结束时间
        /// </summary>
        [Required]
        [DisplayName("入园结束时间")]
        [DataType(DataType.Time)]
        public DateTime InParkTimeEnd { get; set; }

        /// <summary>
        /// 单天总次数（单天票次）2    2   1
        /// </summary>  
        [Required]
        [DisplayName("单天总次数")]
        [Range(1, int.MaxValue)]
        public int InParkTimesPerDay { get; set; }

        /// <summary>
        /// 单公园总次数（单公园票次）2   1
        /// </summary>    
        [Required]
        [DisplayName("单公园总次数")]
        [Range(1, int.MaxValue)]
        public int InParkTimesPerPark { get; set; }

        /// <summary>
        /// 表示入园后有效天数（门票有效天数统一默认为当天，如打印模板单独设置，以打印模板为准，特此说明） 特别套票在此设置，其他默认为0（当天）
        /// </summary>    
        [Required]
        [DisplayName("入园后有效天数")]
        [Range(0, int.MaxValue)]
        public int InParkValidDays { get; set; }


        /// <summary>
        /// 指定有效开始日期
        /// </summary>
        [DisplayName("指定有效开始日期")]
        [DataType(DataType.Time)]
        public DateTime? ValidStartDate { get; set; }


        /// <summary>
        /// 指定有效截止日期
        /// </summary>
        [DisplayName("指定有效截止日期")]
        [DataType(DataType.Time)]
        public DateTime? ValidEndDate { get; set; }
    }
}

