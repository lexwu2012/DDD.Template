using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    public interface IVendorSetAppService: IApplicationService
    {
        /// <summary>
        /// 新增自助售票机配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddVendorSet(AddVendorSetInput input);
        
        /// <summary>
        /// 新增自助售票机配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateVendorSet(UpdateVendorSetInput input);
        
        /// <summary>
        /// 删除自助售票机配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteVendorSet(int id);
        
        /// <summary>
        /// 查找自助售票机配置
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Result<List<TDto>>> SearchVendorSet<TDto>(SearchVendorSetInput input);
        
        /// <summary>
        /// 查找自助售票机用户
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        Task<Result<VendorSet>> VendorSetLogin(string ip);

        /// <summary>
        /// 终端配置
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result<VendorSetDto>> GetVendorSet(int terminalId);

        /// <summary>
        /// 更新发票信息
        /// </summary>
        /// <returns></returns>
        Task<Result> UpdateTicketMax(int terminalId,int qty);

        /// <summary>
        /// 查找自助售票机所有允许账号
        /// </summary>
        /// <returns></returns>
        Task<Result<List<DropdownItem<long>>>> GetValidUsers(int parkId);

        /// <summary>
        /// 获取可用终端号
        /// </summary>
        /// <returns></returns>
        Task<Result<List<DropdownItem>>> GetValidTerminals();

        /// <summary>
        /// 获取自助售票机订单记录
        /// </summary>
        /// <returns></returns>
        Task<Result<List<VendorTicketTrackDto>>> GetVendorTicketTrack(GetVendorTicketTrackInput input,bool isError);

    }
}
