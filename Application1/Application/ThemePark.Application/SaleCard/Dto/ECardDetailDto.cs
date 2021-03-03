using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{

    /// <summary>
    /// 电子年卡详情
    /// </summary>
    public class ECardDetailDto
    {
        /// <summary>
        /// VipCardId 主键
        /// </summary>
        public long VipCardId { get; set; }

        /// <summary>
        /// 年卡卡面编号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// ICNO
        /// </summary>
        public string IcNo { get; set; }

        /// <summary>
        /// 年卡类型
        /// </summary>
        public string TicketClassName { get; set; }

        /// <summary>
        /// 电子年卡编号
        /// </summary>
        public string ECardId { get; set; }

        /// <summary>
        /// 年卡状态
        /// </summary>
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 电子年卡类别
        /// </summary>
        public ECardType ECardType { get; set; }

        /// <summary>
        /// 年卡状态说明
        /// </summary>
        public string StateName => State.DisplayName();

        /// <summary>
        /// 开始有效日期(激活时间)
        /// </summary>    
        public string ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public string ValidDateEnd { get; set; }

        public List<ECardCustoerDto> ECardCustoer { get; set; }

        public List<ECardParkDto> ECardPark { get; set; }

    }

    /// <summary>
    /// 电子年卡类型
    /// </summary>
    public enum ECardType
    {
        /// <summary>
        /// 季卡
        /// </summary>
        [Display(Name = "季卡")]
        SeasonCard = 0,

        /// <summary>
        /// 单人卡
        /// </summary>
        [Display(Name = "单人卡")]
        SingleCard= 5,

        /// <summary>
        /// 双人卡
        /// </summary>
        [Display(Name = "双人卡")]
        DoubleCard= 10,

        /// <summary>
        /// 三人卡
        /// </summary>
        [Display(Name = "三人卡")]
        ThreeCard= 15,
    }

    /// <summary>
    /// 公园信息
    /// </summary>
    [AutoMap(typeof(Park))]
    public class ECardParkDto
    {
        /// <summary>
        /// 公园ID
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }
    }

    /// <summary>
    /// 电子年卡用户信息
    /// </summary>
    public class ECardCustoerDto
    {
        /// <summary>
        /// CustomerID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType? Gender { get; set; }

        /// <summary>
        /// 性别说明
        /// </summary>
        public string GenderName => Gender == null ? "" : Gender.DisplayName();

        /// <summary>
        /// 游客照片[BYTE64字符串]
        /// </summary>
        public string PhotoString { get; set; }

        /// <summary>
        /// 游客姓名
        /// </summary>
        public string CustomName { get; set; }



    }

}
