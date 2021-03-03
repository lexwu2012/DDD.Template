using System.Threading.Tasks;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 通过缓存验票策略
    /// </summary>
    public class CheckCacheStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly ICheckTicketManager _checkTicketManager;

        /// <summary>
        /// 缓存验票策略构造
        /// </summary>
        /// <param name="checkTicketManager"></param>
        public CheckCacheStrategy(ICheckTicketManager checkTicketManager)
        {
            _checkTicketManager = checkTicketManager;
        }

        /// <summary>
        /// 通过缓存验票
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string verifyCode, int terminal)
        {
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);
            if (checkData == null)
                return null;

            switch (checkData.CheckState)
            {
                case CheckState.Checking:
                    return Failed(verifyCode, VerifyType.InvalidTicket, "请勿重刷");

                case CheckState.Used:
                    return Failed(verifyCode, VerifyType.InvalidTicket, "已使用");

                case CheckState.Invalid:
                case CheckState.Expired:
                    return Failed(verifyCode, VerifyType.InvalidTicket, checkData.Message);

                case CheckState.SecondInPark:

                    if (checkData.SecondTicketDto.InParkTimeEnd < System.DateTime.Now)
                    {
                        return Failed(verifyCode, VerifyType.InvalidTicket, "超过二次入园有效时间");
                    }
                    return Success(checkData);//返回二次入园类型

                case CheckState.Idle:

                    //二次入园管理卡不做重刷状态改变
                    if (checkData.VerifyType != VerifyType.SecondCard)
                    {
                        checkData.CheckState = CheckState.Checking;
                    }

                    checkData.Terminal = terminal;

                    bool ruleResult = true;

                    //常规规则
                    if (checkData.VerifyType == VerifyType.CommonTicket ||
                        checkData.VerifyType == VerifyType.MultiTicket ||
                        checkData.VerifyType == VerifyType.VIPCard ||
                        checkData.VerifyType == VerifyType.MultiYearCard)
                    {
                        ruleResult = _checkTicketManager.CheckTicketByRule(checkData);
                    }

                    //入园单规则
                    if (checkData.VerifyType == VerifyType.InparkBill)
                    {
                        ruleResult = _checkTicketManager.CheckInparkBillByRule(checkData);
                    }
                    // 判断验票类型，根据入园规则判断是否可入园，计算可入园人数

                    //将验票结果加到缓存
                    await _checkTicketManager.GetTicketCheckDataCache().SetAsync(checkData.VerifyCode, checkData, CheckDataTimeout);

                    // 返回验票结果
                    return ruleResult ? Success(checkData) : Failed(checkData.VerifyCode, checkData.VerifyType, checkData.Message);
            }
            return Failed(verifyCode, VerifyType.InvalidTicket, "无效票或系统错误");
        }
    }
}
