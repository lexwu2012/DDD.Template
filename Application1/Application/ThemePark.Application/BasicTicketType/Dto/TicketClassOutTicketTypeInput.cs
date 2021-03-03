using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class TicketClassOutTicketTypeInput
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
