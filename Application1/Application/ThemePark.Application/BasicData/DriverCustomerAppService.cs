using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 司陪人员信息
    /// </summary>
    public class DriverCustomerAppService : ThemeParkAppServiceBase, IDriverCustomerAppService
    {
        private readonly IRepository<DriverCustomer> _driverCustomerRepository;

        #region Cotr

        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="driverCustomerRepository"></param>
        public DriverCustomerAppService(IRepository<DriverCustomer> driverCustomerRepository)
        {
            _driverCustomerRepository = driverCustomerRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加司陪信息
        /// </summary>
        public async Task<Result<int>> AddDriverCustomerAsync(DriverCustomerInput input)
        {
            var isExist = await _driverCustomerRepository.AsNoTracking().AnyAsync(o => o.IsActive && o.Idcard == input.Idcard && o.AgencyId == input.AgencyId);
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
            DriverCustomer driver = input.MapTo<DriverCustomer>();
            driver.IsActive = true;
            var id = await _driverCustomerRepository.InsertAndGetIdAsync(driver);
            return Result.FromData(id);
        }

        /// <summary>
        /// 修改司陪信息
        /// </summary>
        public async Task<Result> UpdateDriverCustomerAsync(DriverCustomerInput input, int agencyId)
        {
            var driver = await _driverCustomerRepository.FirstOrDefaultAsync(o => o.Id == input.Id && o.AgencyId == agencyId);
            if (driver == null)
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
            var exist = await _driverCustomerRepository.GetAll().Where(o => o.AgencyId == agencyId && o.Id != input.Id)
                .AnyAsync(o => o.Idcard == input.Idcard);
            if (exist)
            { return Result.FromCode(ResultCode.DuplicateRecord); }

            driver.Name = input.Name;
            driver.Phone = input.Phone;
            driver.Idcard = input.Idcard;
            await _driverCustomerRepository.UpdateAsync(driver);
            return Result.Ok();
        }

        /// <summary>
        /// 删除司陪信息
        /// </summary>
        public async Task<Result> DelDriverCustomerAsync(int id, int agencyId)
        {
            var driver = await _driverCustomerRepository.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id && o.AgencyId == agencyId);
            if (driver == null)
                return Result.FromCode(ResultCode.NoRecord);
            driver.IsActive = false;
            await _driverCustomerRepository.UpdateAsync(driver);
            return Result.Ok();
        }

        /// <summary>
        /// 查询司陪信息
        /// </summary>
        public async Task<IList<DriverCustomerDto>> GetDriverCustomerAsync(int agencyId, IEnumerable<int> driverIds = null)
        {
            IList<DriverCustomer> drivers = new List<DriverCustomer>();

            if (driverIds != null)
            {
                foreach (var i in driverIds)
                {
                    var guide = await _driverCustomerRepository.FirstOrDefaultAsync(o => o.AgencyId == agencyId && o.Id == i);
                    if (guide != null)
                        drivers.Add(guide);
                }
            }
            else
            {
                drivers = await _driverCustomerRepository.AsNoTracking().Where(o => o.AgencyId == agencyId).ToListAsync();
            }
            return drivers.MapTo<IList<DriverCustomerDto>>();
        }

        /// <summary>
        /// 查询司陪信息
        /// </summary>
        public async Task<PageResult<TDto>> GetDriverCustomerAsync<TDto>(IPageQuery<DriverCustomer> query)
        {
            return await _driverCustomerRepository.AsNoTracking().ToPageResultAsync<DriverCustomer, TDto>(query);
        }

        /// <summary>
        /// 查询司陪信息
        /// </summary>
        public async Task<IList<TDto>> GetDriverCustomerListAsync<TDto>(IQuery<DriverCustomer> query)
        {
            return await _driverCustomerRepository.AsNoTracking().ToListAsync<DriverCustomer, TDto>(query);
        }

        #endregion
        
    }
}
