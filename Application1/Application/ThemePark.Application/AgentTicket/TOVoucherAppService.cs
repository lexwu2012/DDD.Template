using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Core.AgentTicket;
using Castle.Core.Internal;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket
{
    public class TOVoucherAppService : ThemeParkAppServiceBase, ITOVoucherAppService
    {
        private readonly IRepository<TOVoucher,string> _tOVoucherRepository;

        public TOVoucherAppService(IRepository<TOVoucher,string> tOVoucherRepository, ITOBodyAppService toBodyAppService, ITOVoucherDomainService toVoucherDomainService)
        {
            _tOVoucherRepository = tOVoucherRepository;
        }

        #region Public Methods

        ///// <summary>
        ///// 根据条件返回凭证
        ///// </summary>
        ///// <typeparam name="TDto"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public async Task<Result<TDto>> GetTOVoucherAsync<TDto>(Query<TOVoucher> query)
        //{
        //    var data = await _tOVoucherRepository.GetAll()<TOVoucher, TDto>(query);
        //    if (data == null)
        //        return Result.FromCode<TDto>(ResultCode.NoRecord);
        //    return Result.FromData(data);
        //}

        /// <summary>
        /// 根据条件返回凭证列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Result<List<TDto>>> GetTOVoucherListAsync<TDto>(Query<TOVoucher> query)
        {
            var list = await _tOVoucherRepository.GetAll().ToListAsync<TOVoucher, TDto>(query);
            if (list.IsNullOrEmpty())
                return Result.FromCode<List<TDto>>(ResultCode.NoRecord);
            return Result.FromData(list);
        }

      

        #endregion
    }
}
