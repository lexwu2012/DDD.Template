using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies
{
    /// <summary>
    /// 代理商预支付辅助类
    /// </summary>
    public static class PreDepositApiHelp
    {
        /// <summary>
        /// 调用中心账户充值接口
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public static async Task<Result> SyncAccountAsync(AccountDepositInput input,int parkId)
        {
            var syncParkRepository = IocManager.Instance.Resolve<IRepository<SyncPark>>();
            var parkAreaAppService = IocManager.Instance.Resolve<IParkAreaAppService>();
            var localParkArea = await parkAreaAppService.GetParkAreaAsync<AccountParkAreaDto>(new Query<ParkArea>(m => m.ParkId == parkId));
            var areas = await parkAreaAppService.GetParkAreasAsync<AccountParkAreaDto>(new Query<ParkArea>(m => m.ParentPath == localParkArea.ParentPath));
            //获取同一度假区公园列表
            var parkIds = areas.Where(p => p.ParkId != null).Select(p => p.ParkId).ToList();
            foreach (var toParkId in parkIds)
            {
                if(toParkId == parkId)
                    continue;
                try
                {
                    //获取中心同步地址
                    var sync = await syncParkRepository.GetAll().FirstAsync(o => o.ParkId == toParkId);
                    var uri = new Uri(sync.SyncUrl);
                    //创建用户的ID
                    await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/PreDeposit/SyncAccountAsync", JsonConvert.SerializeObject(input));
                }
                catch (Exception ex)
                {
                    return Result.FromCode(ResultCode.Fail, $"{toParkId}公园网络连接异常");
                }
            }
            return Result.Ok();

        }

     


    }
}
