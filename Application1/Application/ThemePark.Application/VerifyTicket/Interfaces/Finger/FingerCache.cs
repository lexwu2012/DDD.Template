using System;
using System.Collections.Generic;
using Abp.Dependency;
using ThemePark.Core.CardManage;
using System.Linq;
using ThemePark.EntityFramework;
using ThemePark.Core.MultiTicket;
using ThemePark.Core.ReEnter;
using System.Data.Entity;
using EntityFramework.DynamicFilters;
using ThemePark.Infrastructure.EntityFramework;
using Abp.Runtime.Caching;

namespace ThemePark.Application.VerifyTicket.Finger
{
    /// <inheritdoc />
    /// <summary>
    /// 指纹缓存类
    /// </summary>
    public class FingerCache : ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// 用作存放指纹缓存数据
        /// </summary>
        /// <returns></returns>
        public ITypedCache<string, List<FingerDataItem>> GetDicFingerCache()
        {
            return _cacheManager.GetCache<string, List<FingerDataItem>>("DicFingerCache");
        }

        /// <summary>
        /// 是否已生成数据
        /// </summary>
        private bool _hasCreate;

        ///// <summary>
        ///// 用作存放ZK FPEngXClass实列
        ///// </summary>
        //public static ZKFPClass ZkFPclass = new ZKFPClass();

        /// <summary>
        /// 用作存放指纹缓存数据
        /// </summary>
        //public static ConcurrentDictionary<string, List<FingerDataItem>> DicFinger = new ConcurrentDictionary<string, List<FingerDataItem>>();

        /// <summary>
        /// 闸口上一次按压的指纹
        /// </summary>
        //public static Dictionary<int, object> MDcFinger = new Dictionary<int, object>(64);

        /// <summary>
        ///服务器时间
        /// </summary>
        public DateTime Dbtime = System.DateTime.Now.AddYears(-100);

        public FingerCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 指纹数据
        /// </summary>
        public class FingerDataItem
        {
            /// <summary>
            /// 指纹编号
            /// </summary>
            public long EnrollId { get; set; }

            /// <summary>
            /// 指纹数据 TXW
            /// </summary>
            public byte[] FingerData { get; set; }
        }


        ///// <summary>
        ///// 指纹数据
        ///// </summary>
        //public class ZKFingerDataItem
        //{
        //    /// <summary>
        //    /// 指纹对应票类别
        //    /// </summary>
        //    public FingerType FingerType { get; set; }

        //    /// <summary>
        //    /// 年卡 套票 等唯一ID
        //    /// </summary>
        //    public string Id { get; set; }

        //    /// <summary>
        //    /// 指纹编号
        //    /// </summary>
        //    public int EnrollId { get; set; }

        //    /// <summary>
        //    /// 指纹数据 TXW
        //    /// </summary>
        //    public object FingerData { get; set; }

        //    /// <summary>
        //    /// 指纹数据 ZK
        //    /// </summary>
        //    public object ZkFingerData { get; set; }

        //    /// <summary>
        //    /// IC卡内码/条码
        //    /// </summary>
        //    public string IcBarcode { get; set; }

        //    /// <summary>
        //    /// 身份证
        //    /// </summary>
        //    public string Idnum { get; set; }

        //    /// <summary>
        //    /// 年卡/票类名称
        //    /// </summary>
        //    public string TicketClassName { get; set; }

        //}

        /// <summary>
        /// 指纹对应票类别
        /// </summary>
        public enum FingerType
        {
            /// <summary>
            /// 年卡
            /// </summary>
            VipCard = 1,

            /// <summary>
            /// 套票
            /// </summary>
            MultiTicket = 2,

            /// <summary>
            /// 二次入园票
            /// </summary>
            SecondTicket = 3
        }


        /// <summary>
        /// 生成指纹缓存
        /// </summary>
        public void CreateFingerCache()
        {
            ////每天3:30到4：00 清除缓存,重新全部获取
            //if (IsInTimeInterval(DateTime.Parse(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss")), DateTime.Parse("2010-01-02 03:30:00"), DateTime.Parse("2010-01-02 04:00:00")))
            //{
            //    GetDicFingerCache().ClearAsync();
            //    //DicFinger.Clear();
            //    Dbtime = System.DateTime.Now.AddYears(-100);
            //}

            //生成套票缓存
            GenerateMultiTicketFingerCache();

            //生成年卡缓存
            GenerateYearCardFingerCache();

            //生成二次入园缓存
            GenerateReEnterFingerCache();

            //_hasCreate = true;
            ////重新获取数据时间
            //Dbtime = System.DateTime.Now.AddMinutes(-5);
        }

        /// <summary>
        /// 生成二次入园缓存
        /// </summary>
        private void GenerateReEnterFingerCache()
        {
            var context = IocManager.Instance.Resolve<ThemeParkDbContext>();

            var data = context.ReEnterEnrolls.Where(p => p.ReEnterEnrollState == ReEnterEnrollState.Registered);

            var reEnterFinger = data.Include(p => p.Customer).ToList();

            //二次入园缓存
            foreach (var item in reEnterFinger)
            {
                var fingerData = new FingerDataItem
                {
                    EnrollId = item.Id,
                    FingerData = item.Customer?.FpImage1,
                };
                //：以开头区分年卡、套票、二次入园  1 年卡  2 二次入园  3 套票
                GetDicFingerCache().Set(item.Barcode, new List<FingerDataItem>() { fingerData }, TimeSpan.FromDays(1));
            }
        }

