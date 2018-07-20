using DDD.Domain.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Core.Mapping
{
    public class CheckoffMonitorMap: BaseMap<CheckoffMonitor, int>
    {
        public CheckoffMonitorMap() : base("CHECKOFF_MONITORING")
        {
            #region Required Fields

            #endregion

            #region Properties

            Property(t => t.Id).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.CheckoffModule).HasColumnName("CHECKOFF_MODULE");
            Property(t => t.CheckoffTotal).HasColumnName("CHECKOFF_TOTAL");
            Property(t => t.CheckoffType).HasColumnName("CHECKOFF_TYPE");
            Property(t => t.FinishFlag).HasColumnName("FINISH_FLAG");
            Property(t => t.RandomNumber).HasColumnName("RANDOM_NUMBER");

            #endregion

            //全部实体通用的话可以配在基类
            #region Audit Properties

            Property(t => t.CreationTime).HasColumnName("CREATE_TIME");
            Property(t => t.LastModificationTime).HasColumnName("UPDATE_TIME");
            Property(t => t.Remark).HasColumnName("REMARK");

            Ignore(p => p.CreatorUserId);
            Ignore(p => p.DeletionTime);
            Ignore(p => p.IsDeleted);
            Ignore(p => p.DeleterUserId);

            Ignore(t => t.LastModifierUserId);
            #endregion
        }
    }
}
