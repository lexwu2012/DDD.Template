using System.Collections.Generic;
using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ICustomerAppService : IApplicationService
    {
        /// <summary>
        /// 获取所有的顾客
        /// </summary>
        /// <returns></returns>
        IList<CustomerDto> GetAllCustomers();

        /// <summary>
        /// 获取顾客
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        CustomerDto GetCustomer(GetCustomerPageInput input);

        /// <summary>
        /// 增加顾客
        /// </summary>
        /// <param name="dto"></param>
        void AddCustomer(CustomerDto dto);

        /// <summary>
        /// 增加顾客并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        CustomerDto AddAndReturnCustomer(CustomerDto dto);

        /// <summary>
        /// 删除顾客
        /// </summary>
        /// <param name="dto"></param>
        void DeleteCustomer(DeleteInput dto);

        /// <summary>
        /// 更新顾客
        /// </summary>
        /// <param name="dto"></param>
        void UpdateCustomer(CustomerDto dto);
    }
}
