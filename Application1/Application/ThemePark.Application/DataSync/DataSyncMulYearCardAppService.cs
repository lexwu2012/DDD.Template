using Abp.Domain.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Core.CardManage;
using ThemePark.Core.CardManage.Repositories;
using ThemePark.FaceClient;
using ThemePark.Infrastructure.Application;
using System.Collections.Generic;
using static ThemePark.Application.VerifyTicket.Finger.FingerCache;
using Abp.Dependency;
using ThemePark.Application.VerifyTicket.Finger;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using Abp.Domain.Uow;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 年卡同步
    /// </summary>
    public class DataSyncMulYearCardAppService : ThemeParkAppServiceBase, IDataSyncMulYearCardAppService
    {

        private readonly IRepository<IcBasicInfo, long> _icBasicInfoRepository;
        private readonly IRepository<VIPCard, long> _vIPCardRepository;
        private readonly IRepository<VIPVoucher, long> _vIPVoucherRepository;
        private readonly IRepository<VipVoucherReturn, long> _vipVoucherReturnRepository;
        private readonly IRepository<VipCardReturn, long> _vipCardReturnRepository;
        private readonly ICustomerRepository _parkCustomerRepository;
        private readonly IUserICRepository _parkUserIcRepository;
        private readonly IRepository<IcoperDetail, long> _parkIcoperdetailRepository;
        private readonly IRepository<FillCard, long> _parkFillCardRepository;

        /// <summary>
        /// 
        /// </summary>
        public DataSyncMulYearCardAppService(IRepository<IcBasicInfo, long> icBasicInfoRepository
            , IRepository<VIPCard, long> vIPCardRepository
            , ICustomerRepository parkCustomerRepository
            , IUserICRepository parkUserIcRepository
            , IRepository<IcoperDetail, long> parkIcoperdetailRepository
            , IRepository<FillCard, long> parkFillCardRepository
            , IRepository<VIPVoucher, long> vIPVoucherRepository
            , IRepository<VipVoucherReturn, long> vipVoucherReturnRepository
            , IRepository<VipCardReturn, long> vipCardReturnRepository)
        {
            _icBasicInfoRepository = icBasicInfoRepository;
            _vIPCardRepository = vIPCardRepository;
            _parkCustomerRepository = parkCustomerRepository;
            _parkUserIcRepository = parkUserIcRepository;
            _parkIcoperdetailRepository = parkIcoperdetailRepository;
            _parkFillCardRepository = parkFillCardRepository;
            _vIPVoucherRepository = vIPVoucherRepository;
            _vipVoucherReturnRepository = vipVoucherReturnRepository;
            _vipCardReturnRepository = vipCardReturnRepository;
        }

        /// <summary>
        /// 年卡激活
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardActive(MulYearCardActiveDto dto)
        {
            //电子年卡 
            if (dto.IsEcard)
            {
                var entity = new IcBasicInfo
                {
                    CardNo = dto.CardNo,
                    ECardID = dto.ECardID,
                    IcNo = dto.IcNo,
                    Id = dto.IcBasicInfoId,
                    KindId = dto.KindId,
                    ParkId = dto.ParkId,
                    CreatorUserId = dto.CreatorUserId
                };
                await _icBasicInfoRepository.InsertAndGetIdAsync(entity);

                var entity2 = new VIPCard
                {
                    Id = dto.VipCardId,
                    ParkId = dto.ParkId,
                    IcBasicInfoId = dto.IcBasicInfoId,
                    TicketClassId = dto.TicketClassId,
                    State = VipCardStateType.Actived,
                    CreatorUserId = dto.CreatorUserId,
                    ParkSaleTicketClassId = dto.ParkSaleTicketClassId,
                    ValidDateBegin = dto.ValidDateBegin,
                    ValidDateEnd = dto.ValidDateEnd,
                    ActiveUser = dto.ActiveUser,
                    ActiveTime = dto.ActiveTime,
                    TerminalId = dto.TerminalId
                };
                await _vIPCardRepository.InsertAndGetIdAsync(entity2);
            }
            else
            {
                var vipCardEntity = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.VipCardId);
                var icBasicInfoEntity = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == vipCardEntity.IcBasicInfoId);
                if (vipCardEntity == null || icBasicInfoEntity == null)
                    return Result.FromCode(ResultCode.NoRecord);
                else
                {
                    vipCardEntity.ParkSaleTicketClassId = dto.ParkSaleTicketClassId;
                    vipCardEntity.ValidDateBegin = dto.ValidDateBegin;
                    vipCardEntity.ValidDateEnd = dto.ValidDateEnd;
                    vipCardEntity.State = VipCardStateType.Actived;
                    vipCardEntity.ActiveUser = dto.ActiveUser;
                    vipCardEntity.ActiveTime = dto.ActiveTime;
                    vipCardEntity.TerminalId = dto.TerminalId;
                    await _vIPCardRepository.UpdateAsync(vipCardEntity);//年卡激活不改PARKID

                    icBasicInfoEntity.ECardID = dto.ECardID;
                    await _icBasicInfoRepository.UpdateAsync(icBasicInfoEntity);
                }
            }

            _parkCustomerRepository.AddRange(dto.Customers.MapTo<List<Customer>>());
            await CurrentUnitOfWork.SaveChangesAsync();

            var mulUserIcs = dto.UserICs.Select(o => new UserIC
            {
                CustomerId = o.CustomId,
                VIPCardId = o.VIPCardId,
                IcBasicInfoId = o.IcBasicInfoId,
                Id = o.Id,
                Remark = o.Remark
            }).ToList();

            _parkUserIcRepository.AddRange(mulUserIcs);
            await CurrentUnitOfWork.SaveChangesAsync();


            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            dto.Customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.Id, FingerData = o.Fp1 }));

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.IcNo.ToUpper(), listFinger, TimeSpan.MaxValue);

            var faceId = long.Parse(dto.IcNo, NumberStyles.AllowHexSpecifier);
            foreach (var customer in dto.Customers)
            {
                FaceApi.AddUser(FaceCatalog.VipCard, faceId, customer.PhotoFeature);
            }

            return Result.Ok();
        }


        /// <summary>
        /// 年卡补卡
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardFill(MulYearCardFillDto dto)
        {
            //优先处理缓存
            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(dto.FillCard.OldIcNo.ToUpper());
            if (listFinger == null)
            {
                listFinger = GetFingerData(dto.OldVIPCard.Id);
            }
            if (listFinger != null)
            {
                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.FillCard.NewIcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
            }

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(dto.FillCard.OldIcNo.ToUpper());

            var oldVipcard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.OldVIPCard.Id);
            var newVipcard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.NewVIPCard.Id);
            if (oldVipcard == null || newVipcard == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                oldVipcard.IcBasicInfoId = dto.OldVIPCard.IcBasicInfoId;
                newVipcard.IcBasicInfoId = dto.NewVIPCard.IcBasicInfoId;
                var oldIcBasicInfo = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == oldVipcard.IcBasicInfoId);
                var newIcBasicInfo = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == newVipcard.IcBasicInfoId);
                if (oldIcBasicInfo == null || newIcBasicInfo == null)
                    return Result.FromCode(ResultCode.NoRecord);
                {
                    newVipcard.IcBasicInfo.ECardID = oldIcBasicInfo.ECardID;
                    oldVipcard.IcBasicInfo.ECardID = newIcBasicInfo.ECardID;

                    //写操作记录
                    var icoperdetail = new IcoperDetail
                    {
                        Id = dto.IcoperDetail.Id,
                        VIPCardId = dto.IcoperDetail.Id,
                        ApplyName = dto.IcoperDetail.ApplyName,
                        ApplyPhone = dto.IcoperDetail.ApplyPhone,
                        ApplyPid = dto.IcoperDetail.ApplyPid,
                        State = IcoperDetailStateType.FillCard,
                        TerminalId = dto.IcoperDetail.TerminalId,
                        Remark = dto.IcoperDetail.Remark,
                        ParkId = dto.ParkId
                    };

                    //更新年卡信息
                    await _vIPCardRepository.UpdateAsync(oldVipcard);
                    await _vIPCardRepository.UpdateAsync(newVipcard);
                    //更新IC卡基本信息
                    await _icBasicInfoRepository.UpdateAsync(oldIcBasicInfo);
                    await _icBasicInfoRepository.UpdateAsync(newIcBasicInfo);
                    //更新操作记录
                    await _parkFillCardRepository.InsertAsync(dto.FillCard.MapTo<FillCard>());
                    await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);

                    return Result.Ok();
                }
            }
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
        /// 多园年卡初始化
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardInit(MulYearCardInitDto dto)
        {
            if (dto.isDelVipCard)
            {
                var vipCardEntity = _vIPCardRepository.FirstOrDefault(k => k.Id == dto.VicCardId);
                var icBasicInfoEntity = _icBasicInfoRepository.FirstOrDefault(k => k.Id == dto.IcBasicInfoId);
                if (vipCardEntity == null || icBasicInfoEntity == null)
                    return Result.FromCode(ResultCode.NoRecord);
                else
                {
                    await _vIPCardRepository.DeleteAsync(vipCardEntity);
                    await _icBasicInfoRepository.DeleteAsync(icBasicInfoEntity);
                    return Result.Ok();
                }
            }
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var entity = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == dto.IcBasicInfoId);
                if (entity == null)
                {
                    entity = new IcBasicInfo
                    {
                        CardNo = dto.CardNo,
                        IcNo = dto.IcNo,
                        Id = dto.IcBasicInfoId,
                        KindId = dto.KindId,
                        ParkId = dto.ParkId,
                        CreatorUserId = dto.CreatorUserId
                    };
                }
                else
                {
                    entity.CardNo = dto.CardNo;
                    entity.IcNo = dto.IcNo;
                    entity.KindId = dto.KindId;
                    entity.ParkId = dto.ParkId;
                    entity.IsDeleted = false;
                }
                await _icBasicInfoRepository.InsertOrUpdateAndGetIdAsync(entity);


                var entity2 = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.VicCardId);
                if (entity2 == null)
                {
                    entity2 = new VIPCard
                    {
                        Id = dto.VicCardId,
                        ParkId = dto.ParkId,
                        IcBasicInfoId = dto.IcBasicInfoId,
                        TicketClassId = dto.TicketClassId,
                        State = VipCardStateType.Init,
                        CreatorUserId = dto.CreatorUserId
                    };
                }
                else
                {
                    if (entity2.State != VipCardStateType.Init)
                    {
                        return Result.FromError("IC卡状态异常");
                    }

                    entity2.ParkId = dto.ParkId;
                    entity2.IcBasicInfoId = dto.IcBasicInfoId;
                    entity2.TicketClassId = dto.TicketClassId;
                    entity2.IsDeleted = false;
                }
                await _vIPCardRepository.InsertOrUpdateAndGetIdAsync(entity2);
            }
            return Result.Ok();
        }

        /// <summary>
        /// 年卡挂失
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardLoss(MulYearCardLossDto dto)
        {
            var vipcardEntity = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.VIPCard.Id);
            if (vipcardEntity == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                vipcardEntity.State = dto.VIPCard.State;
                vipcardEntity.LastModificationTime = dto.VIPCard.LastModificationTime;
                vipcardEntity.LastModifierUserId = dto.VIPCard.LastModifierUserId;

                //写操作记录
                var icoperdetail = new IcoperDetail
                {
                    Id = dto.IcoperDetail.Id,
                    VIPCardId = dto.IcoperDetail.Id,
                    ApplyName = dto.IcoperDetail.ApplyName,
                    ApplyPhone = dto.IcoperDetail.ApplyPhone,
                    ApplyPid = dto.IcoperDetail.ApplyPid,
                    State = dto.IcoperDetail.State,
                    TerminalId = dto.IcoperDetail.TerminalId,
                    Remark = dto.IcoperDetail.Remark,
                    ParkId = dto.ParkId
                };
                await _vIPCardRepository.UpdateAsync(vipcardEntity);
                await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);

                return Result.Ok();
            }
        }

        /// <summary>
        /// 年卡续卡
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardRenew(MulYearCardRenewDto dto)
        {
            if (dto.ChangeCard)
            {
                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(dto.FillCard.OldIcNo.ToUpper());
                if (listFinger == null)
                {
                    listFinger = GetFingerData(dto.OldVIPCard.Id);
                }
                if (listFinger != null)
                {
                    IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.FillCard.NewIcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
                }

                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(dto.FillCard.OldIcNo.ToUpper());

                var oldVipcard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.OldVIPCard.Id);
                var newVipcard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.NewVIPCard.Id);
                if (oldVipcard == null || newVipcard == null)
                    return Result.FromCode(ResultCode.NoRecord);
                else
                {
                    oldVipcard.IcBasicInfoId = dto.OldVIPCard.IcBasicInfoId;
                    oldVipcard.ValidDateEnd = dto.OldVIPCard.ValidDateEnd;
                    newVipcard.IcBasicInfoId = dto.NewVIPCard.IcBasicInfoId;
                    newVipcard.ValidDateEnd = dto.NewVIPCard.ValidDateEnd;

                    var oldIcBasicInfo = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == oldVipcard.IcBasicInfoId);
                    var newIcBasicInfo = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.Id == newVipcard.IcBasicInfoId);
                    if (oldIcBasicInfo == null || newIcBasicInfo == null)
                        return Result.FromCode(ResultCode.NoRecord);
                    else
                    {
                        oldVipcard.IcBasicInfo.ECardID = newIcBasicInfo.ECardID;
                        newVipcard.IcBasicInfo.ECardID = oldIcBasicInfo.ECardID;

                        //写操作记录
                        var icoperdetail = new IcoperDetail
                        {
                            Id = dto.IcoperDetail.Id,
                            VIPCardId = dto.IcoperDetail.Id,
                            ApplyName = dto.IcoperDetail.ApplyName,
                            ApplyPhone = dto.IcoperDetail.ApplyPhone,
                            ApplyPid = dto.IcoperDetail.ApplyPid,
                            State = IcoperDetailStateType.FillCard,
                            TerminalId = dto.IcoperDetail.TerminalId,
                            Remark = dto.IcoperDetail.Remark,
                            ParkId = dto.ParkId
                        };

                        //更新年卡信息
                        await _vIPCardRepository.UpdateAsync(oldVipcard);
                        await _vIPCardRepository.UpdateAsync(newVipcard);
                        //更新IC卡基本信息
                        await _icBasicInfoRepository.UpdateAsync(oldIcBasicInfo);
                        await _icBasicInfoRepository.UpdateAsync(newIcBasicInfo);
                        //添加操作记录
                        await _parkFillCardRepository.InsertAsync(dto.FillCard.MapTo<FillCard>());
                        await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);

                        return Result.Ok();
                    }
                }
            }
            else
            {
                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                listFinger = IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().GetOrDefault(dto.FillCard.OldIcNo.ToUpper());
                if (listFinger == null)
                {
                    listFinger = GetFingerData(dto.OldVIPCard.Id);
                }
                if (listFinger != null)
                {
                    IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.FillCard.OldIcNo.ToUpper(), listFinger, TimeSpan.MaxValue);
                }

                var oldVipcard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.OldVIPCard.Id);
                if (oldVipcard == null)
                    return Result.FromCode(ResultCode.NoRecord);
                else
                {
                    oldVipcard.ValidDateEnd = dto.OldVIPCard.ValidDateEnd;

                    //写操作记录
                    var icoperdetail = new IcoperDetail
                    {
                        Id = dto.IcoperDetail.Id,
                        VIPCardId = dto.IcoperDetail.Id,
                        ApplyName = dto.IcoperDetail.ApplyName,
                        ApplyPhone = dto.IcoperDetail.ApplyPhone,
                        ApplyPid = dto.IcoperDetail.ApplyPid,
                        State = IcoperDetailStateType.FillCard,
                        TerminalId = dto.IcoperDetail.TerminalId,
                        Remark = dto.IcoperDetail.Remark,
                        ParkId = dto.ParkId
                    };

                    await _parkFillCardRepository.InsertAsync(dto.FillCard.MapTo<FillCard>());
                    await _vIPCardRepository.UpdateAsync(oldVipcard);
                    await _parkIcoperdetailRepository.InsertAndGetIdAsync(icoperdetail);

                    return Result.Ok();
                }
            }
        }

        /// <summary>
        /// 年卡销售
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardSale(MulYearCardSaleDto dto)
        {
            var vipCard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.VipCardId);
            if (vipCard == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                vipCard.ParkSaleTicketClassId = dto.ParkSaleTicketClassId;
                vipCard.SaleUser = dto.SaleUser;
                vipCard.SaleTime = dto.SaleTime;
                vipCard.State = VipCardStateType.NotActive;
                vipCard.ParkId = dto.ParkId;

                vipCard.TerminalId = dto.TerminalId;
                vipCard.CreatorUserId = dto.CreatorUserId;
                vipCard.SalePrice = dto.SalePrice;
                vipCard.TicketClassId = dto.TicketClassId;

                await _vIPCardRepository.UpdateAsync(vipCard);

                return Result.Ok();
            }
        }

        [DisableAuditing]
        public async Task<Result> MulYearCardUnLoss(MulYearCardUnLossDto dto)
        {



            return Result.Ok();
        }

        /// <summary>
        /// 年卡用户信息修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardUpdate(MulYearCardUpdateDto dto)
        {
            foreach (var customer in dto.Customers)
            {
                await _parkCustomerRepository.UpdateAsync(customer.Id, p => Task.FromResult(customer.MapTo(p)));
            }

            var faceId = long.Parse(dto.IcNo, NumberStyles.AllowHexSpecifier);

            FaceApi.RemoveUser(FaceCatalog.VipCard, faceId);
            foreach (var customer in dto.Customers)
            {
                FaceApi.AddUser(FaceCatalog.VipCard, faceId, customer.PhotoFeature);
            }

            List<FingerDataItem> listFinger = new List<FingerDataItem>();
            dto.Customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.Id, FingerData = o.Fp1 }));

            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(dto.IcNo.ToUpper(), listFinger, TimeSpan.MaxValue);

            return Result.Ok();
        }

        /// <summary>
        /// 退凭证
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardVoucherReturn(MulYearCardVoucherReturnDto dto)
        {
            var vipvoucher = await _vIPVoucherRepository.FirstOrDefaultAsync(p => p.Id == dto.VipVoucherId);
            if (vipvoucher == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                vipvoucher.State = VipVoucherStateType.Cancel;
                vipvoucher.CreatorUserId = dto.CreatorUserId;

                var entity = new VipVoucherReturn();
                entity.ParkId = dto.ParkId;
                entity.Id = dto.Id;
                entity.VipVoucherId = dto.VipVoucherId;
                entity.OriginalTradeInfoId = dto.OriginalTradeInfoId;
                entity.Amount = dto.Amount;
                entity.TerminalId = dto.TerminalId;
                entity.CreatorUserId = dto.CreatorUserId;
                entity.ApplyName = dto.ApplyName;
                entity.ApplyPid = dto.ApplyPid;
                entity.ApplyPhone = dto.ApplyPhone;
                entity.Remark = dto.Remark;
                entity.TradeInfoId = dto.TradeInfoId;

                await _vIPVoucherRepository.UpdateAsync(vipvoucher);
                await _vipVoucherReturnRepository.InsertAsync(entity);

                return Result.Ok();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardVoucherSale(MulYearCardVoucherSaleDto dto)
        {
            var entity = new VIPVoucher
            {
                Id = dto.Id,
                State = VipVoucherStateType.NotActive,
                ParkSaleTicketClassId = dto.ParkSaleTicketClassId,
                CreationTime = DateTime.Now,
                SalePrice = dto.SalePrice,
                ValidDateBegin = DateTime.Now,
                ValidDateEnd = dto.ValidDateEnd,
                TerminalId = dto.TerminalId,
                Barcode = dto.Barcode,
                TradeInfoId = dto.TradeInfoId,
                Invoice = dto.Invoice,
                ParkId = dto.ParkId
            };

            await _vIPVoucherRepository.InsertAsync(entity);

            return Result.Ok();
        }

        /// <summary>
        /// 年卡退卡
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> MulYearCardReturn(MulYearCardReturnDto dto)
        {
            var vipCard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.Id == dto.VIPCard.Id);
            if (vipCard == null)
                return Result.FromCode(ResultCode.NoRecord);
            else
            {
                vipCard.LastModificationTime = vipCard.LastModificationTime;
                vipCard.State = VipCardStateType.Cancel;

                if (dto.VipCardReturn != null && dto.VipCardReturn.VipCardId > 0)
                {
                    await _vipCardReturnRepository.InsertAsync(dto.VipCardReturn.MapTo<VipCardReturn>());
                }

                if (dto.VipVoucherReturn != null && dto.VipVoucherReturn.Id > 0)
                {
                    await _vipVoucherReturnRepository.InsertAsync(dto.VipVoucherReturn.MapTo<VipVoucherReturn>());
                }

                await _vIPCardRepository.UpdateAsync(vipCard);

                IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Remove(vipCard.IcBasicInfo.IcNo.ToUpper());

                return Result.Ok();
            }
        }
    }
}
