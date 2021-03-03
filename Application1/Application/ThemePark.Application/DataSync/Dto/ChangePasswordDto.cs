namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 修改账号的密码
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 账号Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 修改后密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}
