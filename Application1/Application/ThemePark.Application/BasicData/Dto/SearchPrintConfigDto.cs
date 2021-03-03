using System;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapFrom(typeof(PrintTemplateDetail))]
    public class SearchPrintConfigDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 打印模板ID
        /// </summary>
        public int PrintTemplateId { get; set; }

        /// <summary>
        /// 票类ID（促销票类）
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 打印模板名称
        /// </summary>
        [MapFrom(nameof(PrintTemplateDetail.PrintTemplate),nameof(PrintTemplate.TemplateName))]
        public string TemplateName { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        public string ParkSaleTicketClassName { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public PrintTemplateType Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }


    }
}
