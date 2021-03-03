namespace ThemePark.Application.DataSync.Dto
{
    public class MultiTicketPhotoRemoveDto
    {
        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public MultiTicketPhotoRemoveDto(string ticketId)
        {
            TicketId = ticketId;
        }

        public string TicketId { get; set; }
    }
}