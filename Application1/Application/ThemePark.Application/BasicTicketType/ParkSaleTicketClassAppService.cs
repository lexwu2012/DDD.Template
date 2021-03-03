using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Core.CoreCache.InterFaces;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.BasicTicketType
{
    /// <summary>
    /// 促销票类应用服务
    /// </summary>
    public class ParkSaleTicketClassAppService : ThemeParkAppServiceBase, IParkSaleTicketClassAppService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IParkSaleTicketClassCache _parkSaleTicketClassCache;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parkSaleTicketClassRepository"></param>
        /// <param name="cacheManager"></param>
        /// <param name="parkSaleTicketClassCache"></param>
        public ParkSaleTicketClassAppService(IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository, ICacheManager cacheManager, IParkSaleTicketClassCache parkSaleTicketClassCache)
        {
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _cacheManager = cacheManager;
            _parkSaleTicketClassCache = parkSaleTicketClassCache;
        }

        #endregion Ctor

        #region Public Methods

        /// <summary>
        /// 增加公园基础票类门市价
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result> AddParkSaleTicketClassAsync(ParkSaleTicketClassSaveNewInput input)
        {
            //业务规则约束
            if ((input.IsEveryday == false) && (input.SaleStartDate == null) && (input.SaleEndDate == null))
            {
                return Result.FromError("请设置开始售卖日期和结束售卖日期.");
            }

            DateTime saleStartDate = Convert.ToDateTime(input.SaleStartDate);
            DateTime saleEndDate = Convert.ToDateTime(input.SaleEndDate);

            if (DateTime.Compare(saleStartDate, saleEndDate) > 0)
            {
                return Result.FromError("促销开始日期必须小于促销结束日期.");
            }

            //获取重复的数据
            var duplicateRecordList = new List<ParkSaleTicketClass4ParkDto>();

            //入园规则，公园，和票类作为业务主键
            /*
             *  长期促销只能有一条，短期促销的话多条记录中的时间不能重叠
             * 
             */
            var parkSaleTicketClassList = await GetParkSaleTicketClassListAsync<ParkSaleTicketClass4ParkDto>(new Query<ParkSaleTicketClass>(o => o.ParkId == input.ParkId && o.InParkRuleId == input.InParkRuleId && o.TicketClassId == input.TicketClassId && o.TicketClassStatus == TicketClassStatus.Sailing));

            if (parkSaleTicketClassList.Count != 0)
            {
                foreach (var item in parkSaleTicketClassList)
                {
                    //长期促销只能有一条记录
                    if (input.IsEveryday.HasValue && input.IsEveryday.Value && item.IsEveryday.HasValue && item.IsEveryday.Value)
                    {
                        duplicateRecordList.Add(item);
                        //有重复就跳出当前循环
                        break;
                    }

                    //短期促销的话多条记录中的时间不能重叠
                    if ((input.SaleStartDate >= item.SaleStartDate && input.SaleStartDate <= item.SaleEndDate)
                        || (input.SaleEndDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate)
                        || (input.SaleStartDate <= item.SaleStartDate && input.SaleEndDate >= item.SaleEndDate)
                        || (input.SaleStartDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate))
                    {
                        duplicateRecordList.Add(item);
                        break;
                    }
                }
            }

            //如果有重复的数据，将返回重复的数据到客户端
            if (duplicateRecordList.Count != 0)
            {
                string message = string.Empty;

                foreach (var duplicateRecord in duplicateRecordList)
                {
                    message += string.Format("{0},", duplicateRecord.ParkName);
                }

                return Result.FromCode(ResultCode.DuplicateRecord, message.Substring(0, message.LastIndexOf(",", StringComparison.Ordinal)) + "<br>记录已存在.");
            }

            //如果没重复数据，进行数据库操作
            var parkSaleTicketClass = input.MapTo<ParkSaleTicketClass>();

            await _parkSaleTicketClassRepository.InsertAsync(parkSaleTicketClass);


            return Result.FromCode(ResultCode.Ok);
        }

        /// <summary>
        /// 删除公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        public async Task<Result> DeleteParkSaleTicketClassAsync(int id)
        {
            await _parkSaleTicketClassRepository.DeleteAsync(id);

            return Result.Ok();
        }

        /// <summary>
        /// 公园促销票类查询
        /// </summary>
        /// <param name="query">查询输入的参数</param>
        /// <returns>查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkSaleTicketClass> query = null)
        {
            return _parkSaleTicketClassRepository.AsNoTracking().ToPageResultAsync<ParkSaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 通过ID从缓存中获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ParkSaleTicketClassCacheItem> GetOnCacheByIdAsync(int id)
        {
            return await _parkSaleTicketClassCache.GetAsync(id);
        }

        /// <summary>
        /// 根据条件获取门市价
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetParkSaleTicketClassAsync<TDto>(IQuery<ParkSaleTicketClass> query)
        {
            return await _parkSaleTicketClassRepository.AsNoTracking().FirstOrDefaultAsync<ParkSaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 获取票类可入园人数
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Result&lt;System.Int32&gt;&gt;.</returns>
        public async Task<Result<int>> GetTicketClassPersonsAsync(int id)
        {
            var persons = await _parkSaleTicketClassRepository.GetAll()
                    .Where(o => o.Id == id)
                    .Select(o => o.TicketClass.TicketType.Persons)
                    .FirstOrDefaultAsync();

            if (persons == default(int))
            {
                return Result.FromCode<int>(ResultCode.NoRecord);
            }

            return Result.FromData(persons);
        }

        /// <summary>
        /// 获取制定票类名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<string> GetParkSaleTicketClassNameAsync(int id)
        {
            return _parkSaleTicketClassRepository.AsNoTracking().Where(o => o.Id == id)
                        .Select(o => o.SaleTicketClassName).SingleAsync();
        }

        /// <summary>
        /// 根据条件获取门市价列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetParkSaleTicketClassListAsync<TDto>(IQuery<ParkSaleTicketClass> query)
        {
            return await _parkSaleTicketClassRepository.AsNoTracking().ToListAsync<ParkSaleTicketClass, TDto>(query);
        }

        /// <summary>
        /// 获取年卡可销售类型
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ParkSaleTicketClassYearCardDto>> GetSaleCardTypes(int parkId, TicketClassMode ticketClassMode)
        {
            var nowDate = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
            var query = _parkSaleTicketClassRepository.AsNoTracking()
                .Where(p =>p.IsSupportSingle==true && p.TicketClassStatus == TicketClassStatus.Sailing && p.ParkId == parkId && p.TicketClass.TicketClassMode == ticketClassMode && p.IsEveryday == false && p.SaleStartDate <= nowDate && p.SaleEndDate >= nowDate);

            var query1 = _parkSaleTicketClassRepository.AsNoTracking()
                .Where(p => p.IsSupportSingle == true && p.TicketClassStatus == TicketClassStatus.Sailing && p.ParkId == parkId && p.TicketClass.TicketClassMode == ticketClassMode && p.IsEveryday == true);

            query1 = query1.Union(query);

            return await query1.ProjectTo<ParkSaleTicketClassYearCardDto>().ToListAsync();
        }

        /// <summary>
        /// 根据公园ID获取散客票
        /// </summary>
        /// <param name="parkIds"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetSaleTicketByParkIdsAsync<TDto>(List<int> parkIds, DateTime? date)
        {
            var plandate = date?.Date ?? DateTime.Now.Date;
            //取到长期票和短期促销票
            var items = (await GetParkSaleTicketClassListAsync<ParkSaleTicketClassCacheItem>
                (
                    new Query<ParkSaleTicketClass>
                    (
                        m => parkIds.Contains(m.ParkId) && 
                        m.Park.IsActive &&
                        m.IsSupportSingle &&
                        m.TicketClass.TicketClassMode == TicketClassMode.Normal &&
                        m.TicketClassStatus == TicketClassStatus.Sailing &&
                        (plandate >= m.SaleStartDate && plandate <= m.SaleEndDate || m.SaleStartDate == null && m.SaleEndDate == null)
                    ))).ToList();

            //消除重复长期促销票
            items.RemoveAll(p => items.Count(o => o.TicketClassId == p.TicketClassId) > 1 && p.SaleStartDate == null && p.SaleEndDate == null);

            return items.ToList().MapTo<IList<TDto>>();
        }

        /// <summary>
        /// 根据公园ID获取套票票
        /// </summary>
        /// <param name="parkIds"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetMulSaleTicketByParkIdsAsync<TDto>(List<int> parkIds, DateTime? date)
        {
            var plandate = date?.Date ?? DateTime.Now.Date;
            //取到长期票和短期促销票
            var items = (await GetParkSaleTicketClassListAsync<ParkSaleTicketClassCacheItem>
                (
                    new Query<ParkSaleTicketClass>
                    (
                        m => parkIds.Contains(m.ParkId) && m.IsSupportSingle &&
                        m.TicketClass.TicketClassMode == TicketClassMode.MultiParkTicket &&
                        m.TicketClassStatus == TicketClassStatus.Sailing &&
                        (plandate >= m.SaleStartDate && plandate <= m.SaleEndDate || m.SaleStartDate == null && m.SaleEndDate == null)
                    ))).ToList();

            //消除重复长期促销票
            items.RemoveAll(p => items.Count(o => o.TicketClassId == p.TicketClassId) > 1 && p.SaleStartDate == null && p.SaleEndDate == null);

            return items.ToList().MapTo<IList<TDto>>();
        }



        ///// <summary>
        ///// 根据公园ID获取套票
        ///// </summary>
        ///// <param name="parkId"></param>
        ///// <param name="date"></param>
        ///// <returns></returns>
        //public async Task<IList<ParkSaleTicketClassCacheItem>> GetMultiOnCacheByParkIdAsync(int parkId, DateTime? date)
        //{
        //    //var key = parkId.ToString() + "Multi";

        //    date = date?.Date ?? DateTime.Now.Date;
        //    var key = parkId.ToString() + "Multi" + date.Value.ToString("yyyyMMdd");
        //    return await _cacheManager.GetSaleTicketClassesCache().GetAsync(key, async () =>
        //    {
        //        //去到长期票和短期促销票
        //        var items = (await GetParkSaleTicketClassListAsync<ParkSaleTicketClassCacheItem>(new Query<ParkSaleTicketClass>(m => m.ParkId == parkId
        //                                                                                                && m.TicketClass.TicketClassMode == TicketClassMode.MultiParkTicket && m.TicketClassStatus == TicketClassStatus.Sailing
        //                                                                                                && ((date >= m.SaleStartDate && date <= m.SaleEndDate) || (m.SaleStartDate == null && m.SaleEndDate == null))
        //                                                                                               ))).ToList();
        //        //消除重复长期促销票
        //        items.RemoveAll(p => items.Count(o => o.TicketClassId == p.TicketClassId) > 1 && p.SaleStartDate == null && p.SaleEndDate == null);

        //        return items.ToList();
        //    });
        //}


        /// <summary>
        /// 更新公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateParkSaleTicketClassAsync(int id, ParkSaleTicketClassInput input)
        {
            if ((input.IsEveryday == false) && (input.SaleStartDate == null) && (input.SaleEndDate == null))
            {
                return Result.FromError("请设置开始售卖日期和结束售卖日期.");
            }

            DateTime saleStartDate = Convert.ToDateTime(input.SaleStartDate);
            DateTime saleEndDate = Convert.ToDateTime(input.SaleEndDate);

            if (DateTime.Compare(saleStartDate, saleEndDate) > 0)
            {
                return Result.FromError("促销开始日期必须小于促销结束日期.");
            }

            /*
            *  长期促销只能有一条，短期促销的话多条记录中的时间不能重叠
            * 
            */

            var self = await _parkSaleTicketClassRepository.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            //查询排除掉本身记录的其他记录
            var parkSaleTicketClassList = await GetParkSaleTicketClassListAsync<ParkSaleTicketClass4ParkDto>(new Query<ParkSaleTicketClass>(o => o.Id != id
            && o.ParkId == self.ParkId && o.InParkRuleId == self.InParkRuleId
            && o.TicketClassId == self.TicketClassId && o.TicketClassStatus == TicketClassStatus.Sailing));

            //有其他记录
            if (parkSaleTicketClassList.Count != 0)
            {
                foreach (var item in parkSaleTicketClassList)
                {
                    //长期促销、同一促销不同时间
                    if ((input.IsEveryday.HasValue && input.IsEveryday.Value && item.IsEveryday.HasValue && item.IsEveryday.Value)
                            || (input.SaleStartDate >= item.SaleStartDate && input.SaleStartDate <= item.SaleEndDate)
                            || (input.SaleEndDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate)
                            || (input.SaleStartDate <= item.SaleStartDate && input.SaleEndDate >= item.SaleEndDate)
                            || (input.SaleStartDate >= item.SaleStartDate && input.SaleEndDate <= item.SaleEndDate))
                    {
                        return Result.FromCode(ResultCode.DuplicateRecord);
                    }
                }
            }

            await _parkSaleTicketClassRepository.UpdateAsync(id, p => Task.FromResult(input.MapTo(p)));

            return Result.Ok();
        }

        #endregion Public Methods
    }
}