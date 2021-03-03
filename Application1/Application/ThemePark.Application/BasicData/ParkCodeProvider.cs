using System.Linq;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Uow;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Infrastructure.Web;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 公园信息服务
    /// </summary>
    [DisableAuditing]
    public class ParkCodeProvider : IParkCodeProvider, ITransientDependency
    {
        #region Fields
        
        /// <summary>
        /// The _park repository
        /// </summary>
        private readonly IParkRepository _parkRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// </summary>
        public ParkCodeProvider(IParkRepository parkRepository)
        {
            _parkRepository = parkRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 获取公园编号
        /// </summary>
        /// <param name="parkId">The park identifier.</param>
        /// <returns>System.Int32.</returns>
        [UnitOfWork]
        public int ParkCode(int parkId)
        {
            return _parkRepository.GetAll().Where(o => o.Id == parkId).Select(o => o.ParkCode).First();
        }

        #endregion Methods


    }
}