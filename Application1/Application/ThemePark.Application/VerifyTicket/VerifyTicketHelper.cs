using System;
using System.Collections.Generic;
using System.Linq;

namespace ThemePark.Application.VerifyTicket
{
    public class VerifyTicketHelper
    {
        /// <summary>
        /// 计算这张票当天已入园次数
        /// </summary>
        /// <param name="remark">2017-3-20 41 3,2017-3-25 41 2, 入园日期，公园Id，已入该公园次数</param>
        /// <returns></returns>
        public static int GetTodayInParkTimes(string remark)
        {
            if (string.IsNullOrWhiteSpace(remark))
                return 0;
            string[] infoArr = remark.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int todayInParkTimes = 0;

            foreach (var item in infoArr)
            {
                string[] info = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (info[0].Equals(DateTime.Now.Date.ToString("yyyy-MM-dd")))
                {
                    todayInParkTimes += int.Parse(info[2]);
                }
            }
            return todayInParkTimes;
        }

        /// <summary>
        /// 计算这张票进去这个公园的次数
        /// </summary>
        /// <param name="parkId"></param>
        /// <param name="remark">2017-3-20 41 3,2017-3-25 41 2, 入园日期，公园Id，已入该公园次数</param>
        /// <returns></returns>
        public static int GetParkInParkTimes(int parkId, string remark)
        {
            if (string.IsNullOrWhiteSpace(remark))
                return 0;
            string[] infoArr = remark.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int parkInParkTimes = 0;

            foreach (var item in infoArr)
            {
                string[] info = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var inParkId = info[1];
                if (inParkId.Equals(info[1]))
                {
                    parkInParkTimes += int.Parse(info[2]);
                }
            }
            return parkInParkTimes;
        }

        /// <summary>
        /// 获取第一次入园日期
        /// </summary>
        /// <param name="remark">2017-3-20 41 3,2017-3-25 41 2, 入园日期，公园Id，已入该公园次数</param>
        /// <returns></returns>
        public static DateTime GetFirstInParkDate(string remark)
        {
            if (string.IsNullOrWhiteSpace(remark))
                return DateTime.Now;
            string[] inParkInfoArr = remark.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> date = new List<string>();

            foreach (var inParkInfo in inParkInfoArr)
            {
                string[] info = inParkInfo.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //日期
                date.Add(info[0]);
            }
            return Convert.ToDateTime(date.Min()).Date;
        }

        /// <summary>
        /// 判断是否票务的节假日
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool IsHoliday(DateTime today)
        {
            // to do ...
            return false;
        }
    }
}
