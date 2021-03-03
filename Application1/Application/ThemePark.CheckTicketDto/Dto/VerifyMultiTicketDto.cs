namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 验套票返回结果
    /// </summary>
    public class VerifyMultiTicketDto
    {
        /// <summary>
        /// 是否需要指纹
        /// </summary>
        public bool NeedFinger { get; set; }

        /// <summary>
        /// 是否需要照片
        /// </summary>
        public bool NeedPhoto { get; set; }

        /// <summary>
        /// 是否需要登记
        /// </summary>
        public bool NeedRegister { get; set; }

        /// <summary>
        /// 来源公园
        /// </summary>
        public int FromParkId { get; set; }

        /// <summary>
        /// 来源公园名称	
        /// </summary>
        public string FromParkName { get; set; }


        /// <summary>
        /// 条码/IC卡号等唯一码	
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 人脸ID
        /// </summary>
        public long FaceId { get; set; }

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
