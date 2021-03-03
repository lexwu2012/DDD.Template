using System;
using System.Security.Cryptography;

namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary>
    /// 限定时间的数据保护
    /// </summary>
    public interface ITimeLimitedDataProtector 
    {
        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <param name="expiration">指定数据到期时间</param>
        /// <returns>返回从明文加密的数据</returns>
        byte[] Protect(byte[] plaintext, DateTimeOffset expiration);

        /// <summary>
        /// 解密受保护的数据
        /// </summary>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="expiration">解密成功时返回的数据到期时间</param>
        /// <returns>返回解密的明文数据</returns>
        /// <exception cref="CryptographicException">如果 <paramref name="protectedData"/> 是无效的，过期的。</exception>
        byte[] Unprotect(byte[] protectedData, out DateTimeOffset expiration);
    }
}