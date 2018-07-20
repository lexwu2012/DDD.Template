using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;

namespace DDD.Domain.Core.Mapping
{
    public class CheckoffCommandMap : BaseMap<CheckoffCommand, int>
    {
        public CheckoffCommandMap() : base("CHECKOFF_COMMAND")
        {
            #region Required Fields

            #endregion

            #region Properties
            Property(t => t.Id).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Batno).HasColumnName("BATNO");
            Property(t => t.IdCredit).HasColumnName("ID_CREDIT");
            Property(t => t.Amount).HasColumnName("AMOUNT");
            Property(t => t.BankName).HasColumnName("BANK_NAME");
            Property(t => t.AccountNo).HasColumnName("ACCOUNT_NO");
            Property(t => t.AccountName).HasColumnName("ACCOUNT_NAME");
            Property(t => t.MyAccCode).HasColumnName("MY_ACC_CODE");
            Property(t => t.PayType).HasColumnName("PAY_TYPE");
            Property(t => t.ProType).HasColumnName("PRO_TYPE");
            Property(t => t.PayStatus).HasColumnName("PAY_STATUS");
            Property(t => t.UpdateIp).HasColumnName("UPDATE_IP");

            Property(t => t.IdPerson).HasColumnName("ID_PERSON");
            Property(t => t.ProtocolNo).HasColumnName("PROTOCOL_NO");
            Property(t => t.CommandType).HasColumnName("COMMAND_TYPE");
            Property(t => t.PayCount).HasColumnName("PAY_COUNT");
            Property(t => t.NoticFlag).HasColumnName("NOTIC_FLAG");
            Property(t => t.IdGoods).HasColumnName("ID_GOODS");
            Property(t => t.OpenID).HasColumnName("OPEN_ID");
            Property(t => t.Source).HasColumnName("SOURCE");
            Property(t => t.ExternalNumber).HasColumnName("EXTERNAL_NUMBER");
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
        }
    }
}
