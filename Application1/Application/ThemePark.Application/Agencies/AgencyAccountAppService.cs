using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.Agencies.Interfaces;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.Message;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Application.Trade.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Core.TradeInfos;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.Agencies
{
    /// <summary>
    /// 代理商账户应用服务
    /// </summary>
    public class AgencyAccountAppService : ThemeParkAppServiceBase, IAgencyAccountAppService
    {
        #region Fiedls
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<AccountOp, long> _accountOpRepository;
        private readonly IRepository<PreAccountOp, long> _preAccountOpRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly ISmsAppService _smsAppService;
        private readonly ISettingManager _settingManager;
        private readonly IAgencyAppService _agencyAppService;
        private readonly IRepository<SyncPark> _syncParkRepository;
        private readonly IRepository<TradeInfo, string> _tradeinfoRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;


        #endregion Fields

        #region Cotr
        /// <summary>
        /// 构造函数
        /// </summary>
        public AgencyAccountAppService(IRepository<Account> accountRepository, IRepository<AccountOp, long> accountOpRepository
            , IRepository<PreAccountOp, long> preAccountOpRepository
            , IRepository<Agency> agencyRepository
            , ISmsAppService smsAppService
            , ISettingManager settingManager
            , IBackgroundJobManager backgroundJobManager
            , IAgencyAppService agencyAppService, IRepository<SyncPark> syncParkRepository, IRepository<TradeInfo, string> tradeinfoRepository)
        {
            _accountRepository = accountRepository;
            _accountOpRepository = accountOpRepository;
            _agencyAppService = agencyAppService;
            _syncParkRepository = syncParkRepository;
            _tradeinfoRepository = tradeinfoRepository;
            _preAccountOpRepository = preAccountOpRepository;
            this._agencyRepository = agencyRepository;
            this._smsAppService = smsAppService;
            this._settingManager = settingManager;
            this._backgroundJobManager = backgroundJobManager;
        }

        #endregion Cotr

        #region Public Methods

        /// <summary>
        /// 给账户存款操作
        /// </summary>
        /// <param name="accountDto">账户Id</param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> PayAndReChargeAccount(AccountDto accountDto, int parkId)
        {
            var agency = await _agencyAppService.GetAgencyAsync<AgencyAccountDto>(new Query<Agency>(m => m.Id == accountDto.AgencyId));

            var account = await _accountRepository.FirstOrDefaultAsync(agency.AccountId);

            if (account == null)
                return Result.FromCode(ResultCode.InvalidData, "没有此代理商账号");

            //交易类型(公园要求)
            accountDto.TradeInfos.TradeType = accountDto.OpType == OpType.Recharge ? TradeType.Income : TradeType.Outlay;

            var tradeInfoAppService = IocManager.Instance.Resolve<ITradeInfoAppService>();
            //支付(AddTradeInfoAndReturnTradeInfoIdAsyn这个方法里面是消费扣款，不是充值，且没有现金方式，所以不用传agencyId进去，)
            var tradenoResult = await tradeInfoAppService.AddTradeInfoAndReturnTradeInfoIdAsyn(accountDto.TradeInfos, parkId, null);
            if (!tradenoResult.Success)
                return tradenoResult;
            //操作交易操作记录表，动作为充值
            var accountOp = new AccountOp()
            {
                AccountId = agency.AccountId,
                Cash = accountDto.TradeInfos.Amount,
                OpType = accountDto.OpType,
                TradeInfoId = tradenoResult.Data,
            };
            await UpdateAccoutAndOp(accountOp);
            //同步记录到其他公园
            var result = await PreDepositApiHelp.SyncAccountAsync(accountOp.MapTo<AccountDepositInput>(), parkId);
            return result;
        }

        /// <summary>
        /// 代理商挂账\预付款消费
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> ConsumptionAccount(AccountOpInput input, int parkId)
        {
            var accountOp = input.MapTo<AccountOp>();
            await UpdateAccoutAndOp(accountOp);
            //同步记录到其他公园
            await PreDepositApiHelp.SyncAccountAsync(accountOp.MapTo<AccountDepositInput>(), parkId);
            return Result.Ok();
        }

        /// <summary>
        /// 代理商挂账\预付款退款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public async Task<Result> RefundAccount(AccountOpInput input, int parkId)
        {
            var accountOp = input.MapTo<AccountOp>();
            await UpdateAccoutAndOp(accountOp);
            //同步记录到其他公园
            await PreDepositApiHelp.SyncAccountAsync(accountOp.MapTo<AccountDepositInput>(), parkId);
            return Result.Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountOp"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAccoutAndOp(AccountOp accountOp)
        {
            //插入交易操作记录
            await _accountOpRepository.InsertAsync(accountOp);
            if (accountOp.OpType == OpType.Recharge || accountOp.OpType == OpType.Refund)
            {
                //目前没有方特币等其他交易
                await _accountRepository.UpdateAsync(accountOp.AccountId, m => Task.FromResult(m.Balance += accountOp.Cash.Value));
            }
            else
            {
                //目前没有方特币等其他交易
                await _accountRepository.UpdateAsync(accountOp.AccountId, m => Task.FromResult(m.Balance -= accountOp.Cash.Value));
            }
            return Result.Ok();
        }


        /// <summary>
        /// 获取旅行社账户操作记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<AccountOpDto>>> GetAccountOpRecord(GetAccountOpRecord input)
        {
            var agency = await _agencyAppService.GetAgencyAsync<AgencyAccountDto>(new Query<Agency>(m => m.Id == input.AgencyId));
            var startDate = input.SearchStarTime?.Date;
            var endDate = input.SearchEnDateTime?.Date.AddDays(1);
            var result = await _accountOpRepository.GetAllListAsync
              (
                p => p.AccountId == agency.AccountId &&
                p.CreationTime >= startDate &&
                p.CreationTime <= endDate);

            return Result.FromData(result.MapTo<List<AccountOpDto>>().OrderByDescending(p => p.CreationTime).ToList());
        }

        /// <summary>
        /// 获取代理商账户余额
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<AccountMoneyDto>> GetAgencyBalance(GetAgencyBalanceInput input)
        {
            var dto = await _agencyAppService.GetAgencyAsync<AccountMoneyDto>(new Query<Agency>(m => m.Id == input.AgencyId));
            return Result.FromData(dto);
        }



        /// <summary>
        /// 根据条件获取账户操作历史列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList<TDto>> GetAccountOpListAsync<TDto>(IQuery<AccountOp> query)
        {
            return await _accountOpRepository.AsNoTracking().ToListAsync<AccountOp, TDto>(query);
        }

        /// <summary>
        /// 根据条件获取账户信息
        /// </summary>
        public async Task<TDto> GetAccountAsync<TDto>(IQuery<Account> query)
        {
            return await _accountRepository.AsNoTracking().FirstOrDefaultAsync<Account, TDto>(query);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>查询结果</returns>
        public Task<PageResult<TDto>> GetAllPreAccountOpByPageAsync<TDto>(IPageQuery<PreAccountOp> query = null)
        {
            return _preAccountOpRepository.AsNoTracking().ToPageResultAsync<PreAccountOp, TDto>(query);
        }

        /// <summary>
        /// 更新预付款账户信息（包括预付款增减）
        /// </summary>
        public async Task<Result> UpdatePreAccountAsync(PreAccountInput preAccountInput)
        {
            var phoneNoRegex = new Regex(PrePaymentSetting.PhoneNoInputRegexFormater);
            var accountEntity = await _accountRepository.FirstOrDefaultAsync(p => p.Id == preAccountInput.AccountId);
            if (accountEntity == null)
                return Result.FromError("帐号ID不存在");
            else if (!string.IsNullOrEmpty(preAccountInput.FtPeoplePhone) && !phoneNoRegex.IsMatch(preAccountInput.FtPeoplePhone))
                return Result.FromError("预警接收手机号(方特)输入项不符合规范");
            else if (!string.IsNullOrEmpty(preAccountInput.AgencyPeoplePhone) && !phoneNoRegex.IsMatch(preAccountInput.AgencyPeoplePhone))
                return Result.FromError("预警接收手机号(旅游网)输入项不符合规范");
            else if(preAccountInput.Balance>100000000 || preAccountInput.Balance<-100000000)
                return Result.FromError("预付账户余额应介于-100000000与100000000之间");
            else if (preAccountInput.Balance == accountEntity.Balance
                && preAccountInput.AlarmBalance == accountEntity.AlarmBalance
                && preAccountInput.LeastBalance == accountEntity.LeastBalance
                && preAccountInput.FtPeoplePhone == accountEntity.FtPeoplePhone
                && preAccountInput.AgencyPeoplePhone == accountEntity.AgencyPeoplePhone)
                return Result.FromError("没有需要保存的修改项");
            else
            {
                //添加预付款账户操作记录
                if (preAccountInput.Balance != accountEntity.Balance)
                {
                    var preAccountOp = new PreAccountOp
                    {
                        AccountId = accountEntity.Id,
                        Cash = preAccountInput.Balance - accountEntity.Balance,
                        Remark = preAccountInput.Remark,
                        OpType = OpTypeState.Prepay,
                        CurrCash = preAccountInput.Balance
                    };
                    await _preAccountOpRepository.InsertAsync(preAccountOp);
                }

                accountEntity.Balance = preAccountInput.Balance;
                accountEntity.AlarmBalance = preAccountInput.AlarmBalance;
                accountEntity.LeastBalance = preAccountInput.LeastBalance;
                accountEntity.FtPeoplePhone = preAccountInput.FtPeoplePhone;
                accountEntity.AgencyPeoplePhone = preAccountInput.AgencyPeoplePhone;
                accountEntity.Remark = preAccountInput.Remark;
                await _accountRepository.UpdateAsync(accountEntity);

                return Result.Ok();
            }
        }

        /// <summary>
        /// 增加中心旅游网预付款
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddPreAccountAsync(int accountId, decimal balance, string remark)
        {
            var account = await _accountRepository.FirstOrDefaultAsync(p => p.Id == accountId);
            if (account == null)
            {
                return Result.FromError("帐号ID不存在");
            }

            account.Balance = account.Balance + balance;
            await _accountRepository.UpdateAsync(account);

            var preAccountOp = new PreAccountOp();
            preAccountOp.AccountId = accountId;
            preAccountOp.Cash = balance;
            preAccountOp.Remark = remark;
            preAccountOp.OpType = OpTypeState.Prepay;
            preAccountOp.CurrCash = account.Balance;
            await _preAccountOpRepository.InsertAsync(preAccountOp);

            return Result.Ok();
        }

        /// <summary>
        /// 扣减OTA预付款
        /// </summary>
        [UnitOfWork]
        public async Task<Result> DeductMoneyAsync(PreAccountDeductMoneyDto deductMoneyDto)
        {
            var agencyEntity = await this._agencyRepository
                .GetAllIncluding(m => m.Account)
                .FirstOrDefaultAsync(p => p.Id == deductMoneyDto.AgencyId);
            var accountEnity = agencyEntity?.Account;
            if (accountEnity == null)
                return Result.FromCode(ResultCode.InvalidData, "没有此代理商账号");
            else
            {
                //1.从账户中扣除预付款 
                var originalBalance = accountEnity.Balance;
                accountEnity.Balance = accountEnity.Balance - deductMoneyDto.TotalMoney;
                var preAccountOp = new PreAccountOp
                {
                    AccountId = accountEnity.Id,
                    Cash = -deductMoneyDto.TotalMoney,
                    Remark = $"订单:{ deductMoneyDto.OrderId}，核销扣款",
                    OpType = OpTypeState.Deduct,
                    CurrCash = accountEnity.Balance,
                    //CreatorUserId = 10001
                };
                await _accountRepository.UpdateAsync(accountEnity);
                await _preAccountOpRepository.InsertAsync(preAccountOp);

                //2.根据设置的警戒线决定是否触发警报（触发条件：扣款之前余额大于警戒金额且扣款之后余额小于等于警戒金额）
                if (accountEnity.Balance <= accountEnity.AlarmBalance && originalBalance > accountEnity.AlarmBalance)
                {
                    try
                    {
                        await this.SendAlarmMessage(agencyEntity);
                        return Result.Ok("代理商：[" + agencyEntity.AgencyName + "]该次预付款扣款后余额已经低于预警值:" + accountEnity.AlarmBalance);
                    }
                    catch (Exception ex)
                    {
                        //防止发送预警信息失败而影响正常的扣款操作
                    }
                }

                return Result.Ok();
            }
        }

        /// <summary>
        /// 新增代理商账户
        /// </summary>
        /// <param name="accountDataSyncDto"></param>
        /// <returns></returns>
        public async Task<Result> AddAgencyAccount(AccountDataSyncDto accountDataSyncDto)
        {
            await _accountRepository.InsertAsync(accountDataSyncDto.MapTo<Account>());
            return Result.Ok();
        }

        /// <summary>
        /// 更新代理商账户名称
        /// </summary>
        /// <param name="accountDataSyncDto"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAgencyAccountNameAsync(AccountDataSyncDto accountDataSyncDto)
        {
            await _accountRepository.UpdateAsync(accountDataSyncDto.Id, m => Task.FromResult(m.AccountName == accountDataSyncDto.AccountName));
            return Result.Ok();
        }

        #endregion Public Methods

        /// <summary>
        /// 当预付款余额小于等于预警金额时发送预警信息
        /// </summary>
        private async Task SendAlarmMessage(Agency agencyEntity)
        {
            //【注意】代理商-{0}预付账户当前剩余的金额为{1}元，已低于预警金额（{2}）请及时知会代理商补充预付金额，否则将会影响正常出票。
            //【注意】你方在华强方特主题公园预付账户当前的剩余金额为{0}元，已低于预警金额（{1}），请及时补充预付金额，否则将会影响正常出票。
            var innerMessageTemplate = await _settingManager.GetSettingValueAsync(PrePaymentSetting.InnerMessageTemplate);
            var outerMessageTemplate = await _settingManager.GetSettingValueAsync(PrePaymentSetting.OuterMessageTemplate);
            var ftPhoneNos = (agencyEntity.Account.FtPeoplePhone ?? "").Split(PrePaymentSetting.PhoneNoSpliters, StringSplitOptions.RemoveEmptyEntries);
            var agencyPhoneNos = (agencyEntity.Account.AgencyPeoplePhone ?? "").Split(PrePaymentSetting.PhoneNoSpliters, StringSplitOptions.RemoveEmptyEntries);
            //发送对内(方特)通知
            if (!string.IsNullOrWhiteSpace(innerMessageTemplate) && ftPhoneNos.Length > 0)
            {
                var innerMessage = string.Format(innerMessageTemplate, agencyEntity.AgencyName, agencyEntity.Account.Balance, agencyEntity.Account.AlarmBalance);
                await this._smsAppService.SendMessage(ftPhoneNos, innerMessage);
            }

            //ToDo发送对外(旅游网)通知
            if (!string.IsNullOrWhiteSpace(outerMessageTemplate) && agencyPhoneNos.Length > 0)
            {
                var outerMessage = string.Format(outerMessageTemplate, agencyEntity.Account.Balance, agencyEntity.Account.AlarmBalance);
                await this._smsAppService.SendMessage(agencyPhoneNos, outerMessage);
            }
        }

    }
}
