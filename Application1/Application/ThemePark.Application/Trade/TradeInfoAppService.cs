using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using ThemePark.Application.Agencies;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.Payment;
using ThemePark.Application.Payment.Dto;
using ThemePark.Application.Trade.Dto;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;
using ThemePark.Core.TradeInfos;
using ThemePark.Core.TradeInfos.DomainServiceInterfaces;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.Settings;

namespace ThemePark.Application.Trade
{
    /// <summary>
    /// 交易记录应用层服务
    /// </summary>
    public class TradeInfoAppService : ThemeParkAppServiceBase, ITradeInfoAppService
    {
        #region Fields
        private readonly ITradeInfoDomainService _tradeInfoDomainService;
        private readonly IPaymentApiService _paymentApiService;
        private readonly ISettingManager _settingManager;
        private readonly IAgencyAccountAppService _agencyAccountAppService;
        private readonly IAgencyAppService _agencyAppService;
        private readonly IRepository<TradeInfo, string> _tradeInfoRepository;
        private readonly IRepository<Park> _parkRepository;
        #endregion

        #region Cotr
        /// <summary> 
        /// 构造函数
        /// </summary>
        public TradeInfoAppService(ITradeInfoDomainService tradeInfoDomainService, IAgencyAccountAppService agencyAccountAppService, IAgencyAppService agencyAppService,
            ISettingManager settingManager, IPaymentApiService paymentApiService, IRepository<TradeInfo, string> tradeInfoRepository, IRepository<Park> parkRepository)
        {
            _tradeInfoDomainService = tradeInfoDomainService;
            _paymentApiService = paymentApiService;
            _tradeInfoRepository = tradeInfoRepository;
            _parkRepository = parkRepository;
            _settingManager = settingManager;
            _agencyAccountAppService = agencyAccountAppService;
            _agencyAppService = agencyAppService;
        }
        #endregion

        /// <summary>
        ///  新增交易记录并在支付平台为非现金交易的详情付款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <param name="agencyId">团体票挂账用到</param>
        /// <returns></returns>
        public async Task<Result<string>> AddTradeInfoAndReturnTradeInfoIdAsyn(TradeInfoInput input, int parkId, int? agencyId)
        {
            //TradeInfoInput里已经构建好TradeInfoDetails
            var trade = input.MapTo<TradeInfo>();

            //获取交易信息
            var tradeInfo = await _tradeInfoDomainService.AddTradeInfoAsync(trade, parkId);
            //先保存交易记录
            UnitOfWorkManager.Current.SaveChanges();
            //获取公园名称
            var park = await _parkRepository.GetAsync(parkId);

            foreach (var tradeInfoDetail in tradeInfo.TradeInfoDetails)
            {
                switch (tradeInfoDetail.PayModeId)
                {
                    case PayType.PrePay:
                    case PayType.Account:
                        if (agencyId != null)
                            await DealPrePay(tradeInfoDetail, agencyId.Value, parkId);
                        break;
                    case PayType.AliPay:
                    case PayType.WeixinPay:
                        var result = await DealPartnerPay(tradeInfoDetail, tradeInfo, input.AuthCode, park.ParkName);
                        if (!result.Success)
                            return Result.FromCode<string>(result.Code, result.Message);
                        break;
                    default:
                        continue;
                }
            }

            return Result.FromData(tradeInfo.Id);
        }

        /// <summary>
        /// 根据时间获取销售总额，时间参数左开右闭
        /// </summary>
        /// <param name="startTime">大于等于起始时间</param>
        /// <param name="endTime">小于截至时间</param>
        /// <returns>Task&lt;System.Decimal&gt;.</returns>
        [DisableAuditing]
        public decimal GetSomedayTotalSales(DateTime startTime, DateTime endTime)
        {
            //根据收入类型分组
            var tradeinfos = _tradeInfoRepository.GetAll().Where(m => m.CreationTime >= startTime && m.CreationTime < endTime)
                .Select(o => new { o.TradeType, o.Amount }).ToList();

            decimal result = 0;
            tradeinfos.Aggregate(result,
              (r, item) => item.TradeType == TradeType.Outlay ? r - item.Amount : r + item.Amount);

            return result;
        }

