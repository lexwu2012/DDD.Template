using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// Class ParkDto.
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.FullAuditedEntityDto"/>
    [AutoMap(typeof(Park))]
    public class ParkDto : FullAuditedEntityDto
    {
        #region Properties

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public virtual Address Address { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        [Required]
        public int AddressId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(20)]
        public string Email { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [StringLength(20)]
        public string Fax { get; set; }

        /// <summary>
        /// 业务主键
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// (营业中/未营业)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [Required]
        [StringLength(40)]
        public string ParkName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(20)]
        public string Tel { get; set; }

        public bool Checked { get; set; }

        #endregion Properties
    }
}