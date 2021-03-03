using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.ApplicationDto.BasicData
{
    [AutoMap(typeof(InParkRule))]
    public class InParkRuleDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 规则名称
        /// </summary>    
        public string InParkRuleName { get; set; }

        /// <summary>
        /// 是否验指纹
        /// </summary>    
        public bool CheckFinger { get; set; }

        /// <summary>
        /// 是否验证照片
        /// </summary>    
        public bool CheckPhoto { get; set; }

        /// <summary>
        /// 是否有节假日限制
        /// </summary>    
        public bool HolidayLimit { get; set; }

        /// <summary>
        /// 是否有日期限制（如果为true，有效日期由具体票、卡决定）
        /// </summary>    
        public bool DateLimit { get; set; }

        /// <summary>
        /// 入园开始时间
        /// </summary>    
        public System.DateTime InParkTimeBegin { get; set; }

        /// <summary>
        /// 入园结束时间
        /// </summary>    
        public System.DateTime InParkTimeEnd { get; set; }

        /// <summary>
        /// 单天总次数（单天票次）2    2   1
        /// </summary>    
        public int? InParkTimesPerDay { get; set; }

        /// <summary>
        /// 单公园总次数（单公园票次）2   1
        /// </summary>    
        public int? InParkTimesPerPark { get; set; }

        /// <summary>
        /// 表示入园后有效天数（门票有效天数统一默认为当天，如打印模板单独设置，以打印模板为准，特此说明） 特别套票在此设置，其他默认为0（当天）
        /// </summary>    
        public int? InParkValidDays { get; set; }

        /// <summary>
        /// 门票类型：常规票、多园多次套票、年卡、多园年卡
        /// </summary>    
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// Remark
        /// </summary>    
        public string Remark { get; set; }


        /// <summary>
        /// 是否指定有日期
        /// </summary>    
        public bool AppointDate { get; set; }

        /// <summary>
        /// 指定 有效开始日期
        /// </summary>
        public DateTime? ValidStartDate { get; set; }


        /// <summary>
        /// 指定 有效截止日期
        /// </summary>

        public DateTime? ValidEndDate { get; set; }
    }
}

