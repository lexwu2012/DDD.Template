namespace ThemePark.Application.WeChat
{
    internal class OpenIdResult
    {
        public string openid { get; set; }

        public string session_key { get; set; }

        public string errcode { get; set; }

        public string errmsg { get; set; }
    }
}
