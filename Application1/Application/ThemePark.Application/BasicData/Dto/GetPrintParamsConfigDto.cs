using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMap(typeof(TicketPrintSet))]
    public class GetTicketPrintSetsDto
    {

        /// <summary>
        /// 
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 票类名称： 全价票 儿童票 长者票 亲子游 合家欢
        /// </summary>    
        [MapFrom(nameof(TicketPrintSet.TicketClass),nameof(TicketClass.TicketClassName))]
        public string TicketClassName { get; set; }

        /// <summary>
        /// 票面标识
        /// </summary>
        public string TicketMarker { get; set; }

        /// <summary>
        /// 条形码标识
        /// </summary>
        public string BarcodeMarker { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public int? ValidDays { get; set; }

        /// <summary>
        /// 备注1
        /// </summary>
        public string Remark1 { get; set; }

        /// <summary>
        /// 备注2
        /// </summary>
        public string Remark2 { get; set; }
    }
}
