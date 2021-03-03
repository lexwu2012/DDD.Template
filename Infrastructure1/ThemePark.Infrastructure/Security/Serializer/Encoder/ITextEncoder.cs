namespace ThemePark.Infrastructure.Security.Serializer.Encoder
{
    /// <summary>
    /// 字符串编码
    /// </summary>
    public interface ITextEncoder
    {
        /// <summary>
        /// 编码
        /// </summary>
        string Encode(byte[] data);

        /// <summary>
        /// 解码
        /// </summary>
        byte[] Decode(string text);
    }
}
