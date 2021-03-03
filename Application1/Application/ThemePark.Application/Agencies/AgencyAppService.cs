using System;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.UI;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core.Agencies;
using ThemePark.Core.Agencies.Repositories;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Common;
using EnumerableExtensions = ThemePark.Common.EnumerableExtensions;

namespace ThemePark.Application.Agencies
{
    /// <summary>
    /// 代理商
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase"/>
    /// <seealso cref="ThemePark.Application.Agencies.Interfaces.IAgencyAppService"/>
    public class AgencyAppService : ThemeParkAppServiceBase, IAgencyAppService
    {
        #region Fields

        private readonly IAddressDomainService _addressDomainService;
        private readonly IAddressRepository _addressRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly IRepository<AgencyUser,long> _agencyUserRepository;
        private readonly IAgencyUserDomainService _agencyUserDomainService;
        private readonly IRepository<Park> _parkRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IParkAgencyRepository _parkAgencyRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<AgencyType> _agencyTypeRepository;
        private readonly IRepository<User, long> _userRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyAppService"/> class.
        /// </summary>
        /// <param name="agencyRepository">The agency repository.</param>
        /// <param name="addressDomainService">The address domain service.</param>
        /// <param name="addressRepository">The address repository.</param>
        /// <param name="parkAgencyRepository"></param>
        /// <param name="agencyUserDomainService"></param>
        /// <param name="userManager"></param>
        /// <param name="agencyTypeRepository"></param>
        /// <param name="userRepository"></param>
        public AgencyAppService(IRepository<Agency> agencyRepository, IAddressDomainService addressDomainService, IAddressRepository addressRepository,
            IParkAgencyRepository parkAgencyRepository, IAgencyUserDomainService agencyUserDomainService, UserManager userManager,
            IRepository<AgencyType> agencyTypeRepository, IRepository<User, long> userRepository, IRepository<Park> parkRepository, IDataSyncManager dataSyncManager, IRepository<AgencyUser, long> agencyUserRepository)
        {
            _addressDomainService = addressDomainService;
            _addressRepository = addressRepository;
            _parkAgencyRepository = parkAgencyRepository;
            _agencyUserDomainService = agencyUserDomainService;
            _userManager = userManager;
            _agencyTypeRepository = agencyTypeRepository;
            _userRepository = userRepository;
            _parkRepository = parkRepository;
            _dataSyncManager = dataSyncManager;
            _agencyUserRepository = agencyUserRepository;
            _agencyRepository = agencyRepository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 新增代理商（默认新增代理商账户）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<Result> AddAgencyAsync(AddAgencyInput input)
        {
            var agencyType = _agencyTypeRepository.GetAll().First(m => m.Id == input.AgencyTypeId);
            var result = NewAgencyBusinessVerify(input, agencyType);
            if (!result.Success)
                return result;

            //1. 地址
            var address = await _addressDomainService.AddAddressAsync(input.ProvinceId,
                input.CityId, null, null, null);

            var agency = input.MapTo<Agency>();
            agency.AddressId = address.Id;

            //todo: 新增账户后同步到每个公园

            //2. 默认新增一个代理商账户
            agency.Account = new Account
            {
                AccountName = agency.AgencyName
            };

            //3. 新增代理商
            var agencyId = await _agencyRepository.InsertAndGetIdAsync(agency);

            //4. OTA和旅行社新增代理商用户
            if (agencyType.DefaultAgencyType != DefaultAgencyType.Other)
            {
                //创建用户
                var user = input.AddAgencyUserInput.User.MapTo<User>();
                if (agencyType.DefaultAgencyType == DefaultAgencyType.Travel)
                {
                    user.UserType = UserType.AgencyUser;
                }
                if (agencyType.DefaultAgencyType == DefaultAgencyType.Ota || agencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)
                {
                    user.UserType = UserType.AgencyApiUser;
                }
                user.Name = input.AgencyName;
                user.IsActive = input.IsActive;
                user.Password = new PasswordHasher().HashPassword(input.Password);

                var identityResult = await _userManager.CreateAsync(user);

                //创建代理商用户
                if (identityResult.Succeeded)
                {
                    var id = await _agencyUserDomainService.CreateAgencyUserAsync(user, agencyId, agencyType.DefaultAgencyType,
                        input.AddAgencyUserInput.HostIPs, input.AddAgencyUserInput.WeChatNo);
                    agency.AgencyUserId = id;
                }
                else
                {
                    throw new UserFriendlyException(identityResult.Errors.JoinAsString(", "));
                }
            }

            await DataSyncAccount(agency.Account, DataSyncType.AddAgencyAccount);

            return Result.FromData(agencyId);
        }

        /// <summary>
        /// 删除代理商
        /// </summary>
        /// <returns></returns>
        public async Task<Result> DeleteAgencyAsync(int id)
        {
            //删除账号，同步数据？
            //有公园代理商则不给删除

            if( _parkAgencyRepository.GetAll().Any(m=> m.AgencyId == id))
                return Result.FromError("该代理商已经配置公园代理商，不允许删除");
            
            //修改该子代理商的ParentAgencyId
            var childAgency = await _agencyRepository.GetAllListAsync(p => p.ParentAgencyId == id);

            foreach (var agency in childAgency)
            {
                await _agencyRepository.UpdateAsync(agency.Id, p => Task.FromResult(p.ParentAgencyId = null));
            }

            //删除代理商
            await _agencyRepository.DeleteAsync(t => t.Id == id);

            //删除代理商用户
            if (_agencyUserRepository.GetAll().Any(m => m.AgencyId == id))
            {
                var agencyUser = _agencyUserRepository.GetAllIncluding(m => m.User).First(m => m.AgencyId == id);
                var result = await _userManager.DeleteAsync(agencyUser.User, UserType.AgencyUser);
                if (!result.Succeeded)
                {
                    throw new UserFriendlyException(result.Errors.JoinAsString(", "));
                }

                await _agencyUserRepository.DeleteAsync(agencyUser);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 异步更新代理商信息
        /// </summary>
        /// <returns></returns>
        public async Task<Result> UpdateAgencyAsync(int id, UpdateAgencyInput input)
        {
            var result = UpdateAgencyBusinessVerify(input);
            if (!result.Success)
                return result;

            var agency = result.Data;
            var agencyType = agency.AgencyType;

            var address = _addressRepository.Get(input.UpdateAddressInput.Id);
            var newAddr = new Address();

            //1. 更新地址(更改的话)
            if (address.ProvinceId != input.UpdateAddressInput.ProvinceId && address.CityId != input.UpdateAddressInput.CityId)
                newAddr = await _addressDomainService.UpdateAddressAsync(address, input.UpdateAddressInput.ProvinceId,
                    input.UpdateAddressInput.CityId, null, null, null);

            input.AddressId = newAddr.Id != 0 ? newAddr.Id : input.UpdateAddressInput.Id;

            input.MapTo(agency);

            //2. 更新代理商信息
            await _agencyRepository.UpdateAsync(agency);

            //3. 更新账户信息            
            if (agencyType.DefaultAgencyType != DefaultAgencyType.Other)
            {
                //3.1 更新用户
                var user = agency.AgencyUser.User;

                user.IsActive = input.IsActive;
                user.Name = input.AgencyName;
                user.UserName = input.UpdateUserInput.UserName;

                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                {
                    throw new UserFriendlyException(userUpdateResult.Errors.JoinAsString(", "));
                }

                //3.2 更新代理商用户
                await _agencyUserDomainService.UpdateAgencyUserAsync(agency.AgencyUser, agencyType.DefaultAgencyType,
                       input.HostIPs, input.WeChatNo);

            }

            //4. 账户名不一样时同步到公园更改账户名
            if (!agency.AgencyName.Equals(input.AgencyName))
            {
                //4.1 更新Account
                agency.Account.AccountName = input.AgencyName;

                await DataSyncAccount(agency.Account, DataSyncType.UpdateAgencyAccount);
            }

            return Result.FromData(input.Id);
        }

        /// <summary>
        /// 批量更新账户有效期
        /// </summary>
        /// <param name="parkAgencyIds"></param>
        /// <param name="startDateTime"></param>
        /// <param name="expirationDateTime"></param>
        /// <returns></returns>
        public Result BulkUpdateParkAgencyExpiredDate(List<int> parkAgencyIds, DateTime startDateTime, DateTime expirationDateTime)
        {
            //Parallel.ForEach(parkAgencyIds, (id, state, i) =>
            //{
            //    var parkAgency = _parkAgencyRepository.GetAll().First(m => m.Id == id);
            //    parkAgency.StartDateTime = startDateTime;
            //    parkAgency.ExpirationDateTime = expirationDateTime;
            //});

            EnumerableExtensions.ForEach(parkAgencyIds, m =>
            {
                var parkAgency = _parkAgencyRepository.GetAll().First(o => o.Id == m);
                parkAgency.StartDateTime = startDateTime;
                parkAgency.ExpirationDateTime = expirationDateTime;
            });

            return Result.Ok();
        }

        /// <summary>
        /// 获取所有代理商
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetAllAgenciesDropdownAsync(IQuery<Agency> query = null)
        {
            var data = await _agencyRepository.AsNoTracking().Where(query)
                        .Select(p => new DropdownItem() { Value = p.Id, Text = p.AgencyName })
                        .ToDropdownDtoAsync();
            return data;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>查询结果</returns>
        public Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<Agency> query = null)
        {
            return _agencyRepository.AsNoTracking().ToPageResultAsync<Agency, TDto>(query);
        }

        /// <summary>
        /// 获取代理商下拉列表（线下的代理商才能充值）
        /// </summary>
        /// <returns></returns>
        public async Task<DropdownDto> GetPreDeposiAgencyListAsync()
        {
            //这两种代理商类型可以挂账和账户充值
            return await _parkAgencyRepository.AsNoTrackingAndInclude(o => o.Agency.AgencyName)
                .Where(o => o.AgencyType.DefaultAgencyType != DefaultAgencyType.Ota && o.AgencyType.DefaultAgencyType != DefaultAgencyType.OwnOta)
                .OrderBy(o => o.AgencyId)
                .ToDropdownDtoAsync(o => new DropdownItem() { Text = o.Agency.AgencyName, Value = o.AgencyId });
        }

        /// <summary>
        /// 获取旅行社
        /// </summary>
        public Task<TDto> GetAgencyAsync<TDto>(IQuery<Agency> query)
        {
            return _agencyRepository.AsNoTracking().FirstOrDefaultAsync<Agency, TDto>(query);
        }


        /// <summary>
        /// 获取代理商信息
        /// </summary>
        public Task<List<TDto>> GetAgencyListAsync<TDto>(IQuery<Agency> query)
        {
            return _agencyRepository.AsNoTracking().ToListAsync<Agency, TDto>(query);
        }

        /// <summary>
        /// 订单确认更改旅行社电话
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task UpdateAgencyPhoneAsync(int id, string phone)
        {
            var agency = await _agencyRepository.FirstOrDefaultAsync(m => m.Id == id);

            if (agency != null)
                agency.Tel = phone;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///  新增代理商业务验证
        /// </summary>
        /// <param name="input"></param>
        /// <param name="agencyType"></param>
        /// <returns></returns>
        private Result NewAgencyBusinessVerify(AddAgencyInput input, AgencyType agencyType)
        {
            var existed = _agencyRepository.GetAll().Any(p => p.AgencyName == input.AgencyName);
            if (existed)
                return Result.FromError("代理商已存在");

            if (agencyType.DefaultAgencyType != DefaultAgencyType.Other)
            {
                if (string.IsNullOrWhiteSpace(input.AddAgencyUserInput.User.UserName) || string.IsNullOrWhiteSpace(input.Password))
                    return Result.FromError("登录账户名或密码不能为空");

                var existedUser = _userRepository.GetAll().Any(m => m.UserName == input.AddAgencyUserInput.User.UserName);
                if (existedUser)
                    return Result.FromError("该登录账户名已存在");

                if (agencyType.DefaultAgencyType == DefaultAgencyType.Ota || agencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)
                {
                    if (string.IsNullOrWhiteSpace(input.AddAgencyUserInput.HostIPs))
                        return Result.FromError("旅游网IP配置不能为空");
                }
            }
            return Result.Ok();
        }

        /// <summary>
        /// 更新代理商业务验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Result<Agency> UpdateAgencyBusinessVerify(UpdateAgencyInput input)
        {
            var existedDuplicateAgencyName = _agencyRepository.GetAll().Any(m => m.AgencyName == input.AgencyName && m.Id != input.Id);
            if (existedDuplicateAgencyName)
                return Result.FromError<Agency>("代理商名称已存在");

            var agency = _agencyRepository.GetAllIncluding(m => m.AgencyUser, m => m.AgencyType, m => m.Account).First(m => m.Id == input.Id);

            if (agency.AgencyType.DefaultAgencyType != DefaultAgencyType.Other)
            {
                if (string.IsNullOrWhiteSpace(input.UpdateUserInput.UserName))
                    return Result.FromError<Agency>("登录账户名不能为空");

                var existedUser = _userRepository.GetAll().Any(m => m.UserName == input.UpdateUserInput.UserName && m.Id != input.UpdateUserInput.Id);
                if (existedUser)
                    return Result.FromError<Agency>("该登录账户名已存在");

                if (agency.AgencyType.DefaultAgencyType == DefaultAgencyType.Ota || agency.AgencyType.DefaultAgencyType == DefaultAgencyType.OwnOta)
                {
                    if (string.IsNullOrWhiteSpace(input.HostIPs))
                        return Result.FromError<Agency>("旅游网IP配置不能为空");
                }
            }
            return Result.FromData(agency);
        }

        /// <summary>
        /// 数据同步代理商
        /// </summary>
        /// <returns></returns>
        private async Task DataSyncAccount(Account account, DataSyncType dataSyncType)
        {
            var parkList = await _parkRepository.GetAll().Select(p => p.Id).ToListAsync();
            foreach (var parkId in parkList)
            {
                DataSyncInput input = new DataSyncInput()
                {
                    SyncData = JsonConvert.SerializeObject(new AccountDataSyncDto()
                    {
                        Id = account.Id,
                        AccountName = account.AccountName
                    }),
                    SyncType = dataSyncType
                };
                _dataSyncManager.UploadDataToTargetPark(parkId, input);
            }
        }

        #endregion
    }
}