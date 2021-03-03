using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 补卡
    /// </summary>
    public class FillCardInput
    {
        /// <summary>
        /// 旧卡ID
        /// </summary>
        [Required]
        public int OldVipCardId { get; set; }

        /// <summary>
        /// 新卡ID
        /// </summary>
        [Required]
        public int NewVipCardId { get; set; }

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
    }
}
