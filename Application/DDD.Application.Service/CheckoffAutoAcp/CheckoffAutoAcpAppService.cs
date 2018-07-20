using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.Web.Query;

namespace DDD.Application.Service.CheckoffAutoAcp
{
    public class CheckoffAutoAcpAppService : AppServiceBase, ICheckoffAutoAcpAppService
    {
        private readonly IRepositoryWithEntity<Domain.Core.Model.CheckoffAutoAcp> _checkoffAutoAcpRepository;

        public CheckoffAutoAcpAppService(IRepositoryWithEntity<Domain.Core.Model.CheckoffAutoAcp> checkoffAutoAcpRepository)
        {
            _checkoffAutoAcpRepository = checkoffAutoAcpRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result UpdateCheckoffAutoAcp(int id)
        {
            var entity = _checkoffAutoAcpRepository.GetAll().FirstOrDefault(m => m.Id == id);
            if (null == entity)
                return Result.FromError("没有该记录");
            entity.SendTime = DateTime.Now;

            return Result.Ok();
        }

        public Result<IQueryable<Domain.Core.Model.CheckoffAutoAcp>> GetTodayCheckoffAutoAcp()
        {
            var obj = _checkoffAutoAcpRepository.GetAll()
                .Where(m => DbFunctions.DiffDays(m.CreationTime, DateTime.Now) > 0);

            return Result.FromData(obj);
        }

        /// <summary>
        /// 获取指定条件的单条代扣数据
        /// </summary>
        public async Task<TDto> GetCheckoffAutoAcpAsync<TDto>(IQuery<Domain.Core.Model.CheckoffAutoAcp> query)
        {
            return await _checkoffAutoAcpRepository.AsNoTracking().FirstOrDefaultAsync<Domain.Core.Model.CheckoffAutoAcp, TDto>(query);
        }

        /// <summary>
        /// 获取指定条件的代扣数据列表
        /// </summary>
        public async Task<IList<TDto>> GetCheckoffAutoAcpListAsync<TDto>(IQuery<Domain.Core.Model.CheckoffAutoAcp> query)
        {
            return await _checkoffAutoAcpRepository.GetAll().ToListAsync<Domain.Core.Model.CheckoffAutoAcp, TDto>(query);
        }
    }
}
