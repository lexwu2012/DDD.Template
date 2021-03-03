using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using ThemePark.Application.SaleCard.Dto;
using ThemePark.Application.SaleCard.Interfaces;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.VerifyTicketDto.Model;
using ThemePark.Core.TradeInfos;
using ThemePark.Core.TradeInfos.DomainServiceInterfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Core.CardManage.Repositories;
using ThemePark.Core.ParkSale.Repositories;
using ThemePark.Infrastructure;
using Abp.Dependency;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.DataSync.Dto;
using Newtonsoft.Json;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Core.DataSync;
using ThemePark.FaceClient;
using static ThemePark.Application.VerifyTicket.Finger.FingerCache;
using ThemePark.Application.VerifyTicket.Finger;
using CustomerInput = ThemePark.Application.SaleCard.Dto.CustomerInput;
using ThemePark.Application.OTA.DTO;

namespace ThemePark.Application.SaleCard
{
    /// <summary>
    /// 年卡销售应用层服务
    /// </summary>
    public class VIPCardAppService : ThemeParkAppServiceBase, IVIPCardAppService
    {
        #region Fields
        private readonly IRepository<VIPCard, long> _parkVipCardRepository;
        private readonly IRepository<IcBasicInfo, long> _parkIcBasicInfoRepository;

        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;


        private readonly IParkRepository _parkRepository;

        private readonly IIcBasicInfoRepository _icBasicInfoRepository;

        private readonly IVipVoucherRepository _parkVipVoucherRepository;
        private readonly ICustomerRepository _parkCustomerRepository;
        private readonly IUserICRepository _parkUserIcRepository;
        private readonly IRepository<IcoperDetail, long> _parkIcoperdetailRepository;
        private readonly IRepository<FillCard, long> _parkFillCardRepository;
        private readonly IRepository<VipCardReturn, long> _vipCardReturnRepository;
        private readonly IRepository<VipVoucherReturn, long> _vipVoucherReturnRepository;
        private readonly IRepository<TicketClass> _parkTicketClassRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IRepository<VipCardRenewalSet> _parkVipCardRenewalRepository;
        private readonly ISettingManager _settingManager;
        private readonly IUniqueCode _uniqueCode;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRepository<TradeInfo, string> _tradeInfoRepository;
        private readonly ITradeInfoDomainService _tradeInfoDomainService;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly ITradeInfoAppService _tradeInfoAppService;
        #endregion

        /// <summary>
        /// cotr
        /// </summary>

        public VIPCardAppService(IRepository<VIPCard, long> parkVipCardRepository, IVipVoucherRepository parkVipVoucherRepository, ICustomerRepository parkCustomerRepository
            , IUserICRepository parkUserIcRepository, IRepository<IcoperDetail, long> parkIcoperdetailRepository, IRepository<FillCard, long> parkFillCardRepository
            , IRepository<VipCardRenewalSet> parkVipCardRenewalRepository, IRepository<TicketClass> parkTicketClassRepository, ISettingManager settingManager
            , IUniqueCode uniqueCode, IInvoiceRepository invoiceRepository, IRepository<TradeInfo, string> tradeInfoRepository
            , IRepository<VipCardReturn, long> vipCardReturnRepository, ITradeInfoDomainService tradeInfoDomainService, IRepository<TOTicket, string> toTicketRepository
            , IRepository<VipVoucherReturn, long> vipVoucherReturnRepository, IRepository<InvoiceCode> invoiceCodeRepository,
            IInvoiceAppService invoiceAppService, ITradeInfoAppService tradeInfoAppService
            , IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository
            , IRepository<IcBasicInfo, long> parkIcBasicInfoRepository
            , IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository
            , IParkRepository parkRepository
            , IIcBasicInfoRepository icBasicInfoRepository)
        {
            _icBasicInfoRepository = icBasicInfoRepository;
            _parkRepository = parkRepository;
            _tradeInfoDomainService = tradeInfoDomainService;
            _parkVipCardRepository = parkVipCardRepository;
            _parkVipVoucherRepository = parkVipVoucherRepository;
            _parkCustomerRepository = parkCustomerRepository;
            _parkUserIcRepository = parkUserIcRepository;
            _parkIcoperdetailRepository = parkIcoperdetailRepository;
            _parkFillCardRepository = parkFillCardRepository;
            _parkVipCardRenewalRepository = parkVipCardRenewalRepository;
            _parkTicketClassRepository = parkTicketClassRepository;
            _settingManager = settingManager;
            _uniqueCode = uniqueCode;
            _invoiceRepository = invoiceRepository;
            _tradeInfoRepository = tradeInfoRepository;
            _vipCardReturnRepository = vipCardReturnRepository;
            _toTicketRepository = toTicketRepository;
            _vipVoucherReturnRepository = vipVoucherReturnRepository;
            _invoiceCodeRepository = invoiceCodeRepository;
            _invoiceAppService = invoiceAppService;
            _tradeInfoAppService = tradeInfoAppService;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _parkIcBasicInfoRepository = parkIcBasicInfoRepository;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
        }

        /// <summary>
        /// 根据身份证获取年卡用户详细信息
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        public List<VipCardCustomerDto> SearchCustomerDetail(string  idnum)
        {
            return _parkCustomerRepository.GetAll().Where(p => p.Pid == idnum).MapTo<List<VipCardCustomerDto>>();
        }

        /// <summary>
        /// 根据身份证获取年卡用户详细信息
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        public VipCardCustomerDto GetCustomerDetail(string idnum)
        {

            return _parkCustomerRepository.GetAll().Where(p => p.Pid == idnum && p.Photo != null).OrderByDescending(p => p.Id).FirstOrDefault().MapTo<VipCardCustomerDto>();

            //return _parkCustomerRepository.FirstOrDefault(p => p.Pid == idnum && p.Photo!= null).MapTo<VipCardCustomerDto>();
        }

        /// <summary>
        /// 根据身份证获取年卡用户详细信息列表
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        public List<VipCardCustomerDto> GetListCustomerDetail(string idnum)
        {

           return _parkCustomerRepository.GetAll().Where(p => p.Pid == idnum && p.Photo != null).OrderByDescending(p => p.Id).MapTo<List<VipCardCustomerDto>>();
        }


        /// <summary>
        /// 旧年卡批量生成电子年卡卡号
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> CreateOldCardEcardId(int terminalId)
        {
            var icBasicInfos = new List<IcBasicInfo>();
            var icBasicInfoList = _parkIcBasicInfoRepository.GetAll().Where(p => p.ECardID == null).ToList();

            foreach (var icBasicInfo in icBasicInfoList)
            {
                var eCardNo = await _uniqueCode.CreateAsync(CodeType.ECard, AbpSession.LocalParkId, terminalId);
                icBasicInfo.ECardID = eCardNo;
                icBasicInfos.Add(icBasicInfo);
            }

            _icBasicInfoRepository.UpdateRange(icBasicInfos);
            await CurrentUnitOfWork.SaveChangesAsync();

            return Result.Ok();
        }


        /// <summary>
        /// 获取电子年卡照片（OTA）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<string>> SearchECardPhotoAsync(ECardPhotoInput input)
        {
            var customer = await _parkCustomerRepository.FirstOrDefaultAsync(p => p.Id == input.customerId);
            if (customer != null)
            {
                if (customer.Photo == null)
                {
                    return Result.FromError<string>("未登记照片");
                }
                else
                {
                    return new Result<string>(Convert.ToBase64String(customer.Photo));
                }
            }
            else
            {
                return Result.FromError<string>("未获取到相关信息");
            }

        }



        /// <summary>
        /// 获取电子年卡信息（OTA）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<ECardDetailDto>>> SearchECardDetailAsync(ECardDetailInput input)
        {

            //ECardDetailDto eCardDetailDto = new ECardDetailDto();
            //ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
            //List<ECardParkDto> eCardPark = new List<ECardParkDto>();
            //List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();


            //按凭证查询   查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
            var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Barcode == input.queryStr);
            if (vipVoucher != null)
            {


                if (vipVoucher.State != VipVoucherStateType.Actived)
                {
                    return Result.FromError<List<ECardDetailDto>>("年卡凭证未激活");
                }

                ECardDetailDto eCardDetailDto = new ECardDetailDto();
                ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                List<ECardParkDto> eCardPark = new List<ECardParkDto>();
                List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                var vipCardResult = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.VIPCardId);

                var vipCard = vipCardResult.MapTo<ECardVIPCardDto>();
                // var vipCard =await  _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.VIPCardId).MapTo<ECardVIPCardDto>();

                eCardDetailDto.VipCardId = vipCard.Id;

                eCardDetailDto.IcNo = vipCard.IcNo;
                eCardDetailDto.CardNo = vipCard.CardNo;
                eCardDetailDto.ECardId = vipCard.ECardID;

                eCardDetailDto.State = vipCard.State;
                eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");

                //系统暂无判断季卡的字段，暂时不是年卡则判断为季卡
                if (vipCard.InParkValidDays < 300)
                {
                    eCardDetailDto.ECardType = ECardType.SeasonCard;
                }
                else
                {
                    switch (vipCard.Persons)
                    {
                        case 1:
                            eCardDetailDto.ECardType = ECardType.SingleCard;
                            break;
                        case 2:
                            eCardDetailDto.ECardType = ECardType.DoubleCard;
                            break;
                        case 3:
                            eCardDetailDto.ECardType = ECardType.ThreeCard;
                            break;
                    }
                }
                var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                {
                    Id=o.Customer.Id,
                    CustomName = o.Customer.CustomerName,
                    Gender = o.Customer.GenderType,
                    PhoneNumber = o.Customer.PhoneNumber,
                    PhotoString = o.Customer.Photo == null ? null : Convert.ToBase64String(o.Customer.Photo),
                    Pid = o.Customer.Pid
                }).ToList();

                eCardDetailDto.ECardCustoer = listECardCustoerDto;

                if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                {
                    vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                    var parks = vipCard.InParkIdFilter.Split(',');
                    foreach (var parkid in parks)
                    {
                        int parkid1 = int.Parse(parkid.Trim());
                        var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);
                        var park = parkResult.MapTo<ECardParkDto>();
                        eCardPark.Add(park);
                    }

