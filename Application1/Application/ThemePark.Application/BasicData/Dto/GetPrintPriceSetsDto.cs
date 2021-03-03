using System.ComponentModel.DataAnnotations;
using ThemePark.Common;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{

    public class GetPricePrintSetsDto
    {
        /// <summary>
        /// 
        /// </summary>
        public GroupType GroupType { get; set; }

        /// <summary>
        /// 打印价格团体类型
        /// </summary>
        public string PricePrintTypeName => GroupType.DisplayName();

        /// <summary>
        /// 打印价格
        /// </summary>
        public PrintPriceType PrintPriceType { get; set; }

    }


    /// <summary>
    /// 打印价格设置类型 散客/团体
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// 散客
        /// </summary>
        [Display(Name = "散客")]
        NonGroup = 0,

        /// <summary>
        /// 团体
        /// </summary>
        [Display(Name = "团体")]
        Group = 1
    }


}
