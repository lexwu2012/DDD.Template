using System;
using System.Data.Entity;
using System.Linq;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.SqlServer.Test;
using Shouldly;
using Xunit;

namespace DDD.Oracle.Test.OracleTest
{
    public class OracleConnectionTests : OracleTestBaseWithLocalIocManager
    {

        private readonly ICheckoffAutoAcpRepository _checkoffAutoAcpRepository;
        private readonly ICheckoffAutoAcpAppService _checkoffAutoAcpAcpAppService;

        public OracleConnectionTests()
        {
            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }

            _checkoffAutoAcpRepository = LocalIocManager.IocContainer.Resolve<ICheckoffAutoAcpRepository>();
            _checkoffAutoAcpAcpAppService = LocalIocManager.IocContainer.Resolve<ICheckoffAutoAcpAppService>();
        }

        [Fact]
        public void GetSpecifyData()
        {
            //var count = _checkoffAutoAcpRepository.GetAll().Count();

            //count.ShouldNotBeNull();

            //var total = _checkoffAutoAcpRepository.GetAll().Count(m => DbFunctions.DiffDays(m.CreationTime, DateTime.Today) == 0);
            var data = _checkoffAutoAcpRepository.GetAll().Where(m => m.Batno == "M79_SINGLEACP_18921754");
            data.ShouldNotBeNull();
        }

       
    }
}
