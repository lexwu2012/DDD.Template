using System;
using System.Security.Cryptography;

namespace ThemePark.Infrastructure.Security.DataProtection
{
    internal class TimeLimitedDataProtector : ITimeLimitedDataProtector
    {
        private readonly IDataProtector _innerProtector;

        public TimeLimitedDataProtector(IDataProtector innerProtector)
        {
            _innerProtector = innerProtector;
        }

        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <param name="expiration">指定数据到期时间</param>
        /// <returns>返回从明文加密的数据</returns>
        public byte[] Protect(byte[] plaintext, DateTimeOffset expiration)
        {
            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }
            
            var plaintextWithHeader = new byte[checked(8 + plaintext.Length)];
            var ticks = BitConverter.GetBytes(expiration.UtcTicks);
            Buffer.BlockCopy(ticks, 0, plaintextWithHeader, 0, 8);
            Buffer.BlockCopy(plaintext, 0, plaintextWithHeader, 8, plaintext.Length);

            return _innerProtector.Protect(plaintextWithHeader);
        }

        /// <summary>
        /// 解密受保护的数据
        /// </summary>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="expiration">解密成功时返回的数据到期时间</param>
        /// <returns>返回解密的明文数据</returns>
        /// <exception cref="CryptographicException">如果 <paramref name="protectedData"/> 是无效的，过期的。</exception>
        public byte[] Unprotect(byte[] protectedData, out DateTimeOffset expiration)
        {
            if (protectedData == null)
            {
                throw new ArgumentNullException(nameof(protectedData));
            }

            try
            {
                var plaintextWithHeader = _innerProtector.Unprotect(protectedData);
                if (plaintextWithHeader.Length < 8)
                {
                    throw new CryptographicException("无效的加密数据");
                }

                var utcTicksExpiration = BitConverter.ToInt64(plaintextWithHeader, 0);
                var embeddedExpiration = new DateTimeOffset(utcTicksExpiration, TimeSpan.Zero);

                if (DateTimeOffset.UtcNow > embeddedExpiration)
                {
                    throw new CryptographicException("数据已经超过有效期");
                }
                
                var retVal = new byte[plaintextWithHeader.Length - 8];
                Buffer.BlockCopy(plaintextWithHeader, 8, retVal, 0, retVal.Length);
                expiration = embeddedExpiration;
                return retVal;
            }
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("解密失败:" + ex.Message, ex);
            }
        }
    }
}