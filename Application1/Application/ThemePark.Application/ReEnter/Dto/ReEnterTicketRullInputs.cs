namespace ThemePark.Application.ReEnter.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class ReEnterTicketRullInputs
    {
        /// <summary>
        /// 二次入园规则编号
        /// </summary>    
        public int ReEnterEnrollRuleId { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>    
        public TicketClassIds[] TicketClassIds { get; set; }
    }
    public class TicketClassIds
    {
        public int Id { get; set; }
    }
}
