using System;
using System.Collections.Generic;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 业务规则验证基础数据
    /// </summary>
    public class ValidateParams
    {

        /// <summary>
        /// 公园编号
        /// </summary>
        public short ValidParkCode { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime ValidPlanDate { get; set; }

        /// <summary>
        /// 验证订单号
        /// </summary>
        public ValidOrderId ValidOrderId { get; set; }

        /// <summary>
        /// 验证OTA类型 自有渠道：OTA
        /// </summary>
        public ValidOTAType ValidOTAType { get; set; }

        /// <summary>
        /// 票类ID
        /// </summary>
        public List<string> ValidTicketTypeIds { get; set; }

        /// <summary>
        /// 待验证身份证
        /// </summary>
        public List<string> ValidPids { get; set; }

        /// <summary>
        /// 验证身份证可售票数
        /// </summary>
        public List<ValidPidAllowQty> ValidPidAllowQties { get; set; }


        public ValidateParams()
        {
            ValidOrderId = new ValidOrderId() {OtaType = OTAType.OTA};
            ValidPidAllowQties = new List<ValidPidAllowQty>();
            ValidOTAType = new ValidOTAType();
           
        }

    }

    /// <summary>
    /// 验证临时结果保存
    /// </summary>
    public class ValidateResult
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public Park Park { get; set; }

        /// <summary>
        /// 票类详情
        /// </summary>
        public Dictionary<string, AgencySaleTicketClass> TicketTypes { get; set; }

        /// <summary>
        /// 主订单
        /// </summary>
        public TOHeader Order { get; set; }

        /// <summary>
        /// 是否自有渠道
        /// </summary>
        public OTAType OTAType { get; set; }


    }


    /// <summary>
    /// 验证订单号
    /// </summary>
    public class ValidOrderId
    {
        /// <summary>
        /// 第三方订单号(根据第三方订单号判断必须结合AgencyId)
        /// </summary>
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 方特主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 自有渠道或者OTA
        /// </summary>
        public OTAType OtaType { get; set; }


        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }

    }


    /// <summary>
    /// 身份证可允许票类
    /// </summary>
    public class ValidPidAllowQty
    {
        /// <summary>
        /// 身份证
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 订票数量
        /// </summary>
        public int Qty { get; set; }
    }

    /// <summary>
    /// 验证是否自有渠道
    /// </summary>
    public class ValidOTAType
    {
        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }
    }

    /// <summary>
    /// OTA类型
    /// </summary>
    public enum OTAType
    {
        /// <summary>
        /// 自有渠道
        /// </summary>
        OwnOTA = 0,

        /// <summary>
        /// OTA
        /// </summary>
        OTA = 1
    }


}
