namespace ThemePark.Infrastructure.Security.Serializer.Encoder
{
    public static class TextEncodings
    {
        private static readonly ITextEncoder Base64Instance = new Base64TextEncoder();
        private static readonly ITextEncoder Base64UrlInstance = new Base64UrlTextEncoder();

        /// <summary>
        /// Base64 编码字符串
        /// </summary>
        public static ITextEncoder Base64 => Base64Instance;

        /// <summary>
        /// Base64 加 Url 转义编码字符串
        /// </summary>
        public static ITextEncoder Base64Url => Base64UrlInstance;
    }
}
