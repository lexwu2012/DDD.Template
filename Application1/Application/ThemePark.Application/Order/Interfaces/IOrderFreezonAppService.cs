using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Order.Interfaces
{
    /// <summary>
    /// 订单冻结（解冻）应用服务接口
    /// </summary>
    public interface IOrderFreezonAppService : IApplicationService
    {
        /// <summary>
        /// 冻结订单
        /// </summary>
        /// <param name="tohearderId"></param>
        /// <returns></returns>
        Task<Result> FreezonOrderAsync(string tohearderId);

        /// <summary>
        /// 解冻订单
        /// </summary>
        /// <param name="tohearderId"></param>
        /// <returns></returns>
        Task<Result> UnFreezonOrderAsync(string tohearderId);
    }
}
