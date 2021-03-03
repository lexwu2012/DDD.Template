using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt.Dto
{
    [AutoMapFrom(typeof(PrintTemplateDetail))]
    public class TicketTemplateContent
    {
        public virtual PrintTemplate PrintTemplate { get; set; }
    }
}
