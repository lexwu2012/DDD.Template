using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface IAgencySaleTicketClassTemplateAppService : IApplicationService
    {
        /// <summary>
        /// 增加代理商促销门票
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="agencySaleTicketClassSaveNewInputList"></param>
        /// <param name="ticketClassId"></param>
        Task<Result> AddAgencySaleTicketClassTemplateAsync(AgencySaleTicketClassTemplate dto, List<AgencySaleTicketClassSaveNewInput> agencySaleTicketClassSaveNewInputList,int ticketClassId);

        /// <summary>
        /// 删除代理商促销门票
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteAgencySaleTicketClassTemplateAsync(int id);
        
        /// <summary>
        /// 获取代理商促销票类模板列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="agencySaleTicketClassTemplatePageQuery">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<TDto>> GetPagedDataAsync<TDto>(IPageQuery<AgencySaleTicketClassTemplate> agencySaleTicketClassTemplatePageQuery);


        /// <summary>
        /// 根据条件获取代理商促销门票
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetAgencySaleTicketClassTemplateAsync<TDto>(IQuery<AgencySaleTicketClassTemplate> query);

        /// <summary>
        /// 根据条件获取代理商促销门票列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetAgencySaleTicketClassTemplateListAsync<TDto>(IQuery<AgencySaleTicketClassTemplate> query);

        /// <summary>
        /// 根据条件获取代理商促销门票下拉列表
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<DropdownDto> GetPermissionAgencySaleTicketClassTemplateDropdownAsync(Expression<Func<AgencySaleTicketClassTemplate, bool>> exp);

        /// <summary>
        /// 更新代理商促销门票
        /// </summary>
        /// <param name="input"></param>
        Task<Result> UpdateAgencySaleTicketClassTemplateAsync(
            UpdateAgencySaleTicketClassTemplateInput input);
    }
}
