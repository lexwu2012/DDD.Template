using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.WeChat
{
    /// <summary>
    /// ΢�ſ��Žӿ�
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService" />
    public interface IWeChatAppService : IApplicationService
    {
        /// <summary>
        /// ��ȡopenId��session_key
        /// </summary>
        /// <param name="input">The input.</param>
        Task<Result<WeChatAppletDto>> GetOpenIdAsync(GetOpenIdInput input);
    }
}