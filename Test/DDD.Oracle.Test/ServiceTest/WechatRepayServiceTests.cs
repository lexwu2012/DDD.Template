using System.Threading.Tasks;
using DDD.Application.Service.Wechat.Interfaces;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.SqlServer.Test;
using Shouldly;
using Xunit;

namespace DDD.Oracle.Test.ServiceTest
{
    public class WechatRepayServiceTests : OracleTestBaseWithLocalIocManager
    {
        public WechatRepayServiceTests()
        {
            //LocalIocManager.RegisterIfNot<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);

            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }
        }

        [Fact]
        public async void Should_Resolve_Service_Correct()
        {
            var wechatService = LocalIocManager.IocContainer.Resolve<IWechatRepayService>();

            var dto = new GetPayInfoDto
            {
                IdPerson = 1,
                SystemCode = "22",
                Channel = "33"
            };
            var result = wechatService.GetPayInfoAsync(dto);

            result.Success.ShouldBeTrue();

            var count = await wechatService.GetTodayAutoCheckoffTotalAsync();

            count.Data.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}
