using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Common;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 窗口网络取票API帮助类
    /// </summary>
    public class WebTicketApiHelp
    {
        /// <summary>
        /// 查询其他公园网络订单
        /// </summary>
        /// <param name="pidOrTicketCode"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public static async Task<Result<List<GetWebTicketOrderDto>>> GetOrderAsync(string pidOrTicketCode, int parkId)
        {
            var syncParkRepository = IocManager.Instance.Resolve<IRepository<SyncPark>>();
            var parkAreaAppService = IocManager.Instance.Resolve<IParkAreaAppService>();
            var localParkArea = await parkAreaAppService.GetParkAreaAsync<AccountParkAreaDto>(new Query<ParkArea>(m => m.ParkId == parkId));
            var areas = await parkAreaAppService.GetParkAreasAsync<AccountParkAreaDto>(new Query<ParkArea>(m => m.ParentPath == localParkArea.ParentPath));
            //获取同一度假区公园列表
            var parkIds = areas.Where(p => p.ParkId != null).Select(p => p.ParkId).ToList();
            var orderList = new List<GetWebTicketOrderDto>();
            foreach (var toParkId in parkIds)
            {

                if (toParkId == parkId)
                    continue;
                try
                {
                    //获取中心同步地址
                    var sync = await syncParkRepository.GetAll().FirstAsync(o => o.ParkId == toParkId);
                    var uri = new Uri(sync.SyncUrl);
                    //创建用户的ID
                    var jsonResult = await HttpHelper.GetAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/VendorTicket/GetWebTicketOrder?pidOrTicketCode=" + pidOrTicketCode);
                    var result = JsonConvert.DeserializeObject<Result<List<GetWebTicketOrderDto>>>(jsonResult);
                    if (result.Success && result.Data != null)
                        orderList.AddRange(result.Data);
                }
                catch (Exception ex)
                {
                    return Result.FromData(orderList, $"{toParkId}公园网络连接异常");
                }
            }
            return Result.FromData(orderList);

        }

        /// <summary>
        /// 长训其他公园订单详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<Result<List<SearchVendorOrderDto>>> GetOrderDetailAsync(List<GetOrderDetailInput> input)
        {
            var syncParkRepository = IocManager.Instance.Resolve<IRepository<SyncPark>>();
            var source = input.GroupBy(p => p.ParkId);
            var orderList = new List<SearchVendorOrderDto>();
            foreach (var parkData in source)
            {
                var toParkId = parkData.Key;
                try
                {
                    //获取中心同步地址
                    var sync = await syncParkRepository.GetAll().FirstAsync(o => o.ParkId == toParkId);
                    var uri = new Uri(sync.SyncUrl);
                    //创建用户的ID
                    var jsonResult = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/VendorTicket/GetWebTicketOrderDetail", JsonConvert.SerializeObject(parkData.ToList()));
                    var result = JsonConvert.DeserializeObject<Result<List<SearchVendorOrderDto>>>(jsonResult);
                    if (result.Success && result.Data != null)
                        orderList.AddRange(result.Data);
                }
                catch (Exception ex)
                {
                    return Result.FromData(orderList, $"{toParkId}公园网络连接异常");
                }
            }
            return Result.FromData(orderList);

        }

        /// <summary>
        /// 其他公园取窗口网络订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<Result<List<PrintInfo>>> TakeWebTicketAsync(List<ToTicketInput> input)
        {
            var syncParkRepository = IocManager.Instance.Resolve<IRepository<SyncPark>>();
            var printInfos = new List<PrintInfo>();
            foreach (var item in input)
            {
                var toParkId = item.ParkId;
                try
                {
                    //获取中心同步地址
                    var sync = await syncParkRepository.GetAll().FirstAsync(o => o.ParkId == toParkId);
                    var uri = new Uri(sync.SyncUrl);
                    //创建用户的ID
                    var jsonResult = await HttpHelper.PostAsync(uri.AbsoluteUri.Replace(uri.LocalPath, ""), "/Api/VendorTicket/TakeWebticket", JsonConvert.SerializeObject(item));
                    var result = JsonConvert.DeserializeObject<Result<List<PrintInfo>>>(jsonResult);
                    if (result.Success && result.Data != null)
                        printInfos.AddRange(result.Data);
                }
                catch (Exception ex)
                {
                    return Result.FromData(printInfos, $"{toParkId}公园网络连接异常");
                }
            }

            if (printInfos.Any())
                return Result.FromData(printInfos);
            else
            {
                return Result.FromCode<List<PrintInfo>>(ResultCode.Fail);
            }
        }


    }
}
