using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.CardManagement.Dto
{
    /// <summary>
    /// IC卡信息
    /// </summary>
    [AutoMap(typeof(IcBasicInfo))]
    public class IcBasicInfoInput
    {
        /// <summary>
        /// Ic卡内码
        /// </summary>    
        [Required]
        [StringLength(20)]
        public string IcNo { get; set; }
        /// <summary>
        /// IC卡面编号
        /// </summary>    
        [Required]
        [StringLength(50)]
        public string CardNo { get; set; }
        /// <summary>
        /// 类型编号
        /// </summary>    
        [Required]
        public int KindId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(200)]
        public string Remark { get; set; }

        /// <summary>
        /// 基础票类Id(年卡类型)
        /// </summary>
        public int TicketClassId { get; set; }
    }
}
