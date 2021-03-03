using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class AgencyTypeOutTicketTypeInput
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 出票类型 默认、总票、分票
        /// </summary>
        public OutTicketType OutTicketType { get; set; }
    }
}
