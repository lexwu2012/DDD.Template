using System;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.CardManagement.Dto
{
    /// <summary>
    /// 精准查询条件
    /// </summary>
    public class AccurateSearchModel : PageSortInfo
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
        /// 年卡种类名称
        /// </summary>    
        public string TicketClassId { get; set; }

        /// <summary>
        /// 是否精准查询
        /// </summary>
        public bool IsAccurateSearch { get; set; }

        /// <summary>
        /// 初始化开始日期
        /// </summary>
        public DateTime CreationDateBegin { get; set; }

        /// <summary>
        /// 初始化结束日期
        /// </summary>
        public DateTime CreationDateEnd { get; set; }
    }
}
