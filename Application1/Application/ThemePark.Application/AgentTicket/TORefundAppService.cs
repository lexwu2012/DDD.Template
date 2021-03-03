using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket
{
    public class TORefundAppService : ThemeParkAppServiceBase, ITORefundAppService
    {
        #region Fields

        private readonly IRepository<TORefund, long> _tORefundRepository;

        #endregion

        #region Ctor
        public TORefundAppService(IRepository<TORefund, long> tORefundRepository)
        {
            _tORefundRepository = tORefundRepository;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 增加订单退款
        /// </summary>
        /// <param name="dto"></param>
        public void AddTORefund(TORefundDto dto)
        {
            var refund = dto.MapTo<TORefund>();

            _tORefundRepository.Insert(refund);
        }

        /// <summary>
        /// 增加订单退款记录并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TORefundDto AddAndReturnTORefund(TORefundDto dto)
        {
            var refund = dto.MapTo<TORefund>();

            _tORefundRepository.Insert(refund);

            UnitOfWorkManager.Current.SaveChanges();

            refund = _tORefundRepository.Get(refund.Id);

            return refund.MapTo<TORefundDto>();
        }

        /// <summary>
        /// 更新订单退款记录
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateTORefund(TORefundDto dto)
        {
            _tORefundRepository.Update(dto.MapTo<TORefund>());
        }

        /// <summary>
        /// 删除订单退款
        /// </summary>
        /// <param name="deleteInput"></param>
        public void DeleteTORefund(DeleteInput deleteInput)
        {
            _tORefundRepository.Delete(m => m.Id == deleteInput.Id);
        }

        /// <summary>
        /// 获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TORefundDto GetTORefund(TORefundDto dto)
        {
            var re = _tORefundRepository.Get(dto.Id);

            if (re != null)
            {
                return re.MapTo<TORefundDto>();
            }

            return null;
        }

        /// <summary>
        /// 根据订单头Id获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<TORefundDto> GetByHeaderId(TORefundDto dto)
        {

            //var refund = _tORefundRepository.GetAll().Where(t => t.TOHeaderId == dto.TOHeaderId);

            //if(refund != null)
            //{
            //    return refund.MapTo<IList<TORefundDto>>();
            //}

            return null;
        }

        /// <summary>
        /// 根据修改记录Id获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<TORefundDto> GetByModifyId(TORefundDto dto)
        {
            //var refund = _tORefundRepository.GetAll().Where(t => t.TOModifyId == dto.TOModifyId);

            //if (refund != null)
            //{
            //    return refund.MapTo<IList<TORefundDto>>();
            //}

            return null;
        }

        #endregion
    }
}
