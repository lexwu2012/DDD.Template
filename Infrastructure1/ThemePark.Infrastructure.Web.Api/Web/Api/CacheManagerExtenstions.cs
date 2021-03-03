using Abp.Runtime.Caching;

namespace ThemePark.Infrastructure.Web.Api
{
    public static class CoreCacheManagerExtenstions
    {
        /// <summary>
        /// 获取token缓存
        /// </summary>
        public static ITypedCache<string, byte[]> GetTokenCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, byte[]>(ApiConstant.RefreshTokenCacheKey);
        }
    }
}
