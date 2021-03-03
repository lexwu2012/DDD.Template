namespace ThemePark.Application.BasicData.Dto
{
    public class SearchPrintConfigInput
    {

        /// <summary>
        /// 打印模板ID
        /// </summary>
        public int? PrintTemplateId { get; set; }

        /// <summary>
        /// 票类ID（促销票类）
        /// </summary>
        public int? TicketClassId { get; set; }
    }
}
