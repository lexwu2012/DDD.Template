using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.Trade.Dto;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Core.TradeInfos;

namespace ThemePark.Application.Trade
{
    public class PayModeAppService : ThemeParkAppServiceBase, IPayModeAppService
    {
        #region Fields
        private readonly IRepository<PayMode> _payModeRepository;
        #endregion

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="payModeRepository"></param>
        public PayModeAppService(IRepository<PayMode> payModeRepository)
        {
            _payModeRepository = payModeRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 根据Id获取付费方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns>返回该实体Model，其中该实体包含自己所需的字段</returns>
        public Task<TDto> GetPayModeByIdAsync<TDto>(int id)
        {
            return _payModeRepository.GetAll().Where(m => m.Id == id).ProjectTo<TDto>().FirstOrDefaultAsync();
        }

        #endregion
    }
}
