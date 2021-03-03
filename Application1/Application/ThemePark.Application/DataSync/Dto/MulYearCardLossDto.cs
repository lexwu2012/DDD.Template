using Abp.AutoMapper;
using System;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡挂失Dto
    /// </summary>
    [Serializable]
    public class MulYearCardLossDto
    {

        public int ParkId { get; set; }

        public MulVipCardDto VIPCard { get; set; }


        public MulIcoperDetailDto IcoperDetail { get; set; }

    }

    [AutoMap(typeof(IcoperDetail))]
    public class MulIcoperDetailDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long VIPCardId { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public IcoperDetailStateType State { get; set; }

        /// <summary>
        /// 申请操作人姓名
        /// </summary>
        public string ApplyName { get; set; }

        /// <summary>
        /// 申请操作人证件号
        /// </summary>
        public string ApplyPid { get; set; }

        /// <summary>
        /// 申请操作人手机号
        /// </summary>
        public string ApplyPhone { get; set; }

        public string Remark { get; set; }
    }

}
