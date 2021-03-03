using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.WeChat
{
    /// <summary>
    /// 微信开放接口
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService" />
    public interface IWeChatAppService : IApplicationService
    {
        /// <summary>
        /// 获取openId和session_key
        /// </summary>
        /// <param name="input">The input.</param>
        Task<Result<WeChatAppletDto>> GetOpenIdAsync(GetOpenIdInput input);
    }
}