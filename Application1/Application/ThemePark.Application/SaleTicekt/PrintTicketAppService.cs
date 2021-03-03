using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using ThemePark.Application.InPark.Dto;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.Application.SaleCard.Interfaces;
using ThemePark.Core.CardManage;
using ThemePark.Application.SaleCard.Dto;
using ThemePark.Core.BasicData;
using ThemePark.Application.InPark.Interfaces;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.InPark;
using ThemePark.Core.Settings;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 取打印数据
    /// </summary>
    public class PrintTicketAppService : ThemeParkAppServiceBase, IPrintTicketAppService
    {
        #region Fields
        private readonly INonGroupTicketAppService _nonGroupticketAppService;
        private readonly IVIPCardAppService _vipCardAppService;
        private readonly IFareAdjustmentAppService _fareAdjustmentAppService;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IGroupTicketAppService _groupTicketAppService;
        private readonly IRepository<PrintTemplateDetail> _printTemplateDetailRepository;
        private readonly IEnterBillService _enterBillService;
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<InParkBill, string> _inParkBillRepository;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<TicketPrintSet> _ticketPrintSet;
        private readonly IRepository<DefaultPrintSet> _defaultPrintSetRepository;
        private readonly IRepository<AgencyPrintSet> _agencyPrintSetRepository;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// 构造方法
        /// </summary>
        public PrintTicketAppService(IRepository<TOTicket, string> toTicketRepository, IRepository<Park> parkRepository,
            IGroupTicketAppService groupTicketAppService, IRepository<InParkBill, string> inParkBillRepository,
        INonGroupTicketAppService nonGroupticketAppService, IFareAdjustmentAppService fareAdjustmentAppService
            , IVIPCardAppService vipCardAppService, IRepository<PrintTemplateDetail> printTemplateDetailRepository,
         IEnterBillService enterBillService,
        ISettingManager settingManager, IRepository<TicketPrintSet> ticketPrintSet, IRepository<DefaultPrintSet> defaultPrintSetRepository, IRepository<AgencyPrintSet> agencyPrintSetRepository, IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository, IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository)
        {
            _toTicketRepository = toTicketRepository;
            _nonGroupticketAppService = nonGroupticketAppService;
            _fareAdjustmentAppService = fareAdjustmentAppService;
            _groupTicketAppService = groupTicketAppService;
            _vipCardAppService = vipCardAppService;
            _printTemplateDetailRepository = printTemplateDetailRepository;
            _enterBillService = enterBillService;
            _settingManager = settingManager;
            _ticketPrintSet = ticketPrintSet;
            _defaultPrintSetRepository = defaultPrintSetRepository;
            _agencyPrintSetRepository = agencyPrintSetRepository;
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _inParkBillRepository = inParkBillRepository;
            _parkRepository = parkRepository;
        }

        #endregion

        /// <summary>
        /// 售票打印数据
        /// </summary>
        /// <param name="barcode">售票打印传tradeNo,取票打印传Barcode</param>
        /// <param name="type"></param>
        /// <param name="tradeno"></param>
        /// <returns>返回打印数据或者 元素个数为0的List</returns>
        public async Task<Result<List<PrintInfo>>> GetPrintContentTask(PrintTicketType type, string tradeno = null, string barcode = null)
        {
            List<PrintInfo> data = new List<PrintInfo>();

            switch (type)
            {
                //散客售票打印数据
                case PrintTicketType.NonGroupTicket:
                    data = await NonGroupTicketPrintData(tradeno, barcode);
                    break;
                //团体票打印数据
                case PrintTicketType.GroupTicket:
                    data = await GroupTicketPrintData(tradeno, barcode);
                    break;
                //取网络票、取旅行社票
                case PrintTicketType.WebTicket:
                    data = await TakeToTicketPrintData(barcode);
                    break;
                //补票
                case PrintTicketType.FareAdjustment:
                    data = await FareAdjustTicketPrintData(tradeno, barcode);
                    break;
                //年卡凭证打印
                case PrintTicketType.YearCardVoucher:
                    data = await YearCardVoucherPrintData(tradeno, barcode);
                    break;
                //入园单打印
                case PrintTicketType.Admission:
                    data = await InParkBillPrintData(barcode);
                    break;
                //年卡打印 参数为 parkSaleTicketClassId
                case PrintTicketType.YearCard:
                    data = await YearCardPrintData(barcode);
                    break;
                //他园票打印
                case PrintTicketType.OtherNonGroupTicket:
                    data = await OtherNonGroupTicketPrintData(tradeno, barcode);
                    break;
            }

            return Result.FromData(data);
        }

        /// <summary>
        /// 年卡打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> YearCardPrintData(string parkSaleTicketClassId)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();

            PrintInfo data = new PrintInfo
            {
                PrintTemplate = GetPrintYearCardTemplate(int.Parse(parkSaleTicketClassId)),
                PrintTemplateType = PrintTemplateType.YearCard
            };

            printContents.Add(data);

            return printContents;

        }


        ///// <summary>
        ///// 年卡打印数据
        ///// </summary>
        ///// <returns></returns>
        //public async Task<List<PrintInfo>> YearCardPrintData(string vipCardId)
        //{
        //    List<PrintInfo> printContents = new List<PrintInfo>();
        //    int parkSaleTicketClassId;
        //    var cardId = Convert.ToInt64(vipCardId);
        //    var result = await _vipCardAppService.GetYearCardListAsync<VIPCardDto>(new Query<VIPCard>(p => p.Id == cardId));
        //    if (result.Count == 0)
        //    {
        //        var result2 = await _vipCardAppService.GetVoucherDetailInfoByIdAsync(cardId);
        //        if (result2 == null)
        //        {
        //            return printContents;
        //        }
        //        parkSaleTicketClassId = result2.ParkSaleTicketClass.Id;
        //    }
        //    else
        //    {
        //        parkSaleTicketClassId = result[0].ParkSaleTicketClassId.Value;
        //    }

        //    PrintInfo data = new PrintInfo
        //    {
        //        PrintTemplate = GetPrintYearCardTemplate(parkSaleTicketClassId),
        //        PrintTemplateType = PrintTemplateType.YearCard
        //    };

        //    printContents.Add(data);

        //    return printContents;

        //}


        /// <summary>
        /// 入园单打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> InParkBillPrintData(string barcode)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();

            var enterBill = (await _enterBillService.GetInParkBillInfoByIdAsync(barcode)).Data;

            if (enterBill == null)
            {
                printContents.Add(new PrintInfo()
                {
                    TicketContent = new TicketContent()
                    {
                        Barcode = barcode,
                    },
                    PrintTemplate = GetPrintTemplate(PrintTemplateSetting.InParkBillTicketClassId),
                    PrintTemplateType = PrintTemplateType.InParkBill
                });
                return printContents;
            }

            printContents.Add(CreateInParkBillPrintContent(enterBill));

            return printContents;
        }


        /// <summary>
        /// 年卡凭证打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> YearCardVoucherPrintData(string tradeno = null, string barcode = null)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();
            IList<ListVipVoucherDto> listVipVoucherDtoList;
            if (!string.IsNullOrWhiteSpace(tradeno))
            {
                listVipVoucherDtoList = await _vipCardAppService.GetYearCardVoucherListAsync<ListVipVoucherDto>(new Query<VIPVoucher>(p => p.TradeInfoId == tradeno && p.State == VipVoucherStateType.NotActive));
            }
            else
            {
                listVipVoucherDtoList = await _vipCardAppService.GetYearCardVoucherListAsync<ListVipVoucherDto>(new Query<VIPVoucher>(p => p.Barcode == barcode && p.State == VipVoucherStateType.NotActive));
            }

            if (listVipVoucherDtoList.Count == 0)
                return printContents;

            printContents.AddRange(listVipVoucherDtoList.Select(CreateYearCardVoucherPrintContent));

            return printContents;
        }


        /// <summary>
        /// 取散客打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> NonGroupTicketPrintData(string tradeno = null, string barcode = null)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();
            IList<NonGroupTicketDetailDto> nonGroupTicketDetailDtoList;
            if (!string.IsNullOrWhiteSpace(tradeno))
            {
                nonGroupTicketDetailDtoList = await _nonGroupticketAppService.GetNonGroupTicketListAsync<NonGroupTicketDetailDto>(new Query<NonGroupTicket>(p => p.TradeInfoId == tradeno && p.TicketSaleStatus == TicketSaleStatus.Valid));
            }
            else
            {
                nonGroupTicketDetailDtoList = await _nonGroupticketAppService.GetNonGroupTicketListAsync<NonGroupTicketDetailDto>(new Query<NonGroupTicket>(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid));
            }

            if (nonGroupTicketDetailDtoList.Count == 0)
                return printContents;

            printContents.AddRange(nonGroupTicketDetailDtoList.Select(CreateNonGroupTicketPrintContent));
            return printContents;
        }

        /// <summary>
        /// 公园散客门票打印数据
        /// </summary>
        /// <param name="tradeno"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<List<PrintInfo>> OtherNonGroupTicketPrintData(string tradeno = null, string barcode = null)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();
            IList<OtherNonGroupTicketDto> otherNonGroupTicketDtoList;
            if (!string.IsNullOrWhiteSpace(tradeno))
            {
                otherNonGroupTicketDtoList = (await _otherNonGroupTicketRepository.GetAllListAsync(p => p.TradeInfoId == tradeno && p.TicketSaleStatus == TicketSaleStatus.Valid)).MapTo<List<OtherNonGroupTicketDto>>();
            }
            else
            {
                otherNonGroupTicketDtoList = (await _otherNonGroupTicketRepository.GetAllListAsync(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid)).MapTo<List<OtherNonGroupTicketDto>>();
            }

            if (otherNonGroupTicketDtoList.Count == 0)
                return printContents;

            printContents.AddRange(otherNonGroupTicketDtoList.Select(CreateOtherNonGroupTicketPrintContent));

            return printContents;
        }


        /// <summary>
        /// 团体票打印数据
        /// </summary>
        /// <param name="tradeno"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<List<PrintInfo>> GroupTicketPrintData(string tradeno = null, string barcode = null)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();
            IList<GroupTicketDetailDto> groupTicketDetailDtoList;
            if (!string.IsNullOrWhiteSpace(tradeno))
            {
                groupTicketDetailDtoList = await _groupTicketAppService.GetGroupTicketListAsync<GroupTicketDetailDto>(new Query<GroupTicket>(m => m.TradeInfoId == tradeno && m.TicketSaleStatus == TicketSaleStatus.Valid));
            }
            else
            {
                groupTicketDetailDtoList = await _groupTicketAppService.GetGroupTicketListAsync<GroupTicketDetailDto>(new Query<GroupTicket>(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid));
            }

            if (groupTicketDetailDtoList.Count == 0)
                return printContents;
            printContents.AddRange(groupTicketDetailDtoList.Select(CreateGroupTicketPrintContent));

            return printContents;
        }



        /// <summary>
        /// 旅行社取票或网络取票打印数据
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<List<PrintInfo>> TakeToTicketPrintData(string barcode)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();

            var result = await _toTicketRepository.GetAllIncluding(p => p.AgencySaleTicketClass, p => p.Park).FirstAsync(p => p.Id == barcode);
            if (result.TicketSaleStatus != TicketSaleStatus.Valid)
                return printContents;

            printContents.Add(CreateWebTicketPrintContent(result));

            return printContents;
        }

        /// <summary>
        /// 补票打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> FareAdjustTicketPrintData(string tradeno = null, string barcode = null)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();
            IList<FareAdjustmentDto> fareAdjustmentDtoList;
            if (!string.IsNullOrWhiteSpace(tradeno))
            {
                fareAdjustmentDtoList =
                    await _fareAdjustmentAppService.GetFareAdjustmentListAsync<FareAdjustmentDto>(
                        new Query<ExcessFare>(p => p.TradeInfoId == tradeno && p.State == TicketSaleStatus.Valid));

            }
            else
            {
                fareAdjustmentDtoList =
                    await _fareAdjustmentAppService.GetFareAdjustmentListAsync<FareAdjustmentDto>(
                        new Query<ExcessFare>(p => p.Id == barcode && p.State == TicketSaleStatus.Valid));
            }

            if (fareAdjustmentDtoList.Count == 0)
                return printContents;

            printContents.AddRange(fareAdjustmentDtoList.Select(CreateFareAdjustTicketPrintContent));
            return printContents;
        }


        /// <summary>
        /// 重打印数据（散客，团体，网络票，入园单，补票，年卡凭证）打印数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintInfo>> ReprintTicketPrintData(string barcode)
        {
            List<PrintInfo> printContents = new List<PrintInfo>();

            //重打印散客票
            var nonGroupTicket = await _nonGroupticketAppService.GetNonGroupTicketAsync<NonGroupTicketDetailDto>(new Query<NonGroupTicket>(p => p.Id == barcode && p.TicketSaleStatus == TicketSaleStatus.Valid));
            if (nonGroupTicket != null)
            {
                printContents.Add(CreateNonGroupTicketPrintContent(nonGroupTicket));
                return printContents;
            }
            //重打印团体票
            var groupTicket = await _groupTicketAppService.GetGroupTicketAsync<GroupTicketDetailDto>(new Query<GroupTicket>(m => m.Id == barcode && m.TicketSaleStatus == TicketSaleStatus.Valid));
            if (groupTicket != null)
            {
                printContents.Add(CreateGroupTicketPrintContent(groupTicket));
                return printContents;
            }

            //重打印网络票
            var toTicket = await _toTicketRepository.GetAllIncluding(p => p.AgencySaleTicketClass, p => p.Park).Where(m => m.Id == barcode && m.TicketSaleStatus == TicketSaleStatus.Valid).FirstOrDefaultAsync();
            if (toTicket != null)
            {
                printContents.Add(CreateWebTicketPrintContent(toTicket));
                return printContents;
            }

            //重打印入园单
            var inparkBill = (await _inParkBillRepository.FirstOrDefaultAsync(m => m.Id == barcode && m.InParkBillState == InParkBillState.Valid)).MapTo<InParkBillDto>();
            if (inparkBill != null)
            {
                printContents.Add(CreateInParkBillPrintContent(inparkBill));
                return printContents;
            }

            //重打印补票
            var fareAdjustment = await _fareAdjustmentAppService.GetFareAdjustmentAsync<FareAdjustmentDto>(new Query<ExcessFare>(p => p.Id == barcode && p.State == TicketSaleStatus.Valid));
            if (fareAdjustment != null)
            {
                printContents.Add(CreateFareAdjustTicketPrintContent(fareAdjustment));
                return printContents;
            }

            //重打印年卡凭证
            var vipVoucher = await _vipCardAppService.GetYearCardVoucherAsync<ListVipVoucherDto>(new Query<VIPVoucher>(p => p.Barcode == barcode && p.State == VipVoucherStateType.NotActive));
            if (vipVoucher != null)
            {
                printContents.Add(CreateYearCardVoucherPrintContent(vipVoucher));
                return printContents;
            }
            return printContents;


        }


        /// <summary>
        /// 根据ID获取打印模板 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPrintTemplate(int id)
        {
            //假如没有配置打印模板，客户端根据打印类型自动选择默认模板
            return
                  _printTemplateDetailRepository.FirstOrDefaultAsync(p => p.TicketClassId == id && p.Type != PrintTemplateType.YearCard)
                     .Result?.PrintTemplate?.Content;
        }

        /// <summary>
        /// 根据ID获取年卡打印模板 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPrintYearCardTemplate(int id)
        {

            var content = _printTemplateDetailRepository.FirstOrDefaultAsync(p => p.TicketClassId == id && p.Type == PrintTemplateType.YearCard).Result?.PrintTemplate?.Content;
            if (content == null)
            {
                //如果不存在则取同基础票类在售模板
                var nowDate = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                var oldTicketClassId =  _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.Id == id).Result?.TicketClassId;
                var newParkSaleTicketClassId = _parkSaleTicketClassRepository.FirstOrDefaultAsync(p => p.TicketClassId == oldTicketClassId && p.ParkId == AbpSession.LocalParkId && (p.IsEveryday == true || (p.SaleStartDate <= nowDate && p.SaleEndDate >= nowDate))).Result?.Id;

                content = _printTemplateDetailRepository.FirstOrDefaultAsync(p => p.TicketClassId == newParkSaleTicketClassId && p.Type == PrintTemplateType.YearCard).Result?.PrintTemplate?.Content;
            }
            return content;

            ////假如没有配置打印模板，客户端根据打印类型自动选择默认模板
            //return
            //      _printTemplateDetailRepository.FirstOrDefaultAsync(p => p.TicketClassId == id && p.Type == PrintTemplateType.YearCard)
            //         .Result?.PrintTemplate?.Content;
        }


        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private string UserMd5(string barcode)
        {
            var password = barcode + _settingManager.GetSettingValueAsync(TicketSetting.BarcodeMd5).Result;
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2.ToLower();
        }


        /// <summary>
        /// 散客打印字段赋值
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateNonGroupTicketPrintContent(NonGroupTicketDetailDto ticket)
        {
            //基础票类打印配置
            var ticketPrintSet = GetTicketPrintSet(ticket.TicketClassId).Result;

            //默认打印配置
            var printSet = GetNonGroupPrintSet().Result;
            var printPrice = printSet.PrintPriceType == PrintPriceType.SalePrice ? ticket.SalePrice : ticket.Price;
            //散客票打印：团体名称为空，防伪码、二维码暂时不赋值
            PrintInfo data = new PrintInfo()
            {
                TicketContent = new TicketContent()
                {
                    Barcode = ticket.Id,
                    Persons = (ticket.Qty * ticket.Persons).ToString(),
                    Amount = (printPrice * ticket.Qty).ToString("f"),
                    StartTime = ticket.ValidStartDate.ToString("yyyy-MM-dd"),
                    EndTime = ticket.ValidStartDate.AddDays(ticket.ValidDays).ToString("yyyy-MM-dd"),
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ParkName = ticket.ParkName,
                    Pbogus = UserMd5(ticket.Id),
                    TicketMarker = ticketPrintSet.TicketMarker,
                    BarcodeMarker = ticketPrintSet.BarcodeMarker,
                    Remark1 = ticketPrintSet.Remark1,
                    Remark2 = ticketPrintSet.Remark2,
                    ValidDate = ticketPrintSet.ValidDays.HasValue ? ticketPrintSet.ValidDays.ToString() : printSet.ValidDays.ToString(),
                    Price = printPrice.ToString("f"),
                },
                PrintTemplate = GetPrintTemplate(ticket.ParkSaleTicketClassId),
                PrintTemplateType = PrintTemplateType.Ticket
            };

            return data;
        }


        /// <summary>
        /// 团体打印字段赋值
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateGroupTicketPrintContent(GroupTicketDetailDto ticket)
        {
            //基础票类打印配置
            var ticketPrintSet = GetTicketPrintSet(ticket.TicketClassId).Result;

            //默认打印配置
            var printSet = GetGroupPrintSet(ticket.AgencyTypeId, ticket.AgencyId).Result;

            PrintInfo data = new PrintInfo()
            {
                TicketContent = new TicketContent()
                {
                    Barcode = ticket.Id,

                    Persons = (ticket.Qty * ticket.Persons).ToString(),
                    //Amount = (ticket.SalePrice * ticket.Qty).ToString("f"),
                    StartTime = ticket.ValidStartDate.ToString("yyyy-MM-dd"),
                    EndTime = ticket.ValidStartDate.AddDays(ticket.ValidDays).ToString("yyyy-MM-dd"),
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    GroupTypeName = ticket.GroupTypeName,
                    OrderId = ticket.TOHeaderId,
                    ParkName = ticket.ParkName,
                    Pbogus = UserMd5(ticket.Id),
                    TicketMarker = ticketPrintSet.TicketMarker,
                    BarcodeMarker = ticketPrintSet.BarcodeMarker,
                    Remark1 = ticketPrintSet.Remark1,
                    Remark2 = ticketPrintSet.Remark2,
                    ValidDate = printSet.ValidDays.ToString(),
                    //代理商未配置化为null
                    GroupName = printSet.IsPrintAgencyName == null || printSet.IsPrintAgencyName.Value ? ticket.AgencyName : string.Empty,

                },
                PrintTemplate = GetPrintTemplate(ticket.ParkSaleTicketClassId),
                PrintTemplateType = PrintTemplateType.Ticket
            };

            //配置不打印价格（单价总价都不打印）
            if (printSet.IsPrintPrice != null && !printSet.IsPrintPrice.Value)
            {
                data.TicketContent.Price = string.Empty;
                data.TicketContent.Amount = string.Empty;
            }
            else
            {
                //使用默认配置或者代理商单独配置
                var printPrice = printSet.PrintPriceType == PrintPriceType.SalePrice ? ticket.SalePrice : ticket.Price;
                data.TicketContent.Price = printPrice.ToString("f");
                data.TicketContent.Amount = (printPrice * ticket.Qty).ToString("f");
            }

            return data;
        }


        /// <summary>
        /// 网络票打印字段赋值
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateWebTicketPrintContent(TOTicket ticket)
        {

            //基础票类打印配置
            var ticketPrintSet = GetTicketPrintSet(ticket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClassId).Result;

            //默认打印配置
            var printSet = GetGroupPrintSet(ticket.AgencySaleTicketClass.AgencySaleTicketClassTemplate.AgencyTypeId, ticket.AgencySaleTicketClass.AgencyId).Result;

            //网络取票有效日期为当天
            PrintInfo data = new PrintInfo()
            {
                TicketContent = new TicketContent()
                {
                    Barcode = ticket.Id,
                    //Price = ticket.TOVoucher.TOBody.Price.ToString("f"),
                    Persons = (ticket.Qty * ticket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClass.TicketType.Persons).ToString(),
                    //Amount = (ticket.TOVoucher.TOBody.Price * ticket.Qty).ToString("f"),
                    StartTime = ticket.ValidStartDate.ToString("yyyy-MM-dd"),
                    EndTime = ticket.ValidStartDate.AddDays(ticket.ValidDays).ToString("yyyy-MM-dd"),
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ParkName = ticket.TOVoucher.TOBody.Park.ParkName,
                    Pbogus = UserMd5(ticket.Id),
                    TicketMarker = ticketPrintSet.TicketMarker,
                    BarcodeMarker = ticketPrintSet.BarcodeMarker,
                    ValidDate = ticket.ValidDays.ToString(),
                    Remark1 = ticketPrintSet.Remark1,
                    Remark2 = ticketPrintSet.Remark2,
                    OrderId = ticket.TOVoucher.TOBodyId,
                    //代理商未配置化为null
                    GroupName = printSet.IsPrintAgencyName == null || printSet.IsPrintAgencyName.Value ? ticket.TOVoucher.TOBody.TOHeader.Agency.AgencyName : string.Empty,
                },
                PrintTemplate = GetPrintTemplate(ticket.AgencySaleTicketClass.ParkSaleTicketClassId),
                PrintTemplateType = PrintTemplateType.Ticket

            };

            //配置不打印价格（单价总价都不打印）
            if (printSet.IsPrintPrice != null && !printSet.IsPrintPrice.Value)
            {
                data.TicketContent.Price = string.Empty;
                data.TicketContent.Amount = string.Empty;
            }
            else
            {
                //使用默认配置或者代理商单独配置
                var printPrice = printSet.PrintPriceType == PrintPriceType.SalePrice ? ticket.SalePrice : ticket.Price;
                data.TicketContent.Price = printPrice.ToString("f");
                data.TicketContent.Amount = (printPrice * ticket.Qty).ToString("f");
            }


            return data;
        }

        /// <summary>
        /// 构建入园单打印字段
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateInParkBillPrintContent(InParkBillDto ticket)
        {
            PrintInfo data = new PrintInfo
            {
                TicketContent = new TicketContent()
                {
                    ParkName = _parkRepository.GetAsync(ticket.ParkId).Result.ParkName,
                    Pbogus = UserMd5(ticket.Id),
                    Barcode = ticket.Id,
                    Company = ticket.Company,
                    Reasons = ticket.Reasons,
                    PersonNum = ticket.PersonNum.ToString(),
                    ApplyDept = ticket.ApplyDept,
                    ApplyBy = ticket.ApplyBy,
                    ApprovedBy = ticket.ApprovedBy,
                    InParkType = ticket.InParkType.DisplayName(),
                    WorkType = ticket.InParkType == InParkType.Visitor ? InParkType.Visitor.DisplayName() : ticket.WorkType.DisplayName(),
                    InParkChannel = ticket.InParkChannelName,
                    InParkTime = ticket.InParkTimeName,
                    ValidStartDate = ticket.ValidStartDate2,
                    ValidDays = ticket.ValidDays,
                    Remark = ticket.Remark,
                    InparkNotice = ticket.InparkNotice,
                    BillNo = ticket.BillNo,
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
                },
                PrintTemplate = GetPrintTemplate(PrintTemplateSetting.InParkBillTicketClassId),
                PrintTemplateType = PrintTemplateType.InParkBill
            };
            return data;

        }


        /// <summary>
        /// 构建年卡凭证打印字段
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateYearCardVoucherPrintContent(ListVipVoucherDto ticket)
        {
            //基础票类打印配置
            var ticketPrintSet = GetTicketPrintSet(ticket.TicketClassId).Result;

            //年卡凭证打印：团体名称为空，防伪码、二维码暂时不赋值
            PrintInfo data = new PrintInfo
            {
                TicketContent = new TicketContent()
                {
                    ParkName= _parkRepository.GetAsync(AbpSession.LocalParkId).Result.ParkName,
                    Barcode = ticket.Barcode,
                    Price = ticket.SalePrice.ToString("F"),
                    Persons = ticket.Persons.ToString(),
                    //Amount = (ticket.SalePrice * ticket.Persons).ToString("f"),
                    Amount = ticket.SalePrice.ToString("f"),//年卡凭证1条数据1张票 和人数无关
                    StartTime = ticket.ValidDateBegin.Value.ToString("yyyy-MM-dd"),
                    EndTime = ticket.ValidDateEnd.Value.ToString("yyyy-MM-dd"),
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Pbogus = UserMd5(ticket.Barcode),
                    TicketMarker = ticketPrintSet.TicketMarker,
                    BarcodeMarker = ticketPrintSet.BarcodeMarker,
                    Remark1 = ticketPrintSet.Remark1,
                    Remark2 = ticketPrintSet.Remark2,
                },
                PrintTemplate = GetPrintTemplate(ticket.ParkSaleTicketClassId),
                PrintTemplateType = PrintTemplateType.Ticket
            };
            return data;
        }


        /// <summary>
        /// 构建他园票打印字段
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private PrintInfo CreateOtherNonGroupTicketPrintContent(OtherNonGroupTicketDto ticket)
        {
            //todo:它园票目前以当前公园同票种票类模板为准
            var localParkId = int.Parse(ConfigurationManager.AppSettings[AppConfigSetting.LocalParkId]);
            var parkSaleTicketClass =
                _parkSaleTicketClassRepository.FirstOrDefaultAsync(
                    p =>
                        p.TicketClass.TicketTypeId == ticket.TickeTypeId &&
                        p.ParkId == localParkId).Result;
            //基础票类打印配置
            var ticketPrintSet = GetTicketPrintSet(parkSaleTicketClass.TicketClassId).Result;
            //默认打印配置
            var printSet = GetNonGroupPrintSet().Result;
            var printPrice = printSet.PrintPriceType == PrintPriceType.SalePrice ? ticket.SalePrice : ticket.Price;
            PrintInfo data = new PrintInfo()
            {
                TicketContent = new TicketContent()
                {
                    Barcode = ticket.Id,
                    Persons = (ticket.Persons * ticket.Qty).ToString(),
                    Amount = (printPrice * ticket.Qty).ToString("f"),
                    StartTime = ticket.ValidStartDate.ToString("yyyy-MM-dd"),
                    EndTime = ticket.ValidStartDate.AddDays(ticket.ValidDays).ToString("yyyy-MM-dd"),
                    CreationTime = ticket.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ParkName = ticket.ParkName,
                    Pbogus = UserMd5(ticket.Id),
                    TicketMarker = ticketPrintSet.TicketMarker,
                    BarcodeMarker = ticketPrintSet.BarcodeMarker,
                    Remark1 = ticketPrintSet.Remark1,
                    Remark2 = ticketPrintSet.Remark2,
                    ValidDate = ticketPrintSet.ValidDays.HasValue ? ticketPrintSet.ValidDays.ToString() : printSet.ValidDays.ToString(),
                    Price = printPrice.ToString("f"),
                },
                PrintTemplate = GetPrintTemplate(parkSaleTicketClass?.Id ?? 0),
                PrintTemplateType = PrintTemplateType.Ticket

            };
            return data;
        }

        /// <summary>
        /// 构建补票打印字段
        /// </summary>
        /// <param name="fareAdjustment"></param>
        /// <returns></returns>
        private PrintInfo CreateFareAdjustTicketPrintContent(FareAdjustmentDto fareAdjustment)
        {
            PrintInfo data = new PrintInfo()
            {
                TicketContent = new TicketContent()
                {
                    Barcode = fareAdjustment.Id,
                    Price = fareAdjustment.Denomination.ToString("f"),
                    Persons = fareAdjustment.Qty.ToString(),
                    Amount = (fareAdjustment.Denomination * fareAdjustment.Qty).ToString("f"),
                    StartTime = fareAdjustment.ValidEndDate.ToString("yyyy-MM-dd"),
                    EndTime = fareAdjustment.ValidEndDate.ToString("yyyy-MM-dd"),
                    CreationTime = fareAdjustment.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ParkName = _parkRepository.GetAsync(fareAdjustment.ParkId).Result.ParkName,//ticket.ParkName,
                    Pbogus = UserMd5(fareAdjustment.Id),
                    Remark1 = fareAdjustment.Remark
                },
                PrintTemplate = GetPrintTemplate(PrintTemplateSetting.ExcessFareTicketClasId),
                PrintTemplateType = PrintTemplateType.ExcessFare
            };
            return data;
        }


        /// <summary>
        /// 获取基础票类打印设置
        /// </summary>
        /// <returns></returns>
        private async Task<TicketPrintSet> GetTicketPrintSet(int ticketClassId)
        {
            return await _ticketPrintSet.FirstOrDefaultAsync(p => p.TicketClassId == ticketClassId) ?? new TicketPrintSet();
        }


        /// <summary>
        /// 打印参数配置
        /// </summary>
        private async Task<PrintSet> GetNonGroupPrintSet()
        {
            var printSet = new PrintSet();
            printSet.PrintPriceType = (PrintPriceType)int.Parse(await _settingManager.GetSettingValueAsync(TicketSetting.NonGroupPriceSet));
            var setResult = await _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == null) ?? new DefaultPrintSet();
            printSet.ValidDays = setResult.DefaultValidDays;
            return printSet;
        }


        /// <summary>
        /// 获取团体打印参数设置
        /// </summary>
        /// <param name="agencyTypeId"></param>
        /// <param name="agencyId"></param>
        /// <returns></returns>
        private async Task<PrintSet> GetGroupPrintSet(int agencyTypeId, int agencyId)
        {
            var printSet = new PrintSet();
            printSet.PrintPriceType = (PrintPriceType)int.Parse(await _settingManager.GetSettingValueAsync(TicketSetting.GroupPriceSet));
            var defaultResult = await _defaultPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyTypeId == agencyTypeId) ?? new DefaultPrintSet();
            printSet.ValidDays = defaultResult.DefaultValidDays;

            //获取代理商单独配置项
            var groupResult = await _agencyPrintSetRepository.FirstOrDefaultAsync(p => p.AgencyId == agencyId);
            if (groupResult == null)
                return printSet;

            //如果有团体配置，覆盖默认的打印价格类型和有效期配置
            printSet.PrintPriceType = groupResult.PrintPriceType ?? printSet.PrintPriceType;
            printSet.ValidDays = groupResult.ValidDays ?? printSet.ValidDays;
            printSet.IsPrintAgencyName = groupResult.IsPrintAgencyName;
            printSet.IsPrintPrice = groupResult.IsPrintPrice;

            return printSet;
        }





    }

}

