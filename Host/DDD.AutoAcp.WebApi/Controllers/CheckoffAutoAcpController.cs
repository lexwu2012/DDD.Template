using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Logging;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Logger;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.Web.Query;

namespace DDD.AutoAcp.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckoffAutoAcpController : ApiControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICheckoffAutoAcpAppService _checkoffAutoAcpAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CheckoffAutoAcpController(ICheckoffAutoAcpAppService checkoffAutoAcpAppService)
        {
            _checkoffAutoAcpAppService = checkoffAutoAcpAppService;
            _logger = LogHelper.Logger.CreateChildLogger(nameof(WechatRepayController));
        }

        #region Public Methods

        /// <summary>
        /// 获取指定的代扣数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSpecifyCheckoffAutoAcpAsync")]
        public async Task<CheckoffAutoAcpDto> GetSpecifyCheckoffAutoAcpAsync(int id)
        {
            return await _checkoffAutoAcpAppService.GetCheckoffAutoAcpAsync<CheckoffAutoAcpDto>(new Query<CheckoffAutoAcp>(m => m.Id == id));
        }

        /// <summary>
        /// 更新指定的代扣数据并返回dto
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("UpdateSpecifyCheckoffAutoAcpSendTimeAsync")]
        public Result<CheckoffAutoAcpDto> UpdateSpecifyCheckoffAutoAcpSendTimeAsync(int id)
        {
            return _checkoffAutoAcpAppService.UpdateCheckoffAutoAcp(id);
        }
        #endregion
    }
}
