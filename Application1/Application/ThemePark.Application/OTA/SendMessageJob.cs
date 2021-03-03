using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Threading;
using ThemePark.Application.Message;
using ThemePark.Application.VerifyTicket;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.OTA
{

    /// <summary>
    /// 发送短信参数
    /// </summary>
    public class SendMessageJobArgs
    {
        /// <summary>
        /// 订单
        /// </summary>
        public TOMessage TOMessage { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public List<string> Phones { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 发送短信后台作业
    /// </summary>
    public class SendMessageJob : BackgroundJob<SendMessageJobArgs>, ITransientDependency
    {
        private readonly ISmsAppService _smsAppService;
        private readonly IRepository<TOMessage> _toMessageRepository;

        public SendMessageJob(ISmsAppService smsAppService, IRepository<TOMessage> toMessageRepository)
        {
            _smsAppService = smsAppService;
            _toMessageRepository = toMessageRepository;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public override void Execute(SendMessageJobArgs args)
        {
            var result = AsyncHelper.RunSync(() => _smsAppService.SendMessage(args.Phones, args.Message));
            args.TOMessage.Remark = result;
            AsyncHelper.RunSync(() => _toMessageRepository.InsertAsync(args.TOMessage));
        }
    }
}
