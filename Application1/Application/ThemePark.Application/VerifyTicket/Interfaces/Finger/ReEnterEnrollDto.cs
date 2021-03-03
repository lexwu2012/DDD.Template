using System.ComponentModel.DataAnnotations;
using ThemePark.Core.ReEnter;

namespace ThemePark.Application.VerifyTicket.Finger
{
    /// <summary>
    /// 二次入园
    /// </summary>
    public class ReEnterEnrollDto
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>    
        [Required]
        public string Barcode { get; set; }


        /// <summary>
        /// 状态
        /// </summary>    
        public ReEnterEnrollState State { get; set; }

        /// <summary>
        /// 指纹
        /// </summary>
        public byte[] Finger { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// 登记通道
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 二次入园规则
        /// </summary>
        public int ReEnterEnrollRuleId { get; set; }

    }
}
