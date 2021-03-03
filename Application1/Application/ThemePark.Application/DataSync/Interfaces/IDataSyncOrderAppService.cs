using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    public interface IDataSyncOrderAppService
    {
        /// <summary>
        /// 订单下发
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OrderSend(OrderSendDto dto);

        /// <summary>
        /// 订单核销
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OrderConsume(OrderConsumeDto dto);

        /// <summary>
        /// 订单修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OrderModify(OrderModifyDto dto);

        /// <summary>
        /// 同步他园票
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OtherTicketSend(OtherTicketSendDto dto);
        /// <summary>
        /// 接收同步订单更改的数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OrderCancelConfirmAsync(OrderCancelConfirmDto dto);

        /// <summary>
        /// 接收中心同步过来的冻结/解冻订单数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> OrderFreezeOrUnFreezeAsync(FreezeOrderDto dto);

        ///// <summary>
        ///// 接收中心同步过来的订单数据
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns></returns>
        //Task<Result> SendOrderFromParkApi(OrderSendDto dto);
    }
}
