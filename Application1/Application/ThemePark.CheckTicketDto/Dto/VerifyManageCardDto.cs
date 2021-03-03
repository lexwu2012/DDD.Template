namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 验管理卡返回结果
    /// </summary>
    public class VerifyManageCardDto
    {
        /// <summary>
        /// 是否二次入园管理卡
        /// </summary>
        public bool IsSecondCard { get; set; }

        /// <summary>
        /// 是否管理卡
        /// </summary>
        public bool IsManagerCard { get; set; }

        /// <summary>
        /// 持有人	
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }
    }
}
