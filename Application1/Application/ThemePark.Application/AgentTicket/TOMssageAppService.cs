using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket
{
    public class TOMssageAppService : ThemeParkAppServiceBase, ITOMssageAppService
    {
        #region Fields

        private readonly IRepository<TOMessage> _messageRepository;

        #endregion

        #region Ctor

        public TOMssageAppService(IRepository<TOMessage> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 增加门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        public void AddTOModify(TOMessageDto dto)
        {
            var message = dto.MapTo<TOMessage>();

            _messageRepository.Insert(message);
        }

        /// <summary>
        /// 增加门票订单短信并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TOMessageDto AddAndReturnTOModify(TOMessageDto dto)
        {
            var message = dto.MapTo<TOMessage>();

            _messageRepository.Insert(message);

            UnitOfWorkManager.Current.SaveChanges();

            message = _messageRepository.Get(message.Id);

            return message.MapTo<TOMessageDto>();
        }

        /// <summary>
        /// 更新门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateTOModify(TOMessageDto dto)
        {
            _messageRepository.Update(dto.MapTo<TOMessage>());
        }

        /// <summary>
        /// 删除门票订单短信
        /// </summary>
        /// <param name="deleteInput"></param>
        public void DeleteTOModify(DeleteInput deleteInput)
        {
            _messageRepository.Delete(m => m.Id == deleteInput.Id);
        }

        /// <summary>
        /// 获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TOMessageDto GetTOModify(TOMessageDto dto)
        {
            var mes = _messageRepository.Get(dto.Id);

            if (mes != null)
            {
                return mes.MapTo<TOMessageDto>();
            }

            return null;
        }

        /// <summary>
        /// 根据订单头Id获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<TOMessageDto> GetByHeaderId(TOMessageDto dto)
        {
            //var t = _messageRepository.GetAll().Where(c => c.TOHeadId == dto.TOHeadId);

            //if (t != null)
            //{
            //    return t.MapTo<IList<TOMessageDto>>();
            //}

            return null;
        }

        /// <summary>
        /// 根据顾客Id获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<TOMessageDto> GetByCustomerId(TOMessageDto dto)
        {
            var t = _messageRepository.GetAll().Where(c => c.CustomerId == dto.CustomId);

            if (t != null)
            {
                return t.MapTo<IList<TOMessageDto>>();
            }

            return null;
        }
        #endregion
    }
}
