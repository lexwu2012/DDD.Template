using ThemePark.Core.CoreCache.CacheItem;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 检票用的检类数据缓存项
    /// </summary>
    public class TicketClassCacheItem
    {
        /// <summary>
        /// 票类编号
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 门票人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 入园规则编号
        /// </summary>
        public int InParkRullId { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        public string TicketClassName { get; set; }

        /// <summary>
        /// 套票才有可入公园
        /// </summary>
        public string CanInParkIds { get; set; }

        /// <summary>
        /// 入园规则
        /// </summary>
        public InParkRuleCacheItem RuleItem { get; set; }
    }
}