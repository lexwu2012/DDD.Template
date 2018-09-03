using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Logging;
using DDD.Application.Service.Wechat.Interfaces;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Logger;
using DDD.Infrastructure.Web.Application;

namespace DDD.AutoAcp.WebApi.Controllers
{
    /// <summary>
    /// 还款相关接口
    /// </summary>
    //[AllowAuthentications]
    [RoutePrefix("api")]
    public class WechatRepayController : ApiControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWechatRepayService _wechatRepayService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WechatRepayController(IWechatRepayService wechatRepayService)
        {
            _wechatRepayService = wechatRepayService;
            _logger = LogHelper.Logger.CreateChildLogger(nameof(WechatRepayController));
        }

        #region Public Methods

        /// <summary>
        /// 获取还款信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PayInfo")]
        public Result<PayInfoDto> GetPayInfo(GetPayInfoDto payInfo)
        {
            return _wechatRepayService.GetPayInfoAsync(payInfo);
        }

        /// <summary>
        /// 还款接口
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Repay")]
        public Result<PayInfoDto> Repay(GetPayInfoDto payInfo)
        {
            return _wechatRepayService.GetPayInfoAsync(payInfo);
        }

        /// <summary>
        /// 获取当天自动代扣的数量
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("TodayAutoCheckoffTotal")]
        public async Task<Result<int>> GetTodayAutoCheckoffTotalAsync()
        {
            return await _wechatRepayService.GetTodayAutoCheckoffTotalAsync();
        }

        /// <summary>
        /// 获取测试常量
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetConstantData")]
        public Result<int> GetConstantData()
        {
            return _wechatRepayService.GetConstantData();
        }

        #endregion
    }
}