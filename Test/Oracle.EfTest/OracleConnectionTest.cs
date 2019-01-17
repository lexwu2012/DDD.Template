using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.AutoMapper;
using DDD.Infrastructure.Ioc.Dependency;
using Shouldly;
using Xunit;

namespace Oracle.EfTest
{
    public class OracleConnectionTest: OracleTestBaseWithLocalIocManager
    {
        private readonly IAutoMapperInitializer _autoMapperInitializer;
        private readonly ICheckoffAutoAcpAppService _checkoffAutoAcpAppService;

        public OracleConnectionTest()
        {
            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }

            _checkoffAutoAcpAppService = LocalIocManager.IocContainer.Resolve<ICheckoffAutoAcpAppService>();

            _autoMapperInitializer = LocalIocManager.IocContainer.Resolve<IAutoMapperInitializer>();
        }

        [Fact]
        public void UpdateSpecifyData()
        {
            _autoMapperInitializer.Initialize();
            var result = _checkoffAutoAcpAppService.UpdateCheckoffAutoAcp(1196780);

            result.Success.ShouldBeTrue();
        }
    }
}
