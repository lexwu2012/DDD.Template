namespace ThemePark.Infrastructure.Security
{
    /// <summary>
    /// 格式化的安全数据
    /// </summary>
    public interface ISecureDataFormat<TData>
    {
        /// <summary>
        /// 格式化保护数据
        /// </summary>
        /// <param name="data">要保护的数据</param>
        string Protect(TData data);

        /// <summary>
        /// 解除保护
        /// </summary>
        /// <param name="protectedText">受保护的数据</param>
        TData Unprotect(string protectedText);
    }
}
