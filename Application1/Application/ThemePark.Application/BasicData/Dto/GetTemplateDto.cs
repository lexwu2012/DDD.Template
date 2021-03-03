using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 获取打印模板
    /// </summary>
    [AutoMapFrom(typeof(PrintTemplate))]
    public class GetTemplateDto
    {

        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 打印模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }
    }
}
