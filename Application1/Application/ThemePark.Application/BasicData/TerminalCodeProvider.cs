using Abp.Domain.Repositories;
using System.Linq;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Uow;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Web;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 售票终端
    /// </summary>
    /// <seealso cref="ThemePark.Infrastructure.Web.ITerminalCodeProvider" />
    [DisableAuditing]
    public class TerminalCodeProvider : ITerminalCodeProvider, ITransientDependency
    {
        #region Fields

        private readonly IRepository<Terminal> _terminalRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalAppService" /> class.
        /// </summary>
        /// <param name="terminalRepository">The terminal repository.</param>
        public TerminalCodeProvider(IRepository<Terminal> terminalRepository)
        {
            _terminalRepository = terminalRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 获取终端编号
        /// </summary>
        /// <param name="terminalId">The terminal identifier.</param>
        /// <returns>System.Int32.</returns>
        [UnitOfWork]
        public int TerminalCode(int terminalId)
        {
            return _terminalRepository.GetAll().Where(o => o.Id == terminalId).Select(o => o.TerminalCode).FirstOrDefault();
        }

        #endregion Methods
    }
}