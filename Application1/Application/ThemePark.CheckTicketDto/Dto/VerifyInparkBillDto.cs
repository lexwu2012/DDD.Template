namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 验入园单返回的结果
    /// </summary>
    public class VerifyInparkBillDto
    {
        /// <summary>
        /// 唯一码	
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 闸机上显示的票类名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 可过闸人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        public string Remark { get; set; }
    }
}
