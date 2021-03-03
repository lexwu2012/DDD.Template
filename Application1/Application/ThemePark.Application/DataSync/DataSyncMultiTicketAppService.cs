using System;
using ThemePark.Application.DataSync.Dto;
using System.Threading.Tasks;
using Abp.Auditing;
using ThemePark.Application.DataSync.Interfaces;
using Abp.Domain.Repositories;
using ThemePark.Core.ParkSale;
using ThemePark.Core.MultiTicket;
using ThemePark.Core.InPark;
using ThemePark.Core.AgentTicket;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.FaceClient;
using static ThemePark.Application.VerifyTicket.Finger.FingerCache;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Dependency;
using ThemePark.Application.VerifyTicket.Finger;
using ThemePark.Infrastructure.Application;
using ThemePark.Common;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 套票同步
    /// </summary>
    public class DataSyncMultiTicketAppService : ThemeParkAppServiceBase, IDataSyncMultiTicketAppService
    {
        private readonly IRepository<NonGroupTicket, string> _nonGroupTicketRepository;
        private readonly IRepository<GroupTicket, string> _groupTicketRepository;
        private readonly IRepository<TOTicket, string> _toTicketRepository;
        private readonly IRepository<MultiTicketEnterEnroll, long> _multiTicketEnterEnrollRepository;
        private readonly IRepository<TicketInPark, long> _ticketInParkRepository;
        private readonly ICheckTicketManager _checkTicketManager;
        private readonly ITOTicketDomainService _toTicketDomainService;

        /// <summary>
        /// 
        /// </summary>
        public DataSyncMultiTicketAppService(IRepository<NonGroupTicket, string> nonGroupTicketRepository
            , IRepository<MultiTicketEnterEnroll, long> multiTicketEnterEnrollRepository
            , IRepository<TicketInPark, long> ticketInParkRepository
            , IRepository<GroupTicket, string> groupTicketRepository
            , IRepository<TOTicket, string> toTicketRepository
            , ICheckTicketManager checkTicketManager, ITOTicketDomainService toTicketDomainService)
        {
            _nonGroupTicketRepository = nonGroupTicketRepository;
            _multiTicketEnterEnrollRepository = multiTicketEnterEnrollRepository;
            _ticketInParkRepository = ticketInParkRepository;
            _groupTicketRepository = groupTicketRepository;
            _toTicketRepository = toTicketRepository;
            _checkTicketManager = checkTicketManager;
            _toTicketDomainService = toTicketDomainService;
        }

        /// <summary>
        /// 取消套票
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketCancel(MultiTicketCancelDto dto)
        {
            // 移除缓存
            var ticketCheckData =
                await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(dto.Barcode);

            if (ticketCheckData != null)
            {
                _checkTicketManager.GetTicketCheckDataCache().Remove(dto.Barcode);
            }

            if (dto.TicketCategory == TicketCategory.NonGroupTicket)
            {
                var multiTicket = await _nonGroupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket != null)
                {
                    multiTicket.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _nonGroupTicketRepository.UpdateAsync(multiTicket);
                    return Result.Ok();
                }
            }
            else if (dto.TicketCategory == TicketCategory.GroupTicket)
            {
                var multiTicket2 = await _groupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket2 != null)
                {
                    multiTicket2.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _groupTicketRepository.UpdateAsync(multiTicket2);
                    return Result.Ok();
                }
            }
            else
            {
                var toTicket = await _toTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (toTicket != null)
                {
                    toTicket.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _toTicketRepository.UpdateAsync(toTicket);
                    return Result.Ok();
                }

            }

            return Result.FromCode(ResultCode.NoRecord);
        }

        /// <summary>
        /// 同步取消套票（客户端退款用）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketRefund(MultiTicketCancelDto dto)
        {
            // 移除缓存
            var ticketCheckData =
                await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(dto.Barcode);

            if (ticketCheckData != null)
            {
                _checkTicketManager.GetTicketCheckDataCache().Remove(dto.Barcode);
            }

            if (dto.TicketCategory == TicketCategory.OtherNonGroupTicket)
            {
                var othertiTicket = await _nonGroupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (othertiTicket.TicketSaleStatus != TicketSaleStatus.Valid)
                {
                    return Result.FromError("票的当前状态为:" + othertiTicket.TicketSaleStatus.DisplayName() + "，不允许退");
                }
                if (othertiTicket.InparkCounts > 0)
                {
                    return Result.FromError("票已使用，不允许退");
                }

                othertiTicket.TicketSaleStatus = TicketSaleStatus.Refund;
                await _nonGroupTicketRepository.UpdateAsync(othertiTicket);
                return Result.Ok();
            }

            //先判断是否已经登记 已登记 则不让取消
            var multiTicketRe = await _multiTicketEnterEnrollRepository.FirstOrDefaultAsync(p => p.Barcode == dto.Barcode);
            if (multiTicketRe != null)
            {
                return Result.FromError("此套票已使用不允许取消");
            }

            if (dto.TicketCategory == TicketCategory.NonGroupTicket)
            {
                var multiTicket = await _nonGroupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket != null)
                {
                    multiTicket.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _nonGroupTicketRepository.UpdateAsync(multiTicket);
                    return Result.Ok();
                }
            }
            else if (dto.TicketCategory == TicketCategory.GroupTicket)
            {
                var multiTicket2 = await _groupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket2 != null)
                {
                    multiTicket2.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _groupTicketRepository.UpdateAsync(multiTicket2);
                    return Result.Ok();
                }
            }
            else
            {
                var toTicket = await _toTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (toTicket != null)
                {
                    toTicket.TicketSaleStatus = TicketSaleStatus.Refund;
                    await _toTicketRepository.UpdateAsync(toTicket);
                    return Result.Ok();
                }
            }
            return Result.FromError("没有该票类");
        }

        /// <summary>
        /// 套票指纹登记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketEnroll(MultiTicketEnrollDto dto)
        {
            //先判断是否已经登记
            var multiTicket = await _multiTicketEnterEnrollRepository.FirstOrDefaultAsync(p => p.Barcode == dto.Barcode);
            if (multiTicket != null)
            {
                multiTicket.Customer.FpImage1 = dto.Finger;
                await _multiTicketEnterEnrollRepository.UpdateAsync(multiTicket);
                return Result.Ok();
            }

            multiTicket = new MultiTicketEnterEnroll
            {
                Barcode = dto.Barcode,
                Customer = new Customer()
                {
                    Barcode = dto.Barcode,
                    FpImage1 = dto.Finger
                },
                ParkId = dto.FromParkid,
                State = MultiTicketEnterEnrollState.Registered,
                TerminalId = dto.TerminalId
            };

            await _multiTicketEnterEnrollRepository.InsertAndGetIdAsync(multiTicket);

            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            listFinger.Add(new FingerDataItem { EnrollId = multiTicket.Id, FingerData = dto.Finger });

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.Barcode, listFinger, TimeSpan.FromDays(7));


            return Result.Ok();
        }

        /// <summary>
        /// 套票照片登记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketEnrollPhoto(MultiTicketEnrollDto dto)
        {
            var feature = new byte[512];
            if (FaceApi.GetFeatureAndCheckImageFormat(dto.Photo, feature) != 1)
            {
                return Result.FromError("人脸特征提取失败");
            }

            //先判断是否已经登记
            var multiTicket = await _multiTicketEnterEnrollRepository.GetAllIncluding(o => o.Customer)
                .Where(o => o.Barcode == dto.Barcode).FirstOrDefaultAsync();
            if (multiTicket != null)
            {
                multiTicket.Customer.Photo = dto.Photo;
                multiTicket.Customer.PhotoFeature = feature;
                await _multiTicketEnterEnrollRepository.UpdateAsync(multiTicket);
            }
            else
            {
                multiTicket = new MultiTicketEnterEnroll
                {
                    Barcode = dto.Barcode,
                    Customer = new Customer()
                    {
                        Barcode = dto.Barcode,
                        Photo = dto.Photo,
                        PhotoFeature = feature
                    },
                    ParkId = dto.FromParkid,
                    State = MultiTicketEnterEnrollState.Registered,
                    TerminalId = dto.TerminalId
                };

                await _toTicketDomainService.BindFaceInfoAsync(dto.Barcode, multiTicket);
            }

            FaceApi.AddUser(FaceCatalog.Ticket, long.Parse(multiTicket.Barcode), feature);

            return Result.Ok();
        }

        /// <summary>
        /// 套票入园
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketInpark(MultiTicketInparkDto dto)
        {
            // 移除缓存
            var ticketCheckData =
                await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(dto.Barcode);

            if (ticketCheckData != null)
            {
                _checkTicketManager.GetTicketCheckDataCache().Remove(dto.Barcode);
            }

            var ticketClassId = 0;

            // 入园信息，日期 公园ID 人数，如: 2017-3-15 41 3, 
            string inParkText = DateTime.Today.ToString("yyyy-MM-dd")
                + " " + dto.FromParkid + " " + dto.Persons + ",";

            //散客
            if (dto.TicketClassType == TicketCategory.NonGroupTicket)
            {
                var multiTicket = await _nonGroupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket == null)
                {
                    return Result.FromCode(ResultCode.NoRecord, "套票不存在");
                }

                ticketClassId = multiTicket.ParkSaleTicketClass.TicketClassId;
                multiTicket.TicketSaleStatus = dto.TicketSaleStatus;
                multiTicket.InparkCounts = multiTicket.InparkCounts + dto.Persons;
                multiTicket.Remark = (multiTicket.Remark == null ? "" : multiTicket.Remark) + inParkText;
                await _nonGroupTicketRepository.UpdateAsync(multiTicket);

            }
            else if (dto.TicketClassType == TicketCategory.GroupTicket)
            {
                //团体
                var multiTicket = await _groupTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket == null)
                {
                    return Result.FromCode(ResultCode.NoRecord, "套票不存在");
                }

                ticketClassId = multiTicket.ParkSaleTicketClass.TicketClassId;
                multiTicket.TicketSaleStatus = dto.TicketSaleStatus;
                multiTicket.InparkCounts = multiTicket.InparkCounts + dto.Persons;
                multiTicket.Remark = (multiTicket.Remark == null ? "" : multiTicket.Remark) + inParkText;
                await _groupTicketRepository.UpdateAsync(multiTicket);
            }
            else
            {
                //订单票
                var multiTicket = await _toTicketRepository.FirstOrDefaultAsync(p => p.Id == dto.Barcode);
                if (multiTicket == null)
                {
                    return Result.FromCode(ResultCode.NoRecord, "套票不存在");
                }

                ticketClassId = multiTicket.AgencySaleTicketClass.ParkSaleTicketClass.TicketClassId;
                multiTicket.TicketSaleStatus = dto.TicketSaleStatus;
                multiTicket.InparkCounts = multiTicket.InparkCounts + dto.Persons;
                multiTicket.Remark = (multiTicket.Remark == null ? "" : multiTicket.Remark) + inParkText;
                await _toTicketRepository.UpdateAsync(multiTicket);
            }

            var entity = new TicketInPark()
            {
                ParkId = dto.FromParkid,
                Barcode = dto.Barcode,
                Qty = dto.Persons,
                TerminalId = dto.TerminalId,
                TicketClassId = ticketClassId,
                CreationTime = dto.InparkTime,
                Remark = dto.Remark
            };
            await _ticketInParkRepository.InsertAsync(entity);

            return Result.Ok();
        }

        /// <summary>
        /// 从人脸服务器移除套票绑定的照片
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>Result.</returns>
        public Result MultiTicketPhotoRemove(MultiTicketPhotoRemoveDto dto)
        {
            FaceApi.RemoveUser(FaceCatalog.Ticket, long.Parse(dto.TicketId));

            return Result.Ok();
        }

        /// <summary>
        /// 套票同步
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MultiTicketSend(MultiTicketSendDto dto)
        {
            //散客
            if (dto.TicketClassType == TicketCategory.NonGroupTicket)
            {
                var entity = new NonGroupTicket
                {
                    TicketSaleStatus = TicketSaleStatus.Valid,
                    Id = dto.Barcode,
                    Amount = dto.Amount,
                    InparkCounts = dto.InparkCounts,
                    ParkId = dto.FromParkid,
                    ParkSaleTicketClassId = dto.ParkSaleTicketClassId,
                    Price = dto.Price,
                    Qty = dto.Qty,
                    SalePrice = dto.SalePrice,
                    TerminalId = dto.TerminalId,
                    ValidDays = dto.ValidDays,
                    ValidStartDate = dto.ValidStartDate,
                    TradeInfoId = dto.TradeInfoId,
                    CreatorUserId = dto.CreatorUserId,
                    SyncTicketType = SyncTicketType.MultiTicket,
                    InvoiceId = dto.InvoiceId,
                    CustomerId = dto.CustomerId,
                    CreationTime = dto.CreationTime,
                    LastModificationTime = dto.LastModificationTime,
                    LastModifierUserId = dto.LastModifierUserId
                };
                await _nonGroupTicketRepository.InsertAndGetIdAsync(entity);
            }
            else if (dto.TicketClassType == TicketCategory.GroupTicket)
            {
                var entity = new GroupTicket
                {
                    TicketSaleStatus = TicketSaleStatus.Valid,
                    Id = dto.Barcode,
                    Amount = dto.Amount,
                    InparkCounts = dto.InparkCounts,
                    ParkId = dto.FromParkid,
                    ParkSaleTicketClassId = dto.ParkSaleTicketClassId,
                    AgencySaleTicketClassId = dto.AgencySaleTicketClassId,
                    Price = dto.Price,
                    Qty = dto.Qty,
                    SalePrice = dto.SalePrice,
                    TerminalId = dto.TerminalId,
                    ValidDays = dto.ValidDays,
                    ValidStartDate = dto.ValidStartDate,
                    TradeInfoId = dto.TradeInfoId,
                    AgencyId = dto.AgencyId,
                    GroupTypeId = dto.GroupTypeId,
                    CreatorUserId = dto.CreatorUserId,
                    SyncTicketType = SyncTicketType.MultiTicket,
                    CreationTime = dto.CreationTime,
                    InvoiceId = dto.InvoiceId,
                    LastModificationTime = dto.LastModificationTime,
                    LastModifierUserId = dto.LastModifierUserId,
                    TOBodyId = dto.TOBodyId,
                    TOHeaderId = dto.TOHeaderId
                };

                await _groupTicketRepository.InsertAndGetIdAsync(entity);
            }
            else
            {
                var entity = new TOTicket
                {
                    TicketSaleStatus = TicketSaleStatus.Valid,
                    Id = dto.Barcode,
                    Amount = dto.Amount,
                    ParkId = dto.FromParkid,
                    AgencySaleTicketClassId = dto.AgencySaleTicketClassId,
                    Price = dto.Price,
                    Qty = dto.Qty,
                    TerminalId = dto.TerminalId,
                    SalePrice = dto.SalePrice,
                    ValidStartDate = dto.ValidStartDate,
                    InparkCounts = dto.InparkCounts,
                    ValidDays = dto.ValidDays,
                    TOVoucherId = dto.TOVoucherId,
                    CreatorUserId = dto.CreatorUserId,
                    SyncTicketType = SyncTicketType.MultiTicket,
                    ParkSettlementPrice = dto.ParkSettlementPrice,
                    SettlementPrice = dto.SettlementPrice,
                    TicketFormEnum = TicketFormEnum.PaperTicket,

                    CreationTime = dto.CreationTime,
                    InvoiceId = dto.InvoiceId,
                    LastModificationTime = dto.LastModificationTime,
                    LastModifierUserId = dto.LastModifierUserId
                };

                await _toTicketRepository.InsertAndGetIdAsync(entity);
            }
            return Result.Ok();
        }
    }
}
