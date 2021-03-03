using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 订单同步Dto
    /// </summary>
    public class OrderSendDto
    {
        /// <summary>
        /// 订单数据(主订单、子订单、子订单用户信息)
        /// </summary>
        public SendTOHeader OrderInfo { get; set; }

        /// <summary>
        /// 电子票
        /// </summary>
        public ICollection<SendTOTicket> TicketsInfo { get; set; }
    }


    /// <summary>
    /// 待同步TOHeader数据
    /// </summary>
    [AutoMap(typeof(TOHeader))]
    public class SendTOHeader
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 第三方代理商订单号
        /// </summary>
        [StringLength(128)]
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 第三方代理商交易号
        /// </summary>
        [StringLength(128)]
        public string AgentTradeNo { get; set; }

        /// <summary>
        /// 团体编号
        /// </summary>
        public int? GroupInfoId { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 带队类型编号
        /// </summary>
        [Required]
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 人数
        /// </summary>        
        public int Persons { get; set; }

        /// <summary>
        /// 购票总数量
        /// </summary>
        [Required]
        public int Qty { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 有效开始日期（入园日期）
        /// </summary>
        [Required]
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效开始日期（入园日期）
        /// </summary>
        public DateTime? ValidEndDate { get; set; }

        /// <summary>
        /// 团体信息
        /// </summary>
        public SendTOGroupInfo GroupInfo { get; set; }

        /// <summary>
        /// 订单已确认
        /// </summary>
        /// <value><c>true</c> 如果订单已经确认; 否则, <c>false</c>.</value>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 子订单
        /// </summary>
        [Required(ErrorMessage = "缺少子订单信息")]
        public  ICollection<SendTOBody> TOBodies { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType OrderType { get; set; }

    }

    /// <summary>
    /// 待同步TOBody数据
    /// </summary>
    [AutoMap(typeof(TOBody))]
    public class SendTOBody
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        [Required]
        public int ParkId { get; set; }

        /// <summary>
        /// 主订单编号
        /// </summary>
        [Required]
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>
        [Required]
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required]
        public int Seq { get; set; }

        /// <summary>
        /// 入园人数
        /// </summary>
        [Required]
        public int Persons { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>
        [Required]
        public int Qty { get; set; }

        /// <summary>
        /// 子订单状态
        /// </summary>
        [Required]
        public OrderState OrderState { get; set; }

        /// <summary>
        /// 商品单价（原价）
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格（售价）
        /// </summary>
        [Required]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal? SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal? ParkSettlementPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 绑定客户
        /// </summary>
      
        public virtual SendTOCustomer Customer { get; set; }
    }

    /// <summary>
    /// 待同步用户数据
    /// </summary>
    [AutoMap(typeof(Customer))]
    public class SendTOCustomer
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        [StringLength(20)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 指纹模板1
        /// </summary>
        public System.Byte[] Fp1 { get; set; }

        /// <summary>
        /// 指纹模板2
        /// </summary>
        public System.Byte[] Fp2 { get; set; }

        /// <summary>
        /// 指纹图片1
        /// </summary>
        public System.Byte[] FpImage1 { get; set; }

        /// <summary>
        /// 指纹图片2
        /// </summary>
        public System.Byte[] FpImage2 { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType GenderType { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 相片
        /// </summary>
        public System.Byte[] Photo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public System.DateTime Birthday { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

    /// <summary>
    /// 待同步团体信息
    /// </summary>
    [AutoMap(typeof(GroupInfo))]
    public class SendTOGroupInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 代理商编号
        /// </summary>
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 司陪 <example>11,12,13</example>
        /// </summary>
        public string DriverIds { get; set; }

        /// <summary>
        /// 旅游团体名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string GroupInfoName { get; set; }

        /// <summary>
        /// 预订团类型 <seealso cref="ThemePark.Core.BasicData.GroupInfoType"/>
        /// </summary>
        public GroupInfoType GroupInfoType { get; set; }

        /// <summary>
        /// 旅游团成员
        /// </summary>
        /// <value>The group members.</value>
        public  ICollection<SendTOGroupMembers> GroupMembers { get; set; }

        /// <summary>
        /// 带队类型Id
        /// </summary>
        [Required]
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 导游编号 <example>11,12,13</example>
        /// </summary>
        public string GuideIds { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 待同步团体成员
    /// </summary>
    [AutoMap(typeof(GroupMembers))]
    public class SendTOGroupMembers
    {

        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>The customer.</value>
        public SendTOCustomer Customer { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        [Required]
        public int CustomId { get; set; }

        /// <summary>
        /// 团体信息编号
        /// </summary>
        [Required]
        public long GroupInfoId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }
    }


    /// <summary>
    /// 待同步凭证数据
    /// </summary>
    [AutoMap(typeof(TOVoucher))]
    public class SendTOVoucher
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(20)]
        public string Pid { get; set; }

        /// <summary>
        /// 订单序号
        /// </summary>
        [Required]
        public int Seq { get; set; }

        /// <summary>
        /// 子订单编号
        /// </summary>
        [Required]
        public string TOBodyId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }


    /// <summary>
    /// 待同步票数据
    /// </summary>
    [AutoMap(typeof(TOTicket))]
    public class SendTOTicket
    {

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 票数
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 代理商促销票类编号
        /// </summary>
        [Required]
        public int AgencySaleTicketClassId { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        [Required]
        public DateTime ValidStartDate { get; set; }


        /// <summary>
        /// 计划入园日期开始的有效天数
        /// </summary>    
        [Required]
        public int ValidDays { get; set; }

        /// <summary>
        /// 商品单价（原价）
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// 实际价格（售价）
        /// </summary>
        [Required]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 国旅结算价
        /// </summary>
        public decimal? SettlementPrice { get; set; }

        /// <summary>
        /// 公园结算价
        /// </summary>
        public decimal? ParkSettlementPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 门票状态：0 预售 1 有效 2 已入园 3 作废 有效 可能是部分入园
        /// </summary>
        [Required]
        public TicketSaleStatus TicketSaleStatus { get; set; }
        
        /// <summary>
        /// 取票凭证编号
        /// </summary>
        [Required]
        public string TOVoucherId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(512)]
        public string Remark { get; set; }

        /// <summary>
        /// 关联凭证
        /// </summary>
        [Required(ErrorMessage = "缺少凭证数据TOVoucher")]
        public SendTOVoucher TOVoucher { get; set; }

    }

}