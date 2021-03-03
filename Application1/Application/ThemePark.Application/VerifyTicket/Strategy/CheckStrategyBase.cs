using System;
using Abp.Dependency;
using Newtonsoft.Json;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验票策略抽象基类
    /// </summary>
    public abstract class CheckStrategyBase : ITransientDependency
    {
        /// <summary>
        /// 检票数据的超时
        /// </summary>
        protected TimeSpan CheckDataTimeout = TimeSpan.FromHours(8);

        /// <summary>
        /// 成功返回结果
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        protected Result<VerifyDto> Success(TicketCheckData checkData)
        {
            var dto = new VerifyDto
            {
                VerifyType = checkData.VerifyType,

                VerifyData = JsonConvert.SerializeObject(checkData.TicketDto),
                VerifyCode = checkData.VerifyCode
            };
            var result = Result.FromData(dto);
            result.Message = checkData.Message;
            return result;
        }

        /// <summary>
        /// 失败返回结果
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="verifyType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Result<VerifyDto> Failed(string verifyCode, VerifyType verifyType, string message)
        {
            var dto = new VerifyDto()
            {
                VerifyCode = verifyCode,
                VerifyType = verifyType,
                VerifyData = ""
            };
            var result = Result.FromData(dto);
            result.Message = message;
            result.Code = ResultCode.Fail;
            return result;
        }
    }
}
