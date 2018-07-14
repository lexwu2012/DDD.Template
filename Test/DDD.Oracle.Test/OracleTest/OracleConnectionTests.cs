using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Oracle.Test;
using Shouldly;
using Xunit;

namespace DDD.Test.OracleTest
{
    public class OracleConnectionTests : TestBaseWithLocalIocManager
    {

        private readonly ICheckoffAutoAcpRepository _checkoffAutoAcpRepository;

        public OracleConnectionTests()
        {
            _checkoffAutoAcpRepository = LocalIocManager.IocContainer.Resolve<ICheckoffAutoAcpRepository>();
        }

        [Fact]
        public void ConnectionTest()
        {
            var count = _checkoffAutoAcpRepository.GetAll().Count();
            count.ShouldNotBeNull();
        }

        [Fact]
        public void CountTest()
        {
            //var obj = _checkoffAutoAcpRepository.GetAll()
            //    .Where(m => DbFunctions.DiffDays(m.CreateTime, DateTime.Now) > 0)
            //    .OrderBy(m => m.PayStatus)
            //    .GroupBy(m => new { m.MyAccCode, m.PayStatus })
            //    .SelectMany(m => m.Select(o => new { m.Key.MyAccCode, m.Key.PayStatus }));

            //obj.Count().ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void InsertTest()
        {
            //var entity = new CheckoffAutoAcp
            //{
            //    Batno = "M79_SINGLEACP_123456789",
            //    IdCredit = 1474579,
            //    ContractNo = 11538949001,
            //    NumInstalment = "1",
            //    Amount = 10,
            //    BankName = "中国建设银行",
            //    AccountNo = "6232111820006302529",
            //    AccountName = "lex",
            //    MyAccCode = "M79",
            //    PayType = 1,
            //    ProType = "o",
            //    PayStatus = "f",
            //    SendTime = DateTime.Today.AddDays(-10),
            //    CommandType = 1,
            //    CreateTime = DateTime.Now,
            //    PartnerDeduct = 0,
            //    CreditChannel = "GIVEU"
            //};
            //var id = _checkoffAutoAcpRepository.InsertAndGetId(entity);
            //id.ShouldNotBeNull();
        }
    }
}
