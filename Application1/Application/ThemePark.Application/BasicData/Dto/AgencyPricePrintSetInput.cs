using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(AgencyPrintSet))]
    public class AgencyPricePrintSetInput
    {

        /// <summary>
        /// 待修改代理商
        /// </summary>
        public List<AgencyPriceInfo> AgencyInfos { get; set; }

        /// <summary>
        /// 是否打印价格
        /// </summary>
        public bool? IsPrintPrice { get; set; }

        /// <summary>
        /// 打印价格类型
        /// </summary>
        public PrintPriceType? PrintPriceType { get; set; }

        /// <summary>
        /// 是否打印代理商名称
        /// </summary>
        public bool? IsPrintAgencyName { get; set; }
    }

    /// <summary>
    /// 需要设置的代理商
    /// </summary>
    public class AgencyPriceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }
    }


}
