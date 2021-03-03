using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.AgentTicket;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 子订单应用服务
    /// </summary>
    public class TOBodyAppService : ThemeParkAppServiceBase, ITOBodyAppService
    {
        #region Fields

        private readonly IRepository<TOBody,string> _tOBodyRepository;
        private readonly IRepository<TOBodyPre, string> _tOBodyPreRepository;

        #endregion

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tOBodyRepository"></param>
        public TOBodyAppService(IRepository<TOBody,string> tOBodyRepository, IRepository<TOBodyPre, string> tOBodyPreRepository)
        {
            _tOBodyRepository = tOBodyRepository;
            _tOBodyPreRepository = tOBodyPreRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 根据订单表头Id获取订单详情
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public async Task<List<TDto>> GetTOBodyListAsync<TDto>(IQuery<TOBody> query )
        {
            return await _tOBodyRepository.GetAll().ToListAsync<TOBody, TDto>(query);
        }

        /// <summary>
        /// 根据主订单Id删除子订单
        /// </summary>
        /// <param name="toHeadId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteTOBodyListAsync(string toHeadId)
        {
            await _tOBodyRepository.DeleteAsync(m => m.TOHeaderId == toHeadId);
            
            return Result.Ok();
        }

        #endregion
    }
}
