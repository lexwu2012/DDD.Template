using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(PrintTemplateDetail))]
    public class AddConfigInput
    {
        /// <summary>
        /// 打印模板ID
        /// </summary>
        public int PrintTemplateId { get; set; }

        /// <summary>
        /// 促销票类ID
        /// </summary>
        public List<int> ParkSaleTicketClassIds { get; set; }

        /// <summary>
        /// 是否绑定年卡模板
        /// </summary>
        public bool IsYearCard  { get; set; }
    }

    [AutoMapFrom(typeof(PrintTemplateDetail))]
    public class SearchPrintTemplateDetail
    {

        /// <summary>
        /// 票类ID（促销票类ParkSaleticketClassId）
        /// </summary>
        public int TicketClassId { get; set; }
    }
}
