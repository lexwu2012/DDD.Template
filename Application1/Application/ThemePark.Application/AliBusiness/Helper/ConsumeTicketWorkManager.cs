using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Abp.Dependency;
using Castle.Core.Logging;
using ThemePark.Application.AliBusiness.Dto;
using ThemePark.Application.AliBusiness.Enum;
using ThemePark.EntityFramework;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ThemePark.Application.AliBusiness.Helper
{
    /// <summary>
    /// 核销订单工作者
    /// </summary>
    public class ConsumeTicketWorkManager : ISingletonDependency
    {
        #region Fields
        //private readonly ILoggerFactory _loggerFactory;
        //private readonly ILogger _logger;
        private Thread _readOrderthread;
        private Dictionary<int, Thread> activeThreads = new Dictionary<int, Thread>();
        #endregion

        #region cotr
        public ConsumeTicketWorkManager()
        {
            //_loggerFactory = IocManager.Instance.Resolve<ILoggerFactory>();
            //_logger = _loggerFactory.Create(typeof(ConsumeTicketWorkManager));
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            _readOrderthread = new Thread(DoWork) { IsBackground = true };
            _readOrderthread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Parallel.ForEach(activeThreads, thread => { thread.Value.Abort(); });
            _readOrderthread.Abort();
        }

        /// <summary>
        /// 创建各个公园核销线程
        /// </summary>
        protected void DoWork()
        {
            while (true)
            {
                //对本地数据库进行查询需要核销的订单
                var dbContext = IocManager.Instance.Resolve<ThemeParkDbContext>();
                var parkIds = dbContext.TmallOrderDetails.Select(m => m.ParkId).Distinct();

                foreach (var parkId in parkIds)
                {
                    if (!activeThreads.ContainsKey(parkId))
                    {
                        var thread = new Thread(() => IocManager.Instance.Resolve<ConsumeTicketJob>().Execute(parkId))
                        {
                            IsBackground = true
                        };
                        thread.Start();
                        activeThreads.Add(parkId, thread);
                    }
                }
                //轮空6小时再检测是否有新公园订单
                Thread.Sleep(1000 * 60 * 60 * 6);
            }
        }
    }
}
