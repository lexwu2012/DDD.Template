using System;
using System.Linq;
using DDD.Infrastructure.Domain.DbHelper;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Domain.Uow
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransientDependency
    {
        public virtual string GetNameOrConnectionString(ref string schema)
        {
            return InitConnectStr(ref schema);
            //if (ConfigurationManager.ConnectionStrings["Default"] != null)
            //{
            //    return "Default";
            //}

            //if (ConfigurationManager.ConnectionStrings.Count == 1)
            //{
            //    return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            //}

            //throw new Exception("Could not find a connection string definition for the application. Add a  connection string to application .config file.");
        }

        private string InitConnectStr(ref string schema)
        {
            string conStr = string.Empty;

            switch (DbConnectionHelper.DbType)
            {
                case "0"://生产
                    //conStr = _conf.GetDbConStrContextProd();
                    break;
                case "1"://测试
                    conStr = DbConnectionHelper.DbTestConn;
                    break;
                case "2"://开发
                    conStr = DbConnectionHelper.DbDevConn;
                    break;
                case "3"://生产备库
                    conStr = DbConnectionHelper.DbProdOnlyConn;
                    break;
            }
            if (string.IsNullOrWhiteSpace(conStr))
            {
                throw new Exception("数据库连接字符串为空");
            }
            var connectionString = conStr + ";Connect Timeout=60;";

            var conInfos = connectionString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var userIdInfo = conInfos.FirstOrDefault(q => q.ToUpper().Contains("USER ID"));
            var tmpArr = userIdInfo?.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            schema = tmpArr[1];
            schema = schema.ToUpper();

            return conStr;
        }
    }
}
