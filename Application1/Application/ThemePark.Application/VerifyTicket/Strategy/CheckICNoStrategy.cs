using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Application.SaleCard.Interfaces;
using ThemePark.Core.CardManage;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.Application.VerifyTicket.Interfaces;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验IC卡号策略
    /// </summary>
    public class CheckICNoStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly IVIPCardAppService _vipCardAppService;
        private readonly IRepository<ManageCardInfo> _manageCardInfoRepository;
        private readonly ICheckTicketManager _checkTicketManager;

        /// <summary>
        /// 验IC卡策略
        /// </summary>
        /// <param name="vipCardAppService"></param>
        /// <param name="manageCardInfoRepository"></param>
        /// <param name="checkTicketManager"></param>
        public CheckICNoStrategy(IVIPCardAppService vipCardAppService,
            IRepository<ManageCardInfo> manageCardInfoRepository,
            ICheckTicketManager checkTicketManager)
        {
            _vipCardAppService = vipCardAppService;
            _manageCardInfoRepository = manageCardInfoRepository;
            _checkTicketManager = checkTicketManager;
        }

        /// <summary>
        /// 验IC卡号
        /// </summary>
        /// <param name="icno"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string icno, int terminal)
        {
            // 先验是否管理卡/二次入园卡
            var result = await VerifyManageCard(icno, terminal);
            if (result != null)
                return result;

            // 验是否年卡
            result = await VerifyVipCard(icno, terminal);
            if (result != null)
                return result;

            return Failed(icno, VerifyType.InvalidTicket, "无效卡");
        }

        /// <summary>
        /// 验管理卡/二次入园管理卡
        /// </summary>
        /// <param name="icno"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result<VerifyDto>> VerifyManageCard(string icno, int terminal)
        {
            // 找出IC信息
            var manageCardInfo = await _manageCardInfoRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == icno);
            if (manageCardInfo == null)
                return null;

            // 判断有效期
            var today = DateTime.Today;
            if (manageCardInfo.ValidDateEnd == null || manageCardInfo.ValidDateBegin == null)
            {
                return Failed(icno, VerifyType.InvalidTicket, "不在有效期");
            }
            else if (manageCardInfo.ValidDateBegin.Value.Date > today
                 || today > manageCardInfo.ValidDateEnd.Value.Date)
            {
                return Failed(icno, VerifyType.InvalidTicket, "不在有效期");
            }

            var ticketCheckData = new TicketCheckData
            {
                VerifyCode = icno,
                VerifyCodeType = VerifyType.ICNo,
                Terminal = terminal,
                CheckState = manageCardInfo.IcBasicInfo.KindICKind.IsManageCard ? CheckState.Checking : CheckState.Idle,
                VerifyType = manageCardInfo.IcBasicInfo.KindICKind.IsManageCard ? VerifyType.ManageCard : VerifyType.SecondCard,
                ValidDays = (manageCardInfo.ValidDateEnd.Value - manageCardInfo.ValidDateBegin.Value).Days,
                ValidStartDate = manageCardInfo.ValidDateBegin.Value
            };
            ticketCheckData.ManageCardDto = new VerifyManageCardDto
            {
                Id = manageCardInfo.Id.ToString(),
                IsManagerCard = manageCardInfo.IcBasicInfo.KindICKind.IsManageCard,
                IsSecondCard = manageCardInfo.IcBasicInfo.KindICKind.IsTimeCard,
                Owner = manageCardInfo.IcBasicInfo.KindICKind.IsManageCard ? "管理卡" : "二次入园卡"
            };

            if (manageCardInfo.IcBasicInfo.KindICKind.IsManageCard)
            {
                ticketCheckData.AllowPersons = 1;
            }

            await _checkTicketManager.GetTicketCheckDataCache().SetAsync(ticketCheckData.VerifyCode, ticketCheckData, CheckDataTimeout);
            // 返回验票结果
            return Success(ticketCheckData);
        }


        /// <summary>
        /// 验年卡
        /// </summary>
        /// <param name="icno"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result<VerifyDto>> VerifyVipCard(string icno, int terminal)
        {
            //电子年卡为 15位 以1开头的卡号
            if (icno.Length == 15 && icno.StartsWith("0"))
            {
                icno = icno.Substring(1);
            }

            // 找出IC信息
            var vipCard = await _vipCardAppService.GetCardBasicInfoAsync(icno);
            if (vipCard == null)
            {
                return null;
            }

            //判断VIP卡是否激活状态
            if (vipCard.State != VipCardStateType.Actived)
                return Failed(icno, VerifyType.InvalidTicket, vipCard.StateName);

            // 判断有效期
            var today = DateTime.Today;
            if (vipCard.ValidDateEnd == null || vipCard.ValidDateBegin == null)
            {
                return Failed(icno, VerifyType.InvalidTicket, "不在有效期");
            }
            else if (vipCard.ValidDateBegin.Value.Date > today
                 || today > vipCard.ValidDateEnd.Value.Date)
            {
                return Failed(icno, VerifyType.InvalidTicket, "不在有效期");
            }

            var ticketClassCacheItem = await _checkTicketManager.GetParkTicketClassItem(vipCard.ParkSaleTicketClassId.Value);
            if (ticketClassCacheItem == null)
                return Failed(icno, VerifyType.InvalidTicket, "找票类失败");

            var ticketCheckData = new TicketCheckData
            {
                VerifyCode = icno,
                VerifyCodeType = VerifyType.ICNo,
                Terminal = terminal,
                VerifyType = VerifyType.VIPCard,
                Qty = 1,
                CheckState = CheckState.Checking,
                TicketSaleStatus = TicketSaleStatus.Valid,
                ValidDays = (vipCard.ValidDateEnd.Value - vipCard.ValidDateBegin.Value).Days,
                ValidStartDate = vipCard.ValidDateBegin.Value,
                ParkSaleTicketClassId = vipCard.ParkSaleTicketClassId.Value
            };

            // 判断验票类型，根据入园规则判断是否可入园，计算可入园人数
            bool checkTicketByRuleResult = _checkTicketManager.CheckTicketByRule(ticketCheckData);

            if (checkTicketByRuleResult)
            {
                ticketCheckData.VIPCardDto.VipCardId = vipCard.Id;
                //将验票结果加到缓存
                await _checkTicketManager.GetTicketCheckDataCache().SetAsync(ticketCheckData.VerifyCode, ticketCheckData, CheckDataTimeout);
                return Success(ticketCheckData);
            }
            return Failed(ticketCheckData.VerifyCode, ticketCheckData.VerifyType, ticketCheckData.Message);
        }


        /// <summary>
        /// 验证是否为二次入园管理卡
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<Result> CheckBarcodeSecond(string barcode)
        {
            var manageCardInfo = await _manageCardInfoRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == barcode && p.IcBasicInfo.KindICKind.IsTimeCard == true);
            if (manageCardInfo == null)
            {
                return Result.FromError("无效卡");
            }
            if (manageCardInfo.ValidDateEnd < System.DateTime.Today.AddDays(1))
            {
                return Result.FromError("卡已过期");
            }
            return Result.Ok();
        }

    }
}