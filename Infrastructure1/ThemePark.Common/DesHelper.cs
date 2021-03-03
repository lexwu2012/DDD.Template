
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ThemePark.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class DesHelper
    {
        /// <summary>
        /// 对身份证进行三重DES解密
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="idcardType"></param>
        /// <param name="secretKey">appsetting里面的secret</param>
        /// <returns></returns>
        public static string DeCryptTripleDesIdcard(string idcard, string idcardType,string secretKey)
        {
            if (string.IsNullOrEmpty(idcard))
                return string.Empty;

            if (idcardType.Equals("2"))
            {
                return DecryptTripleDES(idcard.Replace(" ", "+"), secretKey.Substring(0, 24));
            }
            return idcard;
        }

        /// <summary> 3DES解密</summary>
        /// <returns>解密串</returns> 		
        /// <param name="aStrString">加密串</param> 
        /// <param name="key">密钥</param> 
        public static string DecryptTripleDES(string aStrString, string key)
        {
            if (string.IsNullOrEmpty(aStrString))
            {
                return aStrString;
            }
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes(key),
                    IV = Encoding.ASCII.GetBytes(key.Substring(0, 8)),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform desDecrypt = des.CreateDecryptor();
                byte[] buffer = Convert.FromBase64String(aStrString);
                return Encoding.ASCII.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// 三重DES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="secretKey">由淘宝提供的码商密钥</param>
        /// <returns></returns>
        public static string TripleDES_Decrypt(string cipherText,string secretKey)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return cipherText;
            string plaintext = cipherText;
            using (TripleDES desAlg = TripleDES.Create())
            {
                desAlg.Key = Encoding.UTF8.GetBytes(secretKey.Substring(0, 24));
                desAlg.IV = Encoding.UTF8.GetBytes(secretKey.Substring(0, 8));
                ICryptoTransform decryptor = desAlg.CreateDecryptor(desAlg.Key, desAlg.IV);
                byte[] bCipherText = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bCipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
