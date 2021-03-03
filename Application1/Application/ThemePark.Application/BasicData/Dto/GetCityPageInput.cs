using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(City))]
    public class GetCityPageInput
    {
        public int? Id { get; set; }
        public string CityName { get; set; }
        /// <summary>
        /// ZipCode
        /// </summary>    
        public string ZipCode { get; set; }
        /// <summary>
        /// ProvinceId
        /// </summary>    
        public int? ProvinceId { get; set; }
    }
}

