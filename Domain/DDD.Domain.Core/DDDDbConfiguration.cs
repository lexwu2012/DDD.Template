using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;

namespace DDD.Domain.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class DDDDbConfiguration : DbConfiguration
    {
        public DDDDbConfiguration()
        {
            this.SetDefaultConnectionFactory(new OracleConnectionFactory());
            this.SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
            this.SetProviderFactory("Oracle.ManagedDataAccess.Client", new OracleClientFactory());
        }
    }
}
