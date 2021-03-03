using Abp.Dependency;
using Abp.Runtime.Caching;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.CoreCache.CacheItem;
using Abp.BackgroundJobs;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.VerifyTicketDto.Dto;
using System.Threading;
using Abp.Configuration;
using Castle.Core.Logging;
using ThemePark.Application.SaleCard.Interfaces;
using ThemePark.VerifyTicketDto.Model;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.Message;
using ThemePark.Application.VerifyTicket.WriteInPark;
using ThemePark.Common;
using ThemePark.Core.MultiTicket;
using ThemePark.Core.Settings;
using ThemePark.EntityFramework;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验票管理器
    /// </summary>
    public class CheckTicketManager : ICheckTicketManager, ISingletonDependency
    {
        public static string TicketCheckDataCacheKey { get; } = "TicketCheckDataCache";

        //private AbpMemoryCacheManager _cacheManager;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ParkSaleTicketClass> _parkSaleTicketClassRepository;
        private readonly IRepository<AgencySaleTicketClass> _agencySaleTicketClassRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ISmsAppService _smsAppService;
        private Thread _threadCheck;
        private bool _exit;
        private TicketTracker _ticketTracker;

        private readonly ISettingManager _settingManager;
        private DateTime _lastCreateFingerZkCacheTime;

        private readonly IRepository<MultiTicketEnterEnroll, long> _multiTicketEnterEnrollRepository;

       // private int _parkId;

        //private ZWJType _fingerType;

        public ILogger Logger { get; set; }

        /// <summary>
        /// 验票管理器构函数
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="parkSaleTicketClassRepository"></param>
        /// <param name="agencySaleTicketClassRepository"></param>
        /// <param name="backgroundJobManager"></param>
        /// <param name="settingManager"></param>
        /// <param name="multiTicketEnterEnrollRepository"></param>
        /// <param name="smsAppService"></param>
        public CheckTicketManager(ICacheManager cacheManager,
            IRepository<ParkSaleTicketClass> parkSaleTicketClassRepository,
            IRepository<AgencySaleTicketClass> agencySaleTicketClassRepository,
            IBackgroundJobManager backgroundJobManager, ISettingManager settingManager, IRepository<MultiTicketEnterEnroll, long> multiTicketEnterEnrollRepository, ISmsAppService smsAppService)
        {
            _cacheManager = cacheManager;
            _parkSaleTicketClassRepository = parkSaleTicketClassRepository;
            _agencySaleTicketClassRepository = agencySaleTicketClassRepository;
            _backgroundJobManager = backgroundJobManager;
            _settingManager = settingManager;
            _lastCreateFingerZkCacheTime = DateTime.Now.AddMinutes(-3);
            _multiTicketEnterEnrollRepository = multiTicketEnterEnrollRepository;
            _smsAppService = smsAppService;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// 取验票数据缓存
        /// </summary>
        /// <returns></returns>
        public ITypedCache<string, TicketInfo> GetTicketInfoCache()
        {
            return _cacheManager.GetCache<string, TicketInfo>("TicketInfoCache");
        }

        /// <summary>
        /// 正在检票的条码缓存
        /// </summary>
        /// <returns></returns>
        public ITypedCache<string, string> GetCheckingCheckTicketCache()
        {
            return _cacheManager.GetCache<string, string>("CheckingCheckTicketCache");
        }

        /// <summary>
        /// 取验票数据缓存
        /// </summary>
        /// <returns></returns>
        public ITypedCache<string, TicketCheckData> GetTicketCheckDataCache()
        {
            return _cacheManager.GetCache<string, TicketCheckData>(TicketCheckDataCacheKey);
        }

        ///// <summary>
        ///// 取票类的验票缓存项
        ///// </summary>
        ///// <param name="parkSaleTicketClassId"></param>
        ///// <returns></returns>
        //public async Task<TicketClassCacheItem> GetParkTicketClassCacheItem(int parkSaleTicketClassId)
        //{
        //    return await _cacheManager.GetCache<int, TicketClassCacheItem>("ParkSaleTicketClassCheckItemCache").GetAsync(parkSaleTicketClassId,
        //            async () =>
        //            {
        //                var parkSaleTicketClass =
        //                    await _parkSaleTicketClassRepository.FirstOrDefaultAsync(parkSaleTicketClassId);
        //                if (parkSaleTicketClass == null)
        //                    return null;

        //                var item = new TicketClassCacheItem
        //                {
        //                    Persons = parkSaleTicketClass.TicketClass.TicketType.Persons,
        //                    CanInParkIds = parkSaleTicketClass.TicketClass.InParkIdFilter,
        //                    InParkRullId = parkSaleTicketClass.InParkRuleId,
        //                    TicketClassId = parkSaleTicketClass.TicketClassId,
        //                    TicketClassName = parkSaleTicketClass.SaleTicketClassName,
        //                    RuleItem = parkSaleTicketClass.InParkRule.MapTo<InParkRuleCacheItem>()
        //                };
        //                return item;
        //            });
        //}

        /// <summary>
        /// 取票类的验票项
        /// </summary>
        /// <param name="parkSaleTicketClassId"></param>
        /// <returns></returns>
        public async Task<TicketClassCacheItem> GetParkTicketClassItem(int parkSaleTicketClassId)
        {

            var parkSaleTicketClass =
                await _parkSaleTicketClassRepository.FirstOrDefaultAsync(parkSaleTicketClassId);
            if (parkSaleTicketClass == null)
                throw new ArgumentNullException($"获取公园可售票类失败, {parkSaleTicketClassId}");

            if (parkSaleTicketClass.TicketClass == null)
            {
                throw new ArgumentNullException($"获取票类失败, parkSaleTicketClassId:{parkSaleTicketClassId} TicketClass:{parkSaleTicketClass.TicketClassId}");
            }

            if (parkSaleTicketClass.InParkRule == null)
            {
                throw new ArgumentNullException($"获取票类入园规则失败, parkSaleTicketClassId:{parkSaleTicketClassId} InParkRule:{parkSaleTicketClass.InParkRuleId}");
            }

            var item = new TicketClassCacheItem
            {
                Persons = parkSaleTicketClass.TicketClass.TicketType.Persons,
                CanInParkIds = parkSaleTicketClass.TicketClass.InParkIdFilter,
                InParkRullId = parkSaleTicketClass.InParkRuleId,
                TicketClassId = parkSaleTicketClass.TicketClassId,
                TicketClassName = parkSaleTicketClass.SaleTicketClassName,
                RuleItem = parkSaleTicketClass.InParkRule.MapTo<InParkRuleCacheItem>()
            };
            return item;
        }

        ///// <summary>
        ///// 取代理商票类的验票缓存项
        ///// </summary>
        ///// <param name="agencySaleTicketClassId"></param>
        ///// <returns></returns>
        //public async Task<TicketClassCacheItem> GetAgencyTicketClassCacheItem(int agencySaleTicketClassId)
        //{
        //    return await _cacheManager.GetCache<int, TicketClassCacheItem>("AgencySaleTicketClassCheckItemCache").GetAsync(agencySaleTicketClassId,
        //            async () =>
        //            {
        //                var agencySaleTicketClass = await _agencySaleTicketClassRepository.FirstOrDefaultAsync(agencySaleTicketClassId);
        //                if (agencySaleTicketClass == null)
        //                    return null;
        //                var item = await GetParkTicketClassCacheItem(agencySaleTicketClass.ParkSaleTicketClassId);
        //                if (item == null)
        //                    return null;
        //                item.TicketClassName = agencySaleTicketClass.ParkSaleTicketClass.SaleTicketClassName;
        //                return item;
        //            });
        //}

        /// <summary>
        /// 取代理商票类的验票缓存项
        /// </summary>
        /// <param name="agencySaleTicketClassId"></param>
        /// <returns></returns>
        public async Task<TicketClassCacheItem> GetAgencyTicketClassItem(int agencySaleTicketClassId)
        {
            var agencySaleTicketClass = await _agencySaleTicketClassRepository.FirstOrDefaultAsync(agencySaleTicketClassId);
            if (agencySaleTicketClass == null)
                throw new ArgumentNullException($"获取代理商可售票类失败, {agencySaleTicketClassId}");
            if (agencySaleTicketClass.ParkSaleTicketClass == null)
                throw new ArgumentNullException($"获取代理商可售票类失败, {agencySaleTicketClassId} ParkSaleTicketClass:{agencySaleTicketClass.ParkSaleTicketClassId}");

            var item = await GetParkTicketClassItem(agencySaleTicketClass.ParkSaleTicketClassId);
            item.TicketClassName = agencySaleTicketClass.ParkSaleTicketClass.SaleTicketClassName;
            return item;
        }

        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="verifyCode">闸机接受码</param>
        /// <param name="id">条码/IC卡号等唯一码</param>
        /// <param name="noPast">剩下没过闸的人数</param>
        /// <param name="remark"></param>
        /// <param name="terminal"></param>
        public async Task<bool> WriteInPark(string verifyCode, string id, int noPast, string remark, int terminal)
        {
            if (verifyCode.Length == 13)
            {
                //写老系统余下票的入园记录
                return await CheckLeftTicketStrategy.WriteInPark(verifyCode, noPast, terminal);

            }
            // 先找出验票缓存数据
            var ticketCheckData = await GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);

            if (ticketCheckData == null)
                return false;

            var jobArgs = WriteInParkHelper.GetInParkJobArgs(ticketCheckData, id, noPast, remark, terminal);
            if (jobArgs == null)
                return false;

            GetTicketCheckDataCache().Set(verifyCode, ticketCheckData);
            if (jobArgs.Persons > 0)
            {
                _backgroundJobManager.Enqueue<WriteInParkJob, WriteInParkJobArgs>(jobArgs);
            }
            return true;
        }


        /// <summary>
        /// 启动线程监控门票数据表记录        
        /// </summary>
        public void Start()
        {
            //int _parkId ;
            if (int.Parse(ConfigurationManager.AppSettings["LocalParkCode"]) == 0)
            {
                return;
            }

            WriteInParkHelper.Init();

            _ticketTracker = new TicketTracker(new ThemeParkDbContext(), this);
            _exit = false;

            //bool failed = false;
            //Work.Retry(() => _parkId = _settingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId), null,
            //    () =>
            //    {
            //        failed = true;
            //        var phones = ConfigurationManager.AppSettings["AlarmPhone"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //        _smsAppService.SendMessage(phones, "启动 CheckTicketManager线程 失败，尝试重新启动或其他处理方式。");
            //    }, 3);

            //if (failed)
            //{
            //    return;
            //}
            //_fingerType = 0;
            //_fingerType = (ZWJType)Convert.ToInt32(IocManager.Instance.Resolve<IVIPCardAppService>().GetCardSettingValue(SaleCardSetting.FingerType, _parkId));

            // ToDo: 将有效票加到缓存

            //启动监控门票线程
            _threadCheck = new Thread(new ThreadStart(this.Update));
            _threadCheck.Start();
        }

        /// <summary>
        /// 根据入园规则和票的有效天数判断是否可入园，计算可入园人数
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        public bool CheckTicketByRule(TicketCheckData checkData)
        {
            #region 根据入园规则判断

            var rule = checkData.GetInParkRull();

            #region 入园时间判断

            var timeOfNow = DateTime.Now.TimeOfDay;

            if (timeOfNow < rule.InParkTimeBegin.TimeOfDay || timeOfNow > rule.InParkTimeEnd.TimeOfDay)
            {
                checkData.CheckState = CheckState.Idle;
                checkData.Message = "不在规定入园时间内";
                return false;
            }

            #endregion 入园时间判断

            #region 是否有节假日限制 
            if (rule.HolidayLimit)
            {
                if (VerifyTicketHelper.IsHoliday(DateTime.Today))
                {
                    checkData.CheckState = CheckState.Idle;
                    checkData.Message = "节假日不能入园";
                    return false;
                }
            }
            #endregion 判断票的状态

            #region  是否有日期限制

            //入园规则中的是否有日期限制选项
            if (rule.DateLimit)
            {
                //入园规则的入园后有效天数是给套票用的
                // 验证是否还没到入园日期(入园日期大于今天，表示未到入园时间)
                if (checkData.ValidStartDate.Date > DateTime.Today)
                {
                    checkData.CheckState = CheckState.Idle;
                    checkData.Message = "未到入园开始日期";
                    return false;
                }

                if (rule.TicketClassMode == TicketClassMode.MultiParkTicket && !string.IsNullOrWhiteSpace(checkData.InParkInfo))
                {
                    //套票已入园 不作判断
                }
                //判断是否过期(启用日期加上有效期)
                else if (checkData.ValidStartDate.AddDays(checkData.ValidDays).Date < DateTime.Today)
                {
                    checkData.CheckState = CheckState.Expired;
                    checkData.Message = "已过有效期";
                    return false;
                }
            }

            //入园信息为空时，按入园规则里面的有效天数验证是否过期
            if (!string.IsNullOrWhiteSpace(checkData.InParkInfo))
            {
                //取第一次入园日期
                var firstInParkDate = VerifyTicketHelper.GetFirstInParkDate(checkData.InParkInfo);

                //判断是否过期(启用日期加上入园规则里面的有效期)
                if (firstInParkDate.AddDays(rule.InParkValidDays) < DateTime.Today)
                {
                    checkData.CheckState = CheckState.Expired;
                    checkData.Message = "已过有效期";
                    return false;
                }
            }

            #endregion 是否有日期限制

            #region 是否有次数限制

            //当天的可入总次数（票次*票数*票包含人数(票类里面的人数)）
            int inParkTimes = rule.InParkTimesPerDay * checkData.GetPersons().Value;

            //当天已入园次数大于可入园次数则返回
            if (VerifyTicketHelper.GetTodayInParkTimes(checkData.InParkInfo) >= inParkTimes)
            {
                checkData.CheckState = CheckState.Invalid;
                checkData.Message = "当天入园次数多";
                return false;
            }

            //单公园的可入总次数（票次*票数*票包含人数）
            var inParkTimesPerPark = rule.InParkTimesPerPark * checkData.GetPersons();

            //单公园已入园次数大于可入园次数则返回
            if (VerifyTicketHelper.GetParkInParkTimes(checkData.ParkId, checkData.InParkInfo) >= inParkTimesPerPark)
            {
                checkData.CheckState = CheckState.Invalid;
                checkData.Message = "本公园入园次数多";
                return false;
            }

            #endregion 是否有次数限制

            #region 判断验票类型，并构造返回结果
            //判断验票类型，并构造返回结果
            switch (rule.TicketClassMode)
            {
                //多园套票
                case TicketClassMode.MultiParkTicket:
                    checkData.VerifyType = VerifyType.MultiTicket;

                    //判断是否已登记
                    bool needRegister = _multiTicketEnterEnrollRepository.GetAll().All(p => p.Barcode != checkData.VerifyCode);

                    //todo:是否登记还是验证通过MultiTicketEnterEnroll来判断
                    checkData.MultiTicketDto = new VerifyMultiTicketDto()
                    {
                        NeedFinger = rule.CheckFinger,
                        NeedRegister = needRegister,
                        NeedPhoto = rule.CheckPhoto,
                        FromParkId = checkData.ParkId,
                        FromParkName = "",
                        Id = checkData.VerifyCode,
                        DisplayName = checkData.GetTicketClassName(),
                        Persons = 1,
                        Remark = "",
                        FaceId = long.Parse(checkData.VerifyCode)
                    };
                    checkData.AllowPersons = 1;
                    break;
                //年卡
                case TicketClassMode.YearCard:
                //多园年卡
                case TicketClassMode.MultiYearCard:
                    checkData.VerifyType = VerifyType.VIPCard;
                    checkData.VIPCardDto = new VerifyVIPCardDto()
                    {
                        NeedFinger = rule.CheckFinger,
                        NeedPhoto = rule.CheckPhoto,
                        Id = checkData.VerifyCode,
                        VipCardId = checkData.VIPCardDto?.VipCardId,
                        DisplayName = checkData.GetTicketClassName(),
                        Persons = 1,
                        Remark = "",
                        FaceId = long.Parse(checkData.VerifyCode, NumberStyles.AllowHexSpecifier)
                    };
                    checkData.AllowPersons = 1;
                    break;
                //常规票
                case TicketClassMode.Normal:
                default:
                    checkData.VerifyType = VerifyType.CommonTicket;
                    checkData.CommonTicketDto = new VerifyCommonTicketDto()
                    {
                        Id = checkData.VerifyCode,
                        DisplayName = checkData.GetTicketClassName(),
                        //Persons = checkData.Persons - checkData.InPersons,
                        Remark = ""
                    };


                    //每天允许入园人数
                    var perday = checkData.GetInParkRull().InParkTimesPerDay * checkData.GetPersons();

                    //总共还剩下多少次数
                    var temp = perday - checkData.InPersons;

                    if (temp < checkData.GetPersons())
                    {
                        checkData.CommonTicketDto.Persons = temp.Value;
                    }
                    else
                    {
                        checkData.CommonTicketDto.Persons = checkData.GetPersons().Value;
                    }
                    checkData.AllowPersons = checkData.CommonTicketDto.Persons;
                    break;
            }
            #endregion 判断验票类型，并构造返回结果

            #endregion 根据入园规则判断
            return true;

        }

        /// <summary>
        /// 检查入园单入园规则
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        public bool CheckInparkBillByRule(TicketCheckData checkData)
        {
            // 验证是否还没到入园日期(入园日期大于今天，表示未到入园时间)
            if (checkData.ValidStartDate.Date > DateTime.Today)
            {
                checkData.CheckState = CheckState.Idle;
                checkData.Message = "未到入园开始日期";
                return false;

            }

            //判断是否过期(启用日期加上有效期)
            if (checkData.ValidStartDate.Date < DateTime.Today && checkData.ValidStartDate.AddDays(checkData.ValidDays).Date < DateTime.Today)
            {
                checkData.CheckState = CheckState.Expired;
                checkData.Message = "已过有效期";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止监控门票线程
        /// </summary>
        public void Stop()
        {
            _exit = true;
            while (_threadCheck.IsAlive)
                Thread.Sleep(1000);
        }

        private void Update()
        {
            //执行一次
            //IocManager.Instance.Resolve<FingerCache>().CreateFingerCache();
            while (!_exit)
            {
                try
                {
                    _ticketTracker.CaptureChange();
                }
                catch (Exception ex)
                {
                    Logger.Error("启动线程监控门票数据表记录 出现错误：" + ex);
                }

                Thread.Sleep(1000 * 30);
            }
        }

        ///// <summary>
        ///// 启动缓存更新线程
        ///// </summary>
        //public void CreateFingerCache()
        //{
        //    IocManager.Instance.Resolve<FingerCache>().CreateFingerCache();
        //    //FingerCache.CreateFingerCache();

        //    //if (_fingerType == ZWJType.ZK)
        //    //{
        //    //    if (DateTime.Now - _lastCreateFingerZkCacheTime > TimeSpan.FromMinutes(3))
        //    //    {
        //    //        _lastCreateFingerZkCacheTime = DateTime.Now;
        //    //        FingerCache.CreateFingerZkCache();
        //    //    }
        //    //}
        //}
    }
}
