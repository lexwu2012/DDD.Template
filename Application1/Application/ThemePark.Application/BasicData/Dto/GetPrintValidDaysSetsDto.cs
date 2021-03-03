using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapFrom(typeof(DefaultPrintSet))]
    public class GetValidDaysPrintSetsDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int? AgencyTypeId { get; set; }

        /// <summary>
        /// 默认有效期
        /// </summary>
        public int DefaultValidDays { get; set; }

        /// <summary>
        /// 代理商类型名称(加 散客)
        /// </summary>
        public string AgencyTypeName { get; set; }
    }
}
