using System;
using System.Collections.Generic;
using System.Globalization;
using Abp.Collections.Extensions;

namespace ThemePark.Infrastructure.Security
{
    /// <summary>
    /// 身份凭据数据
    /// </summary>
    public class AuthenticationProperties
    {
        private readonly IDictionary<string, string> _dictionary ;


        /// <summary>
        /// 身份凭据数据
        /// </summary>
        public IDictionary<string, string> Dictionary => _dictionary;

        /// <summary>
        /// 身份验证凭据数据
        /// </summary>
        public AuthenticationProperties()
        {
            _dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// 身份验证凭据数据
        /// </summary>
        public AuthenticationProperties(IDictionary<string, string> dictionary)
        {
            _dictionary = new Dictionary<string, string>(dictionary, StringComparer.Ordinal);
        }


        /// <summary>
        /// 获取或设置 凭证授权的域名
        /// </summary>
        public string Domain
        {
            get { return _dictionary.GetOrDefault(".domain"); }
            set
            {
                if (value == null)
                    _dictionary.Remove(".domain");
                else
                    _dictionary[".domain"] = value;
            }
        }
        
        /// <summary>
        /// 获取或设置 凭证过期时间
        /// </summary>
        public DateTimeOffset? ExpiresUtc
        {
            get
            {
                string input;
                DateTimeOffset result;
                if (_dictionary.TryGetValue(".expires", out input) &&
                    DateTimeOffset.TryParseExact(input, "r", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
                    return result;

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _dictionary[".expires"] = value.Value.ToString("r", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (!_dictionary.ContainsKey(".expires"))
                        return;
                    _dictionary.Remove(".expires");
                }
            }
        }

        /// <summary>
        /// 获取或设置 下次发行凭证的时间
        /// </summary>
        public DateTimeOffset? IssuedUtc
        {
            get
            {
                string input;
                DateTimeOffset result;
                if (_dictionary.TryGetValue(".issued", out input) &&
                    DateTimeOffset.TryParseExact(input, "r", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
                    return result;

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _dictionary[".issued"] = value.Value.ToString("r", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (!_dictionary.ContainsKey(".issued"))
                        return;
                    _dictionary.Remove(".issued");
                }
            }
        }

        /// <summary>
        /// 获取或设置 是否允许刷新凭证
        /// </summary>
        public bool? AllowRefresh
        {
            get
            {
                string str;
                bool result;
                if (_dictionary.TryGetValue(".refresh", out str) && bool.TryParse(str, out result))
                    return result;
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _dictionary[".refresh"] = value.Value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    if (!_dictionary.ContainsKey(".refresh"))
                        return;
                    _dictionary.Remove(".refresh");
                }
            }
        }


        ///// <summary>
        ///// Gets or sets whether the authentication session is persisted across multiple requests.
        ///// 
        ///// </summary>
        //public bool IsPersistent
        //{
        //    get
        //    {
        //        return _dictionary.ContainsKey(".persistent");
        //    }
        //    set
        //    {
        //        if (_dictionary.ContainsKey(".persistent"))
        //        {
        //            if (value)
        //                return;
        //            _dictionary.Remove(".persistent");
        //        }
        //        else
        //        {
        //            if (!value)
        //                return;
        //            _dictionary.Add(".persistent", string.Empty);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the full path or absolute URI to be used as an http redirect response value.
        ///// 
        ///// </summary>
        //public string RedirectUri
        //{
        //    get
        //    {
        //        string str;
        //        if (!_dictionary.TryGetValue(".redirect", out str))
        //            return null;
        //        return str;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _dictionary[".redirect"] = value;
        //        }
        //        else
        //        {
        //            if (!_dictionary.ContainsKey(".redirect"))
        //                return;
        //            _dictionary.Remove(".redirect");
        //        }
        //    }
        //}
    }
}