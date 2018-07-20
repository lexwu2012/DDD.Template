using System.Configuration;
using DDD.Infrastructure.Common.Security;

namespace DDD.Infrastructure.Domain.DbHelper
{
    public class DbConnectionHelper
    {
        public static string DbType => ConfigurationManager.AppSettings["DBType"];

        public static string DbCatagory => ConfigurationManager.AppSettings["DbCatagory"];

        public static string DbTestConn => DesHelper.Decrypt(ConfigurationManager.AppSettings["connectionstringTest"]);

        public static string DbDevConn => DesHelper.Decrypt(ConfigurationManager.AppSettings["connectionstringDev"]);

        public static string DbProdOnlyConn => DesHelper.Decrypt(ConfigurationManager.AppSettings["connectionstringProdReadOnly"]);
    }

    public enum DBType
    {
        Oracle = 1,

        SqlServer = 2
    }
}
