using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 
    /// </summary>

    [AutoMap(typeof(UserIC))]
    public class UserICECardDto
    {
        public virtual ECardCustomerDto Customer{ get; set; }
    }

    [AutoMap(typeof(VIPCard))]
    public class ECardVIPCardDto
    {

        /// <summary>
        /// Id
        /// </summary>    
        public long Id { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public virtual ICollection<UserICECardDto> UserICs { get; set; }

        /// <summary>
        /// 卡状态
        /// </summary>    
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public DateTime? ValidDateEnd { get; set; }

        /// <summary>
        /// 电子年卡卡号
        /// </summary>   
        [MapFrom(nameof(VIPCard.IcBasicInfo), nameof(IcBasicInfo.ECardID))]
        public string ECardID { get; set; }

        /// <summary>
        /// 实体卡卡号
        /// </summary>   
        [MapFrom(nameof(VIPCard.IcBasicInfo), nameof(IcBasicInfo.IcNo))]
        public string IcNo { get; set; }


        /// <summary>
        /// 实体卡卡面编号
        /// </summary>   
        [MapFrom(nameof(VIPCard.IcBasicInfo), nameof(IcBasicInfo.CardNo))]
        public string CardNo { get; set; }


        /// <summary>
        /// 票类名称
        /// </summary>   
        [MapFrom(nameof(VIPCard.TicketClass), nameof(TicketClass.TicketClassName))]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 多公园列表
        /// </summary>   
        [MapFrom(nameof(VIPCard.TicketClass), nameof(TicketClass.InParkIdFilter))]
        public string InParkIdFilter { get; set; }


        /// <summary>
        /// 票类
        /// </summary>   
        [MapFrom(nameof(VIPCard.TicketClass), nameof(TicketClass.TicketClassMode))]
        public TicketClassMode TicketClassMode { get; set; }

        /// <summary>
        /// 年卡人数
        /// </summary>   
        [MapFrom(nameof(VIPCard.TicketClass), nameof(TicketClass.TicketType), nameof(TicketType.Persons))]
        public int Persons { get; set; }

        /// <summary>
        /// 年卡有效天数
        /// </summary>   
        [MapFrom(nameof(VIPCard.TicketClass), nameof(TicketClass.InParkRule), nameof(InParkRule.InParkValidDays))]
        public int InParkValidDays { get; set; }

    }




    [AutoMap(typeof(Customer))]
    public class ECardCustomerDto
    {
        /// <summary>
        /// CustomerID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType GenderType { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 相片
        /// </summary>
        public Byte[] Photo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }
    }

}
