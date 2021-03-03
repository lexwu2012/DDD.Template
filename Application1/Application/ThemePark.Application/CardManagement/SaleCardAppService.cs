using System;
using System.Threading.Tasks;
using ThemePark.Application.CardManagement.Interfaces;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.CardManagement
{
    /// <summary>
    /// 年卡销售服务接口
    /// </summary>
    public class SaleCardAppService : ThemeParkAppServiceBase, ISaleCardAppService
    {
        #region Fields

        #endregion

        #region Cotr

        /// <summary>
        /// 构造函数
        /// </summary>
        public SaleCardAppService()
        {

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 添加售卡记录
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddSaleCardRecord()
        {
            throw new InvalidOperationException();
        }
        #endregion
    }
}
