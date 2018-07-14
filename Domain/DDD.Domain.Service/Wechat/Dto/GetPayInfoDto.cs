namespace DDD.Domain.Service.Wechat.Dto
{
    public class GetPayInfoDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? IdPerson { get; set; }

        /// <summary>
        /// 系统编码
        /// </summary>
        public string SystemCode { get; set; }

        /// <summary>
        /// 平台渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
