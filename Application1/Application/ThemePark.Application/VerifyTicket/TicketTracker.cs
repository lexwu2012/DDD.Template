using Abp.Dependency;
using System;
using System.Linq;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Core.ParkSale;
using ThemePark.EntityFramework;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 缓存票信息
    /// </summary>
    [Serializable]
    public class TicketInfo
    {
        /// <summary>
        /// 0 NonGroupTicket 1 GroupTicket 2 GroupTicket
        /// </summary>
        public int? TableId { get; set; }

        public int ParkId { get; set; }

        public int Qty { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 本公园总入园次数
        /// </summary>
        public int InparkCounts { get; set; }

        public TicketSaleStatus TicketSaleStatus { get; set; }

        public int ValidDays { get; set; }

        public DateTime ValidStartDate { get; set; }

        public int ParkSaleTicketClassId { get; set; }
    }

    /// <summary>
    /// 变更信息
    /// </summary>
    public class ChangInfo
    {
        /// <summary>
        /// Sys_Change_Version
        /// </summary>
        public long Seq { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 操作 "I"  "U"
        /// </summary>
        public string Op { get; set; }
    }

    /*
    跟踪数据表NonGroupTicket, GroupTicket, TOTicket的更新操作
    将新增加的记录加到缓存，有更新的记录从缓存中清除
    数据库需要打开选项：
      ALTER DATABASE ThemePark SET 
          CHANGE_TRACKING = ON(
            AUTO_CLEANUP = ON,            -- 打开自动清理选项
            CHANGE_RETENTION = 1 HOURS    -- 数据保存期为时    );
            
      ALTER TABLE dbo.NonGroupTicket ENABLE CHANGE_TRACKING;
      ALTER TABLE dbo.GroupTicket ENABLE CHANGE_TRACKING;
      ALTER TABLE dbo.TOTicket ENABLE CHANGE_TRACKING
     
      CREATE VIEW[dbo].[VerifyTicketView]

   AS
    select 0 as TableId, Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,ParkSaleTicketClassId from NonGroupTicket
    union all
    select 1 as TableId,Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,AgencySaleTicketClassId from GroupTicket
    union all
    select 2 as TableId, Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,AgencySaleTicketClassId from TOTicket
  */

    /// <summary>
    /// 跟踪门票数据表更新的类
    /// </summary>
    public class TicketTracker : ISingletonDependency
    {
        private object _syncObject = new object();
        private bool _isInProcessing;//标识当前是否正处于门票数据缓存加载过程中

        private long _seqNonGroupTicket;
        private long _seqGroupTicket;
        private long _seqTOTicket;

        private readonly ThemeParkDbContext _dbContext;

        /// <summary>
        /// 验票管理器
        /// </summary>
        private readonly ICheckTicketManager _checkTicketManager;

        public TicketTracker(ThemeParkDbContext dbContext, ICheckTicketManager checkTicketManagre)
        {
            _seqNonGroupTicket = 0;
            _seqGroupTicket = 0;
            _seqTOTicket = 0;
            _dbContext = dbContext;
            _checkTicketManager = checkTicketManagre;
        }

        /// <summary>
        /// 找出更新的门票记录，维护门票信息缓存
        /// 要定时调用
        /// </summary>
        public void CaptureChange()
        {
            if (!_isInProcessing)
            {
                lock (_syncObject)
                {
                    if (!_isInProcessing)
                    {
                        try
                        {
                            _isInProcessing = true;
                            CaptureTableChange("NonGroupTicket", ref _seqNonGroupTicket);
                            CaptureTableChange("GroupTicket", ref _seqGroupTicket);
                            CaptureTableChange("TOTicket", ref _seqTOTicket);
                        }
                        finally
                        {
                            _isInProcessing = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 数据库捕捉变更数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="seq"></param>
        private void CaptureTableChange(string table, ref long seq)
        {
            string sql = "SELECT CHG.Sys_Change_Version as Seq , Id , Sys_change_Operation as Op "
                + "FROM CHANGETABLE (CHANGES dbo." + table + "," + seq + ") CHG "
                + "order by CHG.Sys_Change_Version";

            var changeInfos = _dbContext.Database.SqlQuery<ChangInfo>(sql).ToList();

            if (table == "NonGroupTicket")
                sql = "select 0 as TableId, Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,ParkSaleTicketClassId from NonGroupTicket";
            else if (table == "GroupTicket")
                sql = "select 1 as TableId, Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,AgencySaleTicketClassId as ParkSaleTicketClassId from GroupTicket";
            else
                sql = "select 2 as TableId, Id, ParkId,Qty,Remark,InparkCounts,TicketSaleStatus,ValidDays,ValidStartDate,AgencySaleTicketClassId as ParkSaleTicketClassId from TOTicket";

            foreach (var item in changeInfos)
            {
                if (item.Op == "I")  // 新增加的记录加到缓存
                {
                    string sqlQuery = sql + " where Id='" + item.Id + "'";
                    var ticket = _dbContext.Database.SqlQuery<TicketInfo>(sqlQuery).FirstOrDefault();

                    if (ticket != null)
                    {
                        _checkTicketManager.GetTicketInfoCache().Set(item.Id, ticket);

                        //_dataSyncManager.AddSaleTicketAmount(ticket.Amount);
                    }
                }
                else if (item.Op == "U") // 修改的记录，使缓存失效
                {
                    _checkTicketManager.GetTicketInfoCache().Remove(item.Id);
                }
            }

            if (changeInfos.Count > 0)
                seq = changeInfos.Last().Seq;
        }

    }
}
