using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapFrom(typeof(AgencyPrintSet))]
    public class SearchAgencyValidDaySetDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 代理商类型名称
        /// </summary>
        public string AgencyTypeName { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public int? ValidDays { get; set; }


    }
}
