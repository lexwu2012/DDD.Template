using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    // TODO:增加注释
    [AutoMapFrom(typeof(City))]
    public class CityDto : EntityDto<long>
    {
        public string AreaName { get; set; }

        public long ParentId { get; set; }

        public string ShortName { get; set; }

        public int? AreaCode { get; set; }

        public int? Zipcode { get; set; }

        public string Pinyin { get; set; }

        public decimal? Lng { get; set; }

        public decimal? Lat { get; set; }
        
        public LevelType LevelType { get; set; }

        public int Sort { get; set; }
    }
}

