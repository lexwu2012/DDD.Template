using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Domain.Core.Model.Repositories.Dto
{
    [AutoMap(typeof(CheckoffAutoAcp))]
    public class CheckoffAutoAcpDto
    {
        public int IdCredit { get; set; }

        public string PayStatus { get; set; }

        public long IdPerson { get; set; }

        public int CommandType { get; set; }

        public string Batno { get; set; }

        public string ProtocolNo { get; set; }

        public long ContractNo { get; set; }

        public string NumInstalment { get; set; }

        public decimal? Amount { get; set; }

        public string BankName { get; set; }

        public string AccountNo { get; set; }

        public string AccountName { get; set; }

        public string MyAccCode { get; set; }

        public short? PayType { get; set; }

        public string ProType { get; set; }
    }
}