        /// <summary>
        /// 生成套票指纹缓存
        /// </summary>
        private void GenerateMultiTicketFingerCache()
        {
            var context = IocManager.Instance.Resolve<ThemeParkDbContext>();

            var data = context.MultiTicketEnterEnrolls.Where(p => p.State == MultiTicketEnterEnrollState.Registered);

            var mulTicketFinger = data.Include(o => o.Customer).ToList();

            //套票指纹
            //var mulTicketFinger = IocManager.Instance.Resolve<FingerService>().GetMulTicketFinger(Dbtime);

            foreach (var multiTicketEnterEnroll in mulTicketFinger)
            {
                var fingerData = GetFingerData(multiTicketEnterEnroll);

                GetDicFingerCache().Set(multiTicketEnterEnroll.Barcode, new List<FingerDataItem>() { fingerData }, TimeSpan.FromDays(7));
            }
        }

        /// <summary>
        /// 生成年卡指纹缓存
        /// </summary>
        private void GenerateYearCardFingerCache()
        {
            var context = IocManager.Instance.Resolve<ThemeParkDbContext>();
            context.DisableFilter(DataFilters.ParkPermission);
            context.DisableFilter(DataFilters.AgencyPermission);

            var data = context.UserICs.Include(o => o.Customer).Where(p => p.VIPCard.State == VipCardStateType.Actived);

            var userIcs = data.Select(o => new { o.VIPCardId, CustomId = o.CustomerId, o.Customer.Pid, o.Customer.Fp1, o.Customer.Fp2 })
                .ToList();
            var vipcard = userIcs.Select(o => o.VIPCardId).Distinct().ToList();
            var vipcards = context.VIPCards.Include(o => o.TicketClass).Include(o => o.IcBasicInfo)
                .Where(p => vipcard.Contains(p.Id))
                .Select(o => new { o.TicketClass.TicketClassName, o.Id, o.IcBasicInfo.IcNo })
                .ToList();


            var group = userIcs.GroupBy(o => o.VIPCardId);
            foreach (var item in group)
            {
                var icno = vipcards.First(p => p.Id == item.Key).IcNo.ToUpper();

                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                foreach (var userIc in item)
                {
                    listFinger.Add(new FingerDataItem()
                    {
                        EnrollId = userIc.CustomId,
                        FingerData = userIc.Fp1
                    });
                }

                GetDicFingerCache().Set(icno, listFinger, TimeSpan.MaxValue);
            }
        }

        /// <summary>
        /// 生成指纹数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static FingerDataItem GetFingerData(MultiTicketEnterEnroll item)
        {
            var data = new FingerDataItem
            {
                EnrollId = item.Id,
                FingerData = item.Customer?.FpImage1,

            };
            return data;
        }

        /// <summary>
        /// 生成中控缓存
        /// </summary>
        public static void CreateFingerZkCache()
        {
            ////中控缓存
            //ZkFPclass.FreeCache();//销毁缓存
            //ZkFPclass.CreateCache();//重新创建
            //foreach (List<FingerCache.FingerDataItem> fts in FingerCache.DicFinger.Values)
            //{
            //    for (int i = 0; i < fts.Count; i++)
            //    {

            //        int nID = fts[i].EnrollId;
            //        if (fts[i].FingerType == FingerType.VipCard)
            //        {
            //            if (fts[i].ZkFingerData != null)
            //            {
            //                ZkFPclass.AddTemplate(nID, fts[i].ZkFingerData);
            //            }
            //        }
            //        else
            //        {
            //            ZkFPclass.AddTemplate(nID, fts[i].FingerData);
            //        }
            //    }
            //}
        }


        /// <summary>
        /// 判断某个时间DateTime是否在一天中的某个时间段（区间）内
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static bool IsInTimeInterval(DateTime time, DateTime startTime, DateTime endTime)
        {
            try
            {
                //判断时间段开始时间是否小于时间段结束时间，如果不是就交换
                if (startTime > endTime)
                {
                    DateTime tempTime = startTime;
                    startTime = endTime;
                    endTime = tempTime;
                }

                //获取以公元元年元旦日时间为基础的新判断时间
                DateTime newTime = new DateTime();
                newTime = newTime.AddHours(time.Hour);
                newTime = newTime.AddMinutes(time.Minute);
                newTime = newTime.AddSeconds(time.Second);

                //获取以公元元年元旦日时间为基础的区间开始时间
                DateTime newStartTime = new DateTime();
                newStartTime = newStartTime.AddHours(startTime.Hour);
                newStartTime = newStartTime.AddMinutes(startTime.Minute);
                newStartTime = newStartTime.AddSeconds(startTime.Second);

                //获取以公元元年元旦日时间为基础的区间结束时间
                DateTime newEndTime = new DateTime();
                if (startTime.Hour > endTime.Hour)
                {
                    newEndTime = newEndTime.AddDays(1);
                }
                newEndTime = newEndTime.AddHours(endTime.Hour);
                newEndTime = newEndTime.AddMinutes(endTime.Minute);
                newEndTime = newEndTime.AddSeconds(endTime.Second);

                if (newTime > newStartTime && newTime < newEndTime)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
