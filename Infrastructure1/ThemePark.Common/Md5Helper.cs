using System.Security.Cryptography;
using System.Text;

namespace ThemePark.Common
{
    /// <summary>
    /// 提供Md5的操作方法
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// MD5加密指定编码的字符串
        /// </summary>
        /// <param name="data">要加密的字符串</param>
        /// <param name="encoding">指定字符编码</param>
        public static string ComputeHash(string data, Encoding encoding)
        {
            return MD5.Create().ComputeHash(data, encoding);
        }

        /// <summary>
        /// MD5加密指定编码的字符串
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="data">要加密的字符串</param>
        /// <param name="encoding">指定字符编码</param>
        public static string ComputeHash(this MD5 md5, string data, Encoding encoding)
        {
            var bytes = md5.ComputeHash(encoding.GetBytes(data));
            var sign = new StringBuilder();
            foreach (var b in bytes)
            {
                sign.Append(b.ToString("x2"));
            }
            return sign.ToString();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Md5Encrypt(string input, Encoding encode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encode.GetBytes(input));
            StringBuilder sb = new StringBuilder(32);
            for (var i = 0; i < t.Length; i++)
            {
                sb.Append(i.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}
