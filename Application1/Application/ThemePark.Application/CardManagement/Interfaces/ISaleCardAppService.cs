using Abp.Application.Services;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.CardManagement.Interfaces
{
    /// <summary>
    /// 年卡销售服务接口
    /// </summary>
    public interface ISaleCardAppService : IApplicationService
    {
        /// <summary>
        /// 添加售卡记录
        /// </summary>
        /// <returns></returns>
        Task<Result> AddSaleCardRecord();
    }
}
