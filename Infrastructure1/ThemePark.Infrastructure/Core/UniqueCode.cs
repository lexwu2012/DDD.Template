using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using ThemePark.Common;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.Infrastructure.Web;

namespace ThemePark.Infrastructure.Core
{
    /// <summary>
    /// 生成唯一码的类型
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// 条码，len=16
        /// </summary>
        Barcode = 1,

        /// <summary>
        /// 订单，len=18
        /// </summary>
        Order = 2,

        /// <summary>
        /// 交易号，len=18
        /// </summary>
        Trade = 3,

        /// <summary>
        /// 取票码，len=18
        /// </summary>
        Voucher = 4,

        /// <summary>
        /// 电子年卡，len=18
        /// </summary>
        ECard = 5
    }

    /// <summary>
    /// 生成唯一码
    /// </summary>
    public class UniqueCode : IUniqueCode, ITransientDependency
    {
        #region Fields

        /// <summary>
        /// redis key
        /// </summary>
        public const string UniqueCodeKey = "UniqueCode:";

        /// <summary>
        /// The Barcode maximum flow
        /// </summary>
        private static readonly int BarMaximumFlow = (int)Math.Pow(2, 16) - 1;

        /// <summary>
        /// The other UniqueCode maximum flow
        /// </summary>
        private static readonly int OtherMaximumFlow = (int)Math.Pow(2, 18) - 1;

        private readonly IAbpRedisCacheDatabaseProvider _abpRedisCacheDatabaseProvider;

        private readonly IParkCodeProvider _parkCodeProvider;

        private readonly ITerminalCodeProvider _terminalCodeProvider;

        private readonly ITablesNumberCreator _tablesNumberCreator;

        private static readonly AsyncLock BarLockObj = new AsyncLock();
        private static readonly AsyncLock OtherLockObj = new AsyncLock();
        private static readonly AsyncLock EcardLockObj = new AsyncLock();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        /// <param name="abpRedisCacheDatabaseProvider">The abp redis cache database provider.</param>
        /// <param name="parkCodeProvider">The park code provider.</param>
        /// <param name="terminalCodeProvider">The terminal code provider.</param>
        public UniqueCode(IAbpRedisCacheDatabaseProvider abpRedisCacheDatabaseProvider, IParkCodeProvider parkCodeProvider, ITerminalCodeProvider terminalCodeProvider, ITablesNumberCreator tablesNumberCreator)
        {
            _abpRedisCacheDatabaseProvider = abpRedisCacheDatabaseProvider;
            _parkCodeProvider = parkCodeProvider;
            _terminalCodeProvider = terminalCodeProvider;
            _tablesNumberCreator = tablesNumberCreator;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 生成唯一码
        /// </summary>
        /// <param name="codeType">唯一码的类型</param>
        /// <param name="parkId">公园Id</param>
        /// <param name="terminalId">终端Id。 codeType为 <see cref="CodeType.Barcode"/> 类型时需要传终端号</param>
        /// <returns>唯一码</returns>
        public async Task<string> CreateAsync(CodeType codeType, int parkId, int terminalId = 0)
        {
            var now = DateTime.Now;

            int parkCode = _parkCodeProvider.ParkCode(parkId);
            var flow = await GetFlow(codeType, terminalId);

            long value = 0;
            switch (codeType)
            {
                case CodeType.Barcode:
                    if (terminalId <= 0)
                    {
                        throw new Exception("wrong termianl id");
                    }

                    int terminal = _terminalCodeProvider.TerminalCode(terminalId);
                    terminal = terminal == 0 ? terminalId : terminal;
                    value = BasicCode.GetBarcode(parkCode, now.Year, now.Month, now.Day, terminal, flow);
                    break;

                case CodeType.Order:
                    value = BasicCode.GetOrderCode(parkCode, now.Year, now.Month, now.Day, flow);
                    break;

                case CodeType.Trade:
                    value = BasicCode.GetTradeCode(parkCode, now.Year, now.Month, now.Day, flow);
                    break;

                case CodeType.Voucher:
                    value = BasicCode.GetVoucherCode(parkCode, now.Year, now.Month, now.Day, flow);
                    break;

                case CodeType.ECard:
                    value = BasicCode.GetECardCode(parkCode, now.Year, now.Month, now.Day, flow);
                    break;
            }

            return value.ToString();
        }

        /// <summary>
        /// 根据唯一码类型生成流水号
        /// </summary>
        /// <param name="codeType">唯一码的类型</param>
        /// <param name="retry">重试</param>
        /// <param name="terminalCode"></param>
        /// <returns>流水号</returns>
        private async Task<int> GetFlow(CodeType codeType, int terminalCode = 0, int retry = 1)
        {
            var database = _abpRedisCacheDatabaseProvider.GetDatabase();

            //TODOCuizj: test HashIncrement method by Concurrent，
            //perhaps throw StackExchange.Redis.RedisConnectionException： no available connection

            int flow = 0;
            switch (codeType)
            {
                case CodeType.Barcode:
                    var key = UniqueCodeKey + CodeType.Barcode + terminalCode;
                    long value = 0;
                    if (database.KeyExists(key))
                    {
                        value = await database.HashIncrementAsync(key, true);
                    }
                    else
                    {
                        using (await BarLockObj.LockAsync())
                        {
                            if (database.KeyExists(key))
                            {
                                value = await database.HashIncrementAsync(key, true);
                            }
                            else
                            {
                                var preflow = await GetLastFlow(codeType);
                                value = await database.HashIncrementAsync(key, true, preflow + 1);
                            }
                        }
                    }

                    flow = (int)(value % BarMaximumFlow);
                    break;

                case CodeType.Order:
                case CodeType.Trade:
                case CodeType.Voucher:
                    var key2 = UniqueCodeKey + codeType;
                    long value2 = 0;
                    if (database.KeyExists(key2))
                    {
                        value2 = await database.HashIncrementAsync(key2, true);
                    }
                    else
                    {
                        using (await OtherLockObj.LockAsync())
                        {
                            if (database.KeyExists(key2))
                            {
                                value2 = await database.HashIncrementAsync(key2, true);
                            }
                            else
                            {
                                var preflow = await GetLastFlow(codeType);
                                value2 = await database.HashIncrementAsync(key2, true, preflow + 1);
                            }
                        }
                    }

                    flow = (int)(value2 % OtherMaximumFlow);
                    break;
                case CodeType.ECard:
                    var key3 = UniqueCodeKey + codeType;
                    long value3 = 0;
                    if (database.KeyExists(key3))
                    {
                        value3 = await database.HashIncrementAsync(key3, true);
                    }
                    else
                    {
                        using (await EcardLockObj.LockAsync())
                        {
                            if (database.KeyExists(key3))
                            {
                                value3 = await database.HashIncrementAsync(key3, true);
                            }
                            else
                            {
                                var preflow = await GetLastECardIDFlow(codeType);
                                value3 = await database.HashIncrementAsync(key3, true, preflow + 1);
                            }
                        }
                    }

                    flow = (int)(value3 % OtherMaximumFlow);
                    break;

                default:
                    break;
            }

            return flow;
        }

        /// <summary>
        /// 获最后一条记录的flow
        /// </summary>
        /// <param name="codeType">Type of the code.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        private async Task<long> GetLastFlow(CodeType codeType)
        {
            var preflow = 0;
            var types = _tablesNumberCreator.GetTableType(codeType);
            foreach (var type in types)
            {
                var pk = (await _tablesNumberCreator.GetTableLastPk(type)).ToString();
                preflow = Math.Max(preflow, DecodeFlow(codeType, pk));
            }

            return preflow;
        }

        /// <summary>
        /// 获最后一条记录的flow
        /// </summary>
        /// <param name="codeType">Type of the code.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        private async Task<long> GetLastECardIDFlow(CodeType codeType)
        {
            var preflow = 0;
            var types = _tablesNumberCreator.GetTableType(codeType);
            foreach (var type in types)
            {
                var pk = (await _tablesNumberCreator.GetLastECardID()).ToString();
                preflow = Math.Max(preflow, DecodeFlow(codeType, pk));
            }

            return preflow;
        }

        /// <summary>
        /// 解码并返回流水号
        /// </summary>
        /// <param name="codeType">Type of the code.</param>
        /// <param name="code">The code.</param>
        public int DecodeFlow(CodeType codeType, string code)
        {
            int park = 0;
            int year = 0;
            int month = 0;
            int day = 0;
            int term = 0;
            int flow = 0;
            switch (codeType)
            {
                case CodeType.Barcode:
                    if (BasicCode.GetBarcodeInfo(code, ref park, ref year, ref month, ref day, ref term, ref flow) != 0)
                    {
                        return flow;
                    }
                    break;
                case CodeType.Order:
                    if (BasicCode.GetOrderCodeInfo(code, ref park, ref year, ref month, ref day, ref flow) != 0)
                    {
                        return flow;
                    }
                    break;
                case CodeType.Trade:
                    if (BasicCode.GetTradeCodeInfo(code, ref park, ref year, ref month, ref day, ref flow) != 0)
                    {
                        return flow;
                    }
                    break;
                case CodeType.Voucher:
                    if (BasicCode.GetVoucherCodeInfo(code, ref park, ref year, ref month, ref day, ref flow) != 0)
                    {
                        return flow;
                    }
                    break;
                case CodeType.ECard:
                    if (BasicCode.GetECardCodeInfo(code, ref park, ref year, ref month, ref day, ref flow) != 0)
                    {
                        return flow;
                    }
                    break;
            }

            return 0;
        }

        #endregion Methods
    }
}