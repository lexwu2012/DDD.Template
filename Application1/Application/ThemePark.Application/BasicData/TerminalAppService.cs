using System.Collections.Generic;
using System.Data.Entity;
using Abp.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;
using ThemePark.Common;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 终端号配置
    /// </summary>
    /// <seealso cref="ThemePark.Application.ThemeParkAppServiceBase"/>
    /// <seealso cref="ThemePark.Application.BasicData.Interfaces.ITerminalAppService"/>
    public class TerminalAppService : ThemeParkAppServiceBase, ITerminalAppService
    {
        #region Fields

        private readonly IRepository<Terminal> _terminalRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalAppService"/> class.
        /// </summary>
        /// <param name="terminalRepository">The terminal repository.</param>
        public TerminalAppService(IRepository<Terminal> terminalRepository)
        {
            _terminalRepository = terminalRepository;
        }

        #endregion Constructors

        #region Methods

        #endregion Methods

        /// <summary>
        /// 获取终端
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<TerminalDto>> GetTerminalIdAsync(GetTerninalIdInput input)
        {
            var dto = await _terminalRepository.FirstOrDefaultAsync(p => p.Ip == input.Ip);
            if (dto == null)
                return Result.FromCode<TerminalDto>(ResultCode.MissEssentialData, "该机器没有配置过终端号");

            return Result.FromData(dto.MapTo<TerminalDto>());
        }

        /// <summary>
        /// 根据Id获取终端列表
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        public Task<DropdownDto> GetTerminalDropdownAsync(IList<int> ids)
        {
            return _terminalRepository.GetAll().AsNoTracking()
                .Where(o => ids.Contains(o.Id)).ToDropdownDtoAsync(o => new DropdownItem() { Text = o.TerminalCode.ToString(), Value = o.Id });
        }

        /// <summary>
        /// 获取终端号配置
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TDto>> SearchTerminal<TDto>(SearchTerminalInput query)
        {
            var queryKey = new Query<Terminal>(p => p.IsDeleted==false);
            if (!string.IsNullOrEmpty(query.Ip))
                queryKey = new Query<Terminal>(queryKey.GetFilter().And(p => p.Ip.Contains(query.Ip)));
            if(query.TerminalCode != 0)
                queryKey = new Query<Terminal>(queryKey.GetFilter().And(p => p.TerminalCode.ToString().Contains(query.TerminalCode.ToString())));
            var result = await _terminalRepository.GetAllListAsync(queryKey.GetFilter());

            return result.MapTo<List<TDto>>();
        }

        /// <summary>
        /// 新增终端号配置 
        /// </summary>
        /// <param name="input">entity</param>
        /// <returns></returns>
        public async Task<Result> AddTerminal(AddTerminalInput input)
        {
            var check = await AddTerminalValidate(input);
            if (!check.Success)
                return check;
            await _terminalRepository.InsertAsync(check.Data);

            //验证包含验证成功的返回信息，成功后返回
            return check;
        }

        /// <summary>
        /// 修改终端号配置
        /// </summary>
        /// <param name="input">entity</param>
        /// <returns></returns>
        public async Task<Result> UpdateTerminal(UpdateTerminalInput input)
        {
            var check = await UpdateTerminalValidate(input);
            if (!check.Success)
                return check;
            await _terminalRepository.UpdateAsync(input.id, p => Task.FromResult(input.MapTo(p)));

            //验证包含验证成功的返回信息，成功后返回
            return check;
        }

        /// <summary>
        /// 删除终端号配置
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<Result> DeleteTerminal(int id)
        {
            await _terminalRepository.DeleteAsync(id);
            return Result.Ok();
        }

        /// <summary>
        /// 新增终端号配置验证
        /// </summary>
        /// <returns></returns>
        private async Task<Result<Terminal>> AddTerminalValidate(AddTerminalInput input)
        {
            //验证终端号已配置过
            var codeCheck = await _terminalRepository.GetAll().AnyAsync(p => p.TerminalCode == input.TerminalCode&& p.ParkId == input.ParkId);
            if (codeCheck)
                return Result.FromError<Terminal>("本公园，该终端号已被使用！");

            //验证IP已配置过
            var ipCheck = await _terminalRepository.GetAll().AnyAsync(p => p.Ip == input.Ip && p.ParkId == input.ParkId);
            if (ipCheck)
                return Result.FromError<Terminal>("本公园，该IP已被使用！");

            var entity = input.MapTo<Terminal>();
            return Result.FromData(entity);
        }

        /// <summary>
        /// 修改终端号配置验证
        /// </summary>
        /// <returns></returns>
        private async Task<Result> UpdateTerminalValidate(UpdateTerminalInput input)
        {
            //验证终端号已配置过
            var codeCheck = await _terminalRepository.GetAll().AnyAsync(p => p.TerminalCode == input.TerminalCode && p.Id != input.id);
            if (codeCheck)
                return Result.FromError<Terminal>("本公园，该终端号已被使用！");

            //验证IP已配置过
            var ipCheck = await _terminalRepository.GetAll().AnyAsync(p => p.Ip == input.Ip && p.Id != input.id);
            if (ipCheck)
                return Result.FromError<Terminal>("本公园，该IP已被使用！");

            return Result.Ok();
        }
    }
}