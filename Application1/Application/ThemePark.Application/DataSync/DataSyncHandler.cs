using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using System;
using ThemePark.Core;
using ThemePark.Core.DataSync;
using ThemePark.EntityFramework;

namespace ThemePark.Application.DataSync
{
    public class DataSyncHandler : IEventHandler<DataSyncEventData>, ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public DataSyncHandler(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        public void HandleEvent(DataSyncEventData eventData)
        {
            switch (eventData.DataSyncType)
            {
                case DataSyncType.UploadTablesUpdateFile: // 上传数据更新文件
                    break;
                case DataSyncType.DownloadTablesUpdateFile: // 下载数据更新文件
                case DataSyncType.UpdateTables: // 更新数据表  //TODO: because Abp.Redis and EFCache use the same redis server now
                    var fixedRedisCache = IocManager.Instance.Resolve<IFixedRedisCache>();
                    if (fixedRedisCache != null) //clear efcache
                    {
                        fixedRedisCache.Purge();
                        _cacheManager.GetUserPermissionCache().Clear();
                        _cacheManager.GetRolePermissionCache().Clear();
                    }
                    break;
                case DataSyncType.UploadSumInfo: // 汇总公园上传过来的信息
                    //TODO: other case use this
                    //redisCache.InvalidateSets(new List<string> { nameof(Address) });
                    break;
                case DataSyncType.MultiTicketCancel: // 套票作废
                    break;
                case DataSyncType.MultiTicketEnroll: // 套票指纹登记
                    break;
                case DataSyncType.MultiTicketEnrollPhoto: // 套票照片登记
                    break;
                case DataSyncType.MultiTicketSend: // 套票同步
                    break;
                case DataSyncType.MultiTicketInpark: // 套票入园同步
                    break;
                case DataSyncType.OrderSend: // 订单下发
                    break;
                case DataSyncType.OrderConsume: // 订单核销
                    break;
                case DataSyncType.OrderModify: // 订单修改
                    break;
                case DataSyncType.OrderRefund: // 订单退款
                    break;
                case DataSyncType.MulYearCardActive: // 年卡激活
                    break;
                case DataSyncType.MulYearCardFill: // 年卡补卡
                    break;
                case DataSyncType.MulYearCardInit: // 年卡初始化
                    break;
                case DataSyncType.MulYearCardLoss: // 年卡挂失
                    break;
                case DataSyncType.MulYearCardRenew: // 年卡续卡
                    break;
                case DataSyncType.MulYearCardSale: // 年卡销售
                    break;
                case DataSyncType.MulYearCardUnLoss: // 年卡解挂
                    break;
                case DataSyncType.MulYearCardUpdate: // 年卡更新信息
                    break;
                case DataSyncType.MulYearCardVoucherReturn: // 退年卡凭证
                    break;
                case DataSyncType.MulYearCardVoucherSale: // 年卡凭证销售
                    break;
                case DataSyncType.MulYearCardReturn: // 年卡退卡
                    break;
            }
        }
    }
}
