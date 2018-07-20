using DDD.Domain.Service.Wechat.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Service.Wechat
{
    public class WechatRepayDomainService : DomainServiceBase, IWechatRepayDomainService
    {
        //private readonly IRepositoryWithEntity<Core.Model.CheckoffAutoAcp> _checkoffAutoAcpRepository;
        private readonly ICheckoffAutoAcpRepository _checkoffAutoAcpRepository;
        private readonly IRepositoryWithEntity<Core.Model.CheckoffCommand> _checkoffCommandRepository;

        public WechatRepayDomainService(IRepositoryWithEntity<Core.Model.CheckoffCommand> checkoffCommandRepository,
            ICheckoffAutoAcpRepository checkoffAutoAcpRepository)
        {
            _checkoffCommandRepository = checkoffCommandRepository;
            _checkoffAutoAcpRepository = checkoffAutoAcpRepository;
        }

        public Result<PayInfoDto> GetPayInfo(GetPayInfoDto payInfoDto)
        {
            var personId = payInfoDto.IdPerson ?? 0;
            //todo: set the static constanct
            string creditChannel = string.IsNullOrWhiteSpace(payInfoDto.Channel) ? "Constant" : payInfoDto.Channel.ToUpper();

            var autoCheckoffQuery = _checkoffAutoAcpRepository.GetAll().Where(a => a.IdPerson == personId && (a.PayStatus == "u" || (a.PayStatus == "k" && a.CommandType == 3)));

            var isWithholding = _checkoffCommandRepository.GetAll().Any(a => a.ProType == "c") || autoCheckoffQuery.Any(a => a.ProType == "c");

            //todo: return the correct result
            var dto = new PayInfoDto();
            return Result.FromData(dto);
        }

        public Result<int> GetCheckoffCommandData()
        {
            var autoCheckoffQuery = _checkoffAutoAcpRepository.GetAll().Where(a => a.IdPerson == 44088219 && (a.PayStatus == "u" || (a.PayStatus == "k" && a.CommandType == 3)));

            var isWithholding = _checkoffCommandRepository.GetAll().Any(a => a.ProType == "c") || autoCheckoffQuery.Any(a => a.ProType == "c");

            //todo: return the correct result
            var data = 1;
            return Result.FromData(data);
        }

        public async Task<Result<int>> GetTodayAutoCheckoffTotalAsync()
        {
            var count = await _checkoffAutoAcpRepository.GetAll().Where(m => DbFunctions.DiffDays(m.CreationTime,DateTime.Today) == 0).CountAsync();

            return Result.FromData(count);
        }
    }
}
