using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.ParkSale;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Core.TradeInfos;
using ThemePark.Core.TradeInfos.DomainServiceInterfaces;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 补票记录服务层
    /// </summary>
    public class FareAdjustmentAppService : ThemeParkAppServiceBase, IFareAdjustmentAppService
    {
        #region Fields
        private readonly IRepository<ExcessFare, string> _excessFareRepository;
        private readonly IRepository<ExcessFareInvalid, string> _excessFareInvalidRepository;
        private readonly IExcessFareDomainService _excessFareDomainService;
        private readonly ITradeInfoAppService _tradeInfoAppService;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly ITradeInfoDomainService _tradeInfoDomainService;
        private readonly IUniqueCode _uniqueCode;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;

        private readonly IRepository<TradeInfo, string> _tradeInfoRepository;
        #endregion


        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="excessFareRepository"></param>
        /// <param name="tradeInfoAppService"></param>
        ///  <param name="excessFareInvalidDomainService"></param>
        ///  <param name="invoiceDomainService"></param>
        ///  <param name="tradeInfoDomainService"></param>
        ///  <param name="excessFareInvalidRepository"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="invoiceAppService"></param>
        /// <param name="invoiceCodeRepository"></param>
        public FareAdjustmentAppService(IRepository<ExcessFare, string> excessFareRepository, ITradeInfoAppService tradeInfoAppService,
            IRepository<ExcessFareInvalid, string> excessFareInvalidRepository, IExcessFareDomainService excessFareInvalidDomainService,
            IInvoiceDomainService invoiceDomainService, ITradeInfoDomainService tradeInfoDomainService, IUniqueCode uniqueCode,
            IInvoiceAppService invoiceAppService, IRepository<InvoiceCode> invoiceCodeRepository, IRepository<TradeInfo, string> tradeInfoRepository)
        {
            _excessFareRepository = excessFareRepository;
            _tradeInfoAppService = tradeInfoAppService;
            _excessFareDomainService = excessFareInvalidDomainService;
            _invoiceDomainService = invoiceDomainService;
            _tradeInfoDomainService = tradeInfoDomainService;
            _uniqueCode = uniqueCode;
            _excessFareInvalidRepository = excessFareInvalidRepository;
            _invoiceAppService = invoiceAppService;
            _invoiceCodeRepository = invoiceCodeRepository;
            _tradeInfoRepository = tradeInfoRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 添加补票记录
        /// </summary>
        /// <returns></returns>
        public async Task<Result<string>> AddFareAdjustmentAndReturnTradeNumAsync(FareAdjustmentAddNewInput input, InvoiceInput invoiceInput, int terminalId, int parkId)
        {
            var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == invoiceInput.InvoiceCode);
            if (null == invoiceCode)
                return Result.FromError<string>("发票代码不存在");

            var verifyResult = BusinessVerify(invoiceInput, input.Qty, invoiceCode.InvoiceNumIsIncrease);

            if (!verifyResult.Success)
                return verifyResult;

            //改交易类型为收入
            input.PayInfo.TradeType = TradeType.Income;

            //生成交易信息（包含支付过程和结果）
            var result = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(input.PayInfo, parkId, null);

            if (result.Success)
            {
                input.FareAdjustment.TradeInfoId = result.Data;

                //每张补票需要一条发票，传入的invoiceInput.InvoceNo是有效可用的，即数据库最大数已经加上1了
                var invoiceNo = long.Parse(invoiceInput.InvoiceNo);

                //根据补票数量生成相应数量的补票记录（输入的补票张数）
                for (var i = 1; i <= input.Qty; i++)
                {
                    input.FareAdjustment.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);

                    //从服务端的配置获取
                    input.FareAdjustment.ParkId = parkId;

                    //1代表一张票
                    input.FareAdjustment.Qty = 1;

                    //计算一张票的总额
                    input.FareAdjustment.Amount = input.FareAdjustment.Denomination * input.FareAdjustment.Qty;

                    //状态为有效
                    input.FareAdjustment.State = TicketSaleStatus.Valid;

                    input.FareAdjustment.TerminalId = terminalId;

                    //发票信息
                    Invoice invoice = new Invoice
                    {
                        Barcode = input.FareAdjustment.Id,
                        InvoiceNo = invoiceNo.ToString(new string('0', invoiceInput.InvoiceNo.Length)),
                        InvoiceCode = invoiceInput.InvoiceCode,
                        IsActive = true,
                        TerminalId = terminalId
                    };

                    //生成发票记录(一张补票对应一张发票)
                    await _invoiceDomainService.AddInvoiceAsync(invoice);

                    //发票号递增
                    if (invoiceCode.InvoiceNumIsIncrease)
                        invoiceNo = invoiceNo + 1;
                    else
                        //发票号递减
                        invoiceNo = invoiceNo - 1;

                    var entity = input.FareAdjustment.MapTo<ExcessFare>();

                    //ExcessFare
                    await _excessFareRepository.InsertAndGetIdAsync(entity);
                }
                return Result.FromData(result.Data);
            }

            return result;
        }

        /// <summary>
        /// 作废票
        /// </summary>
        /// <returns></returns>
        public async Task<Result> CancelTicketAsync(IList<string> fareIds, int terminalId, int parkId)
        {
            /*
             *  1. 计算出总额 2. 生成交易记录 3. 生成作废记录和把补票记录作废
             * 
             */

            var fareList = new List<ExcessFare>();

            //检测作废的票是否有效，有效才可作废
            var inValid = _excessFareRepository.GetAll().Where(m => fareIds.Contains(m.Id)).Any(m => m.State != TicketSaleStatus.Valid);
            if (inValid)
                return Result.FromError("只有有效的补票才能作废");

            decimal amount = 0;
            //默认
            var payType = PayType.Cash;

            //计算出补票记录的总额
            foreach (var fareId in fareIds)
            {
                var fare = await _excessFareDomainService.GetExecessFareByIdAsync(fareId);

                if (fare != null)
                {
                    var tradeInfoDetails = await _tradeInfoRepository.GetAll().Where(m => m.Id == fare.TradeInfoId).Select(m => m.TradeInfoDetails).FirstOrDefaultAsync();
                    if (tradeInfoDetails.Count > 1)
                        return Result.FromError("多种支付交易的补票不能作废");

                    payType = tradeInfoDetails.Select(m => m.PayModeId).First();

                    //已经作废的票不做处理
                    if (fare.State == TicketSaleStatus.Invalid)
                        continue;

                    amount += fare.Denomination;

                    fareList.Add(fare);
                }
            }

            if (fareList.Count == 0)
                return Result.FromCode(ResultCode.NoRecord);

            //生成交易记录
            var tradeInfo = new TradeInfo
            {
                Amount = amount,
                //作废票交易类型为支出
                TradeType = TradeType.Outlay,
                TradeInfoDetails = new List<TradeInfoDetail>() {
                    new TradeInfoDetail
                    {
                        Amount = amount,
                        PayModeId = payType,
                        PayStatus = PayStatus.PaySuccess
                    }
                },
                PayModeId = payType
            };

            var trade = await _tradeInfoDomainService.AddTradeInfoAsync(tradeInfo, parkId);

            //生成作废记录和把补票记录作废
            foreach (var fare in fareList)
            {
                if (fare != null)
                {
                    var entity = new ExcessFareInvalid
                    {
                        //条形码
                        Id = fare.Id,
                        //新交易的交易号
                        TradeInfoId = trade.Id,
                        //原补票的交易号
                        OriginalTradeInfoId = fare.TradeInfoId,
                        TerminalId = terminalId,
                        Remark = fare.Remark,
                        //作废的金额
                        Amount = fare.Amount
                    };

                    //一条补票记录作废后对应一条作废记录
                    await _excessFareInvalidRepository.InsertAsync(entity);

                    fare.State = TicketSaleStatus.Invalid;

                    //更新补票记录状态为无效
                    await _excessFareRepository.UpdateAsync(fare);
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 根据条件获取有效补票记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetFareAdjustmentAsync<TDto>(IQuery<ExcessFare> query)
        {
            return await _excessFareRepository.AsNoTracking().FirstOrDefaultAsync<ExcessFare, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取有效补票记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetFareAdjustmentListAsync<TDto>(IQuery<ExcessFare> query)
        {
            return await _excessFareRepository.AsNoTracking().ToListAsync<ExcessFare, TDto>(query);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 新增网络票业务验证
        /// </summary>
        /// <param name="invoiceInput"></param>
        /// <param name="ticketCount"></param>
        /// <param name="invoiceNumIsIncrease"></param>
        /// <returns></returns>
        public Result<string> BusinessVerify(InvoiceInput invoiceInput, int ticketCount, bool invoiceNumIsIncrease)
        {
            if (string.IsNullOrWhiteSpace(invoiceInput.InvoiceNo) || string.IsNullOrWhiteSpace(invoiceInput.InvoiceCode))
                return Result.FromCode<string>(ResultCode.MissEssentialData, "发票号或发票代码不能为空");

            //检验发票全局唯一
            var existed = _invoiceAppService.CheckIfExisteInValidOrDuplicateInvoice(invoiceInput.InvoiceCode, invoiceInput.InvoiceNo, ticketCount, invoiceNumIsIncrease);
            if (existed)
                return Result.FromError<string>(ResultCode.DuplicateInvoiceRecord.DisplayName());

            return Result.FromCode<string>(ResultCode.Ok);
        }

        #endregion
    }
}
