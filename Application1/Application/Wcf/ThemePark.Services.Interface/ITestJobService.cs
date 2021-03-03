using System.ServiceModel;
using ThemePark.ApplicationDto.Test;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Interface
{
    [ServiceContract(Namespace = "http://www.themepark.com")]
    public interface ITestJobService : IWcfService
    {
        /// <summary>
        /// 获取TestJob
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [OperationContract]
        TestJobDto GetJob(FindTestJobInput input);

        /// <summary>
        /// 插入新的TestJob
        /// </summary>
        /// <param name="job"></param>
        [OperationContract]
        void InsertJob(AddTestJobInput job);
    }
}
