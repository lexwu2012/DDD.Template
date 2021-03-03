using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 用户年卡信息关联
    /// </summary>
    [AutoMap(typeof(UserIC))]
    public  class UserIcDto
    {
        /// <summary>
        /// 客户编号
        /// </summary>    
        public int CustomId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long? IcBasicInfoId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long? VIPCardId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(200)]
        public string Remark { get; set; }

        public virtual CustomerDto CustomCustomer { get; set; }

        public virtual IcBasicInfoDto IcBasicInfo { get; set; }

        public virtual VIPCardDto VIPCard { get; set; }
    }

    /// <summary>
    /// 用户年卡信息关联
    /// </summary>
    [AutoMap(typeof(UserIC))]
    public class UserIcDto2
    {
        /// <summary>
        /// 客户编号
        /// </summary>    
        public int CustomId { get; set; }

    }
}
