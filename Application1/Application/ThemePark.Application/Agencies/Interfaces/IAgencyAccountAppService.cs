using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Interfaces
{
    /// <summary>
    /// 代理商账户应用服务接口
    /// </summary>
    public interface IAgencyAccountAppService : IApplicationService
    {
        /// <summary>
        /// 给账户存款操作
        /// </summary>
        /// <param name="accountDto">账户Id</param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> PayAndReChargeAccount(AccountDto accountDto, int parkId);

        /// <summary>
        /// 更新旅行社账户数据
        /// </summary>
        /// <param name="accountOp"></param>
        /// <returns></returns>
        Task<Result> UpdateAccoutAndOp(AccountOp accountOp);

        /// <summary>
        /// 账户消费操作
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> ConsumptionAccount(AccountOpInput input,int parkId);

        /// <summary>
        /// 更新代理商账户名称
        /// </summary>
        /// <param name="accountDataSyncDto"></param>
        /// <returns></returns>
        Task<Result> UpdateAgencyAccountNameAsync(AccountDataSyncDto accountDataSyncDto);

        /// <summary>
        /// 账户退款操作
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> RefundAccount(AccountOpInput input, int parkId);

        /// <summary>
        /// 获取旅行社账户操作记录
        /// </summary>
        /// <returns></returns>
        Task<Result<List<AccountOpDto>>> GetAccountOpRecord(GetAccountOpRecord input);

        /// <summary>
        /// 获取代理商账户余额
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<AccountMoneyDto>> GetAgencyBalance(GetAgencyBalanceInput input);

        /// <summary>
        /// 根据条件获取账户操作历史列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetAccountOpListAsync<TDto>(IQuery<AccountOp> query);



        /// <summary>
        /// 获取预付款操作列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> GetAllPreAccountOpByPageAsync<TDto>(IPageQuery<PreAccountOp> query = null);

        /// <summary>
        /// 更新预付款账户信息
        /// </summary>
        Task<Result> UpdatePreAccountAsync(PreAccountInput preAccountInput);

        /// <summary>
        /// 根据条件获取账户信息
        /// </summary>
        Task<TDto> GetAccountAsync<TDto>(IQuery<Account> query);

        /// <summary>
        /// 新增预付款（中心票务系统）
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="balance"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<Result> AddPreAccountAsync(int accountId, decimal balance, string remark);

        /// <summary>
        /// 扣减OTA预付款
        /// </summary>
        /// <param name="deductMoneyDtop"></param>
        /// <returns></returns>
        Task<Result> DeductMoneyAsync(PreAccountDeductMoneyDto deductMoneyDtop);

        /// <summary>
        /// 新增代理商账户
        /// </summary>
        /// <param name="accountDataSyncDto"></param>
        /// <returns></returns>
        Task<Result> AddAgencyAccount(AccountDataSyncDto accountDataSyncDto);
    }
}
