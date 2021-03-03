using System;
using System.IO;
using System.Security.Cryptography;

namespace ThemePark.Infrastructure.Security.DataProtection.Aes
{
    internal class AesDataProtector : DataProtector, IDataProtector, IDisposable
    {
        private byte[] _key;
        private byte[] _iv;


        public AesDataProtector(byte[] key, byte[] iv, string appName, string[] purposes)
            : base(appName, "AesDataProtector", purposes)
        {
            _key = key;
            _iv = iv;
        }

        private SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            return new AesCryptoServiceProvider()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
            };
        }

        /// <summary>
        /// 确定指定的加密数据是否需要重新加密。
        /// </summary>
        /// <returns>
        /// 如果必须加再密数据，则为 true；否则为 false。
        /// </returns>
        /// <param name="encryptedData">需评估的加密数据。</param>
        public override bool IsReprotectRequired(byte[] encryptedData)
        {
            return true;
        }

        /// <summary>
        /// 指定基类中回调的 <see cref="M:System.Security.Cryptography.DataProtector.Protect(System.Byte[])"/> 方法中派生类的委托方法。
        /// </summary>
        /// <returns>
        /// 一个包含加密数据的字节数组。
        /// </returns>
        /// <param name="userData">要加密的数据。</param>
        protected override byte[] ProviderProtect(byte[] userData)
        {
            using (var symmetricAlgorithm = CreateSymmetricAlgorithm())
            {
                using (var cryptoTransform = symmetricAlgorithm.CreateEncryptor(_key, _iv))
                {
                    var outputStream = new MemoryStream();
                    using (var cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(userData, 0, userData.Length);
                        cryptoStream.FlushFinalBlock();

                        return outputStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 指定基类中回调的 <see cref="M:System.Security.Cryptography.DataProtector.Unprotect(System.Byte[])"/> 方法中派生类的委托方法。
        /// </summary>
        /// <returns>
        /// 未加密的数据。
        /// </returns>
        /// <param name="encryptedData">要解密的数据。</param>
        protected override byte[] ProviderUnprotect(byte[] encryptedData)
        {
            using (var symmetricAlgorithm = CreateSymmetricAlgorithm())
            {
                using (var cryptoTransform = symmetricAlgorithm.CreateDecryptor(_key, _iv))
                {
                    var outputStream = new MemoryStream();
                    using (var cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();

                        return outputStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            _key = null;
            _iv = null;
        }
    }
}