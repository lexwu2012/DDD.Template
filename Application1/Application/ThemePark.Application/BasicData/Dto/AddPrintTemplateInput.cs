using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(PrintTemplate))]
    public class AddPrintTemplateInput
    {
        /// <summary>
        /// 打印模板名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TemplateName { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        [Required]
        public string Content { get; set; }
    }
}