                    eCardDetailDto.ECardPark = eCardPark;
                }
                else
                {
                    var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                    var park = parkResult.MapTo<ECardParkDto>();
                    eCardPark.Add(park);
                    eCardDetailDto.ECardPark = eCardPark;
                }

                listECardDetailDto.Add(eCardDetailDto);

                return new Result<List<ECardDetailDto>>(listECardDetailDto);


            }
            else
            {
                //按电子年卡卡号、实体卡号查询    查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
                var vipCardModel =await _parkVipCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == input.queryStr || p.IcBasicInfo.ECardID == input.queryStr || p.IcBasicInfo.CardNo == input.queryStr);

                //var vipCardModel = vipCardModelResult;


                if (vipCardModel != null && vipCardModel!=null)
                {
                    if (vipCardModel.State != VipCardStateType.Actived)
                    {
                        return Result.FromError<List<ECardDetailDto>>("年卡未激活");
                    }

                    ECardDetailDto eCardDetailDto = new ECardDetailDto();
                    ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                    List<ECardParkDto> eCardPark = new List<ECardParkDto>();
                    List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                    var vipCard = vipCardModel.MapTo<ECardVIPCardDto>();

                    eCardDetailDto.VipCardId = vipCard.Id;

                    eCardDetailDto.IcNo = vipCard.IcNo;
                    eCardDetailDto.CardNo = vipCard.CardNo;
                    eCardDetailDto.ECardId = vipCard.ECardID;

                    eCardDetailDto.State = vipCard.State;
                    eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                    eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                    eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");
                    if (vipCard.InParkValidDays < 100)
                    {
                        eCardDetailDto.ECardType = ECardType.SeasonCard;
                    }
                    else
                    {
                        switch (vipCard.Persons)
                        {
                            case 1:
                                eCardDetailDto.ECardType = ECardType.SingleCard;
                                break;
                            case 2:
                                eCardDetailDto.ECardType = ECardType.DoubleCard;
                                break;
                            case 3:
                                eCardDetailDto.ECardType = ECardType.ThreeCard;
                                break;
                        }
                    }
                    var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                    {
                        Id = o.Customer.Id,
                        CustomName = o.Customer.CustomerName,
                        Gender = o.Customer.GenderType,
                        PhoneNumber = o.Customer.PhoneNumber,
                        PhotoString = o.Customer.Photo == null ? null : Convert.ToBase64String(o.Customer.Photo),
                        Pid = o.Customer.Pid
                    }).ToList();

                    eCardDetailDto.ECardCustoer = listECardCustoerDto;

                    if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                    {
                        vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                        var parks = vipCard.InParkIdFilter.Split(',');
                        foreach (var parkid in parks)
                        {
                            int parkid1 = int.Parse(parkid.Trim());
                            var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);
                            var park = parkResult.MapTo<ECardParkDto>();
                            eCardPark.Add(park);
                        }

                        eCardDetailDto.ECardPark = eCardPark;
                    }
                    else
                    {
                        var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                        var park = parkResult.MapTo<ECardParkDto>();
                        eCardPark.Add(park);
                        eCardDetailDto.ECardPark = eCardPark;
                    }

                    listECardDetailDto.Add(eCardDetailDto);

                    return new Result<List<ECardDetailDto>>(listECardDetailDto);
                }
                //按手机号、游客身份证查询    查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
                else
                {
                    var userIcList = _parkUserIcRepository.GetAll().Where(p => p.Customer.PhoneNumber == input.queryStr || p.Customer.Pid == input.queryStr);

                    if (userIcList == null)
                    {
                        return Result.FromError<List<ECardDetailDto>>("未查询到相关年卡信息");
                    }
                    var vipCardIdList = userIcList.Select(o => o.VIPCardId).ToList();

                    if (vipCardIdList == null || vipCardIdList.Count == 0)
                    {
                        return Result.FromError<List<ECardDetailDto>>("未查询到相关年卡信息");
                    }

                    var groupVipCardIdList = vipCardIdList.Distinct().ToList();//去重复

                    List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                    foreach (var vipcardId in groupVipCardIdList)
                    {
                        ECardDetailDto eCardDetailDto = new ECardDetailDto();
                        ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                        List<ECardParkDto> eCardPark = new List<ECardParkDto>();

                        var vipCardResult = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipcardId);

                        var vipCard = vipCardResult.MapTo<ECardVIPCardDto>();

                        eCardDetailDto.VipCardId = vipCard.Id;

                        eCardDetailDto.IcNo = vipCard.IcNo;
                        eCardDetailDto.CardNo = vipCard.CardNo;
                        eCardDetailDto.ECardId = vipCard.ECardID;

                        eCardDetailDto.State = vipCard.State;
                        eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                        eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                        eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");
                        if (vipCard.InParkValidDays < 100)
                        {
                            eCardDetailDto.ECardType = ECardType.SeasonCard;
                        }
                        else
                        {
                            switch (vipCard.Persons)
                            {
                                case 1:
                                    eCardDetailDto.ECardType = ECardType.SingleCard;
                                    break;
                                case 2:
                                    eCardDetailDto.ECardType = ECardType.DoubleCard;
                                    break;
                                case 3:
                                    eCardDetailDto.ECardType = ECardType.ThreeCard;
                                    break;
                            }
                        }
                        var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                        {
                            Id = o.Customer.Id,
                            CustomName = o.Customer.CustomerName,
                            Gender = o.Customer.GenderType,
                            PhoneNumber = o.Customer.PhoneNumber,
                            PhotoString = o.Customer.Photo == null ? null : Convert.ToBase64String(o.Customer.Photo),
                            Pid = o.Customer.Pid
                        }).ToList();

                        eCardDetailDto.ECardCustoer = listECardCustoerDto;

                        if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                        {
                            vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                            var parks = vipCard.InParkIdFilter.Split(',');
                            foreach (var parkid in parks)
                            {
                                int parkid1 = int.Parse(parkid.Trim());
                                var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);

                                var park = parkResult.MapTo<ECardParkDto>();
                                eCardPark.Add(park);
                            }

                            eCardDetailDto.ECardPark = eCardPark;
                        }
                        else
                        {
                            var parkResult =await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                            var park = parkResult.MapTo<ECardParkDto>();
                            eCardPark.Add(park);
                            eCardDetailDto.ECardPark = eCardPark;
                        }

                        listECardDetailDto.Add(eCardDetailDto);

                    }

                    return new Result<List<ECardDetailDto>>(listECardDetailDto);

                }

            }

        }

        /// <summary>
        /// 获取电子年卡信息（OTA）无照片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<ECardDetailDto>>> SearchECardDetailNoPhotoAsync(ECardDetailInput input)
        {


            //按凭证查询   查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
            var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Barcode == input.queryStr);
            if (vipVoucher != null)
            {


                if (vipVoucher.State != VipVoucherStateType.Actived)
                {
                    return Result.FromError<List<ECardDetailDto>>("年卡凭证未激活");
                }

                ECardDetailDto eCardDetailDto = new ECardDetailDto();
                ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                List<ECardParkDto> eCardPark = new List<ECardParkDto>();
                List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                var vipCardResult = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.VIPCardId);

                var vipCard = vipCardResult.MapTo<ECardVIPCardDto>();
                // var vipCard =await  _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.VIPCardId).MapTo<ECardVIPCardDto>();

                eCardDetailDto.VipCardId = vipCard.Id;

                eCardDetailDto.IcNo = vipCard.IcNo;
                eCardDetailDto.CardNo = vipCard.CardNo;
                eCardDetailDto.ECardId = vipCard.ECardID;

                eCardDetailDto.State = vipCard.State;
                eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");
                if (vipCard.InParkValidDays < 100)
                {
                    eCardDetailDto.ECardType = ECardType.SeasonCard;
                }
                else
                {
                    switch (vipCard.Persons)
                    {
                        case 1:
                            eCardDetailDto.ECardType = ECardType.SingleCard;
                            break;
                        case 2:
                            eCardDetailDto.ECardType = ECardType.DoubleCard;
                            break;
                        case 3:
                            eCardDetailDto.ECardType = ECardType.ThreeCard;
                            break;
                    }
                }
                var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                {
                    Id = o.Customer.Id,
                    CustomName = o.Customer.CustomerName,
                    Gender = o.Customer.GenderType,
                    PhoneNumber = o.Customer.PhoneNumber,
                    
                    Pid = o.Customer.Pid
                }).ToList();

                eCardDetailDto.ECardCustoer = listECardCustoerDto;

                if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                {
                    vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                    var parks = vipCard.InParkIdFilter.Split(',');
                    foreach (var parkid in parks)
                    {
                        int parkid1 = int.Parse(parkid.Trim());
                        var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);

                        var park = parkResult.MapTo<ECardParkDto>();
                        eCardPark.Add(park);
                    }

                    eCardDetailDto.ECardPark = eCardPark;
                }
                else
                {
                    var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                    var park = parkResult.MapTo<ECardParkDto>();
                    eCardPark.Add(park);
                    eCardDetailDto.ECardPark = eCardPark;
                }

                listECardDetailDto.Add(eCardDetailDto);

                return new Result<List<ECardDetailDto>>(listECardDetailDto);


            }
            else
            {
                //按电子年卡卡号、实体卡号查询    查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
                var vipCardModel = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == input.queryStr || p.IcBasicInfo.ECardID == input.queryStr || p.IcBasicInfo.CardNo == input.queryStr);

                //var vipCardModel = vipCardModelResult;


                if (vipCardModel != null && vipCardModel != null)
                {
                    if (vipCardModel.State != VipCardStateType.Actived)
                    {
                        return Result.FromError<List<ECardDetailDto>>("年卡未激活");
                    }

                    ECardDetailDto eCardDetailDto = new ECardDetailDto();
                    ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                    List<ECardParkDto> eCardPark = new List<ECardParkDto>();
                    List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                    var vipCard = vipCardModel.MapTo<ECardVIPCardDto>();

                    eCardDetailDto.VipCardId = vipCard.Id;

                    eCardDetailDto.IcNo = vipCard.IcNo;
                    eCardDetailDto.CardNo = vipCard.CardNo;
                    eCardDetailDto.ECardId = vipCard.ECardID;

                    eCardDetailDto.State = vipCard.State;
                    eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                    eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                    eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");
                    if (vipCard.InParkValidDays < 100)
                    {
                        eCardDetailDto.ECardType = ECardType.SeasonCard;
                    }
                    else
                    {
                        switch (vipCard.Persons)
                        {
                            case 1:
                                eCardDetailDto.ECardType = ECardType.SingleCard;
                                break;
                            case 2:
                                eCardDetailDto.ECardType = ECardType.DoubleCard;
                                break;
                            case 3:
                                eCardDetailDto.ECardType = ECardType.ThreeCard;
                                break;
                        }
                    }
                    var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                    {
                        Id = o.Customer.Id,
                        CustomName = o.Customer.CustomerName,
                        Gender = o.Customer.GenderType,
                        PhoneNumber = o.Customer.PhoneNumber,
                       
                        Pid = o.Customer.Pid
                    }).ToList();

                    eCardDetailDto.ECardCustoer = listECardCustoerDto;

                    if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                    {
                        vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                        var parks = vipCard.InParkIdFilter.Split(',');
                        foreach (var parkid in parks)
                        {
                            int parkid1 = int.Parse(parkid.Trim());

                            var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);
                            var park = parkResult.MapTo<ECardParkDto>();
                            eCardPark.Add(park);
                        }

                        eCardDetailDto.ECardPark = eCardPark;
                    }
                    else
                    {
                        var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                        var park = parkResult.MapTo<ECardParkDto>();
                        eCardPark.Add(park);
                        eCardDetailDto.ECardPark = eCardPark;
                    }

                    listECardDetailDto.Add(eCardDetailDto);

                    return new Result<List<ECardDetailDto>>(listECardDetailDto);
                }
                //按手机号、游客身份证查询    查询条件（凭证条码、手机号、游客身份证、电子年卡卡号、实体卡号）
                else
                {
                    var userIcList = _parkUserIcRepository.GetAll().Where(p => p.Customer.PhoneNumber == input.queryStr || p.Customer.Pid == input.queryStr);

                    if (userIcList == null)
                    {
                        return Result.FromError<List<ECardDetailDto>>("未查询到相关年卡信息");
                    }
                    var vipCardIdList = userIcList.Select(o => o.VIPCardId).ToList();

                    if (vipCardIdList == null || vipCardIdList.Count == 0)
                    {
                        return Result.FromError<List<ECardDetailDto>>("未查询到相关年卡信息");
                    }

                    var groupVipCardIdList = vipCardIdList.Distinct().ToList();//去重复

                    List<ECardDetailDto> listECardDetailDto = new List<ECardDetailDto>();

                    foreach (var vipcardId in groupVipCardIdList)
                    {
                        ECardDetailDto eCardDetailDto = new ECardDetailDto();
                        ECardCustoerDto eCardCustoerDto = new ECardCustoerDto();
                        List<ECardParkDto> eCardPark = new List<ECardParkDto>();

                        var vipCardResult = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipcardId);

                        var vipCard = vipCardResult.MapTo<ECardVIPCardDto>();

                        eCardDetailDto.VipCardId = vipCard.Id;

                        eCardDetailDto.IcNo = vipCard.IcNo;
                        eCardDetailDto.CardNo = vipCard.CardNo;
                        eCardDetailDto.ECardId = vipCard.ECardID;

                        eCardDetailDto.State = vipCard.State;
                        eCardDetailDto.TicketClassName = vipCard.TicketClassName;
                        eCardDetailDto.ValidDateBegin = vipCard.ValidDateBegin.Value.ToString("yyyy-MM-dd");
                        eCardDetailDto.ValidDateEnd = vipCard.ValidDateEnd.Value.ToString("yyyy-MM-dd");
                        if (vipCard.InParkValidDays < 100)
                        {
                            eCardDetailDto.ECardType = ECardType.SeasonCard;
                        }
                        else
                        {
                            switch (vipCard.Persons)
                            {
                                case 1:
                                    eCardDetailDto.ECardType = ECardType.SingleCard;
                                    break;
                                case 2:
                                    eCardDetailDto.ECardType = ECardType.DoubleCard;
                                    break;
                                case 3:
                                    eCardDetailDto.ECardType = ECardType.ThreeCard;
                                    break;
                            }
                        }
                        var listECardCustoerDto = vipCard.UserICs.Select(o => new ECardCustoerDto
                        {
                            Id = o.Customer.Id,
                            CustomName = o.Customer.CustomerName,
                            Gender = o.Customer.GenderType,
                            PhoneNumber = o.Customer.PhoneNumber,
                           
                            Pid = o.Customer.Pid
                        }).ToList();

                        eCardDetailDto.ECardCustoer = listECardCustoerDto;

                        if (vipCard.TicketClassMode == TicketClassMode.MultiYearCard)
                        {
                            vipCard.InParkIdFilter = vipCard.InParkIdFilter.Replace('，', ',');
                            var parks = vipCard.InParkIdFilter.Split(',');
                            foreach (var parkid in parks)
                            {
                                int parkid1 = int.Parse(parkid.Trim());

                                var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == parkid1);
                                var park = parkResult.MapTo<ECardParkDto>();
                                eCardPark.Add(park);
                            }

                            eCardDetailDto.ECardPark = eCardPark;
                        }
                        else
                        {
                            var parkResult = await _parkRepository.FirstOrDefaultAsync(p => p.Id == AbpSession.LocalParkId);
                            var park = parkResult.MapTo<ECardParkDto>();
                            eCardPark.Add(park);
                            eCardDetailDto.ECardPark = eCardPark;
                        }

                        listECardDetailDto.Add(eCardDetailDto);

                    }

                    return new Result<List<ECardDetailDto>>(listECardDetailDto);

                }

            }

        }



        /// <summary>
        /// 获取年卡基础类型
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TicketClassDto>> GetYearCardTypes(int parkId)
         {
            var query = _parkTicketClassRepository.AsNoTracking().Where(p => p.TicketClassMode == TicketClassMode.YearCard && p.ParkId == parkId);
            return await query.ProjectTo<TicketClassDto>().ToListAsync();
        }

        /// <summary>
        /// 根据查询条件获取年卡凭证信息列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetYearCardVoucherListAsync<TDto>(IQuery<VIPVoucher> query)
        {
            return await _parkVipVoucherRepository.AsNoTracking().ToListAsync<VIPVoucher, TDto>(query);
        }

        /// <summary>
        /// 根据查询条件获取年卡凭证信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetYearCardVoucherAsync<TDto>(IQuery<VIPVoucher> query)
        {
            return await _parkVipVoucherRepository.AsNoTracking().FirstOrDefaultAsync<VIPVoucher, TDto>(query);
        }


        /// <summary>
        /// 根据查询条件获取年卡信息列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetYearCardListAsync<TDto>(IQuery<VIPCard> query)
        {
            return await _parkVipCardRepository.AsNoTracking().ToListAsync<VIPCard, TDto>(query);
        }

        /// <summary>
        /// 获取年卡初始化信息
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        public async Task<VIPCardDto> GetCardBasicInfoAsync(string icno)
        {
            //TODO Hhm: p.IcBasicInfo.KindId == 1
            //var vipCard = await _parkVIPCardRepository.AsNoTracking().OrderByDescending(p => p.CreationTime).FirstAsync(p => p.IcBasicInfo.IcNo == icno && p.IcBasicInfo.KindId==1);
            var vipCard = await _parkVipCardRepository.AsNoTracking().OrderByDescending(p => p.CreationTime).FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == icno && p.IcBasicInfo.KindId == 1);

            return vipCard.MapTo<VIPCardDto>();
        }

        /// <summary>
        /// 获取年卡详细信息
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        public async Task<VIPCardDetailDto> GetCardDetailInfoAsync(string icno)
        {
            //取最后一条正在使用的卡信息（重复使用IC卡）
            var vipCard = await _parkVipCardRepository.AsNoTracking().OrderByDescending(p => p.CreationTime).FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == icno && (p.TicketClass.TicketClassMode == TicketClassMode.YearCard || p.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard));

            return vipCard.MapTo<VIPCardDetailDto>();
        }


        /// <summary>
        /// 获取凭证详细信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<VIPVoucherDto> GetVoucherDetailInfoByIdAsync(long Id)
        {

            var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Id == Id);

            return vipVoucher.MapTo<VIPVoucherDto>();
        }



        /// <summary>
        /// 获取凭证详细信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<VIPVoucherDto> GetVoucherDetailInfoAsync(string barcode, int parkId, int terminalId)
        {

            var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Barcode == barcode);
            //if (vipVoucher == null)
            //{

            //    var toticket = await _toTicketRepository.FirstOrDefaultAsync(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid);
            //    if (toticket != null && (toticket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.YearCard || toticket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard))
            //    {
            //        //网络票年卡激活先生成凭证
            //        var entity = new VIPVoucher
            //        {
            //            State = VipCardStateType.NotActive,
            //            ParkSaleTicketClassId = toticket.AgencySaleTicketClass.ParkSaleTicketClassId,
            //            CreationTime = System.DateTime.Now,
            //            SalePrice = 0,
            //            ValidDateBegin = System.DateTime.Now,
            //            ValidDateEnd = System.DateTime.Now.AddDays(90),
            //            TerminalId = terminalId,
            //            Barcode = barcode,
            //            TradeInfoId = null,
            //            Invoice = null,
            //            ParkId = parkId
            //        };

            //        //await _toTicketRepository.UpdateAsync(toticket);
            //        await _parkVipVoucherRepository.InsertAndGetIdAsync(entity);

            //        vipVoucher = entity;
            //    }
            //}

            return vipVoucher.MapTo<VIPVoucherDto>();
        }

        /// <summary>
        /// 新增售卡记录
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddYearCardVoucherAndReturnTradeNumAsync(SaveYearCardVoucherDto dto, int terminalId, int parkId)
        {
            //1. 业务检测
            var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == dto.invoiceInput.InvoiceCode);

            if (null == invoiceCode)
                return Result.FromError<string>("发票代码不存在");

            dto.TradeInfos.TradeType = TradeType.Income;

            var ticketCount = dto.VoucherInfos.Sum(m => m.Qty);

            //检测是否存在重复发票号或者负数
            var existed = _invoiceAppService.CheckIfExisteInValidOrDuplicateInvoice(dto.invoiceInput.InvoiceCode,
                dto.invoiceInput.InvoiceNo, ticketCount, invoiceCode.InvoiceNumIsIncrease);

            if (existed)
                return Result.FromCode<string>(ResultCode.DuplicateInvoiceRecord);

            //2. 支付
            var tradenoResult = await _tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(dto.TradeInfos, parkId, null);
            if (!tradenoResult.Success)
                return tradenoResult;

            //3. 生成记录
            var result = await AddYearCardVoucherAsync(dto.VoucherInfos, tradenoResult.Data, dto.invoiceInput, parkId, terminalId);
            if (!result.Success)
                return Result.FromError<string>(result.Message);

            return Result.FromData(tradenoResult.Data);
        }


        /// <summary>
        /// 销售年卡凭证
        /// </summary>
        /// <param name="vipVoucherInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="invoiceInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AddYearCardVoucherAsync(List<VIPVoucherInput> vipVoucherInput, string tradeno, InvoiceInput invoiceInput, int parkId, int terminalId)
        {
            var invoices = new List<Invoice>();
            var vipVouchers = new List<VIPVoucher>();
            var mulVipVouchers = new List<VIPVoucher>();
            var invoiceNo = long.Parse(invoiceInput.InvoiceNo);

            var invoiceCode = await _invoiceCodeRepository.GetAll().FirstOrDefaultAsync(m => m.Code == invoiceInput.InvoiceCode);

            foreach (var item in vipVoucherInput)
            {
                if (item == null)
                    continue;

                //每张凭证生成一条记录
                for (int i = 0; i < item.Qty; i++)
                {
                    var entity = new VIPVoucher
                    {
                        State = VipVoucherStateType.NotActive,
                        ParkSaleTicketClassId = item.ParkSaleTicketClassId,
                        SalePrice = item.SalePrice,
                        ValidDateBegin = System.DateTime.Today,
                        ValidDateEnd = System.DateTime.Today.AddDays(item.VoucherValidays),
                        TerminalId = terminalId,
                        Barcode = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId),
                        TradeInfoId = tradeno,
                        Invoice = invoiceNo.ToString(new string('0', invoiceInput.InvoiceNo.Length)),
                        ParkId = parkId
                    };

                    Invoice invoice = new Invoice
                    {
                        Barcode = entity.Barcode,
                        InvoiceNo = invoiceNo.ToString(new string('0', invoiceInput.InvoiceNo.Length)),
                        InvoiceCode = invoiceInput.InvoiceCode.Trim(),
                        IsActive = true,
                        TerminalId = terminalId
                    };

                    //发票号递增
                    if (invoiceCode.InvoiceNumIsIncrease)
                    {
                        invoiceNo = invoiceNo + 1;
                    }
                    else
                    {
                        //发票号递减
                        invoiceNo = invoiceNo - 1;
                    }

                    invoices.Add(invoice);
                    vipVouchers.Add(entity);

                    var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == entity.ParkSaleTicketClassId);
                    if (parkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
                    {
                        mulVipVouchers.Add(entity);
                    }

                }
            }

            _parkVipVoucherRepository.AddRange(vipVouchers);
            await CurrentUnitOfWork.SaveChangesAsync();

            _invoiceRepository.AddRange(invoices);
            await CurrentUnitOfWork.SaveChangesAsync();

            //多园凭证同步
            if (mulVipVouchers.Count > 0)
            {
                foreach (var item in mulVipVouchers)
                {
                    var dto = new MulYearCardVoucherSaleDto();
                    dto.Barcode = item.Barcode;
                    dto.Id = item.Id;

                    dto.ParkId = parkId;
                    dto.ParkSaleTicketClassId = item.ParkSaleTicketClassId;
                    dto.SalePrice = item.SalePrice;
                    dto.State = item.State;
                    dto.TerminalId = item.TerminalId;
                    dto.ValidDateBegin = item.ValidDateBegin;
                    dto.ValidDateEnd = item.ValidDateEnd;
                    dto.CreatorUserId = item.CreatorUserId;
                    dto.TradeInfoId = item.TradeInfoId;
                    dto.Invoice = dto.Invoice;

                    var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                    var syncInput = new DataSyncInput
                    {
                        SyncType = DataSyncType.MulYearCardVoucherSale,
                        SyncData = JsonConvert.SerializeObject(dto)
                    };

                    var parkSaleTicketClass = await _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == item.ParkSaleTicketClassId);
                    var otherParkIds = parkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                    foreach (var parkid in otherParkIds)
                    {
                        if (parkid != parkId.ToString())
                        {
                            dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                        }
                    }
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 年卡销售
        /// </summary>
        /// <param name="vipCardInput"></param>
        /// <param name="vipCardBasicInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AddYearCardAsync(List<VipCardInput> vipCardInput, List<VIPCardBasicInput> vipCardBasicInput, string tradeno, int parkId, int terminalId)
        {
            foreach (var item in vipCardInput)
            {
                if (item == null)
                    continue;

                //每张年卡生成一条记录
                for (int i = 0; i < item.Qty; i++)
                {

                    var vipCardBasic = vipCardBasicInput.FirstOrDefault(p => p.TicketClassId == item.TicketClassId);
                    if (vipCardBasic == null)
                    {
                        return Result.FromError("年卡类别不匹配！！");
                    }

                    var entity = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfoId == vipCardBasic.IcBasicInfoId && p.State == VipCardStateType.Init);
                    if (entity == null)
                    {
                        return Result.FromError("年卡未初始化！！");
                    }
                    entity.SalePrice = item.SalePrice;

                    entity.TicketClassId = item.TicketClassId;

                    entity.State = VipCardStateType.NotActive;
                    entity.ParkSaleTicketClassId = vipCardBasic.ParkSaleTicketClassId;

                    entity.SaleTime = System.DateTime.Now;
                    entity.SaleUser = AbpSession.GetUserId();

                    entity.IcBasicInfoId = vipCardBasic.IcBasicInfoId;
                    vipCardBasicInput.Remove(vipCardBasic);

                    entity.TerminalId = terminalId;
                    entity.TradeInfoId = tradeno;

                    //年卡销售 必须改PARKID
                    entity.ParkId = parkId;

                    await _parkVipCardRepository.UpdateAsync(entity);

                    //多园年卡同步
                    if (entity.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
                    {
                        await MulYearCardSaleDataSync(entity, parkId);
                    }
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 多园年卡销售同步
        /// </summary>
        /// <returns></returns>
        private async Task MulYearCardSaleDataSync(VIPCard vipCard, int parkId)
        {
            var dto = new MulYearCardSaleDto();
            dto.IcBasicInfoId = vipCard.IcBasicInfoId;
            dto.ParkId = parkId;
            dto.ParkSaleTicketClassId = vipCard.ParkSaleTicketClassId;
            dto.SaleTime = vipCard.SaleTime;
            dto.SaleUser = vipCard.SaleUser;
            dto.VipCardId = vipCard.Id;
            dto.TerminalId = vipCard.TerminalId;
            dto.CreatorUserId = vipCard.CreatorUserId;
            dto.SalePrice = vipCard.SalePrice;
            dto.TicketClassId = vipCard.TicketClassId;


            var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
            var syncInput = new DataSyncInput
            {
                SyncType = DataSyncType.MulYearCardSale,
                SyncData = JsonConvert.SerializeObject(dto)
            };

            var otherParkIds = vipCard.TicketClass.InParkIdFilter.Trim().Split(',');
            foreach (var parkid in otherParkIds)
            {
                if (parkid != parkId.ToString())
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
            }
        }





        /// <summary>
        /// 激活电子年卡
        /// </summary>
        /// <param name="cardInfoInput"></param>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> AddEcardYearCardAsync(CardInfoInput cardInfoInput, List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId)
        {
            DateTime validDateEnd;
            DateTime validDateStart;
            List<FingerDataItem> listFinger = new List<FingerDataItem>();

            var voucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Barcode == cardInfoInput.Barcode);
            if (voucher == null)
            {
                return Result.FromError("此凭证不存在！！");
            }
            if (voucher.State != VipVoucherStateType.NotActive)
            {
                return Result.FromError("凭证已激活或挂失等！！");
            }

            if (voucher.ParkSaleTicketClass.InParkRule.AppointDate)
            {
                validDateStart = voucher.ParkSaleTicketClass.InParkRule.ValidStartDate.Value;
                validDateEnd = voucher.ParkSaleTicketClass.InParkRule.ValidEndDate.Value;
            }
            else
            {
                validDateStart = System.DateTime.Today;
                validDateEnd = System.DateTime.Today.AddDays(int.Parse(voucher.ParkSaleTicketClass.InParkRule.InParkValidDays.ToString()));  
            }
            voucher.State = VipVoucherStateType.Actived;

            var eCardNo = await _uniqueCode.CreateAsync(CodeType.ECard, AbpSession.LocalParkId, AbpSession.TerminalId);

            var icno = "1" + long.Parse(eCardNo).ToString("x14");

            var entityIcBasicInfo = new IcBasicInfo
            {
                ECardID = eCardNo,
                CardNo = eCardNo,
                IcNo = icno,
                KindId = 1,
                ParkId = parkId
            };

            await _parkIcBasicInfoRepository.InsertAndGetIdAsync(entityIcBasicInfo);


            var vipcard = new VIPCard
            {
                ParkId = parkId,
                IcBasicInfoId = entityIcBasicInfo.Id,
                TicketClassId = voucher.ParkSaleTicketClass.TicketClassId,
                ParkSaleTicketClassId = voucher.ParkSaleTicketClassId,
                State = VipCardStateType.Actived,
                SalePrice = 0,
                ActiveTime = System.DateTime.Now,
                ActiveUser = AbpSession.UserId,
                ValidDateEnd = validDateEnd,
                ValidDateBegin = validDateStart,
                TerminalId = terminalId

            };
            await _parkVipCardRepository.InsertAndGetIdAsync(vipcard);


            voucher.VIPCardId = vipcard.Id;
            await _parkVipVoucherRepository.UpdateAsync(voucher);


            //每一个客户生成一条记录
            List<Customer> customers = new List<Customer>();
            foreach (var item in customerInput)
            {
                if (item == null)
                    continue;

                var entity = new Customer
                {
                    Address = item.Address,
                    Birthday = Convert.ToDateTime(item.Birthday.Trim()),
                    CustomerName = item.CustomName,
                    GenderType = item.Gender,
                    Mail = item.Mail,
                    PhoneNumber = item.PhoneNumber,
                    Pid = item.Pid,


                    Fp1 = (fingerType == ZWJType.TXW ? (item.Fp1 == null ? null : Convert.FromBase64String(item.Fp1)) : null),
                    FpImage1 = (fingerType == ZWJType.TXW ? (item.FpImage1 == null ? null : Convert.FromBase64String(item.FpImage1)) : null),
                    Fp2 = (fingerType == ZWJType.TXW ? null : (item.Fp1 == null ? null : Convert.FromBase64String(item.Fp1))),
                    FpImage2 = (fingerType == ZWJType.TXW ? null : (item.Fp1 == null ? null : Convert.FromBase64String(item.FpImage1))),
                    Photo = Convert.FromBase64String(item.Photo)
                };


                var feature = new byte[512];
                if (FaceApi.GetFeatureAndCheckImageFormat(entity.Photo, feature) != 1)
                {
                    return Result.FromCode(ResultCode.Fail, $"{entity.CustomerName} 不是有效的人脸");
                }
                entity.PhotoFeature = feature;

                customers.Add(entity);
            }
            _parkCustomerRepository.AddRange(customers);

            customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.Id, FingerData = o.Fp1 }));

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(icno.ToUpper(), listFinger, TimeSpan.MaxValue);


            var userIcs = customers.Select(o => new UserIC
            {
                Customer = o,
                VIPCardId = vipcard.Id
            }).ToList();

            _parkUserIcRepository.AddRange(userIcs);
            await CurrentUnitOfWork.SaveChangesAsync();

            //添加人脸特征到人脸服务器
            foreach (var userIc in userIcs)
            {
                FaceApi.AddUser(FaceCatalog.VipCard, long.Parse(icno, NumberStyles.AllowHexSpecifier),
                    userIc.Customer.PhotoFeature);
            }

            //多园年卡同步
            if (voucher.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                //var dtoInit = new MulYearCardInitDto();
                //dtoInit.CardNo = eCardNo;
                //dtoInit.IcBasicInfoId = entityIcBasicInfo.Id;
                //dtoInit.IcNo = entityIcBasicInfo.IcNo;
                //dtoInit.KindId = entityIcBasicInfo.KindId;
                //dtoInit.ParkId = parkId;
                //dtoInit.TicketClassId = vipcard.TicketClassId;
                //dtoInit.VicCardId = vipcard.Id;
                //dtoInit.CreatorUserId = entityIcBasicInfo.CreatorUserId;

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                //var syncInputInit = new DataSyncInput
                //{
                //    SyncType = DataSyncType.MulYearCardInit,
                //    SyncData = JsonConvert.SerializeObject(dtoInit)
                //};


                var dto = new MulYearCardActiveDto();
                dto.ParkSaleTicketClassId = vipcard.ParkSaleTicketClassId;
                dto.VipCardId = vipcard.Id;
                dto.ValidDateBegin = vipcard.ValidDateBegin;
                dto.ValidDateEnd = vipcard.ValidDateEnd;
                dto.ActiveUser = vipcard.ActiveUser;
                dto.ActiveTime = vipcard.ActiveTime;
                dto.ParkSaleTicketClassId = vipcard.ParkSaleTicketClassId;
                dto.ParkId = parkId;
                dto.Customers = customers.MapTo<List<MulCustomerDto>>();
                dto.TerminalId = vipcard.TerminalId;
                dto.ECardID = eCardNo;
                dto.IsEcard = true;

                dto.CardNo = eCardNo;
                dto.IcBasicInfoId = entityIcBasicInfo.Id;
                dto.IcNo = entityIcBasicInfo.IcNo;
                dto.KindId = entityIcBasicInfo.KindId;
                dto.TicketClassId = vipcard.TicketClassId;
                dto.CreatorUserId = entityIcBasicInfo.CreatorUserId;

                var mulUserIcs = userIcs.Select(o => new MulUserIcDto
                {
                    CustomId = o.CustomerId,
                    VIPCardId = o.VIPCardId,
                    IcBasicInfoId = o.IcBasicInfoId,
                    Id = o.Id,
                    Remark = o.Remark
                }).ToList();
                dto.UserICs = mulUserIcs;

                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardActive,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = voucher.ParkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = eCardNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }
            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = eCardNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);
            }


            return Result.Ok();
        }


        /// <summary>
        /// 激活年卡、年卡凭证
        /// </summary>
        /// <param name="cardInfoInput"></param>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> AddYearCardAsync(CardInfoInput cardInfoInput, List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId)
        {
            DateTime validDateEnd;
            DateTime validDateStart;
            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            //激活年卡
            var vipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == cardInfoInput.VipCardId);
            if (vipcard == null)
            {
                return Result.FromError("此年卡不存在！！");
            }

            if (vipcard.State == VipCardStateType.Actived)
            {
                return Result.FromError("此年卡已激活！！");
            }

            var eCardNo = await _uniqueCode.CreateAsync(CodeType.ECard, AbpSession.LocalParkId, AbpSession.TerminalId);

            //每一个客户生成一条记录
            List<Customer> customers = new List<Customer>();
            foreach (var item in customerInput)
            {
                if (item == null)
                    continue;

                var entity = new Customer
                {
                    Address = item.Address,
                    Birthday = Convert.ToDateTime(item.Birthday.Trim()),
                    CustomerName = item.CustomName,
                    GenderType = item.Gender,
                    Mail = item.Mail,
                    PhoneNumber = item.PhoneNumber,
                    Pid = item.Pid,


                    Fp1 = (fingerType == ZWJType.TXW ? (item.Fp1 == null ? null : Convert.FromBase64String(item.Fp1)) : null),
                    FpImage1 = (fingerType == ZWJType.TXW ? (item.FpImage1 == null ? null : Convert.FromBase64String(item.FpImage1)) : null),
                    Fp2 = (fingerType == ZWJType.TXW ? null : (item.Fp1 == null ? null : Convert.FromBase64String(item.Fp1))),
                    FpImage2 = (fingerType == ZWJType.TXW ? null : (item.Fp1 == null ? null : Convert.FromBase64String(item.FpImage1))),
                    Photo = Convert.FromBase64String(item.Photo)
                };


                var feature = new byte[512];
                if (FaceApi.GetFeatureAndCheckImageFormat(entity.Photo, feature) != 1)
                {
                    return Result.FromCode(ResultCode.Fail, $"{entity.CustomerName} 不是有效的人脸");
                }
                entity.PhotoFeature = feature;

                customers.Add(entity);
            }
            _parkCustomerRepository.AddRange(customers);

            customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.Id, FingerData = o.Fp1 }));

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(cardInfoInput.Icno.ToUpper(), listFinger, TimeSpan.MaxValue);


            //激活凭证
            if (cardInfoInput.Type == 2)
            {
                var voucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Barcode == cardInfoInput.Barcode);
                if (voucher == null)
                {
                    return Result.FromError("此凭证不存在！！");
                }
                if (voucher.State != VipVoucherStateType.NotActive)
                {
                    return Result.FromError("凭证已激活或挂失等！！");
                }

                if (voucher.ParkSaleTicketClass.InParkRule.AppointDate)
                {
                    validDateStart = voucher.ParkSaleTicketClass.InParkRule.ValidStartDate.Value;
                    validDateEnd = voucher.ParkSaleTicketClass.InParkRule.ValidEndDate.Value;
                }
                else
                {
                    validDateStart = System.DateTime.Today;
                    validDateEnd = System.DateTime.Now.AddDays(int.Parse(voucher.ParkSaleTicketClass.InParkRule.InParkValidDays.ToString()));
                }

                //validDateEnd = System.DateTime.Now.AddDays(int.Parse(voucher.ParkSaleTicketClass.InParkRule.InParkValidDays.ToString()));

                //凭证激活 需要更新年卡促销票类ID
                vipcard.ParkSaleTicketClassId = voucher.ParkSaleTicketClassId;

                voucher.State = VipVoucherStateType.Actived;
                voucher.LastModificationTime = System.DateTime.Now;
                voucher.VIPCardId = cardInfoInput.VipCardId;
                await _parkVipVoucherRepository.UpdateAsync(voucher);
            }
            else
            {
                if (vipcard.ParkSaleTicketClass.InParkRule.AppointDate)
                {
                    validDateStart = vipcard.ParkSaleTicketClass.InParkRule.ValidStartDate.Value;
                    validDateEnd = vipcard.ParkSaleTicketClass.InParkRule.ValidEndDate.Value;
                }
                else
                {
                    validDateStart = System.DateTime.Today;
                    validDateEnd = System.DateTime.Now.AddDays(int.Parse(vipcard.ParkSaleTicketClass.InParkRule.InParkValidDays.ToString()));
                }

               
            }


            vipcard.ValidDateBegin = validDateStart;
            vipcard.ValidDateEnd = validDateEnd;
            vipcard.State = VipCardStateType.Actived;

            vipcard.ActiveTime = System.DateTime.Now;
            vipcard.ActiveUser = AbpSession.GetUserId();
  
            await _parkVipCardRepository.UpdateAsync(vipcard);

            var icBasicInfo = await _parkIcBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == vipcard.IcBasicInfoId);
            icBasicInfo.ECardID = eCardNo;

            await _parkIcBasicInfoRepository.UpdateAsync(icBasicInfo);


            var userIcs = customers.Select(o => new UserIC
            {
                Customer = o,
                VIPCardId = vipcard.Id
            }).ToList();

            _parkUserIcRepository.AddRange(userIcs);
            await CurrentUnitOfWork.SaveChangesAsync();

            //添加人脸特征到人脸服务器
            foreach (var userIc in userIcs)
            {
                FaceApi.AddUser(FaceCatalog.VipCard, long.Parse(vipcard.IcBasicInfo.IcNo, NumberStyles.AllowHexSpecifier),
                    userIc.Customer.PhotoFeature);
            }

            //多园年卡同步
            if (vipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var dto = new MulYearCardActiveDto();
                dto.ParkSaleTicketClassId = vipcard.ParkSaleTicketClassId;
                dto.VipCardId = vipcard.Id;
                dto.ValidDateBegin = vipcard.ValidDateBegin;
                dto.ValidDateEnd = vipcard.ValidDateEnd;
                dto.ActiveUser = vipcard.ActiveUser;
                dto.ActiveTime = vipcard.ActiveTime;
                dto.ParkSaleTicketClassId = vipcard.ParkSaleTicketClassId;
                dto.ParkId = parkId;
                dto.Customers = customers.MapTo<List<MulCustomerDto>>();
                dto.TerminalId = vipcard.TerminalId;
                dto.ECardID = eCardNo;
                dto.IsEcard = false;
                dto.IcNo = cardInfoInput.Icno;

                var mulUserIcs = userIcs.Select(o => new MulUserIcDto
                {
                    CustomId = o.CustomerId,
                    VIPCardId = o.VIPCardId,
                    IcBasicInfoId = o.IcBasicInfoId,
                    Id = o.Id,
                    Remark = o.Remark
                }).ToList();
                dto.UserICs = mulUserIcs;

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardActive,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = vipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }
            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);
            }


            return Result.Ok();
        }

        /// <summary>
        /// 修改年卡用户信息
        /// </summary>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> AlterYearCardCustomerAsync(List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId)
        {
            List<FingerDataItem> listFinger = new List<FingerDataItem>();

            var vipcard = new VIPCard();
            List<Customer> customers = new List<Customer>();

            List<Customer> customersCheck = new List<Customer>();
            //人脸特征判断
            foreach (var item in customerInput)
            {
                if (item.Photo != null)
                {
                    var photo = Convert.FromBase64String(item.Photo);

                    var feature = new byte[512];
                    if (FaceApi.GetFeatureAndCheckImageFormat(photo, feature) != 1)
                    {
                        return Result.FromCode(ResultCode.Fail, $"{item.CustomName} 不是有效的人脸");
                    }
                    customersCheck.Add(new Customer { Id = item.Id.Value, Photo = photo, PhotoFeature = feature });
                }
            }



            //修改每一位客户信息
            foreach (var item in customerInput)
            {
                if (item == null)
                    continue;
                var entity = await _parkCustomerRepository.FirstOrDefaultAsync(p => p.Id == item.Id);

                var useric = await _parkUserIcRepository.FirstOrDefaultAsync(p => p.CustomerId == item.Id);
                vipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == useric.VIPCardId);

                entity.Address = item.Address;
                entity.Birthday = Convert.ToDateTime(item.Birthday.Trim());
                entity.CustomerName = item.CustomName;
                entity.GenderType = item.Gender;
                entity.Mail = item.Mail;
                entity.PhoneNumber = item.PhoneNumber;
                entity.Pid = item.Pid;

                if (fingerType == ZWJType.TXW)
                {
                    if (item.Fp1 != null)
                    {
                        entity.Fp1 = Convert.FromBase64String(item.Fp1);
                    }
                    if (item.FpImage1 != null)
                    {
                        entity.FpImage1 = Convert.FromBase64String(item.FpImage1);
                    }
                }
                else
                {
                    if (item.Fp1 != null)
                    {
                        entity.Fp2 = Convert.FromBase64String(item.Fp1);
                    }
                    if (item.FpImage1 != null)
                    {
                        entity.FpImage2 = Convert.FromBase64String(item.FpImage1);
                    }
                }

                entity.Photo = customersCheck.First(p => p.Id == entity.Id).Photo;
                entity.PhotoFeature = customersCheck.First(p => p.Id == entity.Id).PhotoFeature;

                customers.Add(entity);
            }

            //await UnitOfWorkManager.Current.SaveChangesAsync();

            customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.Id, FingerData = o.Fp1 }));

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(vipcard.IcBasicInfo.IcNo.ToUpper(), listFinger, TimeSpan.MaxValue);


            var faceId = long.Parse(vipcard.IcBasicInfo.IcNo, NumberStyles.AllowHexSpecifier);
            FaceApi.RemoveUser(FaceCatalog.VipCard, faceId);
            foreach (var customer in customers)
            {
                FaceApi.AddUser(FaceCatalog.VipCard, faceId, customer.PhotoFeature);
            }

            //多园年卡同步
            if (vipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var customerIds = customers.Select(o => o.Id).ToList();
                await _parkUserIcRepository.GetAll().Where(o => customerIds.Contains(o.CustomerId))
                    .Select(o => new { o.Id, o.CustomerId }).ToListAsync();

                var dto = new MulYearCardUpdateDto
                {
                    ParkId = parkId,
                    Customers = customers.MapTo<List<CustomerDto>>(),
                    IcNo = vipcard.IcBasicInfo.IcNo
                };

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardUpdate,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = vipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }

            return Result.Ok();
        }


        /// <summary>
        /// 分页获取年卡数据
        /// </summary>
        /// <param name="query">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        public async Task<PageResult<ListVipCardDto>> GetAllByPageAsync(SearchVipCardModel query = null)
        {
            var query4 = _parkVipCardRepository.GetAll().GroupJoin(_parkUserIcRepository.GetAll(), o => o.Id, o => o.VIPCardId.Value, (vipCard, userIcs) => new
            {
                CardNo = vipCard.IcBasicInfo.CardNo,
                vipCard.IcBasicInfo.IcNo,
                vipCard.IcBasicInfo.ECardID,
                vipCard.TicketClass.TicketClassName,
                vipCard.ValidDateBegin,
                vipCard.ValidDateEnd,
                vipCard.State,
                vipCard.Id,
                vipCard.TicketClassId,
                vipCard.ParkSaleTicketClassId,
                vipCard.ActiveTime,
                userIcs = userIcs.DefaultIfEmpty().Select(o => o != null ? new
                {
                    o.Customer.CustomerName,
                    o.Customer.GenderType,
                    o.Customer.PhoneNumber,
                    o.Customer.Pid,
                    o.Customer.Birthday,
                } : null)
            });


            var result = Queryable.SelectMany(query4, o => o.userIcs, (a, bs) => new { a.Id, vipCard = a, userIc = bs });

            if (query != null)
            {

                if (!string.IsNullOrWhiteSpace(query.IcNo))
                {
                    if (query.IcNo.Trim().Length != 18)
                    {
                        result = result.Where(o => o.vipCard.IcNo == query.IcNo);
                    }
                    else
                    {
                        result = result.Where(o => o.vipCard.ECardID == query.IcNo);
                    }
                }


                if (query.CustomerName != null)
                {
                    result = result.Where(o => o.userIc.CustomerName == query.CustomerName);
                }
                if (query.PhoneNumber != null)
                {
                    result = result.Where(o => o.userIc.PhoneNumber == query.PhoneNumber);
                }
                if (query.Pid != null)
                {
                    result = result.Where(o => o.userIc.Pid == query.Pid);
                }
                if (query.TicketClassId != null && query.TicketClassId != 0)
                {
                    result = result.Where(o => o.vipCard.TicketClassId == query.TicketClassId);
                }

                if (query.State != null && query.State != 0)
                {
                    result = result.Where(o => o.vipCard.State == query.State);
                }

                if (query.ValidDateBegin != null)
                {
                    result = result.Where(o => o.vipCard.ValidDateBegin >= query.ValidDateBegin);
                }

                if (query.ActiveTime != null)
                {
                    result = result.Where(o => o.vipCard.ActiveTime >= query.ActiveTime);
                }

                if (query.ValidDateEnd != null)
                {
                    query.ValidDateEnd = query.ValidDateEnd.Value.AddDays(1);
                    result = result.Where(o => o.vipCard.ValidDateBegin <= query.ValidDateEnd);
                }

            }

            var data = await result.ToPageResultAsync(query);

            var data2 = data.Data.Select(o => new ListVipCardDto
            {
                CardNo = o.vipCard.CardNo,
                IcNo = o.vipCard.IcNo,
                ECardId = o.vipCard.ECardID,
                TicketClassName = o.vipCard.TicketClassName,
                CustomName = o.userIc != null ? o.userIc.CustomerName : "",
                Birthday = o.userIc?.Birthday?.ToString("yyyy-MM-dd"),
                Gender = o.userIc?.GenderType,
                Id = o.vipCard.Id,
                PhoneNumber = o.userIc != null ? o.userIc.PhoneNumber : "",
                Pid = o.userIc != null ? o.userIc.Pid : "",
                SaleTicketClassId = o.vipCard.ParkSaleTicketClassId,
                State = o.vipCard.State,
                TicketClassId = o.vipCard.TicketClassId,
                ValidDateBegin = o.vipCard.ValidDateBegin != null ? ((DateTime)o.vipCard.ValidDateBegin).ToString("yyyy-MM-dd") : null,
                ValidDateEnd = o.vipCard.ValidDateEnd != null ? ((DateTime)o.vipCard.ValidDateEnd).ToString("yyyy-MM-dd") : null,
                ActiveTime = o.vipCard.ActiveTime != null ? ((DateTime)o.vipCard.ActiveTime).ToString("yyyy-MM-dd") : null,
                ValidDays = 0
            }).ToList();

            var dataResult = new PageResult<ListVipCardDto>
            {
                Code = data.Code,
                Message = data.Message,
                PageIndex = data.PageIndex,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
                Data = data2
            };

            return dataResult;
        }

        /// <summary>
        /// 获取年卡配置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        [DisableAuditing]
        public string GetCardSettingValue(string name, int parkId)
        {
            return _settingManager.GetSettingValueForTenant(name, parkId);
        }


        /// <summary>
        /// 人脸特征检测
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public async Task<Result<string>> CheckFacePhoto(string photo)
        {

            var result = new Result<string>
            {
                Message = "人脸特征识别通过",
                Code = ResultCode.Ok
            };


            var feature = new byte[512];
            if (FaceApi.GetFeatureAndCheckImageFormat(Convert.FromBase64String(photo), feature) != 1)
            {
                result.Code = ResultCode.Fail;
                result.Message = "人脸特征识别失败，请重新拍照";
            }

            return result;
        }

        /// <summary>
        /// 获取续卡价格
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="ticketClassId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<string>> GetRenewalCardPriceAsync(int vipCardId, int ticketClassId, int parkId)
        {
            //先判断是第几次续卡
            var count = await _parkFillCardRepository.AsNoTrackingAndInclude().Where(o => o.VipCardId == vipCardId && o.State == FillCardType.RenewalCard).CountAsync();

            var vipCardRenewal = await _parkVipCardRenewalRepository.FirstOrDefaultAsync(o => o.ParkId == parkId && o.TicketClassId == ticketClassId);

            if (vipCardRenewal == null)
            {
                //多园年卡取值
                var ticketClass = _parkTicketClassRepository.FirstOrDefault(p => p.Id == ticketClassId);
                var ticketClassIdList = _parkTicketClassRepository.GetAllList(p => p.TicketTypeId == ticketClass.TicketTypeId).Select(o => o.Id).ToList();

                vipCardRenewal = await _parkVipCardRenewalRepository.FirstOrDefaultAsync(o => o.ParkId == parkId && ticketClassIdList.Contains(o.TicketClassId));
                if (vipCardRenewal == null)
                {
                    var result = new Result<string>
                    {
                        Message = "未设置续卡价格，请先配置续卡价格！！",
                        Code = ResultCode.Fail
                    };
                    return result;
                }

            }
            if (count < 1)
            {
                return new Result<string>(vipCardRenewal.FirstPrice.ToString());
            }
            else
            {
                return new Result<string>(vipCardRenewal.MulPrice.ToString());
            }

        }

        /// <summary>
        /// 根据ID获取年卡详情
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <returns></returns>
        public async Task<Result<List<ListVipCardDto>>> GetCardInfoByIdAsync(int vipCardId)
        {
            var query = _parkVipCardRepository.GetAll().GroupJoin(_parkUserIcRepository.GetAll(), o => o.Id, o => o.VIPCardId.Value, (vipCard, userIcs) => new
            {
                CardNo = vipCard.IcBasicInfo.CardNo,
                vipCard.IcBasicInfo.IcNo,
                vipCard.IcBasicInfo.ECardID,
                vipCard.TicketClass.TicketClassName,
                vipCard.ValidDateBegin,
                vipCard.ValidDateEnd,
                vipCard.State,
                vipCard.Id,
                vipCard.TicketClassId,
                vipCard.ParkSaleTicketClassId,
                userIcs = userIcs.DefaultIfEmpty().Select(o => o != null ? new
                {
                    CustomId = o.CustomerId,
                    o.Customer.Mail,
                    o.Customer.Address,
                    o.Customer.CustomerName,
                    o.Customer.GenderType,
                    o.Customer.PhoneNumber,
                    o.Customer.Pid,
                    o.Customer.Birthday,
                    o.Customer.Photo,
                    o.Customer.FpImage1,
                    o.Customer.FpImage2
                } : null)
            });

            var query2 = Queryable.SelectMany(query, o => o.userIcs, (a, bs) => new { vipCard = a, userIc = bs });

            var query3 = await query2.Where(o => o.vipCard.Id == vipCardId).ToListAsync();

            var data = query3.Select(o => new ListVipCardDto
            {
                CardNo = o.vipCard.CardNo,
                IcNo = o.vipCard.IcNo,
                ECardId = o.vipCard.ECardID,
                TicketClassName = o.vipCard.TicketClassName,
                CustomName = o.userIc != null ? o.userIc.CustomerName : null,
                Birthday = o.userIc != null ? o.userIc.Birthday?.ToString("yyyy-MM-dd") : null,
                Gender = o.userIc?.GenderType,
                Id = o.vipCard.Id,
                PhoneNumber = o.userIc != null ? o.userIc.PhoneNumber : null,
                Photo = o.userIc != null ? o.userIc.Photo : null,
                FpImage1 = o.userIc != null ? o.userIc.FpImage1 : null,
                FpImage2 = o.userIc != null ? o.userIc.FpImage2 : null,
                Pid = o.userIc != null ? o.userIc.Pid : null,
                SaleTicketClassId = o.vipCard.ParkSaleTicketClassId,
                State = o.vipCard.State,
                TicketClassId = o.vipCard.TicketClassId,
                ValidDateBegin = o.vipCard.ValidDateBegin != null ? ((DateTime)o.vipCard.ValidDateBegin).ToString("yyyy-MM-dd") : null,
                ValidDateEnd = o.vipCard.ValidDateEnd != null ? ((DateTime)o.vipCard.ValidDateEnd).ToString("yyyy-MM-dd") : null,
                ValidDays = 0,
                Mail = o.userIc != null ? o.userIc.Mail : null,
                Address = o.userIc != null ? o.userIc.Address : null,
                CustomerId = o.userIc?.CustomId,
            }).ToList();

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.Photo != null)
                    {
                        item.PhotoString = "data:image/jpg;base64," + Convert.ToBase64String(item.Photo);
                    }
                    if (item.FpImage1 != null)
                    {
                        item.FpImage1String = "data:image/jpg;base64," + Convert.ToBase64String(item.FpImage1);
                    }
                    else if (item.FpImage2 != null)
                    {
                        item.FpImage1String = "data:image/jpg;base64," + Convert.ToBase64String(item.FpImage2);
                    }

                    var date = item.ValidDateEnd == null ? System.DateTime.Now : Convert.ToDateTime(item.ValidDateEnd);
                    item.ValidDays = (date - System.DateTime.Now).Days;
                }
            }
            return new Result<List<ListVipCardDto>>(data);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vipVoucherId"></param>
        /// <returns></returns>
        public async Task<Result<TradeInfoDto>> GetVipVoucherPayDetail(long vipVoucherId)
        {
            var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Id == vipVoucherId);
            if (vipVoucher != null && vipVoucher.TradeInfoId != null)
            {
                var tradeInfo = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.TradeInfoId);
                var dto = tradeInfo.MapTo<TradeInfoDto>();
                dto.Amount = vipVoucher.SalePrice;
                return new Result<TradeInfoDto>(dto);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取年卡支付详情
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <returns></returns>
        public async Task<Result<TradeInfoDto>> GetYearCardPayDetail(long vipCardId)
        {
            var vipCard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipCardId);
            if (vipCard != null && vipCard.TradeInfoId != null)
            {
                var tradeInfo = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipCard.TradeInfoId);
                var dto = tradeInfo.MapTo<TradeInfoDto>();
                dto.Amount = vipCard.SalePrice;
                return new Result<TradeInfoDto>(dto);
            }
            else
            {
                var vipVoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.VIPCardId == vipCardId);
                if (vipVoucher != null && vipVoucher.TradeInfoId != null)
                {
                    var tradeInfo = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipVoucher.TradeInfoId);
                    var dto = tradeInfo.MapTo<TradeInfoDto>();
                    dto.Amount = vipVoucher.SalePrice;
                    return new Result<TradeInfoDto>(dto);
                }
                else
                {
                    return null;
                }

            }

        }



        /// <summary>
        /// 查询年卡信息（闸验 验证身份证）
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<Result<List<ListVipCardDto>>> GetCardInfoByPidAsync(string pid)
        {

            var query = _parkVipCardRepository.GetAll().GroupJoin(_parkUserIcRepository.GetAll(), o => o.Id, o => o.VIPCardId.Value, (vipCard, userIcs) => new
            {
                CardNo = vipCard.IcBasicInfo.CardNo,
                vipCard.IcBasicInfo.IcNo,
                vipCard.IcBasicInfo.ECardID,
                vipCard.TicketClass.TicketClassName,
                vipCard.ValidDateBegin,
                vipCard.ValidDateEnd,
                vipCard.State,
                vipCard.Id,
                vipCard.TicketClassId,
                vipCard.TicketClass.TicketTypeId,
                vipCard.ParkSaleTicketClassId,
                userIcs = userIcs.DefaultIfEmpty().Select(o => o != null ? new
                {
                    o.Customer.Mail,
                    o.Customer.Address,
                    o.Customer.CustomerName,
                    o.Customer.GenderType,
                    o.Customer.PhoneNumber,
                    o.Customer.Pid,
                    o.Customer.Birthday,
                    o.Customer.Photo
                } : null)
            });

            var query2 = Queryable.SelectMany(query, o => o.userIcs, (a, bs) => new { vipCard = a, userIc = bs });

            var query3 = await query2.Where(o =>  o.userIc.Pid == pid).ToListAsync();

            var data = query3.Select(o => new ListVipCardDto
            {
                CardNo = o.vipCard.CardNo,
                IcNo = o.vipCard.IcNo,
                ECardId = o.vipCard.ECardID,
                TicketClassName = o.vipCard.TicketClassName,
                CustomName = o.userIc?.CustomerName,
                Birthday = o.userIc?.Birthday?.ToString("yyyy-MM-dd"),
                Gender = o.userIc?.GenderType,
                Id = o.vipCard.Id,
                PhoneNumber = o.userIc?.PhoneNumber,
                Photo = o.userIc?.Photo,
                Pid = o.userIc?.Pid,
                SaleTicketClassId = o.vipCard.ParkSaleTicketClassId,
                State = o.vipCard.State,
                TicketClassId = o.vipCard.TicketClassId,
                ValidDateBegin = o.vipCard.ValidDateBegin?.ToString("yyyy-MM-dd"),
                ValidDateEnd = o.vipCard.ValidDateEnd?.ToString("yyyy-MM-dd"),
                ValidDays = 0,
                Mail = o.userIc?.Mail,
                Address = o.userIc?.Address,
                TicketTypeId = o.vipCard.TicketTypeId
            }).ToList();

           
            return new Result<List<ListVipCardDto>>(data);

        }



        /// <summary>
        /// 查询年卡信息(完整IC卡信息)
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        public async Task<Result<List<ListVipCardDto>>> GetCardInfoAllAsync(string icno)
        {
            long vipcardId = -1;
            var vipcardResult = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.ECardID == icno || p.IcBasicInfo.IcNo == icno);
            if (vipcardResult != null)
            {
                vipcardId = vipcardResult.Id;
            }
            else
            {
                var userIc = await _parkUserIcRepository.FirstOrDefaultAsync(p => p.Customer.CustomerName == icno || p.Customer.Pid == icno || p.Customer.PhoneNumber == icno);
                vipcardId = userIc.VIPCardId.Value;
            }

            var query = _parkVipCardRepository.GetAll().GroupJoin(_parkUserIcRepository.GetAll(), o => o.Id, o => o.VIPCardId.Value, (vipCard, userIcs) => new
            {
                CardNo = vipCard.IcBasicInfo.CardNo,
                vipCard.IcBasicInfo.IcNo,
                vipCard.IcBasicInfo.ECardID,
                vipCard.TicketClass.TicketClassName,
                vipCard.ValidDateBegin,
                vipCard.ValidDateEnd,
                vipCard.State,
                vipCard.Id,
                vipCard.TicketClassId,
                vipCard.TicketClass.TicketTypeId,
                vipCard.ParkSaleTicketClassId,
                userIcs = userIcs.DefaultIfEmpty().Select(o => o != null ? new
                {
                    o.Customer.Mail,
                    o.Customer.Address,
                    o.Customer.CustomerName,
                    o.Customer.GenderType,
                    o.Customer.PhoneNumber,
                    o.Customer.Pid,
                    o.Customer.Birthday,
                    o.Customer.Photo
                } : null)
            });

            var query2 = Queryable.SelectMany(query, o => o.userIcs, (a, bs) => new { vipCard = a, userIc = bs });

            var query3 = await query2.Where(o => o.vipCard.Id == vipcardId).ToListAsync();

            var data = query3.Select(o => new ListVipCardDto
            {
                CardNo = o.vipCard.CardNo,
                IcNo = o.vipCard.IcNo,
                ECardId = o.vipCard.ECardID,
                TicketClassName = o.vipCard.TicketClassName,
                CustomName = o.userIc?.CustomerName,
                Birthday = o.userIc?.Birthday?.ToString("yyyy-MM-dd"),
                Gender = o.userIc?.GenderType,
                Id = o.vipCard.Id,
                PhoneNumber = o.userIc?.PhoneNumber,
                Photo = o.userIc?.Photo,
                Pid = o.userIc?.Pid,
                SaleTicketClassId = o.vipCard.ParkSaleTicketClassId,
                State = o.vipCard.State,
                TicketClassId = o.vipCard.TicketClassId,
                ValidDateBegin = o.vipCard.ValidDateBegin?.ToString("yyyy-MM-dd"),
                ValidDateEnd = o.vipCard.ValidDateEnd?.ToString("yyyy-MM-dd"),
                ValidDays = 0,
                Mail = o.userIc?.Mail,
                Address = o.userIc?.Address,
                TicketTypeId = o.vipCard.TicketTypeId
            }).ToList();

            data = data.Where(o => o.IcNo == data[0].IcNo).ToList();

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.Photo != null)
                    {
                        item.PhotoString = Convert.ToBase64String(item.Photo);
                    }
                    if (item.Fp1 != null)
                    {
                        item.Fp1String = Convert.ToBase64String(item.Fp1);
                    }
                    var date = item.ValidDateEnd == null ? System.DateTime.Now : Convert.ToDateTime(item.ValidDateEnd);
                    item.ValidDays = (date - System.DateTime.Now).Days;
                }
            }
            return new Result<List<ListVipCardDto>>(data);

        }



        /// <summary>
        /// 查询年卡信息
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        public async Task<Result<List<ListVipCardDto>>> GetCardInfoAsync(string icno)
        {

            var query = _parkVipCardRepository.GetAll().GroupJoin(_parkUserIcRepository.GetAll(), o => o.Id, o => o.VIPCardId.Value, (vipCard, userIcs) => new
            {
                CardNo = vipCard.IcBasicInfo.CardNo,
                vipCard.IcBasicInfo.IcNo,
                vipCard.IcBasicInfo.ECardID,
                vipCard.TicketClass.TicketClassName,
                vipCard.ValidDateBegin,
                vipCard.ValidDateEnd,
                vipCard.State,
                vipCard.Id,
                vipCard.TicketClassId,
                vipCard.TicketClass.TicketTypeId,
                vipCard.ParkSaleTicketClassId,
                userIcs = userIcs.DefaultIfEmpty().Select(o => o != null ? new
                {
                    o.Customer.Mail,
                    o.Customer.Address,
                    o.Customer.CustomerName,
                    o.Customer.GenderType,
                    o.Customer.PhoneNumber,
                    o.Customer.Pid,
                    o.Customer.Birthday,
                    o.Customer.Photo
                } : null)
            });

            var query2 = Queryable.SelectMany(query, o => o.userIcs, (a, bs) => new { vipCard = a, userIc = bs });

            var query3 = await query2.Where(o => o.vipCard.IcNo == icno || o.vipCard.ECardID == icno || o.userIc.CustomerName == icno || o.userIc.Pid == icno || o.userIc.PhoneNumber == icno).ToListAsync();

            var data = query3.Select(o => new ListVipCardDto
            {
                CardNo = o.vipCard.CardNo,
                IcNo = o.vipCard.IcNo,
                ECardId = o.vipCard.ECardID,
                TicketClassName = o.vipCard.TicketClassName,
                CustomName = o.userIc?.CustomerName,
                Birthday = o.userIc?.Birthday?.ToString("yyyy-MM-dd"),
                Gender = o.userIc?.GenderType,
                Id = o.vipCard.Id,
                PhoneNumber = o.userIc?.PhoneNumber,
                Photo = o.userIc?.Photo,
                Pid = o.userIc?.Pid,
                SaleTicketClassId = o.vipCard.ParkSaleTicketClassId,
                State = o.vipCard.State,
                TicketClassId = o.vipCard.TicketClassId,
                ValidDateBegin = o.vipCard.ValidDateBegin?.ToString("yyyy-MM-dd"),
                ValidDateEnd = o.vipCard.ValidDateEnd?.ToString("yyyy-MM-dd"),
                ValidDays = 0,
                Mail = o.userIc?.Mail,
                Address = o.userIc?.Address,
                TicketTypeId = o.vipCard.TicketTypeId
            }).ToList();

            data = data.Where(o => o.IcNo == data[0].IcNo).ToList();

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.Photo != null)
                    {
                        item.PhotoString = Convert.ToBase64String(item.Photo);
                    }
                    if (item.Fp1 != null)
                    {
                        item.Fp1String = Convert.ToBase64String(item.Fp1);
                    }
                    var date = item.ValidDateEnd == null ? System.DateTime.Now : Convert.ToDateTime(item.ValidDateEnd);
                    item.ValidDays = (date - System.DateTime.Now).Days;
                }
            }
            return new Result<List<ListVipCardDto>>(data);

        }

        /// <summary>
        /// 年卡挂失与解挂
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> LossYearCardAsync(int vipCardId, string applyName, string applyPhone, string applyPid, int terminalId, int parkId)
        {
            var entity = new IcoperDetail();

            //挂失解挂年卡
            var vipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipCardId);
            if (vipcard == null)
            {
                return Result.FromError("年卡信息异常！！");
            }
            if (vipcard.State == VipCardStateType.Lost)
            {
                entity.State = IcoperDetailStateType.UnLossed;
                vipcard.State = VipCardStateType.Actived;
            }
            else
            {
                entity.State = IcoperDetailStateType.Lossed;
                vipcard.State = VipCardStateType.Lost;
            }
            vipcard.LastModificationTime = System.DateTime.Now;
            await _parkVipCardRepository.UpdateAsync(vipcard);

            if (!string.IsNullOrWhiteSpace(applyName))
            {
                entity.ApplyName = applyName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(applyPhone))
            {
                entity.ApplyPhone = applyPhone.Trim();
            }
            if (!string.IsNullOrWhiteSpace(applyPid))
            {
                entity.ApplyPid = applyPid.Trim();
            }
            entity.CreationTime = System.DateTime.Now;
            entity.TerminalId = terminalId;
            entity.VIPCardId = vipCardId;
            entity.ParkId = parkId;
            await _parkIcoperdetailRepository.InsertAsync(entity);

            //多园年卡同步
            if (vipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var dto = new MulYearCardLossDto();
                dto.ParkId = parkId;
                dto.IcoperDetail = entity.MapTo<MulIcoperDetailDto>();
                dto.VIPCard = vipcard.MapTo<MulVipCardDto>();

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardLoss,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = vipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }

                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }
            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vipVoucherId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> ReturnVoucherAsync(int vipVoucherId, string applyName, string applyPhone, string applyPid, int terminalId, int parkId)
        {


            var vipvoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.Id == vipVoucherId);

            if (vipvoucher == null)
            {
                return Result.FromError("凭证信息异常");
            }

            if (vipvoucher.State != VipVoucherStateType.NotActive)
            {
                return Result.FromError("已售的年卡凭证才能退");
            }

            var tradeInfoOld = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipvoucher.TradeInfoId);

            if (tradeInfoOld.PayModeId == PayType.MultiPay)
            {
                return Result.FromError("此订单为多种支付 不能退");
            }

            /*
             *  1. 为所有退款的票添加 一条交易记录  2. 添加退票记录  3. 退款的票状态更改
             */

            var tradeInfo = new TradeInfo()
            {
                Amount = vipvoucher.SalePrice,
                TradeType = TradeType.Outlay,
                TradeInfoDetails = new List<TradeInfoDetail>()
                {
                    new TradeInfoDetail()
                    {
                        Amount = vipvoucher.SalePrice,
                        PayModeId = tradeInfoOld.PayModeId
                    }
                },
                PayModeId = tradeInfoOld.PayModeId
            };

            var entityVoucher = new VipVoucherReturn();

            tradeInfo.TradeType = Core.TradeInfos.TradeType.Outlay;
            //为所有退款的票添加 一条交易记录
            var tradeInfoNew = await _tradeInfoDomainService.AddTradeInfoAsync(tradeInfo, parkId);

            vipvoucher.State = VipVoucherStateType.Cancel;
            vipvoucher.LastModificationTime = System.DateTime.Now;
            await _parkVipVoucherRepository.UpdateAsync(vipvoucher);

            if (!string.IsNullOrWhiteSpace(applyName))
            {
                entityVoucher.ApplyName = applyName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(applyPhone))
            {
                entityVoucher.ApplyPhone = applyPhone.Trim();
            }
            if (!string.IsNullOrWhiteSpace(applyPid))
            {
                entityVoucher.ApplyPid = applyPid.Trim();
            }
            entityVoucher.ParkId = parkId;
            entityVoucher.CreationTime = System.DateTime.Now;
            entityVoucher.TerminalId = terminalId;
            entityVoucher.VipVoucherId = vipvoucher.Id;
            entityVoucher.TradeInfoId = tradeInfoNew.Id;
            entityVoucher.OriginalTradeInfoId = vipvoucher.TradeInfoId;
            entityVoucher.Amount = vipvoucher.SalePrice;

            await _vipVoucherReturnRepository.InsertAndGetIdAsync(entityVoucher);


            //多园年卡同步
            if (vipvoucher.ParkSaleTicketClass.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {

                var dto = new MulYearCardVoucherReturnDto();
                dto.ParkId = entityVoucher.ParkId;
                dto.Id = entityVoucher.Id;
                dto.VipVoucherId = entityVoucher.VipVoucherId;
                dto.OriginalTradeInfoId = entityVoucher.OriginalTradeInfoId;
                dto.Amount = entityVoucher.Amount;
                dto.TerminalId = entityVoucher.TerminalId;
                dto.CreatorUserId = entityVoucher.CreatorUserId;
                dto.ApplyName = entityVoucher.ApplyName;
                dto.ApplyPid = entityVoucher.ApplyPid;
                dto.ApplyPhone = entityVoucher.ApplyPhone;
                dto.Remark = entityVoucher.Remark;
                dto.TradeInfoId = entityVoucher.TradeInfoId;

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardVoucherReturn,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = vipvoucher.ParkSaleTicketClass.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }


            return Result.Ok();
        }


        /// <summary>
        /// 退卡
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> ReturnYearCardAsync(int vipCardId, string applyName, string applyPhone, string applyPid, int terminalId, int parkId)
        {
            var entity = new VipCardReturn();
            var entityVoucher = new VipVoucherReturn();


            //退卡
            var vipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == vipCardId);
            if (vipcard == null)
            {
                return Result.FromError("年卡信息异常！！");
            }

            if (vipcard.ParkId != parkId)
            {
                return Result.FromError("不是本公园销售年卡不允许在本公园退卡！！");
            }

            if (vipcard.State != VipCardStateType.Actived && vipcard.State != VipCardStateType.NotActive)
            {
                return Result.FromError("已售、已激活年卡才能退卡！！");
            }

            //退凭证激活的年卡
            if (vipcard.TradeInfoId == null)
            {
                var vipvoucher = await _parkVipVoucherRepository.FirstOrDefaultAsync(p => p.VIPCardId == vipCardId);
                /*
                 *  1. 为所有退款的票添加 一条交易记录  2. 添加退票记录  3. 退款的票状态更改
                 */

                var tradeInfoOld = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipvoucher.TradeInfoId);

                if (tradeInfoOld.PayModeId == PayType.MultiPay)
                {
                    return Result.FromError("此订单为多种支付 不能退");
                }

                var tradeInfo = new TradeInfo()
                {
                    Amount = vipvoucher.SalePrice,
                    TradeType = TradeType.Outlay,
                    TradeInfoDetails = new List<TradeInfoDetail>()
                    {
                        new TradeInfoDetail()
                        {
                            Amount = vipvoucher.SalePrice,
                            PayModeId = tradeInfoOld.PayModeId
                        }
                    },
                    PayModeId = tradeInfoOld.PayModeId
                };

                tradeInfo.TradeType = Core.TradeInfos.TradeType.Outlay;
                //为所有退款的票添加 一条交易记录
                var tradeInfoNew = await _tradeInfoDomainService.AddTradeInfoAsync(tradeInfo, parkId);

                vipcard.State = VipCardStateType.Cancel;
                vipcard.LastModificationTime = System.DateTime.Now;
                await _parkVipCardRepository.UpdateAsync(vipcard);

                vipvoucher.State = VipVoucherStateType.Cancel;
                vipvoucher.LastModificationTime = System.DateTime.Now;
                await _parkVipVoucherRepository.UpdateAsync(vipvoucher);

                if (!string.IsNullOrWhiteSpace(applyName))
                {
                    entityVoucher.ApplyName = applyName.Trim();
                }
                if (!string.IsNullOrWhiteSpace(applyPhone))
                {
                    entityVoucher.ApplyPhone = applyPhone.Trim();
                }
                if (!string.IsNullOrWhiteSpace(applyPid))
                {
                    entityVoucher.ApplyPid = applyPid.Trim();
                }
                entityVoucher.CreationTime = System.DateTime.Now;
                entityVoucher.TerminalId = terminalId;
                entityVoucher.VipVoucherId = vipvoucher.Id;
                entityVoucher.TradeInfoId = tradeInfoNew.Id;
                entityVoucher.ParkId = parkId;
                entityVoucher.OriginalTradeInfoId = vipvoucher.TradeInfoId;
                entityVoucher.Amount = vipvoucher.SalePrice;

                await _vipVoucherReturnRepository.InsertAndGetIdAsync(entityVoucher);


            }
            else
            {

                var tradeInfoOld = await _tradeInfoRepository.FirstOrDefaultAsync(p => p.Id == vipcard.TradeInfoId);

                if (tradeInfoOld.PayModeId == PayType.MultiPay)
                {
                    return Result.FromError("此订单为多种支付 不能退");
                }

                /*
                 *  1. 为所有退款的票添加 一条交易记录  2. 添加退票记录  3. 退款的票状态更改
                 */

                var tradeInfo = new TradeInfo()
                {
                    Amount = vipcard.SalePrice,
                    TradeType = TradeType.Outlay,
                    TradeInfoDetails = new List<TradeInfoDetail>()
                {
                    new TradeInfoDetail()
                    {
                        Amount = vipcard.SalePrice,
                        PayModeId = tradeInfoOld.PayModeId
                    }
                }
                };

                //为所有退款的票添加 一条交易记录
                var tradeInfoNew = await _tradeInfoDomainService.AddTradeInfoAsync(tradeInfo, parkId);

                vipcard.State = VipCardStateType.Cancel;

                vipcard.LastModificationTime = System.DateTime.Now;
                await _parkVipCardRepository.UpdateAsync(vipcard);

                if (!string.IsNullOrWhiteSpace(applyName))
                {
                    entity.ApplyName = applyName.Trim();
                }
                if (!string.IsNullOrWhiteSpace(applyPhone))
                {
                    entity.ApplyPhone = applyPhone.Trim();
                }
                if (!string.IsNullOrWhiteSpace(applyPid))
                {
                    entity.ApplyPid = applyPid.Trim();
                }
                entity.CreationTime = System.DateTime.Now;
                entity.TerminalId = terminalId;
                entity.VipCardId = vipCardId;
                entity.TradeInfoId = tradeInfoNew.Id;
                entity.ParkId = parkId;
                entity.OriginalTradeInfoId = vipcard.TradeInfoId;
                entity.Amount = vipcard.SalePrice;

                await _vipCardReturnRepository.InsertAndGetIdAsync(entity);

            }

            //多园年卡同步

            if (vipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var dto = new MulYearCardReturnDto();
                dto.ParkId = parkId;
                dto.CreatorUserId = entity.CreatorUserId;
                dto.TerminalId = entity.TerminalId;
                dto.Amount = entity.Amount;
                dto.VipCardReturn = entity.MapTo<MulVipCardReturnDto>();
                dto.VipVoucherReturn = entityVoucher.MapTo<MulVipVoucherReturnDto>();

                dto.VIPCard = vipcard.MapTo<MulVipCardDto>();

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardReturn,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = vipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }

            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = vipcard.IcBasicInfo.IcNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);

            }

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(vipcard.IcBasicInfo.IcNo.ToUpper());


            return Result.Ok();
        }



        /// <summary>
        /// 年卡补卡
        /// </summary>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <returns></returns>
        public async Task<Result> FillYearCardAsync(FillCardInput fillCardInput, string tradeno, int parkId, int terminalId)
        {
           

            var oldVipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == fillCardInput.OldVipCardId);

            var oldIcNo= oldVipcard.IcBasicInfo.IcNo.ToUpper();
 
            if (oldVipcard == null)
            {
                return Result.FromError("旧年卡信息异常！！");
            }
            if (oldVipcard.State != VipCardStateType.Actived)
            {
                return Result.FromError("旧卡状态必须为已激活才能换卡！！");
            }
            var newVipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == fillCardInput.NewVipCardId);
            var newIcNo = newVipcard.IcBasicInfo.IcNo.ToUpper();

            if (newVipcard == null)
            {
                return Result.FromError("新年卡信息异常！！");
            }
            if (newVipcard.State != VipCardStateType.Init)
            {
                return Result.FromError("新卡状态必须为初始化才能换卡！！");
            }

            var fillCard = new FillCard
            {
                VipCardId = oldVipcard.Id,
                NewIcNo = newVipcard.IcBasicInfo.IcNo,
                OldIcNo = oldVipcard.IcBasicInfo.IcNo,
                TerminalId = terminalId,
                TradeInfoId = tradeno,
                State = FillCardType.FillCard,
                ParkId = parkId
            };

            //卡信息互换
            var oldIcBasicInfoId = oldVipcard.IcBasicInfoId;
            oldVipcard.IcBasicInfoId = newVipcard.IcBasicInfoId;
            newVipcard.IcBasicInfoId = oldIcBasicInfoId;

            var oldEcardId = oldVipcard.IcBasicInfo.ECardID;
            oldVipcard.IcBasicInfo.ECardID = newVipcard.IcBasicInfo.ECardID;
            newVipcard.IcBasicInfo.ECardID = oldEcardId;



            //写操作记录
            var icoperdetail = new IcoperDetail
            {
                VIPCardId = oldVipcard.Id,
                ApplyName = fillCardInput.ApplyName,
                ApplyPhone = fillCardInput.ApplyPhone,
                ApplyPid = fillCardInput.ApplyPid,
                CreationTime = System.DateTime.Now,
                State = IcoperDetailStateType.FillCard,
                TerminalId = terminalId,
                Remark = "旧卡卡号：" + oldVipcard.IcBasicInfo.IcNo,
                ParkId = parkId

            };

            await _parkVipCardRepository.UpdateAsync(oldVipcard);
            await _parkVipCardRepository.UpdateAsync(newVipcard);
            var fillCardId = await _parkFillCardRepository.InsertAndGetIdAsync(fillCard);

            var icoperdetailId = await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);


            //先更新缓存
            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(oldIcNo.ToUpper());
            if (listFinger == null)
            {
                listFinger = GetFingerData(fillCardInput.OldVipCardId);
            }
            if (listFinger != null)
            {
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(newIcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
            }
            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(oldIcNo.ToUpper());

            //多园年卡同步

            if (oldVipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {

                var dto = new MulYearCardFillDto();
                dto.ParkId = parkId;
                fillCard.Id = fillCardId;
                dto.FillCard = fillCard.MapTo<MulFillCardDto>(); ;

                dto.NewVIPCard = newVipcard.MapTo<MulVipCardDto>();
                dto.OldVIPCard = oldVipcard.MapTo<MulVipCardDto>();
                icoperdetail.Id = icoperdetailId;
                dto.IcoperDetail = icoperdetail.MapTo<MulIcoperDetailDto>();

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardFill,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = oldVipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = newVipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }

                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput3 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput3);
                }

            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = newVipcard.IcBasicInfo.IcNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);

                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput3 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput3);
            }




            return Result.Ok();
        }

        /// <summary>
        /// 获取指纹缓存
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <returns></returns>
        public List<FingerDataItem> GetFingerData(long vipCardId)
        {
            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            var customers = _parkUserIcRepository.GetAll().Where(p => p.VIPCardId == vipCardId).Select(o => new FingerDataItem
            {
                EnrollId = o.Customer.Id,
                FingerData = o.Customer.Fp1
            }).ToList();

            customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.EnrollId, FingerData = o.FingerData }));
            return listFinger;
        }


        /// <summary>
        /// 年卡续卡
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> RenewalYearCardAsync(decimal amount, FillCardInput fillCardInput, string tradeno, int parkId, int terminalId)
        {

            var oldVipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == fillCardInput.OldVipCardId);
            if (oldVipcard == null)
            {
                return Result.FromError("旧年卡信息异常！！");
            }
            if (oldVipcard.State != VipCardStateType.Actived)
            {
                return Result.FromError("旧卡状态必须为已激活才能换卡！！");
            }

            var oldIcNo = oldVipcard.IcBasicInfo.IcNo;

            //后台检验续卡支付价格是否异常
            var renwarlPrice = await GetRenewalCardPriceAsync(fillCardInput.OldVipCardId, oldVipcard.TicketClassId, parkId);
            if (decimal.Parse(renwarlPrice.Data) != amount)
            {
                return Result.FromError("续卡价格异常！！");
            }


            var newVipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == fillCardInput.NewVipCardId);
            if (newVipcard == null)
            {
                return Result.FromError("新年卡信息异常！！");
            }
            if (newVipcard.State != VipCardStateType.Init)
            {
                return Result.FromError("新卡状态必须为初始化才能换卡！！");
            }

            var newIcNo = newVipcard.IcBasicInfo.IcNo;

            var fillCard = new FillCard
            {
                VipCardId = oldVipcard.Id,
                NewIcNo = newVipcard.IcBasicInfo.IcNo,
                OldIcNo = oldVipcard.IcBasicInfo.IcNo,
                TerminalId = terminalId,
                TradeInfoId = tradeno,
                State = FillCardType.RenewalCard,
                ParkId = parkId
            };

            //卡信息互换
            var oldIcBasicInfoId = oldVipcard.IcBasicInfoId;
            oldVipcard.IcBasicInfoId = newVipcard.IcBasicInfoId;
            newVipcard.IcBasicInfoId = oldIcBasicInfoId;

            var oldEcardId = oldVipcard.IcBasicInfo.ECardID;
            oldVipcard.IcBasicInfo.ECardID = newVipcard.IcBasicInfo.ECardID;
            newVipcard.IcBasicInfo.ECardID = oldEcardId;

            //更新续卡后有效期
            if (oldVipcard.ValidDateEnd.Value > System.DateTime.Today)
            {
                oldVipcard.ValidDateEnd = oldVipcard.ValidDateEnd.Value.AddDays(int.Parse(oldVipcard.TicketClass.InParkRule.InParkValidDays.ToString()));
            }
            else
            {
                oldVipcard.ValidDateEnd = System.DateTime.Today.AddDays(int.Parse(oldVipcard.TicketClass.InParkRule.InParkValidDays.ToString()));
            }

            //写操作记录
            var icoperdetail = new IcoperDetail();
            icoperdetail.VIPCardId = oldVipcard.Id;
            icoperdetail.ApplyName = fillCardInput.ApplyName;
            icoperdetail.ApplyPhone = fillCardInput.ApplyPhone;
            icoperdetail.ApplyPid = fillCardInput.ApplyPid;
            icoperdetail.CreationTime = System.DateTime.Now;
            icoperdetail.State = IcoperDetailStateType.ExtendCard;
            icoperdetail.TerminalId = terminalId;
            icoperdetail.Remark = "旧卡卡号：" + oldVipcard.IcBasicInfo.IcNo;
            icoperdetail.ParkId = parkId;
            await _parkVipCardRepository.UpdateAsync(oldVipcard);
            await _parkVipCardRepository.UpdateAsync(newVipcard);
            var fillCardId = await _parkFillCardRepository.InsertAndGetIdAsync(fillCard);

            var icoperdetailId = await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);

            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(oldIcNo.ToUpper());
            if (listFinger == null)
            {
                listFinger = GetFingerData(fillCardInput.OldVipCardId);
            }
            if (listFinger != null)
            {
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(newIcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
            }
            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(oldIcNo.ToUpper());

            //多园年卡同步

            if (oldVipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var dto = new MulYearCardRenewDto();
                dto.ChangeCard = true;

                dto.ParkId = parkId;
                fillCard.Id = fillCardId;
                dto.FillCard = fillCard.MapTo<MulFillCardDto>(); 
                dto.NewVIPCard = newVipcard.MapTo<MulVipCardDto>();
                dto.OldVIPCard = oldVipcard.MapTo<MulVipCardDto>();
                icoperdetail.Id = icoperdetailId;
                dto.IcoperDetail = icoperdetail.MapTo<MulIcoperDetailDto>();

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardRenew,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = oldVipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }


                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = newVipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }

                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput3 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput3);
                }

            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = newVipcard.IcBasicInfo.IcNo;
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput2);

                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput3 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput3);
            }


            return Result.Ok();
        }


        /// <summary>
        /// 年卡续卡(不换卡)
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> RenewalYearCardNoChangeCardAsync(decimal amount, FillCardInput fillCardInput, string tradeno, int parkId, int terminalId)
        {

            var oldVipcard = await _parkVipCardRepository.FirstOrDefaultAsync(p => p.Id == fillCardInput.OldVipCardId);
            if (oldVipcard == null)
            {
                return Result.FromError("旧年卡信息异常！！");
            }
            if (oldVipcard.State != VipCardStateType.Actived)
            {
                return Result.FromError("旧卡状态必须为已激活才能换卡！！");
            }

            //后台检验续卡支付价格是否异常
            var renwarlPrice = await GetRenewalCardPriceAsync(fillCardInput.OldVipCardId, oldVipcard.TicketClassId, parkId);
            if (decimal.Parse(renwarlPrice.Data) != amount)
            {
                return Result.FromError("续卡价格异常！！");
            }


            var fillCard = new FillCard
            {
                VipCardId = oldVipcard.Id,
                NewIcNo = oldVipcard.IcBasicInfo.IcNo,
                OldIcNo = oldVipcard.IcBasicInfo.IcNo,
                TerminalId = terminalId,
                TradeInfoId = tradeno,
                State = FillCardType.RenewalCard,
                ParkId = parkId
            };


            //更新续卡后有效期
            if (oldVipcard.ValidDateEnd.Value > System.DateTime.Today)
            {
                oldVipcard.ValidDateEnd = oldVipcard.ValidDateEnd.Value.AddDays(int.Parse(oldVipcard.TicketClass.InParkRule.InParkValidDays.ToString()));
            }
            else
            {
                oldVipcard.ValidDateEnd = System.DateTime.Today.AddDays(int.Parse(oldVipcard.TicketClass.InParkRule.InParkValidDays.ToString()));
            }

            //写操作记录
            var icoperdetail = new IcoperDetail();
            icoperdetail.VIPCardId = oldVipcard.Id;
            icoperdetail.ApplyName = fillCardInput.ApplyName;
            icoperdetail.ApplyPhone = fillCardInput.ApplyPhone;
            icoperdetail.ApplyPid = fillCardInput.ApplyPid;
            icoperdetail.CreationTime = System.DateTime.Now;
            icoperdetail.State = IcoperDetailStateType.ExtendCard;
            icoperdetail.TerminalId = terminalId;
            icoperdetail.Remark = "旧卡卡号：" + oldVipcard.IcBasicInfo.IcNo;
            icoperdetail.ParkId = parkId;
            await _parkVipCardRepository.UpdateAsync(oldVipcard);

            var fillCardId = await _parkFillCardRepository.InsertAndGetIdAsync(fillCard);

            var icoperdetailId = await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);



            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(oldVipcard.IcBasicInfo.IcNo.ToUpper());
            if (listFinger == null)
            {
                listFinger = GetFingerData(fillCardInput.OldVipCardId);
            }
            if (listFinger != null)
            {
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(oldVipcard.IcBasicInfo.IcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
            }

            //多园年卡同步

            if (oldVipcard.TicketClass.TicketClassMode == TicketClassMode.MultiYearCard)
            {
                var dto = new MulYearCardRenewDto();
                dto.ChangeCard = false;

                dto.ParkId = parkId;
                fillCard.Id = fillCardId;
                dto.FillCard = fillCard.MapTo<MulFillCardDto>();
                dto.OldVIPCard = oldVipcard.MapTo<MulVipCardDto>();
                icoperdetail.Id = icoperdetailId;
                dto.IcoperDetail = icoperdetail.MapTo<MulIcoperDetailDto>();

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MulYearCardRenew,
                    SyncData = JsonConvert.SerializeObject(dto)
                };

                var otherParkIds = oldVipcard.TicketClass.InParkIdFilter.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != parkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }


                //清缓存
                var dto2 = new TicketCheckCacheDto();
                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput2 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                foreach (var parkid in otherParkIds)
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput2);
                }
 

            }
            else
            {
                //清缓存
                var dto2 = new TicketCheckCacheDto();
                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                dto2.Key = oldVipcard.IcBasicInfo.IcNo;

                var syncInput3 = new DataSyncInput
                {
                    SyncType = DataSyncType.TicketCheckCacheClear,
                    SyncData = JsonConvert.SerializeObject(dto2)
                };

                dataSyncManager.UploadDataToTargetPark(parkId, syncInput3);
            }


            return Result.Ok();
        }



        /// <summary>
        /// 获取指纹数据
        /// </summary>
        /// <returns></returns>
        public List<FingerDataDto> GetFingerData(DateTime lastGetTime)
        {
            //var userIcs2 = _parkUserIcRepository.GetAll().ProjectTo<UserIcDto2>().ToList();
            var userIcs = _parkUserIcRepository.GetAll().Where(p => p.VIPCard.State == VipCardStateType.Actived && p.LastModificationTime >= lastGetTime).ProjectTo<UserIcDto>().ToList();

            var listFingerData = new List<FingerDataDto>();
            foreach (var item in userIcs)
            {
                var fingerData = new FingerDataDto();
                fingerData.IcBarcode = item.VIPCard.IcBasicInfo.IcNo;
                fingerData.Idnum = item.CustomCustomer.Pid;
                fingerData.Finger = item.CustomCustomer.Fp1;
                fingerData.ZkFinger = item.CustomCustomer.Fp2;
                fingerData.EnrollId = Convert.ToInt32("1" + item.CustomId);//：以开头区分年卡、套票、二次入园  1 年卡  2 二次入园  3 套票
                fingerData.TicketClassName = item.VIPCard.TicketClass.TicketClassName;
                fingerData.VipCardId = (int)item.VIPCardId;
                listFingerData.Add(fingerData);
            }
            return listFingerData;

        }

        /// <summary>
        /// 根据条件获取年卡记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetVIPCardAsync<TDto>(IQuery<VIPCard> query)
        {
            return await _parkVipCardRepository.AsNoTracking().FirstOrDefaultAsync<VIPCard, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取年卡记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetVIPCardListAsync<TDto>(IQuery<VIPCard> query)
        {
            return await _parkVipCardRepository.AsNoTracking().ToListAsync<VIPCard, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取年卡续卡设置列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetVipCardRenewalSetListAsync<TDto>(IQuery<VipCardRenewalSet> query)
        {
            return await _parkVipCardRenewalRepository.AsNoTracking().ToListAsync<VipCardRenewalSet, TDto>(query);
        }

        /// <summary>
        /// 新增或修改年卡价格设置
        /// </summary>
        /// <param name="yearCardRenewalSetInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result> AddOrUpYearCardRenewalSet(YearCardRenewalSetInput yearCardRenewalSetInput, int parkId, int terminalId)
        {
            var renewalSet = await _parkVipCardRenewalRepository.FirstOrDefaultAsync(p => p.TicketClassId == yearCardRenewalSetInput.TicketClassId && p.ParkId == parkId);
            if (renewalSet == null)
            {
                renewalSet = new VipCardRenewalSet()
                {
                    ParkId = parkId,
                    FirstPrice = yearCardRenewalSetInput.FirstPrice,
                    TicketClassId = yearCardRenewalSetInput.TicketClassId,
                    MulPrice = yearCardRenewalSetInput.MulPrice
                };
            }
            else
            {
                renewalSet.FirstPrice = yearCardRenewalSetInput.FirstPrice;
                renewalSet.TicketClassId = yearCardRenewalSetInput.TicketClassId;
                renewalSet.MulPrice = yearCardRenewalSetInput.MulPrice;
            }
            await _parkVipCardRenewalRepository.InsertOrUpdateAsync(renewalSet);

            return Result.Ok();
        }


        /// <summary>
        /// 自动生成电子年卡完整信息
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AutoCreateVipInfo()
        {
            var agencySaleticketClassId = 1000318;

            var ticketClass = (await _agencySaleTicketClassRepository.GetAsync(agencySaleticketClassId)).MapTo<AgencySaleTicketClassGetTicketClassDto>();

            var eCardNo = await _uniqueCode.CreateAsync(CodeType.ECard, AbpSession.LocalParkId, AbpSession.TerminalId);
            int parkId = 10001;


            var entityIcBasicInfo = new IcBasicInfo
            {
                ECardID = eCardNo,
                CardNo = eCardNo,
                IcNo = eCardNo,
                KindId = 1,
                ParkId = parkId,
                CreatorUserId = 1
            };

            await _parkIcBasicInfoRepository.InsertAndGetIdAsync(entityIcBasicInfo);

            var entityVipCard = new VIPCard
            {
                ParkId = parkId,
                IcBasicInfoId = entityIcBasicInfo.Id,
                TicketClassId = ticketClass.TicketClassId,
                ParkSaleTicketClassId = ticketClass.ParkSaleTicketClassId,
                State = VipCardStateType.Actived,

            };
            await _parkVipCardRepository.InsertAndGetIdAsync(entityVipCard);

            return null;
        }


    }
}