        /// <summary>
        /// 获取特定时间段的销售总额(收入-支出)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        public decimal GetSalesAmountByTime(DateTime timeFrom, DateTime timeTo)
        {
            //timeFrom = DateTime.Parse("2018-01-30 09:48:58.503");
            //timeTo = DateTime.Parse("2018-02-01 11:38:05.377");
            var tradeInfos = _tradeInfoRepository.GetAll()
                .Where(m => m.CreationTime >= timeFrom && m.CreationTime < timeTo)
                .Select(o => new { o.TradeType, o.Amount }).ToList();

            var initalAmount = 0m;
            var salesAmount = tradeInfos.Aggregate(initalAmount,
              (r, item) => item.TradeType == TradeType.Outlay ? r - item.Amount : r + item.Amount);

            return salesAmount;
        }

        /// <summary>
        /// 挂账预支付消费操作
        /// </summary>
        /// <returns></returns>
        private async Task DealPrePay(TradeInfoDetail tradeInfoDetail, int agencyId, int parkId)
        {
            var agency = await _agencyAppService.GetAgencyAsync<AgencyAccountDto>(new Query<Agency>(m => m.Id == agencyId));

            var accountOp = new AccountOpInput()
            {
                AccountId = agency.AccountId,
                Cash = tradeInfoDetail.Amount,
                OpType = OpType.Consumption,
                TradeInfoId = tradeInfoDetail.TradeInfoId
            };

            //账户消费
            await _agencyAccountAppService.ConsumptionAccount(accountOp, parkId);
        }


        /// <summary>
        /// 第三方平台支付操作
        /// </summary>
        /// <returns></returns>
        private async Task<Result> DealPartnerPay(TradeInfoDetail tradeInfoDetail, TradeInfo tradeInfo, string authCode, string parkName)
        {
            // 获取支付身份、秘钥
            var partnerCode = await _settingManager.GetSettingValueAsync(PaymentSetting.Ticket.PartnerCode);
            var innerRsaPrivateKey = await _settingManager.GetSettingValueAsync(PaymentSetting.Ticket.InnerRsaPrivateKey);
            var partner = new PaymentPartner(partnerCode, innerRsaPrivateKey);

            //调用支付接口
            var payResult = await _paymentApiService.PartnerScanCodePayAsync(partner, new PaymentPartnerScanCodePayData()
            {
                AuthCode = authCode,
#if DEBUG
                //测试支付平台
                Platform = PaymentPayPlatform.Test,
#else
                Platform = tradeInfoDetail.PayModeId == PayType.AliPay ? PaymentPayPlatform.Alipay : PaymentPayPlatform.WxPay,
#endif
                UnifiedOrder = new PaymentUnifedOrderData()
                {
                    OutOrderNo = tradeInfoDetail.Id,
                    OrderFee = tradeInfoDetail.Amount,
                    OutOrderDesc = parkName + "-窗口售票系统-收银台",
                },
            });

            //支付失败整单作废
            if (payResult.Code != PaymentResultCode.Ok || payResult.Data?.Status != PaymentOrderStatus.Success)
            {
                await _tradeInfoDomainService.PayFailTask(tradeInfo);
                return Result.FromError(payResult.Message);
            }

            //支付成功，更新预支付交易状态
            await _tradeInfoDomainService.PaySuccessTask(tradeInfoDetail, payResult.Data.PayOrderNo);

            //判断支付金额和返回的支付数据是否一致
            if (tradeInfoDetail.Amount != payResult.Data.OrderFee)
                return Result.FromCode(ResultCode.ServerError);
            return Result.Ok();
        }
    }
}

