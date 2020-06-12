using System;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class CheckoffAutoAcp : FullAuditedEntity, IAggregateRoot
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

        public string TransferStatus { get; set; }

        public string AsyncPayStatus { get; set; }

        public string AsyncTransferStatus { get; set; }

        public string Remark { get; set; }

        public DateTime? SendTime { get; set; }

        //public DateTime? UpdateTime { get; set; }

        public string UpdateIp { get; set; }

        //public int? UpdateUser { get; set; }

        public short? RamNumber { get; set; }

        //public DateTime? CreateTime { get; set; }

        public string CreditModel { get; set; }


        public string CreditChannel { get; set; }

        public int PartnerDeduct { get; set; }
    }
}
