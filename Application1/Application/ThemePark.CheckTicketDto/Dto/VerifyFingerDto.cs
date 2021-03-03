namespace ThemePark.VerifyTicketDto.Dto
{
    public class VerifyFingerDto
    {
        /// <summary>
        /// 指纹对应的验票码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 条码/IC卡号等唯一码	
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
