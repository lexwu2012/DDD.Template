using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Users.Dto
{
    public class UpdatePhoneInput
    {
        ///// <summary>
        ///// 旧手机号
        ///// </summary>
        //[Required]
        //[StringLength(11)]
        //public string OldPhoneNum { get; set; }

        /// <summary>
        /// 新手机号
        /// </summary>
        [Required]
        [StringLength(11)]
        public string NewPhoneNum { get; set; }
    }
}
