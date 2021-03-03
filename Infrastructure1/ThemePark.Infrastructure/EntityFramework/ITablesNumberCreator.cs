using System;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Core;

namespace ThemePark.Infrastructure.EntityFramework
{
    public interface ITablesNumberCreator
    {
        /// <summary>
        /// 获取实体对应数据表的编号
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Int32.</returns>
        int GetTableNo(Type type);

        /// <summary>
        /// 返回最后一条数据的Primary key
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        Task<object> GetTableLastPk(Type type);

        /// <summary>
        /// 返回最后一条数据的ECardID
        /// </summary>
        /// <returns>long as string or string.</returns>
        Task<object> GetLastECardID();

        /// <summary>
        /// 获取生成对应编码类型的表实体类型
        /// </summary>
        /// <param name="codeType">Type of the code.</param>
        /// <returns>Type[].</returns>
        Type[] GetTableType(CodeType codeType);
    }
}
