using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class CheckoffCommand : FullAuditedEntity, IAggregateRoot
    {

        public string PayStatus { get; set; }

        public int? CommandType { get; set; }

        public int? IdPerson { get; set; }


        public int? IdCredit { get; set; }

        public decimal? Amount { get; set; }

        public string AccountName { get; set; }

        public string AccountNo { get; set; }

        public string BankName { get; set; }

        public string ProtocolNo { get; set; }
        
        public short? PayType { get; set; }

        public string UpdateIp { get; set; }

        public long? UpdateUser { get; set; }

        public string MyAccCode { get; set; }

        public string Batno { get; set; }
       
        public decimal? PayCount { get; set; }

        public decimal NoticFlag { get; set; }

        public string ProType { get; set; }

        public int? IdGoods { get; set; }
       
        public string OpenID { get; set; }

        public string Source { get; set; }

        public string ExternalNumber { get; set; }

        public string Remark { get; set; }
    }
}
