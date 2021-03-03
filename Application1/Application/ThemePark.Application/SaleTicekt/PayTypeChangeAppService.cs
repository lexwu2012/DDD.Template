using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.Agencies;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.ParkSale;
using ThemePark.Core.TradeInfos;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleTicekt
{
    public class PayTypeChangeAppService : ThemeParkAppServiceBase, IPayTypeChangeAppService
    {
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<TradeInfoDetail, string> _tradeinfoDetailRepository;
        private readonly IRepository<TradeInfo, string> _tradeinfoRepository;
        private readonly IRepository<ChangePayModeTrack, long> _changePayModeTrackRepository;
        private readonly IRepository<AccountOp, long> _accountOpRepository;
        private readonly IRepository<Account> _accountRepository;

        private readonly IRepository<VIPCard, long> _vipCardRepository;
        private readonly IRepository<VIPVoucher, long> _vipVoucherRepository;
        private readonly IAgencyAccountAppService _agencyAccountAppService;

        public PayTypeChangeAppService(IRepository<NonGroupTicket, string> nonGroupTicketRepository
            , IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository
            , IRepository<GroupTicket, string> groupTicketRepository
            , IRepository<User, long> userRepository
            , IRepository<TradeInfoDetail, string> tradeinfoDetailRepository
            , IRepository<TradeInfo, string> tradeinfoRepository
            , IRepository<ChangePayModeTrack, long> changePayModeTrackRepository
            , IRepository<AccountOp, long> accountOpRepository
            , IRepository<Account> accountRepository
            , IRepository<VIPCard, long> vipCardRepository
            , IRepository<VIPVoucher, long> vipVoucherRepository, IAgencyAccountAppService agencyAccountAppService)
        {
            _nonGroupTicketRepository = nonGroupTicketRepository;
            _groupTicketRepository = groupTicketRepository;
            _userRepository = userRepository;
            _tradeinfoDetailRepository = tradeinfoDetailRepository;
            _tradeinfoRepository = tradeinfoRepository;
            _changePayModeTrackRepository = changePayModeTrackRepository;
            _accountOpRepository = accountOpRepository;
            _accountRepository = accountRepository;
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
            _vipCardRepository = vipCardRepository;
            _vipVoucherRepository = vipVoucherRepository;
            _agencyAccountAppService = agencyAccountAppService;
        }


        /// <summary>
        /// 根据其中一条码获取整笔交易的售票详情
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<Result<List<GetSaleDetailDto>>> GetSaleDetail(string barcode)
        {
            var parkId = int.Parse(ConfigurationManager.AppSettings["LocalParkId"]);
            if (string.IsNullOrEmpty(barcode))
                return Result.FromCode<List<GetSaleDetailDto>>(ResultCode.NoRecord);

            var check = await _nonGroupTicketRepository.GetAll().AnyAsync(p => p.Id == barcode && DbFunctions.DiffDays(p.CreationTime, DateTime.Today) == 0 && p.ParkId == parkId);
            if (check)
            {
                var nonGroupTicket = await _nonGroupTicketRepository.GetAsync(barcode);
                return await GetNonGroupSaleDetail(nonGroupTicket.TradeInfoId);
            }

            var checkOtherTicket = await _otherNonGroupTicketRepository.GetAll().AnyAsync(p => p.Id == barcode && DbFunctions.DiffDays(p.CreationTime, DateTime.Today) == 0 && p.ParkId == parkId);
            if (checkOtherTicket)
            {
                var otherNonGroupTicket = await _otherNonGroupTicketRepository.GetAsync(barcode);
                return await GetOtherNonGroupSaleDetail(otherNonGroupTicket.TradeInfoId);
            }
            var groupTicket = await _groupTicketRepository.GetAll().FirstOrDefaultAsync(p => p.Id == barcode && DbFunctions.DiffDays(p.CreationTime, DateTime.Today) == 0 && p.ParkId == parkId);
            if (groupTicket != null)
            {
                return await GetGroupSaleDetail(groupTicket.TradeInfoId);
            }

            var vipcard = await _vipCardRepository.GetAll().FirstOrDefaultAsync(p =>  p.IcBasicInfo.IcNo == barcode && DbFunctions.DiffDays(p.SaleTime, DateTime.Today) == 0 && p.ParkId == parkId);
            if (vipcard != null)
            {
                if(vipcard.State!= VipCardStateType.Actived && vipcard.State!= VipCardStateType.NotActive)
                {
                    return Result.FromError<List<GetSaleDetailDto>>(vipcard.State.DisplayName()+ "状态的年卡不允许更改支付方式");
                }

                if (!string.IsNullOrWhiteSpace(vipcard.TradeInfoId))
                {
                    return await GetVipCardSaleDetail(vipcard.TradeInfoId);
                }
                else
                {
                    var vipCardVoucher = await _vipVoucherRepository.GetAll().FirstOrDefaultAsync(p => p.VIPCardId == vipcard.Id && DbFunctions.DiffDays(p.CreationTime, DateTime.Today) == 0 && p.ParkId == parkId);
                    if (vipCardVoucher != null)
                    {
                        return await GetVipVoucherSaleDetail(vipCardVoucher.TradeInfoId);
                    }
                }
            }

            var vipVoucher = await _vipVoucherRepository.GetAll().FirstOrDefaultAsync(p => p.Barcode == barcode && DbFunctions.DiffDays(p.CreationTime, DateTime.Today) == 0 && p.ParkId == parkId);
            if (vipVoucher != null)
            {
                if (vipVoucher.State !=  VipVoucherStateType .Actived && vipVoucher.State != VipVoucherStateType.NotActive)
                {
                    return Result.FromError<List<GetSaleDetailDto>>(vipVoucher.State.DisplayName() + "状态的年卡不允许更改支付方式");
                }

                return await GetVipVoucherSaleDetail(vipVoucher.TradeInfoId);
            }

            return Result.FromCode<List<GetSaleDetailDto>>(ResultCode.NoRecord);

        }

        /// <summary>
        /// 取年卡凭证销售详情
        /// </summary>
        /// <param name="tradeinfoId"></param>
        /// <returns></returns>
        private async Task<Result<List<GetSaleDetailDto>>> GetVipVoucherSaleDetail(string tradeinfoId)
        {
            List<GetSaleDetailDto> results = new List<GetSaleDetailDto>();
            var vipVouchers = await _vipVoucherRepository.GetAll().Include(p => p.TradeInfo).Where(p => p.TradeInfoId == tradeinfoId).ToListAsync();
            foreach (var vipVoucher in vipVouchers)
            {
                if (vipVoucher.State != VipVoucherStateType.Actived && vipVoucher.State != VipVoucherStateType.NotActive)
                {
                    return Result.FromError<List<GetSaleDetailDto>>("订单中存在 " + vipVoucher.State.DisplayName() + "状态的凭证不允许更改支付方式");
                }

                    var result = new GetSaleDetailDto()
                {
                    TicketName = vipVoucher.ParkSaleTicketClass.SaleTicketClassName,
                    StateDisplayName = vipVoucher.State.DisplayName(),
                    Creator = _userRepository.GetAll().FirstAsync(p => p.Id == vipVoucher.CreatorUserId.Value).Result.Name,
                    PayModeId = vipVoucher.TradeInfo.PayModeId,
                    TradeinfoId = tradeinfoId,
                    CreationTime = vipVoucher.CreationTime,
                    Amount = vipVoucher.SalePrice,
                    SalePrice = vipVoucher.SalePrice,
                    Qty = 1,
                    Id = vipVoucher.Barcode.ToString(),
                    
                };
                var tradeinfoDetail =
                    _tradeinfoDetailRepository.GetAllListAsync(p => p.TradeInfoId == vipVoucher.TradeInfoId).Result;

                tradeinfoDetail.ForEach(p => result.PayMode += p.PayModeId.DisplayName());
                results.Add(result);
            }
            return Result.FromData(results);
        }

        /// <summary>
        /// 取年卡销售详情
        /// </summary>
        /// <param name="tradeinfoId"></param>
        /// <returns></returns>
        private async Task<Result<List<GetSaleDetailDto>>> GetVipCardSaleDetail(string tradeinfoId)
        {
            List<GetSaleDetailDto> results = new List<GetSaleDetailDto>();
            var vipCards = await _vipCardRepository.GetAll().Include(p => p.TradeInfo).Where(p => p.TradeInfoId == tradeinfoId).ToListAsync();
            foreach (var vipCard in vipCards)
            {
                if (vipCard.State !=  VipCardStateType.Actived && vipCard.State != VipCardStateType.NotActive)
                {
                    return Result.FromError<List<GetSaleDetailDto>>("订单中存在 "+vipCard.State.DisplayName() + "状态的年卡不允许更改支付方式");
                }

                var result = new GetSaleDetailDto()
                {
                    TicketName = vipCard.ParkSaleTicketClass.SaleTicketClassName,
                    StateDisplayName = vipCard.State.DisplayName(),
                    Creator = _userRepository.GetAll().FirstAsync(p => p.Id == vipCard.SaleUser.Value).Result.Name,
                    PayModeId = vipCard.TradeInfo.PayModeId,
                    CreationTime = vipCard.CreationTime,
                    Amount = vipCard.SalePrice,
                    SalePrice = vipCard.SalePrice,
                    Qty = 1,
                    Id = vipCard.IcBasicInfo.IcNo.ToString(),
                    TradeinfoId=tradeinfoId
                };
                var tradeinfoDetail =
                    _tradeinfoDetailRepository.GetAllListAsync(p => p.TradeInfoId == vipCard.TradeInfoId).Result;
                tradeinfoDetail.ForEach(p => result.PayMode += p.PayModeId.DisplayName());
                results.Add(result);
            }
            return Result.FromData(results);
        }



        /// <summary>
        /// 支付方式更改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> PayTypeChange(PayTypeChangeInput input)
        {

            var tradeinfo = await _tradeinfoRepository.GetAsync(input.TradeInfoId);
            switch (input.ChangeType)
            {
                case ChangeType.BankCardToCash:
                    return await DealChange(PayType.BankCard, PayType.Cash, tradeinfo);
                case ChangeType.CashToBankCard:
                    return await DealChange(PayType.Cash, PayType.BankCard, tradeinfo);
                case ChangeType.PrePayToCash:
                    return await DealChange(PayType.PrePay, PayType.Cash, tradeinfo);
                case ChangeType.CashToPrePay:
                    return await DealChange(PayType.Cash, PayType.PrePay, tradeinfo);
                case ChangeType.AccoundToCash:
                    return await DealChange(PayType.Account, PayType.Cash, tradeinfo);
                case ChangeType.CashToAccound:
                    return await DealChange(PayType.Cash, PayType.Account, tradeinfo);
                default:
                    return Result.FromCode(ResultCode.InvalidData);
            }

        }


        /// <summary>
        /// 处理转换
        /// </summary>
        /// <param name="sourcePayType"></param>
        /// <param name="targetPayType"></param>
        /// <param name="tradeInfo"></param>
        /// <returns></returns>
        private async Task<Result> DealChange(PayType sourcePayType, PayType targetPayType, TradeInfo tradeInfo)
        {
            var check = await PayTypeChangeCheck(sourcePayType, targetPayType, tradeInfo);
            if (!check.Success)
                return check;

            tradeInfo.PayModeId = targetPayType;
            tradeInfo.TradeInfoDetails.First().PayModeId = targetPayType;
            await _tradeinfoRepository.UpdateAsync(tradeInfo);

            //现金转挂账、预付款 需要添加操作记录
            if (targetPayType == PayType.Account || targetPayType == PayType.PrePay)
            {
                var grouopticket = await _groupTicketRepository.GetAllIncluding(m=>m.Agency).FirstOrDefaultAsync(p => p.TradeInfoId == tradeInfo.Id);

                await _agencyAccountAppService.ConsumptionAccount(new AccountOpInput()
                {
                    AccountId = grouopticket.Agency.AccountId,
                    Cash = tradeInfo.Amount,
                    OpType = OpType.Consumption,
                    TradeInfoId = tradeInfo.Id
                }, grouopticket.ParkId);
            }

            //挂账、预付款 转现金
            if (sourcePayType == PayType.Account || sourcePayType == PayType.PrePay)
            {
                var grouopticket = await _groupTicketRepository.GetAllIncluding(m => m.Agency).FirstOrDefaultAsync(p => p.TradeInfoId == tradeInfo.Id);
                //账户退款
                await _agencyAccountAppService.RefundAccount(new AccountOpInput()
                {
                    AccountId = grouopticket.Agency.AccountId,
                    Cash = tradeInfo.Amount,
                    OpType = OpType.Refund,
                    TradeInfoId = tradeInfo.Id
                }, grouopticket.ParkId);
            }

            //插入转换记录
            await _changePayModeTrackRepository.InsertAsync(new ChangePayModeTrack()
            {
                TradeinfoId = tradeInfo.Id,
                SourcePayType = sourcePayType,
                TargetPayType = targetPayType
            });

            return Result.Ok();

        }

        /// <summary>
        /// 支付转换业务验证
        /// </summary>
        /// <param name="sourcePayType"></param>
        /// <param name="targetPayType"></param>
        /// <param name="tradeInfo"></param>
        /// <returns></returns>
        private async Task<Result> PayTypeChangeCheck(PayType sourcePayType, PayType targetPayType, TradeInfo tradeInfo)
        {
            if (tradeInfo.PayModeId != sourcePayType)
                return Result.FromCode(ResultCode.InvalidData);

            //散客票不允许转挂账、预付款
            if ((targetPayType == PayType.PrePay || targetPayType == PayType.Account) && (await IsNonGroupTicket(tradeInfo.Id) || await IsVipCard(tradeInfo.Id) || await IsVipVoucher(tradeInfo.Id)))
                return Result.FromCode(ResultCode.InvalidData, "散客票/年卡/年卡凭证不允许转挂账、预付款");

            if (targetPayType == PayType.PrePay)
            {
                var groupticket = await _groupTicketRepository.GetAllIncluding(m => m.Agency).FirstOrDefaultAsync(p => p.TradeInfoId == tradeInfo.Id);
                var account = await _accountRepository.GetAsync(groupticket.Agency.AccountId);
                if (account.Balance < tradeInfo.Amount)
                    return Result.FromCode(ResultCode.InvalidData, "预存款金额不足");
            }

            return Result.Ok();
        }

        /// <summary>
        /// 判断是否年卡
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsVipCard(string tradeinfoId)
        {
            return await _vipCardRepository.GetAll().AnyAsync(p => p.TradeInfoId == tradeinfoId);
        }

        /// <summary>
        /// 判断是否年卡凭证
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsVipVoucher(string tradeinfoId)
        {
            return await _vipVoucherRepository.GetAll().AnyAsync(p => p.TradeInfoId == tradeinfoId);
        }

        /// <summary>
        /// 判断是否团体票
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsNonGroupTicket (string tradeinfoId)
        {
            return await _nonGroupTicketRepository.GetAll().AnyAsync(p => p.TradeInfoId == tradeinfoId);
        }


        /// <summary>
        /// 取散客票购票详情
        /// </summary>
        /// <param name="tradeinfoId"></param>
        /// <returns></returns>
        private async Task<Result<List<GetSaleDetailDto>>> GetNonGroupSaleDetail(string tradeinfoId)
        {
            List<GetSaleDetailDto> results = new List<GetSaleDetailDto>();
            var nonGroupTickets = await _nonGroupTicketRepository.GetAll().Include(p => p.TradeInfo).Where(p => p.TradeInfoId == tradeinfoId).ToListAsync();
            foreach (var ticket in nonGroupTickets)
            {
                var result = new GetSaleDetailDto()
                {
                    TicketName = ticket.ParkSaleTicketClass.SaleTicketClassName,
                    StateDisplayName = ticket.TicketSaleStatus.DisplayName(),
                    Creator = _userRepository.GetAll().FirstAsync(p => p.Id == ticket.CreatorUserId.Value).Result.Name,
                    PayModeId = ticket.TradeInfo.PayModeId

                };
                var tradeinfoDetail =
                    _tradeinfoDetailRepository.GetAllListAsync(p => p.TradeInfoId == ticket.TradeInfoId).Result;
                tradeinfoDetail.ForEach(p => result.PayMode += p.PayModeId.DisplayName());
                results.Add(ticket.MapTo(result));
            }
            return Result.FromData(results);
        }


        /// <summary>
        /// 取他园票购票详情
        /// </summary>
        /// <param name="tradeinfoId"></param>
        /// <returns></returns>
        private async Task<Result<List<GetSaleDetailDto>>> GetOtherNonGroupSaleDetail(string tradeinfoId)
        {
            List<GetSaleDetailDto> results = new List<GetSaleDetailDto>();
            var nonGroupTickets = await _otherNonGroupTicketRepository.GetAll().Include(p => p.TradeInfo).Where(p => p.TradeInfoId == tradeinfoId).ToListAsync();
            foreach (var ticket in nonGroupTickets)
            {
                var result = new GetSaleDetailDto()
                {
                    TicketName = ticket.ParkSaleTicketClass.SaleTicketClassName,
                    StateDisplayName = ticket.TicketSaleStatus.DisplayName(),
                    Creator = _userRepository.GetAll().FirstAsync(p => p.Id == ticket.CreatorUserId.Value).Result.Name,
                    PayModeId = ticket.TradeInfo.PayModeId

                };
                var tradeinfoDetail =
                    _tradeinfoDetailRepository.GetAllListAsync(p => p.TradeInfoId == ticket.TradeInfoId).Result;
                tradeinfoDetail.ForEach(p => result.PayMode += p.PayModeId.DisplayName());
                results.Add(ticket.MapTo(result));
            }
            return Result.FromData(results);
        }
        /// <summary>
        /// 取团体购票详情
        /// </summary>
        /// <param name="tradeinfoId"></param>
        /// <returns></returns>
        private async Task<Result<List<GetSaleDetailDto>>> GetGroupSaleDetail(string tradeinfoId)
        {

            List<GetSaleDetailDto> results = new List<GetSaleDetailDto>();
            var nonGroupTickets = await _groupTicketRepository.GetAll().Include(p => p.TradeInfo).Where(p => p.TradeInfoId == tradeinfoId).ToListAsync();
            foreach (var ticket in nonGroupTickets)
            {
                var result = new GetSaleDetailDto()
                {
                    TicketName = ticket.AgencySaleTicketClass.AgencySaleTicketClassName,
                    AgencyName = ticket.Agency.AgencyName,
                    StateDisplayName = ticket.TicketSaleStatus.DisplayName(),
                    Creator = _userRepository.GetAll().FirstAsync(p => p.Id == ticket.CreatorUserId.Value).Result.Name,
                    PayModeId = ticket.TradeInfo.PayModeId

                };
                var tradeinfoDetail =
                    _tradeinfoDetailRepository.GetAllListAsync(p => p.TradeInfoId == ticket.TradeInfoId).Result;
                tradeinfoDetail.ForEach(p => result.PayMode += p.PayModeId.DisplayName());

                results.Add(ticket.MapTo(result));
            }
            return Result.FromData(results);
        }


    }
}
