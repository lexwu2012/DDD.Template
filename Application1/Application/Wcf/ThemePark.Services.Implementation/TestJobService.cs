using System;
using System.ServiceModel;
using ThemePark.Application.Test;
using ThemePark.ApplicationDto.Test;
using ThemePark.Services.Interface;

namespace ThemePark.Services.Implementation
{
    public class TestJobService : ITestJobService, IDisposable
    {
        #region Fields

        private readonly ITestJobAppService _testJobAppService;

        private int _count;

        #endregion

        #region Ctor
        /// <summary>
        /// 初始化 <see cref="TestJobService"/> 类的新实例。
        /// </summary>
        public TestJobService(ITestJobAppService testJobAppService)
        {
            _testJobAppService = testJobAppService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 获取TestJob
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TestJobDto GetJob(FindTestJobInput input)
        {
            Console.WriteLine("sessionid: {0}", OperationContext.Current.SessionId);
            _count ++;
            Console.WriteLine($"count: {_count}");
            return _testJobAppService.GetJob(input);
        }

        /// <summary>
        /// 插入新的TestJob
        /// </summary>
        /// <param name="job"></param>
        public void InsertJob(AddTestJobInput job)
        {
            _testJobAppService.InsertJob(job);
        }

        #endregion

        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose()
        {
            Console.WriteLine(this.GetType().Name + " disposed.");
        }
    }
}
