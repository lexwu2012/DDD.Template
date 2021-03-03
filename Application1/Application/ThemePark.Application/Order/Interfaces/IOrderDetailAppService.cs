using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.Order.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Order.Interfaces
{
    /// <summary>
    /// 订单查询详情应用服务接口
    /// </summary>
    public interface IOrderDetailAppService : IApplicationService
    {

        /// <summary>
        /// 中心通过API获取公园订单信息（由于票/入园等信息不是实时同步过来中心，所以需要通过公园API接口查询公园的票信息）
        /// </summary>
        /// <param name="toheaderId"></param>
        /// <returns></returns>
        Task<Result<OrderDetailDto>> GetOrderDetailFromCentreOrParkApiAsync(string toheaderId);

        /// <summary>
        /// 获取订单当前状态
        /// </summary>
        /// <param name="toheaderId"></param>
        /// <returns>所有子订单状态信息</returns>
        Task<Result<OrderDetailDto>> GetOrderDetailAsync(string toheaderId);
    }
}
