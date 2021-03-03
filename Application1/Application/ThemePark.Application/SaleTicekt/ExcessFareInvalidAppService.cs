using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 作废票应用服务
    /// </summary>
    public class ExcessFareInvalidAppService : ThemeParkAppServiceBase, IExcessFareInvalidAppService
    {
        #region Fields
        private readonly IRepository<ExcessFareInvalid,string> _excessFareInvalidRepository;
        #endregion


        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="excessFareInvalidRepository"></param>
        public ExcessFareInvalidAppService(IRepository<ExcessFareInvalid,string> excessFareInvalidRepository)
        {
            _excessFareInvalidRepository = excessFareInvalidRepository;
        }
        #endregion


        #region Public Methods

        /// <summary>
        /// 添加作废票记录
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddExcessFareInvalid(ExcessFareInvalidDto dto)
        {
            var entity = dto.MapTo<ExcessFareInvalid>();

            await _excessFareInvalidRepository.InsertAsync(entity);

            return Result.Ok();
        }

        #endregion
    }
}
