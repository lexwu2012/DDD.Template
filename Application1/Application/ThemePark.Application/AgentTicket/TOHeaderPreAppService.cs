using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Application.Order.Dto;
using ThemePark.Application.Order.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Common;
using ThemePark.Core.Agencies.Repositories;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.AgentTicket.Repositories;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 预订单应用服务
    /// </summary>
    public class TOHeaderPreAppService : ThemeParkAppServiceBase, ITOHeaderPreAppService
    {
        #region Fields

        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IOrderPreDomainService _orderPreDomainService;
        private readonly ITOHeaderPreRepository _tOHeaderPreRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IRepository<TOBody, string> _tOBodyRepository;
        private readonly IRepository<TOHeader, string> _toheaderRepository;
        private readonly IParkAppService _parkAppService;
        private readonly IOrderDetailAppService _orderDetailAppService;
        #endregion Fields

        #region Cotr

        /// <summary>
        /// Initializes a new instance of the <see cref="TOHeaderPreAppService"/> class.
        /// </summary>
        public TOHeaderPreAppService(ITOHeaderPreRepository tOHeaderPreRepository, IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository,
            IRepository<TOBody, string> tOBodyRepository, IOrderPreDomainService orderPreDomainService, IAgencyRepository agencyRepository, IRepository<TOHeader, string> toheaderRepository, IParkAppService parkAppService, IOrderDetailAppService orderDetailAppService)
        {
            _tOHeaderPreRepository = tOHeaderPreRepository;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _tOBodyRepository = tOBodyRepository;
            _orderPreDomainService = orderPreDomainService;
            _agencyRepository = agencyRepository;
            _toheaderRepository = toheaderRepository;
            _parkAppService = parkAppService;
            _orderDetailAppService = orderDetailAppService;
        }

        #endregion Cotr

        #region Public Methods       

        /// <summary>
        ///  中心/旅行社查询分页的预订单列表（有确认的话将获取确认信息）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageResult<TOHeaderPreDto>> GetTOHeaderPre4CentreOrTravelAsync(TravelSearchOrderDto query = null)
        {
            //获取所有旅行社预订单
            var tOHeaderPreList = await GetPagedTravelPreOrdersAsync<TOHeaderPreDto>(query);

            if (tOHeaderPreList.Data.Any())
            {
                foreach (var order in tOHeaderPreList.Data)
                {
                    if (order.TOBodyPres.Count != 0)
                        order.ParkName = order.TOBodyPres.First().ParkName;

                    //已确认的订单，如果是在预订单的基础上新加了票类，将在下面把新加的票类添加进列表显示在主界面
                    if (order.MainOrderState != MainOrderState.NotConfirm || order.MainOrderState != MainOrderState.OrderCancel)
                    {
                        //确认的子订单
                        var tobodyList = _tOBodyRepository.GetAllIncluding(x => x.AgencySaleTicketClass.ParkSaleTicketClass).Where(m => m.TOHeaderId == order.TOHeaderPreId);
                        //从确认订单中获取是否已核销
                        order.IsConsume = tobodyList.Any(m => m.OrderState == OrderState.TradeSuccess);

                        var sameAgencySaleTicketClassId = new List<int>();

                        foreach (var tobodyPre in order.TOBodyPres)
                        {
                            foreach (var tobody in tobodyList)
                            {
                                if (tobody.AgencySaleTicketClassId == tobodyPre.AgencySaleTicketClassId)
                                {
                                    //预订子订单的确认票数
                                    tobodyPre.ConfirmQty = tobody.Qty;
                                    sameAgencySaleTicketClassId.Add(tobody.AgencySaleTicketClassId);
                                    break;
                                }
                            }
                        }
                        //在确认子订单中选出预订单没有的已确定票类
                        var notSameTOBodies = tobodyList.Where(m => !sameAgencySaleTicketClassId.Contains(m.AgencySaleTicketClassId));
                        if (notSameTOBodies.Any())
                        {
                            foreach (var notSameToBody in notSameTOBodies)
                            {
                                var parkSaleTicketClass = notSameToBody.AgencySaleTicketClass.ParkSaleTicketClass;
                                //构建多出的确认子订单的预订单信息
                                var tobody = new TOBodyPreDto
                                {
                                    SaleTicketClassName = parkSaleTicketClass.SaleTicketClassName,
                                    Price = parkSaleTicketClass.Price,
                                    SalePrice = parkSaleTicketClass.SalePrice,
                                    SettlementPrice = notSameToBody.AgencySaleTicketClass.SettlementPrice,
                                    Qty = 0,
                                    ConfirmQty = notSameToBody.Qty,
                                    OrderState = notSameToBody.OrderState
                                };

                                order.TOBodyPres.Add(tobody);
                            }
                        }
                    }

                    //目前订单没有做日结，所以需要手动检测订单是否已过期，过期将不再显示确认按钮
                    order.IsExpired = DateTime.Now.Date.CompareTo(order.ValidStartDate.Date) > 0;
                }
            }
            return tOHeaderPreList;
        }

        /// <summary>
        /// 搜索旅行社预订单信息列表(旅行社用)
        /// </summary>
        public async Task<PageResult<TDto>> GetPagedTravelPreOrdersAsync<TDto>(TravelSearchOrderDto query = null)
        {
            query = query ?? new TravelSearchOrderDto();

            if (query.SortFields.Count == 0)
                query.SortFields.Add("CreationTime desc");

            var expression = query.GetFilter().And(p => p.OrderType == OrderType.TravelOrder);

            if (query.ParkId != null)
                expression = expression.And(p => p.TOBodyPres.Any(o => o.ParkId == query.ParkId));

            //if (string.IsNullOrWhiteSpace(query.Id))
            //    //去除关闭状态的预订单
            //expression = expression.And(p => p.MainOrderState == query.MainOrderState.Value);

            if (string.IsNullOrWhiteSpace(query.Id))
                //去除关闭状态的预订单
                expression = query.MainOrderState != null ? expression.And(p => p.MainOrderState == query.MainOrderState.Value)
                    : expression.And(p => p.MainOrderState != MainOrderState.OrderCancel);

            var searchQuery = new PageQuery<TOHeaderPre>(expression)
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortFields = query.SortFields
            };

            var result = _tOHeaderPreRepository.GetAll().Where(searchQuery);

            //根据权限过滤主订单，不在权限范围内的子订单TOBodyPres.Count为0
            result = result.Where(m => m.TOBodyPres.Count != 0);

            return await result.ToPageResultAsync<TOHeaderPre, TDto>(new Query<TOHeaderPre>(), query);
        }

        /// <summary>
        /// 旅行社关闭预订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Result> CancelTravelOrderAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return Result.FromError("订单号不能为空");

            var toheaderPre = await _tOHeaderPreRepository.GetAllIncluding(m => m.TOBodyPres).FirstOrDefaultAsync(m => m.Id == orderId);
            if (toheaderPre == null)
                return Result.FromError("不能关闭该订单");
            if (toheaderPre.MainOrderState != MainOrderState.NotConfirm)
                return Result.FromError("只有未确认状态下的订单才能被关闭");

            //更新订单状态
            toheaderPre.MainOrderState = MainOrderState.OrderCancel;
            toheaderPre.TOBodyPres.ForEach(m => m.OrderState = OrderState.OrderCancel);

            return Result.Ok();
        }

        /// <summary>
        /// 新增预订订单
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&lt;System.String&gt;&gt;.</returns>
        public async Task<Result<string>> AddOrderPreAsync(AddAgencyReserveInput input)
        {
            var newHeader = input.MapTo<TOHeaderPre>();

            //1. 日期
            //just date, no datetime
            newHeader.ValidStartDate = input.ValidStartDate.Date;
            if (newHeader.ValidStartDate < DateTime.Now.Date)
            {
                return Result.FromCode<string>(ResultCode.NoRecord, "入园日期不能早于今天，请重新订票！");
            }

            //2. 促销票有效
            if (newHeader.TOBodyPres.Count == 0)
                return Result.FromCode<string>(ResultCode.InvalidData, "至少要选择一种票");

            if (!await CheckAgencySaleTicketClassValid(newHeader))
            {
                return Result.FromError<string>("订单包含无效票类。");
            }

            //3. 订单规则
            var ruleResult = await CheckAgencyRule(newHeader);
            if (!ruleResult.Success)
            {
                return Result.FromError<string>(ruleResult.Message);
            }

            //添加司机和导游到GroupInfo
            newHeader.GroupInfo.GuideIds = string.Join(",", input.GroupInfo.GuideIdList.Select(p => p.ToString()));
            newHeader.GroupInfo.DriverIds = string.Join(",", input.GroupInfo.DriverIdList.Select(p => p.ToString()));

            var result = await _orderPreDomainService.AddOrderPreAsync(newHeader);

            return Result.FromData(result.Id);
        }

        /// <summary>
        /// 旅行社站点对预订单进行更改
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> UpdateOrderPreAsync(TOHeaderPreEditInput input)
        {
            //去除数量为0的票类（中心会有这样的情况）
            input.TOBodyPres.RemoveAll(m => m.Qty == 0);

            var newHeaderPre = input.MapTo<TOHeaderPre>();

            var header = await _tOHeaderPreRepository.GetAsync(input.Id);

            //未确认状态才允许修改
            if (header.MainOrderState != MainOrderState.NotConfirm)
                return Result.FromError("订单不是未确认状态，不允许修改。");

            //1. 日期
            newHeaderPre.ValidStartDate = header.ValidStartDate;
            newHeaderPre.GroupTypeId = header.GroupTypeId;
            newHeaderPre.AgencyId = header.AgencyId;

            //2. 促销票有效
            if (newHeaderPre.TOBodyPres.Count == 0)
                return Result.FromCode(ResultCode.InvalidData, "至少要选择一种票");

            if (!await CheckAgencySaleTicketClassValid(newHeaderPre))
            {
                return Result.FromError("订单包含无效票类。");
            }

            //3. 订单规则
            var ruleResult = await CheckAgencyRule(newHeaderPre);
            if (!ruleResult.Success)
            {
                return ruleResult;
            }

            input.MapTo(header);
            header.GroupInfo.GuideIds = input.GroupInfo.GuideIds;
            header.GroupInfo.DriverIds = input.GroupInfo.DriverIds;

            await _orderPreDomainService.UpdateOrderPreAsync(header);

            return Result.Ok();
        }

        /// <summary>
        /// 中心和旅行社根据Id获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TOHeaderPreDetailDto> GetTravelOrderDetailByIdAsync(string id)
        {
            //获取订单头信息
            var model = await GetTOHeaderPreAsync<TOHeaderPreDetailDto>(new Query<TOHeaderPre>(o => o.Id == id));

            if (model.MainOrderState != MainOrderState.NotConfirm && model.MainOrderState != MainOrderState.OrderCancel)
            {
                var toheader = await _toheaderRepository.GetAllIncluding(m => m.TOBodies).FirstAsync(m => m.Id == id);
                model.ConfirmAmount = toheader.Amount;
                var parkId = toheader.TOBodies.First().ParkId;
                model.ParkName = _parkAppService.GetParkAsync<ParkDto>(new Query<Park>(o => o.Id == parkId)).Result.ParkName;
            }
            else
            {
                model.ParkName = model.TOBodyPres.First().ParkName;
            }

            if (model.MainOrderState == MainOrderState.Warranted)
            {
                //获取订单体信息(包括票的所有信息，如入园，退款等)
                var orderDetail = await _orderDetailAppService.GetOrderDetailFromCentreOrParkApiAsync(id);
                model.OrderDetailDto = orderDetail?.Data;
            }

            //已确认的订单，如果是在预订单的基础上新加了票类，将在下面把新加的票类添加进列表显示在主界面
            if (model.MainOrderState != MainOrderState.NotConfirm || model.MainOrderState != MainOrderState.OrderCancel)
            {
                //确认的子订单
                var tobodyList = _tOBodyRepository.GetAllIncluding(m => m.AgencySaleTicketClass.ParkSaleTicketClass).Where(m => m.TOHeaderId == model.Id);

                var sameAgencySaleTicketClassId = new List<int>();

                foreach (var tobodyPre in model.TOBodyPres)
                {
                    foreach (var tobody in tobodyList)
                    {
                        if (tobody.AgencySaleTicketClassId == tobodyPre.AgencySaleTicketClassId)
                        {
                            //预订子订单的确认票数
                            tobodyPre.ConfirmQty = tobody.Qty;
                            sameAgencySaleTicketClassId.Add(tobody.AgencySaleTicketClassId);
                            break;
                        }
                    }
                }
                //在确认子订单中选出预订单没有的已确定票类
                var notSameTOBodies = tobodyList.Where(m => !sameAgencySaleTicketClassId.Contains(m.AgencySaleTicketClassId));
                if (notSameTOBodies.Any())
                {
                    foreach (var notSameToBody in notSameTOBodies)
                    {
                        var parkSaleTicketClass = notSameToBody.AgencySaleTicketClass.ParkSaleTicketClass;
                        //构建多出的确认子订单的预订单信息
                        var tobody = new TOBodyPreDetailDto
                        {
                            Id = notSameToBody.Id,
                            SaleTicketClassName = parkSaleTicketClass.SaleTicketClassName,
                            Price = parkSaleTicketClass.Price,
                            SalePrice = parkSaleTicketClass.SalePrice,
                            SettlementPrice = notSameToBody.AgencySaleTicketClass.SettlementPrice,
                            Qty = 0,
                            ConfirmQty = notSameToBody.Qty,
                            OrderState = notSameToBody.OrderState
                        };

                        model.TOBodyPres.Add(tobody);
                    }
                }
            }

            model.IsExpired = DateTime.Now.Date.CompareTo(model.ValidStartDate.Date) > 0;

            return model;
        }

        /// <summary>
        /// 根据条件查找预订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetTOHeaderPreAsync<TDto>(IQuery<TOHeaderPre> query)
        {
            return await _tOHeaderPreRepository.GetAll().FirstOrDefaultAsync<TOHeaderPre, TDto>(query);
        }

        /// <summary>
        /// 根据条件查找预订单列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> GetTOHeadPreListAsync<TDto>(IQuery<TOHeaderPre> query)
        {
            return await _tOHeaderPreRepository.GetAll().ToListAsync<TOHeaderPre, TDto>(query);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 检测是否满足代理商规则
        /// </summary>
        /// <param name="headerPre">The header pre.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        private async Task<Result> CheckAgencyRule(TOHeaderPre headerPre)
        {
            //parkid agencytype grouptype
            var agencyId = headerPre.AgencyId;
            var parkId = headerPre.TOBodyPres.First().ParkId;
            var agencyType = await _agencyRepository.GetAgencyTypeAsync(parkId, agencyId, o => new { o.Id });
            if (agencyType == null)
            {
                return Result.FromError("代理商类型不存在。");
            }

            var rule = await _agencyRepository.GetAgencyRuleAsync(parkId, agencyType.Id, headerPre.GroupTypeId, o => new { o.MinQty, o.MaxQty });
            if (rule == null)
            {
                return Result.FromError("代理商规则不存在。");
            }

            //基础票类的价格为0且包含司机和导游的（司机票和导游票），不算入代理商规则人数里面
            var guide = GuideOrDriverTicket.Guide.DisplayName();
            var dirver = GuideOrDriverTicket.Driver.DisplayName();
            var except = await _agencySaleTicketClassRepository.GetAll().Where(
                    o => (o.ParkSaleTicketClass.TicketClass.TicketClassName.Contains(guide) || o.ParkSaleTicketClass.TicketClass.TicketClassName.Contains(dirver))
                    && o.ParkId == parkId)
                    .Select(o => o.Id).ToListAsync();
            //除掉司机和导游票的子订单
            var countTOBodyPres = headerPre.TOBodyPres.Where(o => !except.Contains(o.AgencySaleTicketClassId));
            int totalQty = 0;
            foreach (var toBodyPre in countTOBodyPres)
            {
                var persons =
                    _agencySaleTicketClassRepository.AsNoTracking().First(o => o.Id == toBodyPre.AgencySaleTicketClassId
                                                                               && o.Status == TicketClassStatus.Sailing).ParkSaleTicketClass.TicketClass.TicketType.Persons;
                totalQty += toBodyPre.Qty * persons;
            }

            if (totalQty < rule.MinQty || totalQty > rule.MaxQty)
            {
                return Result.FromError($"预订人数必须满足{rule.MinQty}到{rule.MaxQty}之间");
            }

            return Result.Ok();
        }

        ///// <summary>
        ///// 旅行社下单业务验证
        ///// </summary>
        ///// <param name="toHead"></param>
        ///// <returns></returns>
        //private async Task<Result<string>> VerifyBusinessRule(TOHeaderPre toHead)
        //{
        //    //判断入园日期是否过期
        //    if (toHead.ValidStartDate.Date < DateTime.Now.Date)
        //    {
        //        return Result.FromCode<string>(ResultCode.NoRecord, "入园日期不能早于今天，请重新订票！");
        //    }

        //    if (toHead.TOBodyPres.Count == 0)
        //        return Result.FromCode<string>(ResultCode.InvalidData, "至少要选择一种票");

        //    int totalQty = 0;

        //    foreach (var toBody in toHead.TOBodyPres)
        //    {
        //        //查询该促销票
        //        var agencySaleTicketClass = _agencySaleTicketClassRepository.AsNoTracking().FirstOrDefault(o => o.Id == toBody.AgencySaleTicketClassId && o.Status == TicketClassStatus.Sailing);
        //        if (agencySaleTicketClass == null)
        //            return Result.FromCode<string>(ResultCode.InvalidData, "该促销票类不存在或已下架");

        //        //查询基础票类
        //        var ticketClass = await _ticketClassRepository.FirstOrDefaultAsync(agencySaleTicketClass.ParkSaleTicketClass.TicketClassId);

        //        //子订单
        //        toBody.Price = agencySaleTicketClass.Price;
        //        toBody.SalePrice = agencySaleTicketClass.SalePrice;
        //        toBody.SettlementPrice = agencySaleTicketClass.SettlementPrice;
        //        toBody.ParkSettlementPrice = agencySaleTicketClass.ParkSettlementPrice;
        //        toBody.Amount = agencySaleTicketClass.SalePrice * toBody.Qty;
        //        toBody.Persons = ticketClass.TicketType.Persons * toBody.Qty;
        //        toBody.ConfirmPersons = 0;

        //        //主订单
        //        toHead.Amount += toBody.Amount;
        //        toHead.Qty += toBody.Qty;
        //        toHead.Persons += toBody.Persons;

        //        //基础票类的价格为0且包含司机和导游的（司机票和导游票），不算入代理商规则人数里面
        //        if (ticketClass.StandardPrice == 0 && (ticketClass.TicketClassName.Contains(GuideOrDriverTicket.Guide.DisplayName())
        //            || ticketClass.TicketClassName.Contains(GuideOrDriverTicket.Driver.DisplayName())))
        //            continue;

        //        //用来验证代理商规则人数(导游票和司机票不包含在里面)
        //        totalQty += toBody.Qty;
        //    }

        //    var parkArray = toHead.TOBodyPres.Select(o => o.ParkId).Distinct();

        //    var agencyTypeArray = _parkAgencyRepository.GetAll().Where(p => p.AgencyId == toHead.AgencyId)
        //        .Select(o => o.AgencyTypeId).Distinct();

        //    var qtySum = await _parkAgencyTypeGroupTypeRepository.GetAll()
        //        .Where(o => parkArray.Contains(o.ParkId)
        //                    && o.GroupTypeId == toHead.GroupTypeId
        //                    && agencyTypeArray.Contains(o.AgencyTypeId))
        //        .Select(p => new { p.AgencyRule.MinQty, p.AgencyRule.MaxQty })
        //        .FirstOrDefaultAsync();

        //    if (qtySum == null)
        //        return Result.FromError<string>("没有找到对应的公园代理商团体类型");

        //    if (totalQty < qtySum.MinQty || totalQty > qtySum.MaxQty)
        //    {
        //        return Result.FromError<string>($"预订人数必须满足{qtySum.MinQty}到{qtySum.MaxQty}之间");
        //    }

        //    return Result.FromCode<string>(ResultCode.Ok);
        //}

        /// <summary>
        /// Checks the ticket classes are valid.
        /// </summary>
        /// <param name="headerPre">The header pre.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> CheckAgencySaleTicketClassValid(TOHeaderPre headerPre)
        {
            var agencySaleTicketClassIds = headerPre.TOBodyPres.Select(o => o.AgencySaleTicketClassId);
            var date = headerPre.ValidStartDate.Date;

            return await _agencySaleTicketClassRepository.AsNoTracking()
                .CountAsync(o => agencySaleTicketClassIds.Contains(o.Id) && o.Status == TicketClassStatus.Sailing &&
                (o.SaleStartDate.HasValue && date >= o.SaleStartDate || !o.SaleStartDate.HasValue) &&
                (o.SaleEndDate.HasValue && date <= o.SaleEndDate || !o.SaleEndDate.HasValue)) == agencySaleTicketClassIds.Count();
        }

        #endregion Private Methods
    }
}