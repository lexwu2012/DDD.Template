using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Specifications;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.BasicData;
using ThemePark.Core.OrderTrack;
using ThemePark.Core.ParkSale;
using ThemePark.Core.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt
{
    public class VendorSetAppServicecs : IVendorSetAppService
    {
        private readonly IRepository<VendorSet> _vendorSetRepository;
        private readonly IRepository<InvoiceCode> _invoiceCodeRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<SysUser, long> _sysUserRepository;
        private readonly IRepository<ParkArea> _parkAreaRepository;
        private readonly IRepository<Terminal> _terminalRepository;
        private readonly IRepository<VendorTicketTrack> _vendorTicketTrackRepository;
        private readonly IRepository<Park> _parkRepository;

        public VendorSetAppServicecs(IRepository<VendorSet> vendorSetRepository, UserManager userManager, IRepository<InvoiceCode> invoiceCodeRepository, IRepository<Invoice, long> invoiceRepository, IRepository<SysUser, long> sysUserRepository, IRepository<ParkArea> parkAreaRepository, IRepository<Terminal> terminalRepository, IRepository<VendorTicketTrack> vendorTicketTrackRepository, IRepository<Park> parkRepository)
        {
            _vendorSetRepository = vendorSetRepository;
            _userManager = userManager;
            _invoiceCodeRepository = invoiceCodeRepository;
            _invoiceRepository = invoiceRepository;
            _sysUserRepository = sysUserRepository;
            _parkAreaRepository = parkAreaRepository;
            _terminalRepository = terminalRepository;
            _vendorTicketTrackRepository = vendorTicketTrackRepository;
            _parkRepository = parkRepository;
        }

        /// <summary>
        /// 新增自助售票机配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddVendorSet(AddVendorSetInput input)
        {
            var check = await AddVendorSetValidate(input);
            if (!check.Success)
                return check;
            await _vendorSetRepository.InsertAsync(check.Data);

            //验证包含验证成功的返回信息，成功后返回
            return check;
        }

        /// <summary>
        /// 更新自助售票机配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateVendorSet(UpdateVendorSetInput input)
        {
            var check = await UpdateVendorSetValidate(input);
            if (!check.Success)
                return check;
            await _vendorSetRepository.UpdateAsync(input.Id, p => Task.FromResult(input.MapTo(p)));
            return Result.Ok();
        }

        /// <summary>
        /// 删除自助售票机配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteVendorSet(int id)
        {
            await _vendorSetRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 查询自助售票机配置
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<TDto>>> SearchVendorSet<TDto>(SearchVendorSetInput input)
        {
            var filter = new Query<VendorSet>(p => true).GetFilter();
            if (!string.IsNullOrEmpty(input.UserName))
                filter = filter.And(p => p.User.UserName.Contains(input.UserName));
            var result = (await _vendorSetRepository.GetAllIncluding(p => p.User, p => p.Terminal).Where(filter).ToListAsync()).MapTo<List<TDto>>();

            return Result.FromData(result);
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<Result<VendorSet>> VendorSetLogin(string ip)
        {
            var data = await _vendorSetRepository.GetAllIncluding(p => p.User,o=>o.Terminal).FirstOrDefaultAsync(p => p.Terminal.Ip.Contains(ip));
            if (data?.User == null)
                return Result.FromCode<VendorSet>(ResultCode.InvalidSign);
            return Result.FromData(data);
        }

        /// <summary>
        /// 获取账号对应发票信息
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result<VendorSetDto>> GetVendorSet(int terminalId)
        {
            var data = await _vendorSetRepository.FirstOrDefaultAsync(p => p.Terminal.TerminalCode == terminalId);
            if (data == null)
                return Result.FromCode<VendorSetDto>(ResultCode.NoRecord, "该机器没配置过终端号");
            return Result.FromData(data.MapTo<VendorSetDto>());
        }

        /// <summary>
        /// 新增自助售票机配置验证
        /// </summary>
        /// <returns></returns>
        private async Task<Result<VendorSet>> AddVendorSetValidate(AddVendorSetInput input)
        {
            //验证终端号已配置过
            var configCheck = await _vendorSetRepository.GetAll().AnyAsync(p => p.TerminalId == input.TerminalId);
            if (configCheck)
                return Result.FromError<VendorSet>("终端号已被使用");

            var entity = input.MapTo<VendorSet>();
            return Result.FromData(entity);
        }

        /// <summary>
        /// 新增自助售票机配置验证
        /// </summary>
        /// <returns></returns>
        private async Task<Result> UpdateVendorSetValidate(UpdateVendorSetInput input)
        {
            //验证终端号已配置过
            var configCheck = await _vendorSetRepository.GetAll().AnyAsync(p => p.TerminalId == input.TerminalId && p.Id != input.Id);
            if (configCheck)
                return Result.FromError<VendorSet>("终端号已被使用");

            return Result.Ok();
        }

        /// <summary>
        /// 更新发票配置
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public async Task<Result> UpdateTicketMax(int terminalId, int qty)
        {
            var entity = await _vendorSetRepository.FirstOrDefaultAsync(p => p.Terminal.TerminalCode == terminalId);
            if (entity == null || entity.TicketMax < qty)
                return Result.FromCode(ResultCode.Fail);
            entity.TicketMax -= qty;

            await _vendorSetRepository.UpdateAsync(entity);
            return Result.Ok();
        }


        /// <summary>
        /// 获取公园可用自助售票机账号
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result<List<DropdownItem<long>>>> GetValidUsers(int parkId)
        {
            var parkArea = await _parkAreaRepository.GetAllListAsync(p => p.ParkId == parkId);
            //公园所有可用账户
            var dtos = (await _sysUserRepository.GetAllListAsync()).Where(p => parkArea.Any(o => o.Id == p.ParkAreaId || o.ParkId != null && o.ParentPath.Contains(p.ParkAreaId.ToString()))).ToList();
            //允许账号重复
            return Result.FromData(dtos.Select(p => new DropdownItem<long>() { Text = p.User.UserName, Value = p.Id }).ToList());
        }


        /// <summary>
        /// 获取可用终端号
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<DropdownItem>>> GetValidTerminals()
        {
            var terminals = await _terminalRepository.GetAllListAsync();
            //已用
            var used = await _vendorSetRepository.GetAllListAsync();
            //terminals.RemoveAll(p => used.Any(o => o.TerminalId == p.Id));
            return Result.FromData(terminals.Select(p => new DropdownItem() { Text = p.HostName, Value = p.Id }).ToList());
        }


        /// <summary>
        /// 查询订单记录
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public async Task<Result<List<VendorTicketTrackDto>>> GetVendorTicketTrack(GetVendorTicketTrackInput input, bool isError)
        {
            var query = new Query<VendorTicketTrack>(p => p.IsError == isError).GetFilter();
            if (input.TerminalId.HasValue)
                query = query.And(p => p.TerminalId == input.TerminalId);

            if (!string.IsNullOrEmpty(input.SearchId))
                query = query.And(p => p.TOHeaderId == input.SearchId || p.TradeinfoId == input.SearchId);
            if (input.CreationTime.HasValue)
                query = query.And(p => DbFunctions.DiffDays(p.CreationTime, input.CreationTime.Value) == 0);
            else
                query = query.And(p => DbFunctions.DiffDays(p.CreationTime, DateTime.Now) == 0);
            var dtos = (await _vendorTicketTrackRepository.GetAllListAsync(query)).MapTo<List<VendorTicketTrackDto>>();
            dtos.ForEach(GetParkAndTerminal);
            return Result.FromData(dtos);
        }

        /// <summary>
        /// 给VendorTicketTrackDto 赋值公园名称和终端名称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private void GetParkAndTerminal(VendorTicketTrackDto dto)
        {
            dto.HostName = dto.TerminalId == 0 ? null : _terminalRepository.FirstOrDefaultAsync(p=>p.TerminalCode==dto.TerminalId).Result.HostName;
            dto.ParkName = _parkRepository.GetAsync(dto.ParkId).Result.ParkName;
        }
    }
}
