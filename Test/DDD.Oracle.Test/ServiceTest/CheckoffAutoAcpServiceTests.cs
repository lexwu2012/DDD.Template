﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DDD.Application.Service.CheckoffAutoAcp.Interfaces;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.AutoMapper;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Query;
using Shouldly;
using Xunit;

namespace DDD.Oracle.Test.ServiceTest
{
    public class CheckoffAutoAcpServiceTests: OracleTestBaseWithLocalIocManager
    {
        private readonly IAutoMapperInitializer _autoMapperInitializer;
        private readonly ICheckoffAutoAcpAppService _checkoffAutoAcpAppService;

        public CheckoffAutoAcpServiceTests()
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
            _autoMapperInitializer.Initial();
            var result = _checkoffAutoAcpAppService.UpdateCheckoffAutoAcp(1196780);

            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async void GetSpecifyAutoAcpData()
        {
            try
            {
                _autoMapperInitializer.Initial();
                var result = await _checkoffAutoAcpAppService.GetCheckoffAutoAcpAsync<CheckoffAutoAcpDto>(new Query<CheckoffAutoAcp>(m => m.Id == 1196780));

                result.ShouldNotBeNull();
            }
            catch (Exception exception)
            {
                
                throw exception;
            }
            
        }

        [Fact]
        public async void GetListAutoAcpData()
        {
            _autoMapperInitializer.Initial();
            var result = await _checkoffAutoAcpAppService.GetCheckoffAutoAcpListAsync<CheckoffAutoAcpDto>(new Query<CheckoffAutoAcp>(m => m.Id == 1196780));

            result.ShouldNotBeNull();
        }

        [Fact]
        public void CountTest()
        {
            var result = _checkoffAutoAcpAppService.GetTodayCheckoffAutoAcp();
            result.Data.Count().ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void InsertTest()
        {
            var entity = new CheckoffAutoAcp
            {
                Batno = "M79_SINGLEACP_123456789",
                IdCredit = 1474579,
                ContractNo = 11538949001,
                NumInstalment = "1",
                Amount = 10,
                BankName = "中国建设银行",
                AccountNo = "6232111820006302529",
                AccountName = "lex",
                MyAccCode = "M79",
                PayType = 1,
                ProType = "o",
                PayStatus = "f",
                SendTime = DateTime.Today.AddDays(-10),
                CommandType = 1,
                CreationTime = DateTime.Now,
                PartnerDeduct = 0,
                CreditChannel = "GIVEU"
            };
            
        }
    }
}
