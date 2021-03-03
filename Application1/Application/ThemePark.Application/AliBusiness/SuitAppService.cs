using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Json;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.Application.AliBusiness.Helper;
using ThemePark.Application.AliBusiness.Interfaces;
using ThemePark.Core.AliPartner;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// 维权应用服务
    /// </summary>
    public class SuitAppService : ThemeParkAppServiceBase, ISuitAppService
    {
        #region Fields
        private readonly IRepository<TmallSuit, string> _tmallSuitRepository;
        private readonly IRepository<TmallOrderDetail, string> _tmallOrderDetailRepository;
        #endregion

        #region cotr
        /// <summary>
        /// cotr
        /// </summary>
        public SuitAppService(IRepository<TmallSuit, string> tmallSuitRepository, IRepository<TmallOrderDetail, string> tmallOrderDetailRepository)
        {
            _tmallSuitRepository = tmallSuitRepository;
            _tmallOrderDetailRepository = tmallOrderDetailRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 当消费者在淘宝平台发起维权,商家在后台同意维权之后，淘宝会通知码商系统订单已经维权成功。
        /// 码商系统在收到维权成功通知后需要给淘宝收到通知的反馈，同时可以根据自己的业务需要做维权成功后的处理，例如：作废码等。
        /// 注意，码商对接此接口需保持幂等，若已处理过维权成功请求，请直接返回成功。
        /// </summary>
        [DisableAuditing]
        public Result HandleSuitBusiness(SuitNotificationDto dto)
        {
            //业务验证
            var suitAlready = CheckIfSuitAlready(dto);

            if (suitAlready)
            {
                Logger.Error("订单号：" + dto.OuterId + "订单维权失败，此订单可能已经维权通过");
                return Result.FromError("订单维权失败，此订单可能已经维权通过");
            }

            var tmallOrderDetail = _tmallOrderDetailRepository.FirstOrDefault(m => m.Id == dto.OuterId);

            if (tmallOrderDetail == null)
            {
                Logger.Error("订单号：" + dto.OuterId + "维权失败，本地没有找到对应订单信息");
                return Result.FromError("订单号" + dto.OuterId + "维权失败，本地没有找到对应订单信息");
            }

            //作废码
            var returnResult = AliBusinessHelper.OrderCancel(tmallOrderDetail.Id, dto.OrganizerNick, tmallOrderDetail.ParkId.ToString());

            if (returnResult?.ResultStatus != ResultState.OrderCancelSuccess && returnResult?.ResultStatus != ResultState.OrderCancelFailAlreadyUsed)
            {
                Logger.Error("订单：" + tmallOrderDetail.Id + "维权失败，原因为：" + returnResult?.ToJsonString());
                return Result.FromError("订单：" + tmallOrderDetail.Id + "维权失败，原因为：" + returnResult?.Message);
            }

            var newSuit = new TmallSuit
            {
                Id = dto.OuterId,
                OuterId = dto.OuterId,
                SuitResult = dto.SuitResult,
                ItemTitle = dto.ItemTitle,
                OrganizerNick = dto.OrganizerNick,
                RefundFee = dto.RefundFee
            };
            _tmallSuitRepository.InsertAndGetId(newSuit);
            tmallOrderDetail.Status = TradeStatus.TRADE_CLOSED.ToString();

            /*
             * 天猫发起维权成功的前提是商家后台已经同意维权或者买家申请维权商家没处理导致超时，这时款是已经返回给买家了，
             * 所以这种情况即使订单已使用，也需要返回成功给天猫，不然会一直发送维权请求过来
             */
            if (returnResult?.ResultStatus == ResultState.OrderCancelFailAlreadyUsed)
            {
                Logger.Error("订单：" + tmallOrderDetail.Id + "已使用，直接返回维权成功");
            }
            else
            {
                Logger.Info("订单号：" + dto.OuterId + "维权成功，订单已取消");
            }
            return Result.Ok();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 检测是否已经通过维权，已经维权过则直接返回
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private bool CheckIfSuitAlready(SuitNotificationDto dto)
        {
            //1表示维权成功，后续会增加新的类型
            if (!dto.SuitResult.Equals("1"))
                return true;

            var suit = _tmallSuitRepository.FirstOrDefault(m => m.Id == dto.OuterId);

            //已经存在维权记录
            if (suit == null)
                return false;

            //结果为1时代表已经维权过
            if (suit.SuitResult.Equals("1"))
                return true;

            suit.SuitResult = "1";
            return false;
        }

        #endregion

    }
}
