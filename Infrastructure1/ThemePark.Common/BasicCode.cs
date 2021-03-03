using System;
using System.Runtime.InteropServices;

namespace ThemePark.Common
{
    /// <summary>
    /// 条码、取票码、订单号、交易号生成
    /// </summary>
    public static class BasicCode
    {
        /// <summary>
        /// 条码
        /// </summary>
        [DllImport("TicketUtility.dll ")]
        public static extern Int64 GetBarcode(int park, int year, int month, int day, int term, int flow);

        /// <summary>
        /// 根据参数生成取票码，返回值转换为十进制整数就可以得到取票码字符串
        /// </summary>
        [DllImport("TicketUtility.dll ")]
        public static extern Int64 GetVoucherCode(int park, int year, int month, int day, int flow);

        /// <summary>
        /// 根据参数生成订单号，返回值转换为十进制整数就可以得到订单号字符串
        /// </summary>
        [DllImport("TicketUtility.dll ")]
        public static extern Int64 GetOrderCode(int park, int year, int month, int day, int flow);

        /// <summary>
        /// 根据参数生成交易号，返回值转换为十进制整数就可以得到交易号字符串
        /// </summary>
        [DllImport("TicketUtility.dll ")]
        public static extern Int64 GetTradeCode(int park, int year, int month, int day, int flow);

        /// <summary>
        /// 根据参数生成电子年卡，返回值转换为十进制整数就可以得到电子年卡字符串
        /// </summary>
        [DllImport("TicketUtility.dll ")]
        public static extern Int64 GetECardCode(int park, int year, int month, int day, int flow);

        /// <summary>
        /// 反编译条码，根据条码得到生成条码的信息
        /// park公园编号、year 年、month月、day日、flow流水号、term终端号
        /// 反编译正确返回TRUE，否则返回FALSE
        /// </summary>
        [DllImport("TicketUtility.dll")]
        public static extern int GetBarcodeInfo(string barcode, ref int park, ref int year, ref int month, ref int day, ref int term, ref int flow);

        /// <summary>
        /// 反编译取票码，根据取票码得到生成取票码的信息
        /// </summary>
        [DllImport("TicketUtility.dll")]
        public static extern int GetVoucherCodeInfo(string code, ref int park, ref int year, ref int month, ref int day, ref int flow);

        /// <summary>
        /// 反编译订单号，根据订单号得到生成订单号的信息
        /// </summary>
        [DllImport("TicketUtility.dll")]
        public static extern int GetOrderCodeInfo(string code, ref int park, ref int year, ref int month, ref int day, ref int flow);

        /// <summary>
        /// 反编译交易号，根据订单号得到生成交易号的信息
        /// </summary>
        [DllImport("TicketUtility.dll")]
        public static extern int GetTradeCodeInfo(string code, ref int park, ref int year, ref int month, ref int day, ref int flow);

        /// <summary>
        /// 反编译电子年卡号，根据电子年卡得到生成电子年卡的信息
        /// </summary>
        [DllImport("TicketUtility.dll")]
        public static extern int GetECardCodeInfo(string code, ref int park, ref int year, ref int month, ref int day, ref int flow);
    }
}
