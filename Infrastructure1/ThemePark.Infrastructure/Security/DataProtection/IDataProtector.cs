using System.Security.Cryptography;

namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary>
    /// 数据保护
    /// </summary>
    public interface IDataProtector
    {
        /// <summary>
        /// 加密保护指定的明文数据。
        /// </summary>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <returns>返回从明文加密的数据</returns>
        byte[] Protect(byte[] plaintext);

        /// <summary>
        /// 解密受保护的数据
        /// </summary>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <returns>返回解密的明文数据</returns>
        /// <exception cref="CryptographicException">如果 <paramref name="protectedData"/> 是无效的。</exception>
        byte[] Unprotect(byte[] protectedData);
    }
}
