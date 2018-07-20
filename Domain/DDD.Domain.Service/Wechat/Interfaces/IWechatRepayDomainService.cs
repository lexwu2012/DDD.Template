using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Domain.Application;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Service.Wechat.Interfaces
{
    public interface IWechatRepayDomainService : IDomainService
    {
        Result<PayInfoDto> GetPayInfo(GetPayInfoDto payInfoDto);

        Task<Result<int>> GetTodayAutoCheckoffTotalAsync();

        Result<int> GetCheckoffCommandData();
    }
}
