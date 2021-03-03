using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.OTA.DTO;
using ThemePark.Application.SaleCard.Dto;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Model;

namespace ThemePark.Application.SaleCard.Interfaces
{
    /// <summary>
    /// 年卡相关接口
    /// </summary>
    public  interface IVIPCardAppService : IApplicationService
    {
        /// <summary>
        /// 根据身份证获取年卡用户详细信息（最新记录）
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        VipCardCustomerDto GetCustomerDetail(string idnum);

        /// <summary>
        /// 根据身份证获取年卡用户详细信息列表
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        List<VipCardCustomerDto> GetListCustomerDetail(string idnum);

        /// <summary>
        /// 根据身份证获取年卡用户详细信息
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        List<VipCardCustomerDto> SearchCustomerDetail(string idnum);

        /// <summary>
        /// 旧年卡批量生成电子年卡卡号
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> CreateOldCardEcardId(int terminalId);

        /// <summary>
        /// 获取用户照片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<string>> SearchECardPhotoAsync(ECardPhotoInput input);

        /// <summary>
        /// 查询年卡信息（闸验 验证身份证）
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        Task<Result<List<ListVipCardDto>>> GetCardInfoByPidAsync(string pid);


        /// <summary>
        /// 获取电子年卡信息（OTA）无照片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<List<ECardDetailDto>>> SearchECardDetailNoPhotoAsync(ECardDetailInput input);


        /// <summary>
        /// 获取电子年卡信息（OTA）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<List<ECardDetailDto>>> SearchECardDetailAsync(ECardDetailInput input);

        /// <summary>
        /// 查询年卡信息（关联的完整人员 多人年卡）
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        Task<Result<List<ListVipCardDto>>> GetCardInfoAllAsync(string icno);


        /// <summary>
        /// 根据查询条件获取年卡凭证信息列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetYearCardVoucherListAsync<TDto>(IQuery<VIPVoucher> query);

        /// <summary>
        /// 根据查询条件获取年卡信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetYearCardListAsync<TDto>(IQuery<VIPCard> query);

        /// <summary>
        /// 获取年卡基础类型
        /// </summary>
        /// <returns></returns>
        Task<IList<TicketClassDto>> GetYearCardTypes(int parkId);

        /// <summary>
        /// 获取凭证信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<VIPVoucherDto> GetVoucherDetailInfoByIdAsync(long Id);

        /// <summary>
        /// 获取年卡初始化信息
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        Task<VIPCardDto> GetCardBasicInfoAsync(string  icno);
        /// <summary>
        /// 获取年卡详细信息
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        Task<VIPCardDetailDto> GetCardDetailInfoAsync(string icno);

        /// <summary>
        /// 获取年卡凭证信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        Task<VIPVoucherDto> GetVoucherDetailInfoAsync(string barcode, int parkId, int terminalId);

        /// <summary>
        /// 年卡凭证销售
        /// </summary>
        /// <param name="vipVoucherInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="invoiceInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AddYearCardVoucherAsync(List<VIPVoucherInput> vipVoucherInput, string tradeno, InvoiceInput invoiceInput, int parkId, int terminalId);

        /// <summary>
        /// 年卡销售
        /// </summary>
        /// <param name="vipCardInput"></param>
        /// <param name="vipCardBasicInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AddYearCardAsync(List<VipCardInput> vipCardInput, List<VIPCardBasicInput> vipCardBasicInput, string tradeno, int parkId, int terminalId);

        /// <summary>
        /// 年卡挂失与解挂
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> LossYearCardAsync(int vipCardId,string applyName,string applyPhone,string applyPid, int terminalId, int parkId);

        /// <summary>
        /// 退卡
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> ReturnYearCardAsync(int vipCardId, string applyName, string applyPhone, string applyPid, int terminalId,int parkId);

        /// <summary>
        /// 退凭证
        /// </summary>
        /// <param name="vipVoucherId"></param>
        /// <param name="applyName"></param>
        /// <param name="applyPhone"></param>
        /// <param name="applyPid"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> ReturnVoucherAsync(int vipVoucherId, string applyName, string applyPhone, string applyPid, int terminalId, int parkId);

