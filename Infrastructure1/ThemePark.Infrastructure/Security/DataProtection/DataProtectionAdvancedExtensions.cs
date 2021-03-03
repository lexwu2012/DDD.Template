using System;
using System.Security.Cryptography;
using System.Text;
using Abp.Extensions;
using ThemePark.Infrastructure.Security.Serializer.Encoder;

namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary></summary>
    public static class DataProtectionAdvancedExtensions
    {
        private static readonly Encoding SecureUtf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        private static readonly ITextEncoder TextEncoder = TextEncodings.Base64Url; 

        #region IDataProtector

        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <returns>返回从明文加密的数据</returns>
        public static string Protect(this IDataProtector protector, string plaintext)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            try
            {
                var protectBytes = protector.Protect(SecureUtf8Encoding.GetBytes(plaintext));
                return TextEncoder.Encode(protectBytes);
            }
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("加密失败:" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <returns>返回解密的明文数据</returns>
        /// <exception cref="CryptographicException">如果 <paramref name="protectedData"/> 是无效的，过期的。</exception>
        public static string Unprotect(this IDataProtector protector, string protectedData)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (protectedData == null)
            {
                throw new ArgumentNullException(nameof(protectedData));
            }

            try
            {
                var protectBytes = TextEncoder.Decode(protectedData);
                var plaintextBytes = protector.Unprotect(protectBytes);
                return SecureUtf8Encoding.GetString(plaintextBytes);
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

        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this IDataProtector protector, byte[] protectedData, out byte[] plaintext)
        {
            try
            {
                plaintext = protector.Unprotect(protectedData);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                return false;
            }
        }
        
        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this IDataProtector protector, string protectedData, out string plaintext)
        {
            try
            {
                plaintext = protector.Unprotect(protectedData);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                return false;
            }
        }

        #endregion


        #region ITimeLimitedDataProtector

        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <param name="lifetime">指定数据到期时间</param>
        /// <returns>返回从明文加密的数据</returns>
        public static byte[] Protect(this ITimeLimitedDataProtector protector, byte[] plaintext, TimeSpan lifetime)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            return protector.Protect(plaintext, DateTimeOffset.UtcNow + lifetime);
        }

        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <param name="expiration">指定数据到期时间</param>
        /// <returns>返回从明文加密的数据</returns>
        public static string Protect(this ITimeLimitedDataProtector protector, string plaintext,
            DateTimeOffset expiration)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            try
            {
                var protectBytes = protector.Protect(SecureUtf8Encoding.GetBytes(plaintext), expiration);
                return TextEncoder.Encode(protectBytes);
            }
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("加密失败:" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 加密保护指定的明文数据，数据在指定的时间过期。
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="plaintext">需要保护的明文数据</param>
        /// <param name="lifetime">指定数据到期时间</param>
        /// <returns>返回从明文加密的数据</returns>
        public static string Protect(this ITimeLimitedDataProtector protector, string plaintext, TimeSpan lifetime)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            return Protect(protector, plaintext, DateTimeOffset.Now + lifetime);
        }

        /// <summary>
        /// 解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="expiration">解密成功时返回的数据到期时间</param>
        /// <returns>返回解密的明文数据</returns>
        /// <exception cref="CryptographicException">如果 <paramref name="protectedData"/> 是无效的，过期的。</exception>
        public static string Unprotect(this ITimeLimitedDataProtector protector, string protectedData,
            out DateTimeOffset expiration)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            if (protectedData == null)
            {
                throw new ArgumentNullException(nameof(protectedData));
            }

            try
            {
                var protectBytes = TextEncoder.Decode(protectedData);
                var plaintextBytes = protector.Unprotect(protectBytes, out expiration);
                return SecureUtf8Encoding.GetString(plaintextBytes);
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

        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <param name="expiration">解密成功时返回的数据到期时间</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this ITimeLimitedDataProtector protector, byte[] protectedData, out byte[] plaintext, out DateTimeOffset expiration)
        {
            try
            {
                plaintext = protector.Unprotect(protectedData, out expiration);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                expiration = default(DateTimeOffset);
                return false;
            }
        }
        
        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <param name="expiration">解密成功时返回的数据到期时间</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this ITimeLimitedDataProtector protector, string protectedData, out string plaintext, out DateTimeOffset expiration)
        {
            try
            {
                plaintext = protector.Unprotect(protectedData, out expiration);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                expiration = default(DateTimeOffset);
                return false;
            }
        }



        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this ITimeLimitedDataProtector protector, byte[] protectedData, out byte[] plaintext)
        {
            try
            {
                DateTimeOffset expiration;
                plaintext = protector.Unprotect(protectedData, out expiration);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试解密受保护的数据
        /// </summary>
        /// <param name="protector"></param>
        /// <param name="protectedData">需要解密的加密数据</param>
        /// <param name="plaintext">如果解密成功则返回解密的明文数据</param>
        /// <returns>解密是否成功</returns>
        public static bool TryUnprotect(this ITimeLimitedDataProtector protector, string protectedData, out string plaintext)
        {
            try
            {
                DateTimeOffset expiration;
                plaintext = protector.Unprotect(protectedData, out expiration);
                return true;
            }
            catch (CryptographicException)
            {
                plaintext = null;
                return false;
            }
        }

        /// <summary>
        /// 把当前数据保护程序转换为 <see cref="ITimeLimitedDataProtectionProvider"/>
        /// </summary>
        public static ITimeLimitedDataProtectionProvider ToTimeLimitedDataProtector(
            this IDataProtectionProvider protectionProvider)
        {
            if (protectionProvider == null)
            {
                throw new ArgumentNullException(nameof(protectionProvider));
            }

            return protectionProvider.As<ITimeLimitedDataProtectionProvider>()
                   ?? new TimeLimitedDataProtectionProvider(protectionProvider);
        }

        #endregion
    }
}
