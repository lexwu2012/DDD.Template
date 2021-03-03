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
        /// ȷ��ָ���ļ��������Ƿ���Ҫ���¼��ܡ�
        /// </summary>
        /// <returns>
        /// ���������������ݣ���Ϊ true������Ϊ false��
        /// </returns>
        /// <param name="encryptedData">�������ļ������ݡ�</param>
        public override bool IsReprotectRequired(byte[] encryptedData)
        {
            return true;
        }

        /// <summary>
        /// ָ�������лص��� <see cref="M:System.Security.Cryptography.DataProtector.Protect(System.Byte[])"/> �������������ί�з�����
        /// </summary>
        /// <returns>
        /// һ�������������ݵ��ֽ����顣
        /// </returns>
        /// <param name="userData">Ҫ���ܵ����ݡ�</param>
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
        /// ָ�������лص��� <see cref="M:System.Security.Cryptography.DataProtector.Unprotect(System.Byte[])"/> �������������ί�з�����
        /// </summary>
        /// <returns>
        /// δ���ܵ����ݡ�
        /// </returns>
        /// <param name="encryptedData">Ҫ���ܵ����ݡ�</param>
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
        /// ִ�����ͷŻ����÷��й���Դ��ص�Ӧ�ó����������
        /// </summary>
        public void Dispose()
        {
            _key = null;
            _iv = null;
        }
    }
}