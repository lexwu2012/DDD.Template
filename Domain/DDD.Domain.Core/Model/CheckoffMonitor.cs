using DDD.Domain.Auditing;
using DDD.Domain.BaseEntities;
using DDD.Domain.Common.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Core.Model
{
    public class CheckoffMonitor: FullAuditedEntity, IAggregateRoot
    {
        public string CheckoffType { get; set; }

        public string CheckoffModule { get; set; }

        public long CheckoffTotal { get; set; }

        public int FinishFlag { get; set; }

        public long RandomNumber { get; set; }

        public string Remark { get; set; }
    }
}
