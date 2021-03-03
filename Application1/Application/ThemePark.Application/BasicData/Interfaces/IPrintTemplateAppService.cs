using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    public interface IPrintTemplateAppService:IApplicationService
    {
        /// <summary>
        /// 新增打印模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<int>> AddPrintTemplateAsync(AddPrintTemplateInput input);


        /// <summary>
        /// 修改打印模板
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> UpdatePrintTemplateAsync(UpdatePrintTemplateInput input,int id);

        /// <summary>
        /// 获取所有打印模板名称
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetAllFileNamesDto>>> GetAllFileNameAsync();

        /// <summary>
        /// 通过模板名称获取打印模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Task<Result<GetTemplateDto>> GetPrintTemplate(string templateName);

        /// <summary>
        /// 获取促销票\年卡\补票\入园单
        /// isContainUsed = false，将刨除已配置模板的促销票类
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetPrintTicketDto>>> GetPrintTicket(int parkid, bool isContainUsed);

        /// <summary>
        /// 获取促销票\年卡\补票\入园单
        /// isContainUsed = false，将刨除已配置模板的促销票类
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetPrintTicketDto>>> GetPrintYearCard(int parkid);

        /// <summary>
        /// 增加打印模板配置
        /// </summary>
        /// <returns></returns>
        Task<Result> AddPrintTemplateConfig(AddConfigInput input);


        /// <summary>
        /// 查询打印模板配置
        /// </summary>
        /// <returns></returns>
        Task<Result<List<SearchPrintConfigDto>>> SearchConfig(SearchPrintConfigInput input);


        /// <summary>
        /// 查询打印模板配置
        /// </summary>
        /// <returns></returns>
        Task<Result<List<SearchAgencyPriceSetDto>>> SearchAgencyPriceSet(SearchAgencyPriceSetInput input,int parkId);

        

        /// <summary>
        /// 保存团体价格设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> SaveOrInsertAgencyPricePrintSet(AgencyPricePrintSetInput input);


        /// <summary>
        /// 查询代理商有效期设置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        Task<Result<List<SearchAgencyValidDaySetDto>>> SearchAgencyValidDaysSet(SearchAgencyValidDaysSetInput input, int parkId);



        /// <summary>
        /// 保存团体有效期设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> SaveOrInsertAgencyValidDaysPrintSet(AgencyValidDaysSetInput input);




        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<Result> DeleteConfigs(List<int> ids);

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateConfig(UpdatePrintConfig input);

        /// <summary>
        /// 获取所有基础票类打印参数配置
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetTicketPrintSetsDto>>> GetTicketPrintSets(int parkId);

        /// <summary>
        /// 保存基础票类打印参数
        /// </summary>
        /// <returns></returns>
        Task<Result> SaveOrInsertTicketPrintSet(GetTicketPrintSetsDto input);

        /// <summary>
        /// 获取价格默认打印设置
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetPricePrintSetsDto>>> GetPricePrintSets();

        /// <summary>
        /// 保存价格设置
        /// </summary>
        /// <returns></returns>
        Task<Result> SavePricePrintSet(PricePrintSetInput input);

            /// <summary>
        /// 获取有效期默认打印设置
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetValidDaysPrintSetsDto>>> GetValidDaysPrintSets();


        /// <summary>
        /// 保存基础票类打印参数
        /// </summary>
        /// <returns></returns>
        Task<Result> SaveOrInsertValidDaysPrintSet(ValidDaysPrintSetInput input);

    
    }
}
