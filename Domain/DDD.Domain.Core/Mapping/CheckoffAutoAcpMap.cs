using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;

namespace DDD.Domain.Core.Mapping
{
    public class CheckoffAutoAcpMap : BaseMap<CheckoffAutoAcp, int>
    {
        public CheckoffAutoAcpMap() : base("CHECKOFF_AUTO_ACP")
        {
            #region Required Fields

            #endregion

            #region Properties
            Property(t => t.Id).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Batno).HasColumnName("BATNO");
            Property(t => t.IdCredit).HasColumnName("ID_CREDIT");
            Property(t => t.ContractNo).HasColumnName("CONTRACT_NO");
            Property(t => t.NumInstalment).HasColumnName("NUM_INSTALMENT");
            Property(t => t.Amount).HasColumnName("AMOUNT");
            Property(t => t.BankName).HasColumnName("BANK_NAME");
            Property(t => t.AccountNo).HasColumnName("ACCOUNT_NO");
            Property(t => t.AccountName).HasColumnName("ACCOUNT_NAME");
            Property(t => t.MyAccCode).HasColumnName("MY_ACC_CODE");
            Property(t => t.PayType).HasColumnName("PAY_TYPE");
            Property(t => t.ProType).HasColumnName("PRO_TYPE");
            Property(t => t.PayStatus).HasColumnName("PAY_STATUS");
            Property(t => t.TransferStatus).HasColumnName("TRANSFER_STATUS");
            Property(t => t.AsyncPayStatus).HasColumnName("ASYNC_PAY_STATUS");
            Property(t => t.AsyncTransferStatus).HasColumnName("ASYNC_TRANSFER_STATUS");
            Property(t => t.SendTime).HasColumnName("SEND_TIME");
            Property(t => t.UpdateIp).HasColumnName("UPDATE_IP");
            
            Property(t => t.IdPerson).HasColumnName("ID_PERSON");
            Property(t => t.ProtocolNo).HasColumnName("PROTOCOL_NO");
            Property(t => t.CommandType).HasColumnName("COMMAND_TYPE");
            Property(t => t.RamNumber).HasColumnName("RAM_NUMBER");
            Property(t => t.CreditModel).HasColumnName("CREDIT_MODEL");
            Property(t => t.PartnerDeduct).HasColumnName("PARTNER_DEDUCT");

            Property(t => t.CreditChannel).HasColumnName("CREDIT_CHANNEL");

            #endregion

            #region Audit Properties

            Property(t => t.CreationTime).HasColumnName("CREATE_TIME");
            Property(t => t.LastModifierUserId).HasColumnName("UPDATE_USER");
            Property(t => t.LastModificationTime).HasColumnName("UPDATE_TIME");
            Property(t => t.Remark).HasColumnName("REMARK");

            Ignore(p => p.CreatorUserId);
            Ignore(p => p.DeletionTime);
            Ignore(p => p.IsDeleted);
            Ignore(p => p.DeleterUserId);
            #endregion

            //HasRequired(m => m.Address).WithMany().WillCascadeOnDelete(false);
        }
    }
}
