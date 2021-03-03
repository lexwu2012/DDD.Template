using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// Interface ITerminalAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface ITerminalAppService : IApplicationService
    {
        /// <summary>
        /// 获取终端号
        /// </summary>
        /// <returns></returns>
        Task<Result<TerminalDto>> GetTerminalIdAsync(GetTerninalIdInput input);

        /// <summary>
        /// 根据Id获取终端列表
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetTerminalDropdownAsync(IList<int> ids);

        /// <summary>
        /// 新增终端号配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddTerminal(AddTerminalInput input);

        /// <summary>
        /// 修改终端号配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateTerminal(UpdateTerminalInput input);

        /// <summary>
        /// 删除终端号配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteTerminal(int id);

        /// <summary>
        /// 查找终端号配置
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<TDto>> SearchTerminal<TDto>(SearchTerminalInput query);
    }
}