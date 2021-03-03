using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 预订子订单应用服务
    /// </summary>
    public class TOBodyPreAppService : ThemeParkAppServiceBase, ITOBodyPreAppService
    {
        #region Fields

        private readonly IRepository<TOBodyPre, string> _tOBodyPreRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="tOBodyPreRepository"></param>
        public TOBodyPreAppService(IRepository<TOBodyPre, string> tOBodyPreRepository)
        {
            _tOBodyPreRepository = tOBodyPreRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 根据条件获取所有子订单
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> GetTOBodyPreListAsync<TDto>(IQuery<TOBodyPre> query)
        {
            return await _tOBodyPreRepository.GetAll().ToListAsync<TOBodyPre, TDto>(query);
        }
        #endregion
    }
}
