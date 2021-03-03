using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    [AutoMapTo(typeof(Park))]
    public class UpdateParkInput
    {
        public UpdateParkInput()
        {
            AddressInput = new UpdateAddressInput();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>    
        [Required(AllowEmptyStrings = false)]
        public string ParkName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>    
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>    
        public string Email { get; set; }
        /// <summary>
        /// 传真
        /// </summary>    
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
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        public UpdateAddressInput AddressInput { get; set; }
    }
}
