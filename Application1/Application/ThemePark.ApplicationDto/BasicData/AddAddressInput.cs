using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// Class AddAddressInput.
    /// </summary>
    [AutoMapTo(typeof(Address))]
    public class AddAddressInput
    {
        /// <summary>
        /// Gets or sets the province identifier.
        /// </summary>
        /// <value>The province identifier.</value>
        [Required]
        public long ProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the city identifier.
        /// </summary>
        /// <value>The city identifier.</value>
        [Required]
        public long CityId { get; set; }

        /// <summary>
        /// Gets or sets the county identifier.
        /// </summary>
        /// <value>The county identifier.</value>
        public long? CountyId { get; set; }

        /// <summary>
        /// Gets or sets the street identifier.
        /// </summary>
        /// <value>The street identifier.</value>
        public long? StreetId { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        [StringLength(200)]
        public string Detail { get; set; }
    }
}
