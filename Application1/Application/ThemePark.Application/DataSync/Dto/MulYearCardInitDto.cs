using System;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡初始化Dto
    /// </summary>
    [Serializable]
    public class MulYearCardInitDto
    {
        /// <summary>
        /// 是否是 删除Vipcard
        /// </summary>
        public bool isDelVipCard { get; set; }

        /// <summary>
        /// ParkId
        /// </summary>
        public int ParkId { get; set; }

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
        /// 
        /// </summary>
        public long IcBasicInfoId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long VicCardId { get; set; }

        /// <summary>
        /// 基础票类Id(年卡类型)
        /// </summary>
        public int TicketClassId { get; set; }


        public long? CreatorUserId { get; set; }

    }
}
