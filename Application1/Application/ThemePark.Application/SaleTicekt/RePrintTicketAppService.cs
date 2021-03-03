using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Castle.Components.DictionaryAdapter;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.CardManage;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.Core.ParkSale.DomainServiceInterfaces;
using ThemePark.Core.TradeInfos;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 重打印门票服务
    /// </summary>
    public class RePrintTicketAppService : ThemeParkAppServiceBase, IRePrintTicketAppService
    {
        #region Fields

        private readonly IGroupTicketDomainService _groupTicketDomainService;
        private readonly IInvoiceDomainService _invoiceDomainService;
        private readonly IReprintDomainService _reprintDomainService;
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IRepository<TradeInfo, string> _tradeInfoRepository;
        private readonly IRepository<InParkBill, string> _inParkBillRepository;
        private readonly IRepository<ExcessFare, string> _excessFareRepository;
        private readonly IRepository<VIPVoucher, long> _vipVoucherRepository;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        #endregion

        #region Cotr

        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="groupTicketDomainService"></param>
        /// <param name="invoiceDomainService"></param>
        /// <param name="reprintDomainService"></param>
        /// <param name="nonGroupticketRepository"></param>
        /// <param name="excessFareRepository"></param>
        /// <param name="inParkBillRepository"></param>
        /// <param name="groupTicketRepository"></param>
        /// <param name="tradeInfoRepository"></param>
        /// <param name="vipVoucherRepository"></param>
        /// <param name="invoiceAppService"></param>
        /// <param name="invoiceCodeRepository"></param>
        /// <param name="toTicketRepository"></param>
        public RePrintTicketAppService(IGroupTicketDomainService groupTicketDomainService, IRepository<ExcessFare, string> excessFareRepository,
            IInvoiceDomainService invoiceDomainService, IRepository<InParkBill, string> inParkBillRepository,
            IReprintDomainService reprintDomainService, IRepository<NonGroupTicket, string> nonGroupticketRepository,
            IRepository<TOTicket, string> toTicketRepository,
            IRepository<GroupTicket, string> groupTicketRepository, IRepository<TradeInfo, string> tradeInfoRepository,
            IRepository<VIPVoucher, long> vipVoucherRepository, IInvoiceAppService invoiceAppService, IRepository<InvoiceCode> invoiceCodeRepository, IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository)
        {
            _groupTicketDomainService = groupTicketDomainService;
            _invoiceDomainService = invoiceDomainService;
            _reprintDomainService = reprintDomainService;
            _nonGroupTicketRepository = nonGroupticketRepository;
            _toTicketRepository = toTicketRepository;
            _groupTicketRepository = groupTicketRepository;
            _tradeInfoRepository = tradeInfoRepository;
            _inParkBillRepository = inParkBillRepository;
            _excessFareRepository = excessFareRepository;
            _vipVoucherRepository = vipVoucherRepository;
            _invoiceAppService = invoiceAppService;
            _invoiceCodeRepository = invoiceCodeRepository;
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 根据条码获取有效的票(或默认取最后一笔订单的所有票)
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminalId"></param>
        /// <param name="localParkId"></param>
        /// <returns>列表数据</returns>
        public async Task<Result<List<ReprintTicketPageRecord>>> GetTicketByBarCodeAsync(string barcode, int terminalId, int localParkId)
        {
            //默认找最后一笔交易的所有票
            if (string.IsNullOrWhiteSpace(barcode))
            {
                var latestTrade = _tradeInfoRepository.GetAllList(m => m.CreatorUserId == AbpSession.UserId && m.TradeType == TradeType.Income).OrderByDescending(m => m.CreationTime).FirstOrDefault();
                if (latestTrade != null)
                {
                    //散客票
                    if (await _nonGroupTicketRepository.AsNoTracking().AnyAsync(p => p.TradeInfoId == latestTrade.Id))
                    {
                        var nonGroupTicket = await GetNonGroupTicketByTradeIdAsync(latestTrade.Id);
                        //包含他园票
                        if (
                            await _otherNonGroupTicketRepository.AsNoTracking()
                                .AnyAsync(p => p.TradeInfoId == latestTrade.Id))
                        {
                            var list = new List<ReprintTicketPageRecord>();
                            var otherNonGroupTicketList = await GetOtherNonGroupTicketByTradeIdAsync(latestTrade.Id);
                            list.AddRange(otherNonGroupTicketList);
                            list.AddRange(nonGroupTicket);
                            //他园票和散客票
                            return Result.FromData(list);
                        }

                        return Result.FromData(nonGroupTicket);
                    }

                    //团体票
                    if (await _groupTicketRepository.AsNoTracking().AnyAsync(p => p.TradeInfoId == latestTrade.Id))
                    {
                        var groupTicket = await GetGroupTicketByTradeIdAsync(latestTrade.Id);
                        //包含他园票
                        if (
                            await _otherNonGroupTicketRepository.AsNoTracking()
                                .AnyAsync(p => p.TradeInfoId == latestTrade.Id))
                        {
                            var list = new List<ReprintTicketPageRecord>();
                            var otherNonGroupTicketList = await GetOtherNonGroupTicketByTradeIdAsync(latestTrade.Id);
                            list.AddRange(otherNonGroupTicketList);
                            list.AddRange(groupTicket);
                            //他园票和团体票
                            return Result.FromData(list);
                        }

                        return Result.FromData(groupTicket);
                    }
                    //他园票
                    if (await _otherNonGroupTicketRepository.AsNoTracking().AnyAsync(p => p.TradeInfoId == latestTrade.Id))
                        return Result.FromData(await GetOtherNonGroupTicketByTradeIdAsync(latestTrade.Id));

                }
                return Result.FromCode<List<ReprintTicketPageRecord>>(ResultCode.NoRecord, "");
            }

            var recordList = new List<ReprintTicketPageRecord>();

            Result<ReprintTicketPageRecord> result;
            var message = "没有查询到该票信息或者该票已无效";

            //散客票
            if (await _nonGroupTicketRepository.AsNoTracking().AnyAsync(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid))
            {
                result = await GetNonGroupTicketRecordAsync(barcode, localParkId);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //团体票
            if (await _groupTicketRepository.AsNoTracking().AnyAsync(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid))
            {
                result = await GetGroupTicketRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //网络票
            if (await _toTicketRepository.AsNoTracking().AnyAsync(p => p.Id == barcode && p.TicketFormEnum == TicketFormEnum.PaperTicket && p.TicketSaleStatus == TicketSaleStatus.Valid))
            {
                result = await GetTOTicketRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //入园单
            if (await _inParkBillRepository.AsNoTracking().AnyAsync(p => p.Id == barcode && p.InParkBillState == InParkBillState.Valid))
            {
                result = await GetInParkBillRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //补票
            if (await _excessFareRepository.AsNoTracking().AnyAsync(p => p.Id == barcode && p.State == TicketSaleStatus.Valid))
            {
                result = await GetExcessFareRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //年卡凭证(未激活的才能重打印)
            if (await _vipVoucherRepository.AsNoTracking().AnyAsync(p => p.Barcode == barcode && p.State == VipVoucherStateType.NotActive))
            {
                result = await GetVipVoucherRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            //他园票
            if (
                await _otherNonGroupTicketRepository.AsNoTracking()
                    .AnyAsync(m => m.Id == barcode && m.TicketSaleStatus == TicketSaleStatus.Valid))
            {
                result = await GetOtherNonGroupTicektRecordAsync(barcode);
                if (result.Success)
                    recordList.Add(result.Data);
                else
                    message = result.Message;
            }

            if (recordList.Count == 0)
                return Result.FromError<List<ReprintTicketPageRecord>>(message);

            return Result.FromData(recordList);
        }

        /// <summary>
        /// 生成重打印记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> GenerateReprintRecord(RePrintTicketModel model, int terminalId)
        {
            if (model.Inputs.Count <= 0)
                return Result.FromCode(ResultCode.InvalidData, "打印数据不能为空");

            Reprint print;

            //重新生成发票号打印，需要验证传入的发票号是否跟当前系统中的发票号重复
            if (model.RenewInvoice)
            {
                //取旧发票的发票代码，一笔订单的所有发票都是同一个发票代码
                var barCode1 = model.Inputs[0].BarCode;
                var oldInvoice1 = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barCode1 && m.IsActive);
                if (oldInvoice1 != null)
                {
                    //发票代码
                    var invoiceCode1 = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == oldInvoice1.InvoiceCode);

                    //发票号全局唯一
                    var existed = _invoiceAppService.CheckIfExisteInValidOrDuplicateInvoice(oldInvoice1.InvoiceCode, model.InvoiceNo,
                        model.Inputs.Count, invoiceCode1.InvoiceNumIsIncrease);

                    if (existed)
                        return Result.FromError(ResultCode.DuplicateInvoiceRecord.DisplayName());
                }
              
                for (var i = 0; i < model.Inputs.Count; i++)
                {
                    //当前有效的发票
                    var currentInvoiceNo = model.InvoiceNo;
                    //取旧发票的发票代码，一笔订单的所有发票都是同一个发票代码
                    var currentBarCode = model.Inputs[i].BarCode;
                    //需要取出每个旧的发票作废
                    var oldInvoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == currentBarCode && m.IsActive);

                    //发票号不为空
                    if (oldInvoice != null)
                    {
                        //发票代码
                        var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == oldInvoice.InvoiceCode);

                        //客户端传过来的都是一样的发票号，所以要给列表中的不同票分配不同的发票号
                        //第一条记录是正确的发票号，其他的需要加1
                        if (i != 0)
                        {
                            var invoiceNo = long.Parse(model.InvoiceNo);

                            //发票号递增
                            if (invoiceCode.InvoiceNumIsIncrease)
                                invoiceNo = invoiceNo + i;
                            else
                                //发票号递减
                                invoiceNo = invoiceNo - i;

                            //model.Inputs[i].InvoiceNo = invoiceNo.ToString(new string('0', model.Inputs[i].InvoiceNo.Length));
                            currentInvoiceNo = invoiceNo.ToString(new string('0', model.InvoiceNo.Length));
                        }
                     
                        var entity = new Invoice
                        {
                            Barcode = currentBarCode,
                            //前端已经把发票号自动加1了
                            InvoiceNo = currentInvoiceNo,
                            //共用旧发票的发票代码
                            InvoiceCode = oldInvoice.InvoiceCode,
                            IsActive = true,
                            TerminalId = terminalId
                        };

                        //获取新的发票Id给重打印记录
                        var result = await _invoiceDomainService.AddInvoiceAndReturnIdAsync(entity);
                        if (!result.Success)
                            return result;

                        //作废旧的发票
                        oldInvoice.IsActive = false;

                        //插入打印记录
                        print = new Reprint
                        {
                            Barcode = currentBarCode,
                            NewInvoiceId = result.Data,
                            OldInvoiceId = oldInvoice.Id,
                            Remark = model.Remark,
                            TerminalId = terminalId
                        };

                        await _reprintDomainService.AddReprint(print);

                        //更改票的原发票号为新的
                        await ChangeTicketInvoiceToNew(model.PrintTicketType, currentBarCode, result.Data, currentInvoiceNo);
                    }
                    else//没有发票号
                    {
                        //入园单打印，没有发票号和发票代码
                        //还有自助售票机出的票（散客票）也是没有发票号和发票代码的
                        //...

                        //插入打印记录
                        print = new Reprint
                        {
                            Barcode = model.Inputs[i].BarCode,
                            Remark = model.Remark,
                            TerminalId = terminalId
                        };

                        await _reprintDomainService.AddReprint(print);
                    }
                }
            }
            else//原发票号打印
            {
                foreach (var input in model.Inputs)
                {
                    var oldInvoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == input.BarCode && m.IsActive);

                    print = new Reprint
                    {
                        //发票为空的话是入园单重打印
                        //还有自助售票机出的票（散客票）也是没有发票号和发票代码的
                        //...
                        Barcode = input.BarCode,
                        NewInvoiceId = oldInvoice?.Id,
                        OldInvoiceId = oldInvoice?.Id,
                        Remark = model.Remark,
                        TerminalId = terminalId
                    };
                    await _reprintDomainService.AddReprint(print);
                }
            }
            return Result.Ok();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 获取散客重打印数据
        /// </summary>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetNonGroupTicketRecordAsync(string barcode, int localParkId)
        {
            var nonGroupTicket =
                await
                    _nonGroupTicketRepository.FirstOrDefaultAsync(
                        m => m.Id == barcode && m.TicketSaleStatus == TicketSaleStatus.Valid);

            if (nonGroupTicket == null)
                return Result.FromError<ReprintTicketPageRecord>("该散客票无效或不存在");

            // 他园票不能在他园重打印
            if (nonGroupTicket.ParkId != localParkId)
                return Result.FromError<ReprintTicketPageRecord>("非本公园出售的票不允许打印");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            //自助售票机售票会出现散客票没有发票号和发票代码的正常业务逻辑
            //if (invoice == null)
            //    return Result.FromError<ReprintTicketPageRecord>("该散客票的发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = nonGroupTicket.ParkSaleTicketClass.SaleTicketClassName,
                InvoiceCode = invoice != null ? invoice.InvoiceCode : "",
                InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                PrintTicketType = PrintTicketType.NonGroupTicket
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 根据交易号获取散客重打印数据
        /// </summary>
        /// <returns></returns>
        private async Task<List<ReprintTicketPageRecord>> GetNonGroupTicketByTradeIdAsync(string tradeId)
        {
            List<ReprintTicketPageRecord> records = new EditableList<ReprintTicketPageRecord>();
            //选取有效的票
            var nonGroupTicketList =
                await
                    _nonGroupTicketRepository.GetAllListAsync(
                        m => m.TradeInfoId == tradeId && m.TicketSaleStatus == TicketSaleStatus.Valid);

            foreach (var nonGroupTicket in nonGroupTicketList)
            {
                var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == nonGroupTicket.Id && m.IsActive);
                var record = new ReprintTicketPageRecord
                {
                    BarCode = nonGroupTicket.Id,
                    SaleTicketName = nonGroupTicket.ParkSaleTicketClass.SaleTicketClassName,
                    InvoiceCode = invoice != null ? invoice.InvoiceCode : "",
                    InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                    PrintTicketType = PrintTicketType.NonGroupTicket
                };

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// 根据交易号获取团体重打印数据
        /// </summary>
        /// <returns></returns>
        private async Task<List<ReprintTicketPageRecord>> GetGroupTicketByTradeIdAsync(string tradeId)
        {
            List<ReprintTicketPageRecord> records = new EditableList<ReprintTicketPageRecord>();
            //选取有效的票
            var groupTicketList =
                await
                    _groupTicketRepository.GetAllListAsync(
                        m => m.TradeInfoId == tradeId && m.TicketSaleStatus == TicketSaleStatus.Valid);

            foreach (var groupTicket in groupTicketList)
            {
                var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == groupTicket.Id && m.IsActive);
                var record = new ReprintTicketPageRecord
                {
                    BarCode = groupTicket.Id,
                    SaleTicketName = groupTicket.ParkSaleTicketClass.SaleTicketClassName,
                    InvoiceCode = invoice != null ? invoice.InvoiceCode : "",
                    InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                    PrintTicketType = PrintTicketType.GroupTicket
                };

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// 根据交易号获取他园票重打印数据
        /// </summary>
        /// <returns></returns>
        private async Task<List<ReprintTicketPageRecord>> GetOtherNonGroupTicketByTradeIdAsync(string tradeId)
        {
            List<ReprintTicketPageRecord> records = new EditableList<ReprintTicketPageRecord>();
            //选取有效的票
            var otherNonGroupTicketList =
                await
                    _otherNonGroupTicketRepository.GetAllListAsync(
                        m => m.TradeInfoId == tradeId && m.TicketSaleStatus == TicketSaleStatus.Valid);

            foreach (var otherNonGroupTicket in otherNonGroupTicketList)
            {
                var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == otherNonGroupTicket.Id && m.IsActive);
                var record = new ReprintTicketPageRecord
                {
                    BarCode = otherNonGroupTicket.Id,
                    SaleTicketName = otherNonGroupTicket.ParkSaleTicketClass.SaleTicketClassName,
                    InvoiceCode = invoice != null ? invoice.InvoiceCode : "",
                    InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                    PrintTicketType = PrintTicketType.OtherNonGroupTicket
                };

                records.Add(record);
            }

            return records;
        }


        /// <summary>
        /// 获取团体票信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetGroupTicketRecordAsync(string barcode)
        {
            //有效的团体票
            var groupTicket =
                await
                    _groupTicketDomainService.GetGroupTicketByBarcodeAsync<GroupTicket4RePrintDto>(
                        new Query<GroupTicket>(m => m.Id == barcode && m.TicketSaleStatus == TicketSaleStatus.Valid));

            if (groupTicket == null)
                return Result.FromError<ReprintTicketPageRecord>("该团体票无效或不存在");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            if (invoice == null)
                return Result.FromError<ReprintTicketPageRecord>("该团体票的发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = groupTicket.SaleTicketName,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceNo = invoice.InvoiceNo,
                PrintTicketType = PrintTicketType.GroupTicket
            };

            return Result.FromData(record);
        }



        /// <summary>
        /// 获取网络票
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetTOTicketRecordAsync(string barcode)
        {
            //网络票
            var toTicket =
                await
                    _toTicketRepository.FirstOrDefaultAsync(
                        p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid && p.TicketFormEnum == TicketFormEnum.PaperTicket);

            if (toTicket == null)
                return Result.FromError<ReprintTicketPageRecord>("该网络票无效或不存在");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            //自助售票机取网络票会出现网络票没有发票号和发票代码的正常业务逻辑
            //if (invoice == null)
            //    return Result.FromError<ReprintTicketPageRecord>("该网络票的发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = toTicket.AgencySaleTicketClass.AgencySaleTicketClassName,
                InvoiceCode = invoice != null ? invoice.InvoiceCode : "",
                InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                PrintTicketType = PrintTicketType.WebTicket
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 获取入园单
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetInParkBillRecordAsync(string barcode)
        {
            //入园票
            var inparkBill =
                await
                    _inParkBillRepository.FirstOrDefaultAsync(
                        p => p.Id == barcode && p.InParkBillState == InParkBillState.Valid);

            if (inparkBill == null)
                return Result.FromError<ReprintTicketPageRecord>("该入园单无效或不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = "入园单",
                PrintTicketType = PrintTicketType.Admission
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 获取补票记录
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetExcessFareRecordAsync(string barcode)
        {
            //补票
            var excessFare =
                await
                    _excessFareRepository.FirstOrDefaultAsync(
                        p => p.Id == barcode && p.State == TicketSaleStatus.Valid);

            if (excessFare == null)
                return Result.FromError<ReprintTicketPageRecord>("该补票无效或不存在");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            if (invoice == null)
                return Result.FromError<ReprintTicketPageRecord>("该补票的发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = "补票",
                InvoiceCode = invoice.InvoiceCode,
                InvoiceNo = invoice.InvoiceNo,
                PrintTicketType = PrintTicketType.FareAdjustment
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 获取年卡凭证
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetVipVoucherRecordAsync(string barcode)
        {
            //年卡凭证
            var vipVoucher =
                await
                    _vipVoucherRepository.FirstOrDefaultAsync(
                        p => p.Barcode == barcode && p.State == VipVoucherStateType.NotActive);

            if (vipVoucher == null)
                return Result.FromError<ReprintTicketPageRecord>("该年卡凭证无效或不存在");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            if (invoice == null)
                return Result.FromError<ReprintTicketPageRecord>("该年卡凭证发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = vipVoucher.ParkSaleTicketClass.SaleTicketClassName,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceNo = invoice.InvoiceNo,
                PrintTicketType = PrintTicketType.YearCardVoucher
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 获取他园票记录
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task<Result<ReprintTicketPageRecord>> GetOtherNonGroupTicektRecordAsync(string barcode)
        {
            //他园票
            var otherNonGroupTicekt =
                await
                    _otherNonGroupTicketRepository.FirstOrDefaultAsync(
                        p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid);

            if (otherNonGroupTicekt == null)
                return Result.FromError<ReprintTicketPageRecord>("该他园票无效或不存在");

            var invoice = await _invoiceDomainService.GetInvoiceAsync(m => m.Barcode == barcode && m.IsActive);

            //if (invoice == null)
            //    return Result.FromError<ReprintTicketPageRecord>("该他园票的发票信息不存在");

            var record = new ReprintTicketPageRecord
            {
                BarCode = barcode,
                SaleTicketName = "他园票",
                InvoiceCode = invoice?.InvoiceCode,
                InvoiceNo = invoice?.InvoiceNo,
                PrintTicketType = PrintTicketType.OtherNonGroupTicket
            };

            return Result.FromData(record);
        }

        /// <summary>
        /// 更改票的原发票号为新的
        /// </summary>
        /// <param name="printTicketType"></param>
        /// <param name="barCode"></param>
        /// <param name="newInvoiceId"></param>
        /// <param name="invoiceNo"></param>
        private async Task ChangeTicketInvoiceToNew(PrintTicketType printTicketType, string barCode, long newInvoiceId, string invoiceNo)
        {
            switch (printTicketType)
            {
                case PrintTicketType.NonGroupTicket:
                    {
                        var nonGroupTicket = await _nonGroupTicketRepository.FirstOrDefaultAsync(barCode);
                        nonGroupTicket.InvoiceId = newInvoiceId;
                        break;
                    }
                case PrintTicketType.GroupTicket:
                    {
                        var groupTicket = await _groupTicketRepository.FirstOrDefaultAsync(barCode);
                        groupTicket.InvoiceId = newInvoiceId;
                        break;
                    }
                case PrintTicketType.WebTicket:
                    {
                        var toTicket = await _toTicketRepository.FirstOrDefaultAsync(barCode);
                        toTicket.InvoiceId = newInvoiceId;
                        break;
                    }
                case PrintTicketType.OtherNonGroupTicket:
                    {
                        var otherNonGroupTicket = await _otherNonGroupTicketRepository.FirstOrDefaultAsync(barCode);
                        otherNonGroupTicket.InvoiceId = newInvoiceId;
                        break;
                    }
                case PrintTicketType.YearCardVoucher:
                    {
                        var vipVoucher = await _vipVoucherRepository.FirstOrDefaultAsync(m => m.Barcode == barCode);
                        vipVoucher.Invoice = invoiceNo;
                        break;
                    }
            }
        }

        #endregion
    }
}
