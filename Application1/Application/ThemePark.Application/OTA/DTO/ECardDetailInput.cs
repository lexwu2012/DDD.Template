using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class ECardDetailInput 
    {

        /// <summary>
        /// 公园编号
        /// </summary>
        [Required]
        public short parkid { get; set; }

        /// <summary>
        /// 查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
        /// </summary>
        [Required]
        public string queryStr { get; set; }
    }

    /// <summary>
    /// 查询年卡照片
    /// </summary>
    public class ECardPhotoInput
    {

        /// <summary>
        /// 公园编号
        /// </summary>
        [Required]
        public short parkid { get; set; }

        /// <summary>
        /// customerId
        /// </summary>
        [Required]
        public long customerId { get; set; }
    }
}
