using System.Threading.Tasks;
using Abp.Runtime.Caching;

namespace ThemePark.Application.VerifyTicket.Interfaces
{
    /// <summary>
    /// 验票管理器
    /// </summary>
    public interface ICheckTicketManager
    {
        ///// <summary>
        ///// 正在检票的条码
        ///// </summary>
        //ConcurrentDictionary<string, string> CheckingDictionary { get; set; }

        ITypedCache<string, string> GetCheckingCheckTicketCache();

        /// <summary>
        /// 取验票数据缓存
        /// </summary>
        /// <returns></returns>
        ITypedCache<string, TicketCheckData> GetTicketCheckDataCache();

        /// <summary>
        /// 门票信息缓存
        /// </summary>
        /// <returns></returns>
        ITypedCache<string, TicketInfo> GetTicketInfoCache();

        ///// <summary>
        ///// 取票类的验票缓存项
        ///// </summary>
        ///// <param name="parkSaleTicketClassId"></param>
        ///// <returns></returns>
        //Task<TicketClassCacheItem> GetParkTicketClassCacheItem(int parkSaleTicketClassId);
        Task<TicketClassCacheItem> GetParkTicketClassItem(int parkSaleTicketClassId);

        ///// <summary>
        ///// 取代理商票类的验票缓存项
        ///// </summary>
        ///// <param name="agencySaleTicketClassId"></param>
        ///// <returns></returns>
        //Task<TicketClassCacheItem> GetAgencyTicketClassCacheItem(int agencySaleTicketClassId);
        Task<TicketClassCacheItem> GetAgencyTicketClassItem(int agencySaleTicketClassId);

        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="noPast"></param>
        /// <param name="remark"></param>
        /// <param name="terminal"></param>
        Task<bool> WriteInPark(string verifyCode, string id, int noPast, string remark, int terminal);

        /// <summary>
        /// 启动门票监控线程
        /// </summary>
        void Start();

        void Stop();

        /// <summary>
        /// 根据入园规则和票的有效天数判断是否可入园，计算可入园人数
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        bool CheckTicketByRule(TicketCheckData checkData);
        bool CheckInparkBillByRule(TicketCheckData checkData);
    }
}
