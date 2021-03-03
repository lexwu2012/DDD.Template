using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{

    public class PricePrintSetInput
    {
        /// <summary>
        /// 
        /// </summary>
        public GroupType GroupType { get; set; }


        /// <summary>
        /// 打印价格
        /// </summary>
        public PrintPriceType PrintPriceType { get; set; }
    }
}
