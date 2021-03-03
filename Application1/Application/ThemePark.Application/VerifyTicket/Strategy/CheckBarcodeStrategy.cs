using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Core.AgentTicket;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.Core.ParkSale;
using ThemePark.Application.VerifyTicket.Interfaces;
using Abp.Dependency;
using ThemePark.Core.InPark;
using ThemePark.EntityFramework;
using ThemePark.Common;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验条码票策略
    /// </summary>
    public class CheckBarcodeStrategy : CheckStrategyBase, ICheckStrategy
    {
        private readonly ICheckTicketManager _checkTicketManager;

        /// <summary>
        /// 验条码构造函数
        /// </summary>
        public CheckBarcodeStrategy(ICheckTicketManager checkTicketManager)
        {
            _checkTicketManager = checkTicketManager;
        }

        /// <summary>
        /// 验条码票
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(string barcode, int terminal)
        {
            var result = await VerifyBarcodeTicket(barcode, terminal);
            if (result != null)
                return result;

            //入园单
            result = await VerifyInParkBill(barcode, terminal);
            if (result != null)
                return result;

            return Failed(barcode, VerifyType.InvalidTicket, "无效票");
        }

        #region 通过SQL查询视图再验票

        private async Task<Result<VerifyDto>> VerifyBarcodeTicket(string barcode, int terminal)
        {
            // 先从缓存里取（缓存来自于ticketTracker追踪到新增的票时set进去缓存）
            TicketInfo ticket = await _checkTicketManager.GetTicketInfoCache().GetAsync(barcode, async s =>
            {
                // 根据条码查询票信息
                // 票缓存没有时（可能过期），再根据条码在视图里查询票信息
                string sql = "select * from VerifyTicketView where Id='" + barcode + "'";
                //确保释放对象，防止内存泄漏
                using (var dbContext = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
                {
                    return await dbContext.Object.Database.SqlQuery<TicketInfo>(sql).FirstOrDefaultAsync();
                }
            });

            if (ticket == null)
            {
                return null;
            }

            //删除票信息缓存
            await _checkTicketManager.GetTicketInfoCache().RemoveAsync(barcode);

            switch (ticket.TicketSaleStatus)
            {
                case TicketSaleStatus.Invalid:
                    return Failed(barcode, VerifyType.CommonTicket, TicketSaleStatus.Invalid.DisplayName());

                case TicketSaleStatus.Valid: //有效票则继续验证
                    break;

                case TicketSaleStatus.InPark:
                    return Failed(barcode, VerifyType.CommonTicket, "已全部入园");

                case TicketSaleStatus.Refund:
                    return Failed(barcode, VerifyType.CommonTicket, TicketSaleStatus.Refund.DisplayName());

                case TicketSaleStatus.Expire:
                    return Failed(barcode, VerifyType.CommonTicket, TicketSaleStatus.Expire.DisplayName());

                case TicketSaleStatus.Freezon:
                    return Failed(barcode, VerifyType.CommonTicket, TicketSaleStatus.Freezon.DisplayName());

                case TicketSaleStatus.PreSale:
                    return Failed(barcode, VerifyType.CommonTicket, TicketSaleStatus.PreSale.DisplayName());

                default:
                    return Failed(barcode, VerifyType.CommonTicket, "无效票");
            }

            #region 取入园规则

            // 构造验票数据（入园规则）
            TicketClassCacheItem ticketClassCacheItem = null;
            if (ticket.TableId == 1 || ticket.TableId == 2)
            {
                ticketClassCacheItem = await _checkTicketManager.GetAgencyTicketClassItem(ticket.ParkSaleTicketClassId);
            }
            else if (ticket.TableId == 0)
            {
                ticketClassCacheItem = await _checkTicketManager.GetParkTicketClassItem(ticket.ParkSaleTicketClassId);
            }

            if (ticketClassCacheItem != null && (ticketClassCacheItem.RuleItem.TicketClassMode == TicketClassMode.YearCard || ticketClassCacheItem.RuleItem.TicketClassMode == TicketClassMode.MultiYearCard))
            {
                return Failed(barcode, VerifyType.InvalidTicket, "含年卡需在窗口兑换激活");
            }

            #endregion 取入园规则

            var ticketCheckData = new TicketCheckData
            {
                VerifyCode = barcode,
                VerifyCodeType = VerifyType.Barcode,
                Terminal = terminal,
                VerifyType = VerifyType.CommonTicket,
                CheckState = CheckState.Checking,
                TicketSaleStatus = (TicketSaleStatus)ticket.TicketSaleStatus,
                ValidDays = ticket.ValidDays,
                ValidStartDate = ticket.ValidStartDate,
                ParkId = ticket.ParkId,
                InPersons = ticket.InparkCounts,
                Qty = ticket.Qty,
                InParkInfo = ticket.Remark, // 2017-3-20 41 3,2017-3-25 41 2,
                ParkSaleTicketClassId = ticket.ParkSaleTicketClassId
            };

            //给缓存数据提前加上入园时需要更新的表
            if (ticket.TableId == 0)
            {
                ticketCheckData.EntityType = typeof(NonGroupTicket);
                ticketCheckData.TableName = ticketCheckData.EntityType.Name;
                ticketCheckData.TableId = 0;
            }

            else if (ticket.TableId == 1)
            {
                ticketCheckData.EntityType = typeof(GroupTicket);
                ticketCheckData.TableName = ticketCheckData.EntityType.Name;
                ticketCheckData.TableId = 1;
            }

            else if (ticket.TableId == 2)
            {
                ticketCheckData.EntityType = typeof(TOTicket);
                ticketCheckData.TableName = ticketCheckData.EntityType.Name;
                ticketCheckData.TableId = 2;
            }

            //根据入园规则判断是否可入园，计算可入园人数
            bool checkTicketByRuleResult = _checkTicketManager.CheckTicketByRule(ticketCheckData);

            //将验票结果加到缓存
            await _checkTicketManager.GetTicketCheckDataCache().SetAsync(ticketCheckData.VerifyCode, ticketCheckData, CheckDataTimeout);

            // 返回验票结果
            return checkTicketByRuleResult ? Success(ticketCheckData) : Failed(ticketCheckData.VerifyCode, ticketCheckData.VerifyType, ticketCheckData.Message);
        }
        #endregion

        /// <summary>
        /// 验证入园单
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result<VerifyDto>> VerifyInParkBill(string barcode, int terminal)
        {
            var inParkBillRepository = IocManager.Instance.Resolve<IRepository<InParkBill, string>>();

            var bill = await inParkBillRepository.FirstOrDefaultAsync(barcode);

            if (bill == null)
                return null;

            switch (bill.InParkBillState)
            {
                case InParkBillState.Entered:
                    return Failed(barcode, VerifyType.InparkBill, "已入园");

                case InParkBillState.Valid: //有效票则继续验证
                    break;
                case InParkBillState.Cancel:
                    return Failed(barcode, VerifyType.InparkBill, "已作废");
                case InParkBillState.Expire:
                    return Failed(barcode, VerifyType.InparkBill, "已过期");

                default:
                    return Failed(barcode, VerifyType.InparkBill, "已作废");
            }

            var allowPersons = bill.PersonNum - bill.InparkCounts;

            var ticketCheckData = new TicketCheckData
            {
                VerifyCode = barcode,
                VerifyCodeType = VerifyType.Barcode,
                ValidStartDate = bill.ValidStartDate,
                ValidDays = bill.ValidDays,
                Terminal = terminal,
                VerifyType = VerifyType.InparkBill,
                CheckState = CheckState.Checking,
                InParkBillState = bill.InParkBillState,
                AllowPersons = allowPersons,

                InPersons = bill.InparkCounts,

                BillPersons = bill.PersonNum
            };
            ticketCheckData.InparkBillDto = new VerifyInparkBillDto
            {
                Id = barcode,
                Persons = allowPersons,
                DisplayName = "入园单",
                Remark = "入园单"
            };

            bool checkInparkBillByRuleResult = _checkTicketManager.CheckInparkBillByRule(ticketCheckData);

            //将验票结果加到缓存
            await _checkTicketManager.GetTicketCheckDataCache().SetAsync(ticketCheckData.VerifyCode, ticketCheckData, CheckDataTimeout);

            // 返回验票结果
            return checkInparkBillByRuleResult ? Success(ticketCheckData) : Failed(ticketCheckData.VerifyCode, ticketCheckData.VerifyType, ticketCheckData.Message);
        }
    }
}

