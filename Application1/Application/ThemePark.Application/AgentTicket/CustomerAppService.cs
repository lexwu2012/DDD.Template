using Abp.AutoMapper;
using System.Collections.Generic;
using System.Linq;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;

namespace ThemePark.Application.AgentTicket
{
    public class CustomerAppService : ThemeParkAppServiceBase, ICustomerAppService
    {

        #region Fields

        private readonly ICustomerRepository _customerRepository;

        #endregion

        #region Ctor

        public CustomerAppService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 获取所有的顾客
        /// </summary>
        /// <returns></returns>
        public IList<CustomerDto> GetAllCustomers()
        {
            return _customerRepository.GetAllList().MapTo<IList<CustomerDto>>();
        }

        /// <summary>
        /// 获取顾客
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public CustomerDto GetCustomer( GetCustomerPageInput input )
        {
            var customer = _customerRepository.GetAll().Where( p => p.Pid == input.Pid );

            return customer.MapTo<CustomerDto>();
        }

        /// <summary>
        /// 增加顾客
        /// </summary>
        /// <param name="dto"></param>
        public void AddCustomer(CustomerDto dto)
        {
            var customer = dto.MapTo<Customer>();

            _customerRepository.Insert(customer);
        }

        /// <summary>
        /// 增加顾客并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        public CustomerDto AddAndReturnCustomer(CustomerDto dto)
        {
            var customer = dto.MapTo<Customer>();

            _customerRepository.Insert(customer);

            UnitOfWorkManager.Current.SaveChanges();

            return customer.MapTo<CustomerDto>();
        }

        /// <summary>
        /// 删除顾客
        /// </summary>
        /// <param name="dto"></param>
        public void DeleteCustomer(DeleteInput input)
        {
            _customerRepository.Delete(c => c.Id == input.Id);
        }

        /// <summary>
        /// 更新顾客
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateCustomer(CustomerDto dto)
        {
            _customerRepository.Update(dto.MapTo<Customer>());
        }

        #endregion
    }
}