        /// <summary>
        /// 激活年卡、年卡凭证
        /// </summary>
        /// <param name="cardInfoInput"></param>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> AddYearCardAsync(CardInfoInput cardInfoInput,List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId);


        /// <summary>
        /// 激活电子年卡
        /// </summary>
        /// <param name="cardInfoInput"></param>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> AddEcardYearCardAsync(CardInfoInput cardInfoInput, List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId);

        /// <summary>
        /// 修改年卡用户信息
        /// </summary>
        /// <param name="customerInput"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result> AlterYearCardCustomerAsync( List<CustomerInput> customerInput, int terminalId, ZWJType fingerType, int parkId);

        /// <summary>
        /// 分页获取年卡数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<ListVipCardDto>> GetAllByPageAsync(SearchVipCardModel query = null);

        /// <summary>
        /// 查询年卡信息
        /// </summary>
        /// <param name="icno">IC卡内码、手机号、身份证、姓名</param>
        /// <returns></returns>
        Task<Result<List<ListVipCardDto>>> GetCardInfoAsync(string icno);

        /// <summary>
        /// 获取年卡支付详情
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <returns></returns>
        Task<Result<TradeInfoDto>> GetYearCardPayDetail(long vipCardId);

        /// <summary>
        /// 获取凭证支付详情
        /// </summary>
        /// <param name="vipVoucherId"></param>
        /// <returns></returns>
        Task<Result<TradeInfoDto>> GetVipVoucherPayDetail(long vipVoucherId);

        /// <summary>
        /// 根据ID获取年卡详情
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <returns></returns>
        Task<Result<List<ListVipCardDto>>> GetCardInfoByIdAsync(int vipCardId);

        /// <summary>
        /// 获取年卡配置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        string GetCardSettingValue(string name, int parkId);

        /// <summary>
        /// 获取年卡续卡价格
        /// </summary>
        /// <param name="vipCardId"></param>
        /// <param name="ticketClassId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result<string>> GetRenewalCardPriceAsync(int vipCardId,int ticketClassId, int parkId);

        /// <summary>
        /// 人脸识别检测
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        Task<Result<string>> CheckFacePhoto(string photo);

        /// <summary>
        /// 年卡补卡
        /// </summary>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <param name="fingerType"></param>
        /// <returns></returns>
        Task<Result> FillYearCardAsync(FillCardInput fillCardInput, string tradeno, int parkId, int terminalId);

        /// <summary>
        /// 年卡续卡
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> RenewalYearCardAsync(decimal amount, FillCardInput fillCardInput, string tradeno, int parkId, int terminalId);

        /// <summary>
        /// 年卡续卡(不换卡)
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="fillCardInput"></param>
        /// <param name="tradeno"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> RenewalYearCardNoChangeCardAsync(decimal amount, FillCardInput fillCardInput, string tradeno, int parkId, int terminalId);

        /// <summary>
        /// 获取指纹数据
        /// </summary>
        /// <returns></returns>
        List<FingerDataDto> GetFingerData(DateTime lastGetTime);

        /// <summary>
        /// 根据条件获取年卡记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetVIPCardAsync<TDto>(IQuery<VIPCard> query);

        /// <summary>
        /// 根据条件获取年卡记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetVIPCardListAsync<TDto>(IQuery<VIPCard> query);

        /// <summary>
        /// 根据查询条件获取年卡凭证信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetYearCardVoucherAsync<TDto>(IQuery<VIPVoucher> query);

        /// <summary>
        /// 根据条件获取年卡续卡价格配置列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetVipCardRenewalSetListAsync<TDto>(IQuery<VipCardRenewalSet> query);

        /// <summary>
        /// 新增或修改续卡价格
        /// </summary>
        /// <param name="yearCardRenewalSetInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> AddOrUpYearCardRenewalSet(YearCardRenewalSetInput yearCardRenewalSetInput, int parkId, int terminalId);

        /// <summary>
        /// 新增售卡记录
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="terminalId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result<string>> AddYearCardVoucherAndReturnTradeNumAsync(SaveYearCardVoucherDto dto,
            int terminalId, int parkId);
    }
}
