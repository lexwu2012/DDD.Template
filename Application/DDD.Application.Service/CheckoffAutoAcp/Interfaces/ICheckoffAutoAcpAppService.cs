using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.Domain.Application;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.Web.Query;

namespace DDD.Application.Service.CheckoffAutoAcp.Interfaces
{
    public interface ICheckoffAutoAcpAppService : IApplicationService
    {
        Result<CheckoffAutoAcpDto> UpdateCheckoffAutoAcp(int id);

        Result<IQueryable<Domain.Core.Model.CheckoffAutoAcp>> GetTodayCheckoffAutoAcp();

        Task<TDto> GetCheckoffAutoAcpAsync<TDto>(IQuery<Domain.Core.Model.CheckoffAutoAcp> query);

        /// <summary>
        /// 获取指定条件的代扣数据列表
        /// </summary>
       Task<IList<TDto>> GetCheckoffAutoAcpListAsync<TDto>(IQuery<Domain.Core.Model.CheckoffAutoAcp> query);
    }
}
