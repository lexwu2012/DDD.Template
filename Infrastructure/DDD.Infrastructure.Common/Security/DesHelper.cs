using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Common.Security
{
    public class DesHelper
    {
        private const string DefaultKey = "12345678";

        public static string Encrypt(string plainText, string key = null)
        {
            if (plainText == string.Empty)
            {
                return plainText;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = DefaultKey;
            }

            using (var provider = new DESCryptoServiceProvider())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                provider.Key = Encoding.ASCII.GetBytes(key);
                provider.IV = Encoding.ASCII.GetBytes(key);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                key = DefaultKey;
            }

            var bytes = Convert.FromBase64String(encryptedText);
            using (var provider = new DESCryptoServiceProvider())
            {
                provider.Key = Encoding.ASCII.GetBytes(key);
                provider.IV = Encoding.ASCII.GetBytes(key);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, provider.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
    }
}
