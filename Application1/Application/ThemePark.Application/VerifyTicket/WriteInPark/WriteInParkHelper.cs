using System.Collections.Generic;
using System.Configuration;
using ThemePark.Common;
using ThemePark.Core.Settings;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket.WriteInPark
{
    /// <summary>
    /// 
    /// </summary>
    public class WriteInParkBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static int LocalParkId;

        /// <summary>
        /// 取写入记录的参数
        /// </summary>
        /// <param name="ticketCheckData"></param>
        /// <param name="id"></param>
        /// <param name="noPast"></param>
        /// <param name="remark"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public virtual WriteInParkJobArgs GetJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            return null;
        }

        /// <summary>
        /// 写入园记录到数据库
        /// </summary>
        /// <param name="args"></param>
        public virtual void UpdateDB(WriteInParkJobArgs args)
        {

        }
    }

    /// <summary>
    /// 写入园记录帮助类
    /// </summary>
    public static class WriteInParkHelper
    {
        private static Dictionary<VerifyType, WriteInParkBase> _dicWriteInPark;

        public static void Init()
        {
            _dicWriteInPark = new Dictionary<VerifyType, WriteInParkBase>();
            _dicWriteInPark.Add(VerifyType.SecondTicket, new SecondTicketInPark());
            _dicWriteInPark.Add(VerifyType.VIPCard, new VIPCardInPark());
            _dicWriteInPark.Add(VerifyType.MultiYearCard, new MultiYearCardInPark());
            _dicWriteInPark.Add(VerifyType.ManageCard, new ManageCardInPark());
            _dicWriteInPark.Add(VerifyType.InparkBill, new InparkBillInPark());
            _dicWriteInPark.Add(VerifyType.CommonTicket, new CommonTicketInPark());
            _dicWriteInPark.Add(VerifyType.MultiTicket, new MultiTicketInPark());

            WriteInParkBase.LocalParkId = ConfigurationManager.AppSettings[AppConfigSetting.LocalParkId].As(0);
        }

        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="ticketCheckData"></param>
        /// <param name="id"></param>
        /// <param name="noPast"></param>
        /// <param name="remark"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public static WriteInParkJobArgs GetInParkJobArgs(TicketCheckData ticketCheckData, string id, int noPast, string remark, int terminal)
        {
            if (_dicWriteInPark.ContainsKey(ticketCheckData.VerifyType))
            {
                var inPark = _dicWriteInPark[ticketCheckData.VerifyType];
                return inPark.GetJobArgs(ticketCheckData, id, noPast, remark, terminal);
            }
            return null;
        }

        public static void UpdateDB(WriteInParkJobArgs args)
        {
            if (_dicWriteInPark.ContainsKey(args.VerifyType))
            {
                var inPark = _dicWriteInPark[args.VerifyType];
                inPark.UpdateDB(args);
            }
        }

    }
}
