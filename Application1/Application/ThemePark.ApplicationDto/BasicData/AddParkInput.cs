using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// Class AddParkInput.
    /// </summary>
    [AutoMapTo(typeof(Park))]
    public class AddParkInput
    {
        public AddParkInput()
        {
            AddressInput = new AddAddressInput();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the address input.
        /// </summary>
        /// <value>The address input.</value>
        public AddAddressInput AddressInput { get; set; }

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

        /// <summary>
        /// (营业中/未营业)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }

        #endregion Properties
    }
}