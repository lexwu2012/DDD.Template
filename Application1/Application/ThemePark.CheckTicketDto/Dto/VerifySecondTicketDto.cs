using System;

namespace ThemePark.VerifyTicketDto.Dto
{
    public class VerifySecondTicketDto
    {
        /// <summary>
        /// 是否需要验指纹
        /// </summary>
        public bool NeedCheckFp { get; set; }

        /// <summary>
        /// 是否需要验照片
        /// </summary>
        public bool NeedCheckPhoto { get; set; }

        /// <summary>
        /// 条码	
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 人脸ID
        /// </summary>
        public long FaceId { get; set; }
        /// <summary>
        /// 二次入园登记ID
        /// </summary>
        public string EnrollId { get; set; }

        /// <summary>
        /// 闸机上显示的票类名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 可过闸人数
        /// </summary>
        public int Persons { get; set; }

        public DateTime? InParkTimeEnd { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        public string Remark { get; set; }
    }
}
