using System;
using Abp.BackgroundJobs;
using Abp.Dependency;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.Application.VerifyTicket.WriteInPark;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 写入园记录工作的参数
    /// </summary>
    [Serializable]
    public class WriteInParkJobArgs
    {
        public VerifyType VerifyType { get; set; }

        /// <summary>
        /// 门票来源数据表：NonGroupTicket, GroupTicket, TOTicket
        /// </summary>
        public string TableName { get; set; }

        public Type EntityType { get; set; }

        /// <summary>
        /// 当前公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 接受码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 本次入园人数
        /// </summary>
        public int Persons { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 闸机终端号
        /// </summary>
        public int Terminal { get; set; }

        /// <summary>
        /// VerifyCode查询出的唯一码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 可入公园ID
        /// </summary>
        public string InParkIdFilter { get; set; }

        public int TicketClassId { get; set; }

        /// <summary>
        /// 总入园人数
        /// </summary>
        public int InParkCount { get; set; }

        /// <summary>
        /// 票的状态
        /// </summary>
        public int TicketSaleStatus { get; set; }

        /// <summary>
        /// 二次入园登记Id
        /// </summary>
        public int ReEnterEnrollId { get; set; }
    }

    /// <summary>
    /// 写入园记录工作
    /// </summary>
    public class WriteInParkJob : BackgroundJob<WriteInParkJobArgs>, ITransientDependency
    {
        public override void Execute(WriteInParkJobArgs args)
        {
            WriteInParkHelper.UpdateDB(args);
        }
    }
}
