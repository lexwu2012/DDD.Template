using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    [AutoMapFrom(typeof(Address))]
    public class AddressDto : EntityDto
    {
        public long? ProvinceId { get; set; }

        public string Province { get; set; }

        public long? CityId { get; set; }

        public string City { get; set; }

        public long? CountyId { get; set; }

        public string County { get; set; }

        public long? StreetId { get; set; }

        public string Street { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        public string Detail { get; set; }
    }
}
