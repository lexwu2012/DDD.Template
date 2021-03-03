using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.InPark.Interfaces;
using ThemePark.Core.InPark;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.InPark
{
    /// <summary>
    /// 入园记录应用服务
    /// </summary>
    public class TicketInParkAppService : ThemeParkAppServiceBase, ITicketInParkAppService
    {
        #region Fields
        private readonly IRepository<TicketInPark, long> _ticketInParkRepository;
        #endregion Fields

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        public TicketInParkAppService(IRepository<TicketInPark, long> ticketInParkRepository)
        {
            _ticketInParkRepository = ticketInParkRepository;
        }
        #endregion Cotr

        #region Public Methods

        /// <summary>
        /// 根据条件获取入园记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TDto> GetTicketInParkAsync<TDto>(IQuery<TicketInPark> query)
        {
            return await _ticketInParkRepository.AsNoTracking().FirstOrDefaultAsync<TicketInPark, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取入园记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetTicketInParkListAsync<TDto>(IQuery<TicketInPark> query)
        {
            return await _ticketInParkRepository.AsNoTracking().ToListAsync<TicketInPark, TDto>(query);
        }

        #endregion Public Methods
    }
}
