using DDD.Application.Service.Wechat.Interfaces;
using DDD.Infrastructure.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Domain.Service.Wechat.Interfaces;
using DDD.Infrastructure.Web.Application;

namespace DDD.Application.Service.Wechat
{
    public class WechatRepayService : AppServiceBase, IWechatRepayService
    {
        private readonly IWechatRepayDomainService _wechatRepayDomainService;

        public WechatRepayService(IWechatRepayDomainService wechatRepayDomainService)
        {
            _wechatRepayDomainService = wechatRepayDomainService;
            Logger = LogHelper.Logger.CreateChildLogger(nameof(WechatRepayService));
        }

        public string ContractDetail(string jsonData)
        {
            string subject = "获取合同详细发生异常";

            return subject;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payInfo"></param>
        /// <returns></returns>
        public Result<PayInfoDto> GetPayInfoAsync(GetPayInfoDto payInfo)
        {
            var result = _wechatRepayDomainService.GetPayInfo(payInfo);
            if (!result.Success)
            {
                //todo: send email?
            }

            return result;
        }

        public async Task<Result<int>> GetTodayAutoCheckoffTotalAsync()
        {
            return await _wechatRepayDomainService.GetTodayAutoCheckoffTotalAsync();
        }

        public Result<int> GetConstantData()
        {
            return _wechatRepayDomainService.GetCheckoffCommandData();
        }
    }
}
