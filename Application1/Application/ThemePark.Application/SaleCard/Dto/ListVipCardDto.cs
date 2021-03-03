using System.ComponentModel.DataAnnotations;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public  class ListVipCardDto
    {
        /// <summary>
        /// VipCardId
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 游客ID
        /// </summary>
        public long? CustomerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 年卡类型
        /// </summary>
       public string TicketClassName { get; set; }

        /// <summary>
        /// 基础票种
        /// </summary>
        public string TicketTypeId { get; set; }


        /// <summary>
        /// 年卡卡面编号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// ICNO
        /// </summary>
        public string IcNo { get; set; }


        /// <summary>
        /// 电子年卡编号
        /// </summary>
        public string ECardId { get; set; }


        /// <summary>
        /// 证件号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public GenderType? Gender { get; set; }

        /// <summary>
        /// 性别说明
        /// </summary>
        public string GenderName => Gender==null?"": Gender.DisplayName();
        
        /// <summary>
        /// 年卡状态
        /// </summary>
        public VipCardStateType State { get; set; }

        /// <summary>
        /// 年卡状态说明
        /// </summary>
        public string StateName => State.DisplayName();

        /// <summary>
        /// 激活时间
        /// </summary>    
        public string ActiveTime { get; set; }


        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public string ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public string ValidDateEnd { get; set; }

        /// <summary>
        /// 促销票类ID
        /// </summary>
        public long? SaleTicketClassId { get; set; }
        /// <summary>
        /// 剩余有效天数
        /// </summary>
        public int ValidDays { get; set; }

        /// <summary>
        /// 票类ID
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// 游客照片[BYTE64字符串]
        /// </summary>
        public string PhotoString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Fp1 { get; set; }

        /// <summary>
        /// 游客指纹[BYTE64字符串]
        /// </summary>
        public string Fp1String { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] FpImage1 { get; set; }

        /// <summary>
        /// 游客指纹[BYTE64字符串]
        /// </summary>
        public string FpImage1String { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public byte[] FpImage2 { get; set; }

        /// <summary>
        /// 游客指纹[BYTE64字符串]
        /// </summary>
        public string FpImage2String { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// QQ号
        /// </summary>
        public string Qq { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        public string Weixin { get; set; }


    }
}
