using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket
{
    public class TravelOrderConfirmAppService : ThemeParkAppServiceBase, ITravelOrderConfirmAppService
    {
        #region Fields

        private readonly IRepository<TravelOrderConfirm, long> _travelOrderConfirmAppServiceRepository;

        #endregion

        #region Cotr

        public TravelOrderConfirmAppService(IRepository<TravelOrderConfirm, long> travelOrderConfirmAppServiceRepository)
        {
            _travelOrderConfirmAppServiceRepository = travelOrderConfirmAppServiceRepository;
        }

        #endregion

        #region Public Methods

        // <summary>
        /// 增加订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        public void AddTravelOrderConfirm(TravelOrderConfirmDto dto)
        {
            var confirm = dto.MapTo<TravelOrderConfirm>();

            _travelOrderConfirmAppServiceRepository.Insert(confirm);
        }

        /// <summary>
        /// 增加订单确认记录并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TravelOrderConfirmDto AddAndReturnTravelOrderConfirm(TravelOrderConfirmDto dto)
        {
            var confirm = dto.MapTo<TravelOrderConfirm>();

            _travelOrderConfirmAppServiceRepository.Insert(confirm);

            UnitOfWorkManager.Current.SaveChanges();

            confirm = _travelOrderConfirmAppServiceRepository.Get(confirm.Id);

            return confirm.MapTo<TravelOrderConfirmDto>();
        }

        /// <summary>
        /// 更新订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateTravelOrderConfirm(TravelOrderConfirmDto dto)
        {
            _travelOrderConfirmAppServiceRepository.Update(dto.MapTo<TravelOrderConfirm>());
        }

        /// <summary>
        /// 删除订单确认记录
        /// </summary>
        /// <param name="deleteInput"></param>
        public void DeleteTravelOrderConfirm(DeleteInput deleteInput)
        {
            _travelOrderConfirmAppServiceRepository.Delete(m => m.Id == deleteInput.Id);
        }

        /// <summary>
        /// 根据订单头Id获取订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<TravelOrderConfirmDto> GetByTOHeaderId(TravelOrderConfirmDto dto)
        {
            //var t = _travelOrderConfirmAppServiceRepository.GetAll().Where(c => c.OrderId == dto.TOHeadId);

            //if (t != null)
            //{
            //    return t.MapTo<IList<TravelOrderConfirmDto>>();
            //}

            return null;
        }

        /// <summary>
        /// 获取订单确认
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TravelOrderConfirmDto GetTravelOrderConfirm(TravelOrderConfirmDto dto)
        {
            return _travelOrderConfirmAppServiceRepository.Get(dto.Id).MapTo<TravelOrderConfirmDto>();
        }

        #endregion
    }
}
