using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 导游应用服务
    /// </summary>
    public class GuideCustomerAppService : ThemeParkAppServiceBase, IGuideCustomerAppService
    {
        private readonly IRepository<GuideCustomer> _guideCustRepository;

        #region Cotr

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="guideCustRepository"></param>
        public GuideCustomerAppService(IRepository<GuideCustomer> guideCustRepository)
        {
            _guideCustRepository = guideCustRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加导游信息
        /// </summary>
        public async Task<Result<int>> AddGuideCustAsync(GuideCustInput input)
        {
            var isExist = await _guideCustRepository.AsNoTracking().AnyAsync(o => o.Idcard == input.Idcard && o.IsActive && o.AgencyId == input.AgencyId);
            if (isExist)
            {
                return Result.FromCode<int>(ResultCode.DuplicateRecord);
            }
            if (!Regex.IsMatch(input.Idcard.Trim(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$"))
            {
                return Result.FromError<int>("请输入正确的身份证号格式！");
            }
            if (!Regex.IsMatch(input.Phone.Trim(), @"^(13|15|17|18)[0-9]{9}$"))
            {
                return Result.FromError<int>("请输入正确的手机号格式！");
            }
            GuideCustomer guide = input.MapTo<GuideCustomer>();
            guide.IsActive = true;

            var id = await _guideCustRepository.InsertAndGetIdAsync(guide);

            return Result.FromData(id);
        }

        /// <summary>
        /// 修改导游信息
        /// </summary>
        public async Task<Result> UpdateGuideCustAsync(GuideCustInput input, int agencyId)
        {
            var guide = await _guideCustRepository.FirstOrDefaultAsync(o => o.Id == input.Id && o.AgencyId == agencyId);
            if (guide == null)
                return Result.FromCode(ResultCode.NoRecord);

            if (!Regex.IsMatch(input.Idcard.Trim(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$"))
            {
                return Result.FromError("请输入正确的身份证号格式！");
            }
            if (!Regex.IsMatch(input.Phone.Trim(), @"^(13|15|17|18)[0-9]{9}$"))
            {
                return Result.FromError("请输入正确的手机号格式！");
            }
            //判断修改是否与已有数据重复
            var exist = await _guideCustRepository.GetAll().Where(o => o.AgencyId == agencyId && o.Id != input.Id)
                .AnyAsync(o => o.Idcard == input.Idcard);
            if (exist)
            { return Result.FromCode(ResultCode.DuplicateRecord); }
            guide.Name = input.Name;
            guide.Phone = input.Phone;
            guide.Idcard = input.Idcard;
            await _guideCustRepository.UpdateAsync(guide);
            return Result.Ok();

        }
        /// <summary>
        /// 删除导游信息
        /// </summary>
        public async Task<Result> DelGuideCustAsync(int id, int agencyId)
        {
            var guide = await _guideCustRepository.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id && o.AgencyId == agencyId);
            if (guide == null)
                return Result.FromCode(ResultCode.NoRecord);
            guide.IsActive = false;
            await _guideCustRepository.UpdateAsync(guide);
            return Result.Ok();
        }

        /// <summary>
        /// 查询导游信息
        /// </summary>
        public async Task<IList<GuideCustomerDto>> GetAgencyGuidesAsync(int agencyId, IList<int> guideIds = null)
        {
            IList<GuideCustomer> guides = new List<GuideCustomer>();

            if (guideIds != null)
            {
                foreach (var i in guideIds)
                {
                    var guide = await _guideCustRepository.FirstOrDefaultAsync(o => o.AgencyId == agencyId && o.Id == i);
                    if (guide != null)
                        guides.Add(guide);
                }
            }
            else
            {
                guides = await _guideCustRepository.AsNoTracking().Where(o => o.AgencyId == agencyId).ToListAsync();
            }
            return guides.MapTo<List<GuideCustomerDto>>();
        }
        /// <summary>
        /// 查询导游信息
        /// </summary>
        public async Task<PageResult<TDto>> GetAgencyGuidesAsync<TDto>(IPageQuery<GuideCustomer> query)
        {
            return await _guideCustRepository.AsNoTracking().ToPageResultAsync<GuideCustomer, TDto>(query);
        }

        /// <summary>
        /// 查询导游信息
        /// </summary>
        public async Task<IList<TDto>> GetGuideCustomerListAsync<TDto>(IQuery<GuideCustomer> query)
        {
            return await _guideCustRepository.AsNoTracking().ToListAsync<GuideCustomer, TDto>(query);
        }

        #endregion
        
    }
}
