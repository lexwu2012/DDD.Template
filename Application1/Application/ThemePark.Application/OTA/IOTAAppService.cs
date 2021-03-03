using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.OTA.DTO;
using ThemePark.Infrastructure.Application;
using ThemePark.Application.SaleCard.Dto;

namespace ThemePark.Application.OTA
{
    public interface IOTAAppService: IApplicationService
    {

        /// <summary>
        /// 获取电子年卡照片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<string>> GetVipCardPhotoAsync(ECardPhotoInput input);

        /// <summary>
        /// 获取电子年卡信息(无照片)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<ECardDetailDto>>> SearchECardDetailNoPhotoAsync(ECardDetailInput input);

        /// <summary>
        /// 获取电子年卡信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<ECardDetailDto>>> SearchECardDetailAsync(ECardDetailInput input);

        /// <summary>
        /// 电商下单接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<OTAOrderDto>> AddOTAOrderAsync(OrderInput input);

        /// <summary>
        /// 预订订单接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<OTAPreOrderDto>>PreOrderAsync(OrderInput input);

        /// <summary>
        /// 订单支付接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<OTAPayOrderDto>>PayOrderAsync(PayOrderInput input);


        /// <summary>
        /// 取消订单,返回主订单号
        /// </summary>
        Task<Result<string>> CancelOTAOrderAsync(OTACancelOrderInput input);


        /// <summary>
        /// Api电商查询订单详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<OTAOrderDetailDto>>> SearchOTAOrderAsync(OTADetailInput input);

        /// <summary>
        /// 检查身份证订票数量
        /// </summary>
        /// <returns></returns>
        Task<Result<List<CheckPidDto>>> CheckOTAPidAsync(CheckPidInput input);

        /// <summary>
        /// 修改订单身份证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> ModifyPidAsync(ModifyPidInput input);


        /// <summary>
        /// 改签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> ModifyPlandateAsync(OTAModifyPlandateInput input);

        /// <summary>
        /// 通过电话号码查询订单
        /// </summary>
        Task<Result<List<OTASearchByPhoneDto>>> SearchOTAOrderByPhone(string phone);

    }
}

