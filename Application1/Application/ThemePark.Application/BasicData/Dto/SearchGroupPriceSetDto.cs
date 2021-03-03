using Abp.AutoMapper;
using ThemePark.Common;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapFrom(typeof(AgencyPrintSet))]
    public class SearchAgencyPriceSetDto
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
        /// 是否打印价格
        /// </summary>
        public bool? IsPrintPrice { get; set; }

        /// <summary>
        /// 打印价格类型
        /// </summary>
        public PrintPriceType? PrintPriceType { get; set; }

        /// <summary>
        /// 打印价格类型名称
        /// </summary>
        public string PrintPriceTypeName => PrintPriceType?.DisplayName();

        /// <summary>
        /// 是否打印团体名称
        /// </summary>
        public bool? IsPrintAgencyName { get; set; }


    }
}
