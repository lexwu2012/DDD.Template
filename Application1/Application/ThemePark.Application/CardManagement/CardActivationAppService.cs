using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.CardManagement.Dto;
using ThemePark.Application.CardManagement.Interfaces;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.CardManagement
{
    /// <summary>
    /// IC卡初始化应用服务
    /// </summary>
    public class CardActivationAppService : ThemeParkAppServiceBase, ICardActivationAppService
    {
        #region Fields
        private readonly IRepository<IcBasicInfo, long> _icBasicInfoRepository;
        private readonly IRepository<VIPCard, long> _vIPCardRepository;
        private readonly IRepository<ICKind> _iCKindRepository;
        private readonly IRepository<ManageCardInfo> _manageCardInfoRepository;
        private readonly IRepository<TicketClass> _ticketClassRepository;

        #endregion

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        public CardActivationAppService(IRepository<IcBasicInfo, long> icBasicInfoRepository, IRepository<VIPCard, long> vIPCardRepository, IRepository<ICKind> iCKindRepository,
            IRepository<ManageCardInfo> manageCardInfoRepository
            , IRepository<TicketClass> ticketClassRepository)
        {
            _icBasicInfoRepository = icBasicInfoRepository;
            _vIPCardRepository = vIPCardRepository;
            _iCKindRepository = iCKindRepository;
            _manageCardInfoRepository = manageCardInfoRepository;
            _ticketClassRepository = ticketClassRepository;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 添加IC卡基础信息记录
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddIcBasicInfoAsync(int parkId, IList<IcBasicInfoInput> inputs)
        {
            if (inputs.Count != 0)
            {
                ////检查有没重复的卡
                //foreach (var input in inputs)
                //{
                //    var existed = _icBasicInfoRepository.GetAll().Any(m => m.IcNo == input.IcNo);

                //    if (existed)
                //        return Result.FromCode(ResultCode.DuplicateRecord);
                //}

                //添加IC卡信息，给年卡添加记录
                foreach (var input in inputs)
                {
                    var entity = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.IcNo == input.IcNo);
                    if (entity != null)
                    {
                        if (entity.ParkId != parkId)
                        {
                            return Result.FromError("只能在第一次初始化的公园重新初始化");
                        }

                        var vipCard = await _vIPCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfoId == entity.Id);
                        if (vipCard != null && vipCard.State != VipCardStateType.Init)
                        {
                            return Result.FromError("已售、已激活的卡不允许重新初始化");
                        }

                        var ticketClassOld = await _ticketClassRepository.FirstOrDefaultAsync(p => p.Id == vipCard.TicketClassId);

                        entity.ParkId = parkId;
                        entity.CardNo = input.CardNo;
                        entity.KindId = input.KindId;

                        var cardDisplayName = EnumExtensions.DisplayName(KindOfIcCard.YearCard);

                        //查询出年卡类型记录
                        var icKind = _iCKindRepository.AsNoTracking().Where(m => m.KindName.Equals(cardDisplayName)).FirstOrDefault();

                        if (icKind == null)
                            return Result.FromCode(ResultCode.MissEssentialData, "IC卡类型没在数据库配置!");
                        //如果是年卡，给年卡表添加记录
                        if (input.KindId == icKind.Id)
                        {
                            //如果原初始化 不是年卡 则插入年卡数据
                            if (vipCard == null)
                            {
                                vipCard = new VIPCard
                                {
                                    ParkId = parkId,
                                    IcBasicInfoId = entity.Id,
                                    TicketClassId = input.TicketClassId,
                                    State = VipCardStateType.Init
                                };
                            }

                            vipCard.ParkId = parkId;
                            vipCard.IcBasicInfoId = entity.Id;
                            vipCard.TicketClassId = input.TicketClassId;

                            await _icBasicInfoRepository.UpdateAsync(entity);
                            var vicCardId = await _vIPCardRepository.InsertOrUpdateAndGetIdAsync(vipCard);

                            //多园年卡同步
                            var ticketClass = await _ticketClassRepository.FirstOrDefaultAsync(p => p.Id == input.TicketClassId && p.ParkId == parkId);
                            if (ticketClass.TicketClassMode == TicketClassMode.MultiYearCard)
                            {
                                await MulYearCardInitDataSync(vipCard, entity, entity.Id, parkId, vicCardId, ticketClass.InParkIdFilter, vipCard.CreatorUserId);
                            }
                            else if(ticketClassOld.TicketClassMode == TicketClassMode.MultiYearCard)
                            {
                                //多园年卡修改成单园年卡 则删除其它公园数据
                                var dto = new MulYearCardInitDto();
                                dto.isDelVipCard = true;
                                dto.VicCardId = vipCard.Id;
                                dto.IcBasicInfoId = entity.Id;
                                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                                var syncInput = new DataSyncInput
                                {
                                    SyncType = DataSyncType.MulYearCardInit,
                                    SyncData = JsonConvert.SerializeObject(dto)
                                };

                                var otherParkIds = ticketClassOld.InParkIdFilter.Trim().Split(',');
                                foreach (var parkid in otherParkIds)
                                {
                                    if (parkid != parkId.ToString())
                                    {
                                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                                    }
                                }
                            }


                        }
                        else
                        {
                            //如果不是年卡则删除初始化错误的信息
                            
                            //多园年卡同步删除
                            if (ticketClassOld.TicketClassMode == TicketClassMode.MultiYearCard)
                            {
                                var dto = new MulYearCardInitDto();
                                dto.isDelVipCard = true;
                                dto.VicCardId = vipCard.Id;
                                dto.IcBasicInfoId = entity.Id;
                                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                                var syncInput = new DataSyncInput
                                {
                                    SyncType = DataSyncType.MulYearCardInit,
                                    SyncData = JsonConvert.SerializeObject(dto)
                                };

                                var otherParkIds = ticketClassOld.InParkIdFilter.Trim().Split(',');
                                foreach (var parkid in otherParkIds)
                                {
                                    if (parkid != parkId.ToString())
                                    {
                                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                                    }
                                }
                            }

                            if (vipCard != null)
                            {
                                await _vIPCardRepository.DeleteAsync(vipCard);
                            }

                        }
                    }
                    else
                    {
                        entity = input.MapTo<IcBasicInfo>();
                        entity.ParkId = parkId;

                        var icBasicInfoId = await _icBasicInfoRepository.InsertAndGetIdAsync(entity);
                        var cardDisplayName = EnumExtensions.DisplayName(KindOfIcCard.YearCard);

                        //查询出年卡类型记录
                        var icKind = _iCKindRepository.AsNoTracking().Where(m => m.KindName.Equals(cardDisplayName)).FirstOrDefault();

                        if (icKind == null)
                            return Result.FromCode(ResultCode.MissEssentialData, "IC卡类型没在数据库配置!");
                        //如果是年卡，给年卡表添加记录
                        if (input.KindId == icKind.Id)
                        {
                            var vipCard = new VIPCard
                            {
                                ParkId = parkId,
                                IcBasicInfoId = icBasicInfoId,
                                TicketClassId = input.TicketClassId,
                                State = VipCardStateType.Init
                            };

                            var vicCardId = await _vIPCardRepository.InsertAndGetIdAsync(vipCard);

                            //多园年卡同步
                            var ticketClass = await _ticketClassRepository.FirstOrDefaultAsync(p => p.Id == input.TicketClassId && p.ParkId == parkId);
                            if (ticketClass.TicketClassMode == TicketClassMode.MultiYearCard)
                            {
                                await MulYearCardInitDataSync(vipCard, entity, icBasicInfoId, parkId, vicCardId, ticketClass.InParkIdFilter, vipCard.CreatorUserId);
                            }

                        }
                    }                 
                }

                return Result.Ok();
            }
            return Result.FromCode(ResultCode.InvalidData);
        }


        /// <summary>
        /// 多园年卡初始化同步
        /// </summary>
        /// <returns></returns>
        private async Task MulYearCardInitDataSync(VIPCard vipCard, IcBasicInfo icBasicInfo,long icBasicInfoId, int parkId,long vicCardId,string inParkIdFilter,long ? creatorUserId)
        {
            var dto = new MulYearCardInitDto();
            dto.isDelVipCard = false;
            dto.CardNo = icBasicInfo.CardNo;
            dto.IcBasicInfoId = icBasicInfoId;
            dto.IcNo = icBasicInfo.IcNo;
            dto.KindId = icBasicInfo.KindId;
            dto.ParkId = parkId;
            dto.TicketClassId = vipCard.TicketClassId;
            dto.VicCardId = vicCardId;
            dto.CreatorUserId = creatorUserId;

            var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
            var syncInput = new DataSyncInput
            {
                SyncType = DataSyncType.MulYearCardInit,
                SyncData = JsonConvert.SerializeObject(dto)
            };

            var otherParkIds = inParkIdFilter.Trim().Split(',');
            foreach (var parkid in otherParkIds)
            {
                if (parkid != parkId.ToString())
                {
                    dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                }
            }
        }


        /// <summary>
        /// 添加管理卡领用记录
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddManageCardRequisitionAsync(CardRequisitionInput input)
        {
            if (input != null)
            {
                var icKind = await _icBasicInfoRepository.FirstOrDefaultAsync(m => m.Id == input.IcBasicInfoId);
                //年卡不能领用（类型为1是年卡）
                if(icKind.KindId.Equals(1))
                    return Result.FromError("年卡不允许被领用");

                var existed = _manageCardInfoRepository.AsNoTracking().Any(m => m.IcBasicInfoId == input.IcBasicInfoId);

                if(existed)
                    return Result.FromError("此卡已经被领用！");

                var manageCardInfo = input.MapTo<ManageCardInfo>();

                await _manageCardInfoRepository.InsertAndGetIdAsync(manageCardInfo);

                return Result.Ok();

            }
            return Result.FromCode(ResultCode.InvalidData);
        }

        /// <summary>
        /// 根据条件查询管理卡领用信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetManageCardRequisitionAsync<TDto>(IQuery<ManageCardInfo> query)
        {
            return await _manageCardInfoRepository.AsNoTracking().FirstOrDefaultAsync<ManageCardInfo, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取IC卡基础信息记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetIcBasicInfoAsync<TDto>(IQuery<IcBasicInfo> query)
        {
            return await _icBasicInfoRepository.AsNoTracking().FirstOrDefaultAsync<IcBasicInfo, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取IC卡基础信息记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetIcBasicInfoListAsync<TDto>(IQuery<IcBasicInfo> query)
        {
            return await _icBasicInfoRepository.AsNoTracking().ToListAsync<IcBasicInfo, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取IC卡基础信息记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetIcBasicInfoListByCriteriaAsync<TDto>(IQuery<IcBasicInfo> query)
        {
            return await _icBasicInfoRepository.AsNoTracking().ToListAsync<IcBasicInfo, TDto>(query);
        }

        /// <summary>
        /// 获取管理卡和年卡分页数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageResult<IcBasicInfoDetailDto>> GetAllByPageAsync(AccurateSearchModel query = null)
        {
            if (query != null)
            {
                IQueryable<IcBasicInfoDetailDto> result;

                //简单查询（只根据IcNo查询）
                if (!query.IsAccurateSearch)
                {
                    //管理卡
                    result = _icBasicInfoRepository.AsNoTracking().Select(
                        m => new IcBasicInfoDetailDto
                        {
                            CardNo = m.CardNo,
                            IcNo = m.IcNo,
                            Id = m.Id,
                            CreationTime = m.CreationTime,
                            KindName = m.KindICKind.KindName,
                            CreatorUserId = m.CreatorUserId.Value
                        });
                }
                else
                {
                    //精确查询
                    //查询年卡
                    if (query.KindId.Equals(1))
                    {
                        //年卡
                        result = _icBasicInfoRepository.GetAll()
                       .Join(_vIPCardRepository.GetAll(), icBasicInfo => icBasicInfo.Id, vipCard => vipCard.IcBasicInfoId,
                       (icBasicInfo, vipCard) => new IcBasicInfoDetailDto()
                       {
                           CardNo = icBasicInfo.CardNo,
                           IcNo = icBasicInfo.IcNo,
                           Id = icBasicInfo.Id,
                           CreationTime = vipCard.CreationTime,
                           KindName = icBasicInfo.KindICKind.KindName,
                           VipCardId = vipCard.Id,
                           TicketClassId = vipCard.TicketClassId,
                           CreatorUserId = icBasicInfo.CreatorUserId.Value
                       });

                        if (query.TicketClassId != null)
                        {
                            result = result.Where(o => o.TicketClassId.ToString() == query.TicketClassId);
                        }

                    }
                    else
                    {
                        //管理卡
                        result = _icBasicInfoRepository.AsNoTracking().Where(m => m.KindId == query.KindId).Select(
                            m => new IcBasicInfoDetailDto
                            {
                                CardNo = m.CardNo,
                                IcNo = m.IcNo,
                                Id = m.Id,
                                CreationTime = m.CreationTime,
                                KindName = m.KindICKind.KindName,
                                CreatorUserId = m.CreatorUserId.Value
                            });
                        //result = _icBasicInfoRepository.AsNoTracking()
                        //    .GroupJoin(_manageCardInfoRepository.GetAll(), icBasicInfo => icBasicInfo.Id, manageCardInfo => manageCardInfo.IcBasicInfoId,
                        //    (icBasicInfo, manageCardInfo) => new IcBasicInfoDetailDto
                        //    {
                        //        CardNo = icBasicInfo.CardNo,
                        //        IcNo = icBasicInfo.IcNo,
                        //        Id = icBasicInfo.Id,
                        //        CreationTime = icBasicInfo.CreationTime,
                        //        KindName = icBasicInfo.KindICKind.KindName,
                        //        CreatorUserId = icBasicInfo.CreatorUserId.Value
                        //        //RequisitionTime = manageCardInfo
                        //    });

                    }

                }

                if (!string.IsNullOrWhiteSpace(query.IcNo))
                {
                    result = result.Where(o => o.IcNo == query.IcNo);
                }

                if (!string.IsNullOrWhiteSpace(query.CardNo))
                {
                    result = result.Where(o => o.CardNo == query.CardNo);
                }

                return await result.ToPageResultAsync(query);
            }

            return null;
        }

        #endregion
    }
}
