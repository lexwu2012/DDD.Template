using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(PrintTemplateDetail))]
    public class UpdatePrintConfig
    {
        /// <summary>
        /// id
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 票类ID（促销票类）
        /// </summary>
        public int PrintTemplateId { get; set; }
    }
}
