using System;
using System.Security.Cryptography;
using System.Text;

namespace ThemePark.Infrastructure.Security.DataProtection.Aes
{
    /// <summary>
    /// 使用 <see cref="SymmetricAlgorithm"/> 加密保护数据
    /// </summary>
    public class AesDataProtectionProvider : IDataProtectionProvider, IDisposable
    {
        private readonly string _appName;
        private byte[] _keyBytes;

        /// <summary>
        /// 创建一个新的 <see cref="AesDataProtectionProvider"/>
        /// </summary>
        public AesDataProtectionProvider(string appName, string key)
        {
            _appName = appName;
            _keyBytes = Encoding.UTF8.GetBytes(key);
        }


        /// <summary>
        /// 创建一个数据保护器
        /// </summary>
        /// <param name="purposes">保护数据增加的额外的标识</param>
        public IDataProtector Create(params string[] purposes)
        {
            using (var keyHash = SHA256.Create())
            {
                using (var ivHash = MD5.Create())
                {
                    var key = keyHash.ComputeHash(_keyBytes);
                    var iv = ivHash.ComputeHash(_keyBytes);

                    return new AesDataProtector(key, iv, _appName, purposes);
                }
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            _keyBytes = null;
        }
    }
}
