﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class User : FullAuditedEntity<long>, IAggregateRoot<long>
    {
        public string Name { get; set; }

        ///// <summary>
        ///// 身份证
        ///// </summary>
        //public long PIdentity { get; set; }
        
        public Address Address { get; set; }

        public string Remark { get; set; }
    }
}
