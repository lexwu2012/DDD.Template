using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Newtonsoft.Json;
using ThemePark.Common;
using ThemePark.Core;
using ThemePark.Core.Authorization.Repositories;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.WeChat
{
    /// <summary>
    /// Class WeChatAppService.
    /// </summary>
    /// <seealso cref="ThemePark.Application.WeChat.IWeChatAppService" />
    public class WeChatAppService : IWeChatAppService
    {
        /// <summary>
        /// The base URL
        /// </summary>
        public const string BaseUrl = "https://api.weixin.qq.com/";
        private const string Url = "sns/jscode2session?";
        private const string Para = "appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code";

        private readonly IWeChatAppletRepository _weChatAppletRepository;

        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeChatAppService" /> class.
        /// </summary>
        /// <param name="weChatAppletRepository">The we chat applet repository.</param>
        /// <param name="cacheManager">The cache manager.</param>
        public WeChatAppService(IWeChatAppletRepository weChatAppletRepository, ICacheManager cacheManager)
        {
            _weChatAppletRepository = weChatAppletRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取微信用户Id
        /// </summary>
        /// <param name="input">The input.</param>
        public async Task<Result<WeChatAppletDto>> GetOpenIdAsync(GetOpenIdInput input)
        {
            if (!string.IsNullOrEmpty(input.Token))
            {
                var item = await _cacheManager.GetWeChatAppletCache().GetOrDefaultAsync(input.Token);
                if (item != null)
                {
                    return Result.FromData(new WeChatAppletDto(item) { Token = input.Token });
                }
            }

            var setting = await _weChatAppletRepository.GetAll().FirstOrDefaultAsync(o => o.AppId == input.AppId);
            if (setting == null)
            {
                return Result.FromCode<WeChatAppletDto>(ResultCode.Fail);
            }

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            var para = "appid=" + setting.AppId + "&secret=" + setting.Secret + "&js_code=" + input.Code + "&grant_type=authorization_code";

            var response = await client.GetAsync(Url + para);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return Result.FromError<WeChatAppletDto>(string.Join(", ", ex.GetAllExceptionMessage()));
            }

            var str = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OpenIdResult>(str);

            if (result != null && string.IsNullOrEmpty(result.errmsg))
            {
                var item = new AppletCacheItem() { OpenId = result.openid, SessionKey = result.session_key };

                var model = new WeChatAppletDto(item);
                //cache
                await _cacheManager.GetWeChatAppletCache().SetAsync(model.Token, item);

                return Result.FromData(model);
            }
            else
            {
                return Result.FromError<WeChatAppletDto>(result?.errmsg);
            }
        }
    }
}
