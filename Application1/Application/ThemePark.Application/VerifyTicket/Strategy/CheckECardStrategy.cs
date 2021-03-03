using Abp.Dependency;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.SaleCard.Interfaces;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket.Strategy
{
    public class CheckECardStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly ICheckTicketManager _checkTicketManager;
        private readonly IRepository<IcBasicInfo, long> _icBasicInfoRepository;


        /// <summary>
        /// 验条码构造函数
        /// </summary>
        public CheckECardStrategy(ICheckTicketManager checkTicketManager
            , IRepository<IcBasicInfo, long> icBasicInfoRepository)
        {
            _checkTicketManager = checkTicketManager;
            _icBasicInfoRepository = icBasicInfoRepository;
        }

        /// <summary>
        /// 验条码票
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string barcode, int terminal)
        {
            // 查询身份证关联的VIP卡
            var icBasicInfo = await _icBasicInfoRepository.FirstOrDefaultAsync(p => p.ECardID == barcode);
            if (icBasicInfo != null)
            {
                string icNo = icBasicInfo.IcNo.ToUpper();
                if (icNo != "") // 走IC卡验票逻辑
                    return await IocManager.Instance.Resolve<CheckICNoStrategy>().Verify(icNo, terminal);
            }

            return Failed(barcode, VerifyType.InvalidTicket, "无效条码");
        }

    }
}
