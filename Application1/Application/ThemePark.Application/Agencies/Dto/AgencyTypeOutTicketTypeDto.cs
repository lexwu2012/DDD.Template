using Abp.AutoMapper;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(AgencyType))]
    public class AgencyTypeOutTicketTypeDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 出票类型
        /// </summary>
        public OutTicketType OutTicketType { get; set; }

    }
}
