namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 取照片返回结果
    /// </summary>
    public class GetPhotoDto
    {
        /// <summary>
        /// 照片数量 
        /// </summary>
        public int Count { get; set; }

        public int FaceId { get; set; }
        /// <summary>
        /// 人脸特征
        /// </summary>
        public string Feature { get; set; }
        /// <summary>
        /// 照片1
        /// </summary>
        public string Photo1 { get; set; }

        /// <summary>
        /// 照片2
        /// </summary>
        public string Photo2 { get; set; }

        /// <summary>
        /// 照片3
        /// </summary>
        public string Photo3 { get; set; }
    }
}
