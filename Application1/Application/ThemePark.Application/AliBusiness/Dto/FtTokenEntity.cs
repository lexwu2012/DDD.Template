namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 获取令牌结果
    /// </summary>
    public class FtTokenEntity
    {
        /// <summary>
        /// token数据
        /// </summary>
        public TokenEntity Data { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int ResultStatus { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        
    }

    /// <summary>
    /// token数据
    /// </summary>
    public class TokenEntity
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public string ExpiredTime { get; set; }
    }
}
