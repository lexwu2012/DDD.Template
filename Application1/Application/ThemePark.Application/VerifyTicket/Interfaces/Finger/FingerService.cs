using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Core.CardManage;
using ThemePark.Core.MultiTicket;
using ThemePark.Core.ReEnter;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.VerifyTicketDto.Model;
using ThemePark.Application.DataSync.Dto;
using Abp.Dependency;
using ThemePark.Application.DataSync.Interfaces;
using Newtonsoft.Json;
using Abp.Configuration;
using Abp.Application.Services;
using ThemePark.Core.BasicData;
using ThemePark.Core.DataSync;
using ThemePark.Core.ReEnter.Repositories;
using ThemePark.Core.Settings;
using static ThemePark.Application.VerifyTicket.Finger.FingerCache;

namespace ThemePark.Application.VerifyTicket.Finger
{
    /// <summary>
    /// 指纹服务方法
    /// </summary>
    public class FingerService : IApplicationService
    {
        private readonly ICheckTicketManager _checkTicketManager;
        private readonly ISettingManager _settingManager;
        private readonly IMultiTicketEnterEnrollRepository _multiTicketEnterEnrollRepository;
        private readonly IReEnterEnrollRepository _reEnterEnrollRepository;
        private readonly IRepository<ReEnterTicketRull> _reEnterTicketRullRepository;
        private readonly IRepository<UserIC, long> _userICRepository;
        private readonly IRepository<VIPCard, long> _vipCardRepository;


        /// <summary>
        /// 
        /// </summary>
        public FingerService(ICheckTicketManager checkTicketManager
            , IMultiTicketEnterEnrollRepository multiTicketEnterEnrollRepository
            , IReEnterEnrollRepository reEnterEnrollRepository
            , IRepository<ReEnterTicketRull> reEnterTicketRullRepository
            , IRepository<UserIC, long> userICRepository
            , ISettingManager settingManager, IRepository<VIPCard, long> vipCardRepository)
        {
            _checkTicketManager = checkTicketManager;
            _multiTicketEnterEnrollRepository = multiTicketEnterEnrollRepository;
            _reEnterEnrollRepository = reEnterEnrollRepository;
            _reEnterTicketRullRepository = reEnterTicketRullRepository;
            _userICRepository = userICRepository;
            _settingManager = settingManager;
            _vipCardRepository = vipCardRepository;
        }

        FingerCache fingerCache = IocManager.Instance.Resolve<FingerCache>();

        /// <summary>
        /// 获取照片
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<GetPhotoDto>> GetPhoto(string verifyCode, string id)
        {
            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);

            if (checkData == null)
            {
                return Result.FromError<GetPhotoDto>("无此验票记录");
            }

            var photos = new GetPhotoDto();

            //套票取照片
            if (checkData.VerifyType == VerifyType.MultiTicket)
            {
                var multiTicketEnterEnroll = await _multiTicketEnterEnrollRepository.GetAll()
                    .Where(o => o.Barcode == checkData.VerifyCode)
                    .Select(o => o.Customer.Photo)
                    .FirstOrDefaultAsync();
                if (multiTicketEnterEnroll == null)
                {
                    return Result.FromError<GetPhotoDto>("未登记照片");
                }

                photos.Photo1 = Convert.ToBase64String(multiTicketEnterEnroll);
            }
            else if (checkData.VerifyType == VerifyType.VIPCard)//年卡取照片
            {
                var icNo = long.Parse(id).ToString("x15");
                if (icNo.StartsWith("0"))
                {
                    icNo = icNo.Substring(1);
                }

                var vipCardId = await _vipCardRepository.GetAll().Where(o => o.IcBasicInfo.IcNo == icNo).Select(o => o.Id).FirstAsync();

                var customerPhotos = await _userICRepository.AsNoTracking().Where(p => p.VIPCardId.HasValue && p.VIPCardId.Value == vipCardId)
                    .Select(o => o.Customer.Photo).ToListAsync();

                if (!customerPhotos.Any())
                {
                    return Result.FromError<GetPhotoDto>("未登记照片");
                }

                var list = new List<int>();

                var p1 = customerPhotos.ElementAtOrDefault(0);
                if (p1 != null)
                {
                    photos.Photo1 = Convert.ToBase64String(p1);
                }
                var p2 = customerPhotos.ElementAtOrDefault(1);
                if (p2 != null)
                {
                    photos.Photo2 = Convert.ToBase64String(p2);
                }
                var p3 = customerPhotos.ElementAtOrDefault(2);
                if (p3 != null)
                {
                    photos.Photo3 = Convert.ToBase64String(p3);
                }
            }
            else//二次入园取照片
            {
                var reEnterEnroll = await _reEnterEnrollRepository.GetAll()
                    .Where(o => o.Barcode == checkData.VerifyCode && o.ReEnterEnrollState == ReEnterEnrollState.Registered)
                    .Select(o => o.Customer.Photo)
                    .FirstOrDefaultAsync();
                if (reEnterEnroll == null)
                {
                    return Result.FromError<GetPhotoDto>("未登记照片");
                }

                photos.Photo1 = Convert.ToBase64String(reEnterEnroll);
            }

            return Result.FromData(photos);
        }



        /// <summary>
        /// 登记照片
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="photoData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result> RegisterPhoto(string verifyCode, string id, byte[] photoData, int terminal)
        {
            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);

            if (checkData == null)
            {
                return new Result(ResultCode.NoRecord, "无此验票记录");
            }

            //套票登记 
            if (checkData.VerifyType == VerifyType.MultiTicket)
            {
                var multiTicketEnterEnroll = await _multiTicketEnterEnrollRepository.FirstOrDefaultAsync(p => p.Barcode == checkData.VerifyCode);
                if (multiTicketEnterEnroll == null)
                {
                    multiTicketEnterEnroll = new MultiTicketEnterEnroll();
                }
                multiTicketEnterEnroll.Barcode = checkData.VerifyCode;
                multiTicketEnterEnroll.Customer.Photo = photoData;
                multiTicketEnterEnroll.TerminalId = terminal;
                multiTicketEnterEnroll.ParkId = _settingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId);

                await _multiTicketEnterEnrollRepository.InsertOrUpdateAsync(multiTicketEnterEnroll);

                //同步套票数据
                var dto = new MultiTicketEnrollDto
                {
                    Barcode = checkData.VerifyCode,
                    EnrollTime = System.DateTime.Now,
                    Finger = null,
                    FromParkid = checkData.ParkId,
                    Photo = photoData,
                    TerminalId = terminal
                };

                var _dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MultiTicketEnrollPhoto,
                    SyncData = JsonConvert.SerializeObject(dto)
                };
                var otherParkIds = checkData.GetCanInParkIds().Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != multiTicketEnterEnroll.ParkId.ToString())
                    {
                        _dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }
            else //二次入园登记
            {
                //判断可二次入园次数是否超过限制
                var count = await _reEnterEnrollRepository.CountAsync(p => p.Barcode == checkData.VerifyCode && p.ReEnterEnrollState == ReEnterEnrollState.InParked);
                var reEnterTicketRull = await _reEnterTicketRullRepository.FirstOrDefaultAsync(p => p.TicketClassId == checkData.GetTicketClassCacheItem().TicketClassId);
                if (count >= reEnterTicketRull.ReEnterEnrollRule.ValidCount)
                {
                    return new Result(ResultCode.Fail, "已超出二次入园次数");
                }

                //判断是否在可二次入园登记时间
                if (reEnterTicketRull.ReEnterEnrollRule.EnrollStartTime.TimeOfDay > System.DateTime.Now.TimeOfDay || System.DateTime.Now.TimeOfDay > reEnterTicketRull.ReEnterEnrollRule.EnrollEndTime.TimeOfDay)
                {
                    return new Result(ResultCode.Fail, "当前时间不允许二次入园");
                }

                var reEnterEnroll = await _reEnterEnrollRepository.FirstOrDefaultAsync(p => p.Barcode == checkData.VerifyCode && p.ReEnterEnrollState == ReEnterEnrollState.Registered);
                if (reEnterEnroll == null)
                {
                    reEnterEnroll = new ReEnterEnroll();
                }

                reEnterEnroll.Barcode = checkData.VerifyCode;

                reEnterEnroll.Customer = new Customer()
                {
                    Photo = photoData
                };
                reEnterEnroll.ReEnterEnrollRuleId = reEnterTicketRull.ReEnterEnrollRuleId;
                reEnterEnroll.TerminalId = terminal;
                reEnterEnroll.ReEnterEnrollState = ReEnterEnrollState.Registered;

                await _reEnterEnrollRepository.InsertOrUpdateAsync(reEnterEnroll);
            }

            return Result.Ok();

        }


        /// <summary>
        /// 登记指纹
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result> RegisterFinger(string verifyCode, string id, ZWJType fingerType, byte[] fingerData, int terminal)
        {
            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);

            if (checkData == null)
            {
                return new Result(ResultCode.NoRecord, "无此验票记录");
            }

            switch (fingerType)
            {
                case ZWJType.ZK:
                    return await ZkRegisterFinger(checkData, fingerData, terminal);
                case ZWJType.TXW:
                    return await TxwRegisterFinger(checkData, fingerData, terminal);
            }

            return new Result(ResultCode.Fail, "登记失败");

        }

        private async Task<Result> RigisterFingerDB(TicketCheckData checkData, byte[] fingerData, int terminal)
        {
            var barcode = checkData.VerifyCode;
            //套票登记 
            if (checkData.VerifyType == VerifyType.MultiTicket)
            {

                var multiTicketEnterEnroll =
                    await _multiTicketEnterEnrollRepository.FirstOrDefaultAsync(p => p.Barcode == barcode);

                if (multiTicketEnterEnroll != null)
                {
                    return new Result(ResultCode.Fail, "套票已登记指纹");
                }

                multiTicketEnterEnroll = new MultiTicketEnterEnroll
                {
                    Barcode = barcode,
                    Customer = new Customer() { Barcode = barcode, FpImage1 = fingerData },
                    TerminalId = terminal,
                    State = MultiTicketEnterEnrollState.Registered,
                    ParkId = _settingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId)
                };

                var multiTicketEnterEnrollId = await _multiTicketEnterEnrollRepository.InsertAndGetIdAsync(multiTicketEnterEnroll);

                var fingerDataModel = new FingerDataItem
                {
                    EnrollId = multiTicketEnterEnrollId,
                    FingerData = fingerData,
                };

                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                listFinger.Add(fingerDataModel);

                fingerCache.GetDicFingerCache().Set(barcode, listFinger, TimeSpan.FromDays(7));

                //List<FingerDataItem> listFinger = null;

                //var fingerlist = fingerCache.GetDicFingerCache().GetOrDefault(barcode);
                //if (fingerlist != null)
                //{
                //    listFinger = fingerlist;
                //    for (int j = 0; j < listFinger.Count; j++)
                //    {
                //        if (listFinger[j].EnrollId == fingerDataModel.EnrollId)
                //        {
                //            listFinger.RemoveAt(j);
                //            listFinger.Add(fingerDataModel);
                //        }
                //        else
                //        {
                //            listFinger.Add(fingerDataModel);
                //        }
                //    }
                //    fingerCache.GetDicFingerCache().Set(barcode, listFinger,TimeSpan.FromDays(7));
                //}
                //else
                //{
                //    listFinger = new List<FingerDataItem> { fingerDataModel };
                //    fingerCache.GetDicFingerCache().Set(barcode, listFinger, TimeSpan.FromDays(7));
                //}


                //同步套票数据

                var dto = new MultiTicketEnrollDto
                {
                    Barcode = barcode,
                    EnrollTime = System.DateTime.Now,
                    Finger = fingerData,
                    FromParkid = checkData.ParkId,
                    TerminalId = terminal
                };

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
                var syncInput = new DataSyncInput
                {
                    SyncType = DataSyncType.MultiTicketEnroll,
                    SyncData = JsonConvert.SerializeObject(dto)
                };
                var otherParkIds = checkData.GetTicketClassCacheItem().CanInParkIds.Trim().Split(',');
                foreach (var parkid in otherParkIds)
                {
                    if (parkid != multiTicketEnterEnroll.ParkId.ToString())
                    {
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }
            else //二次入园登记
            {
                var ticketClassId = checkData.GetTicketClassCacheItem().TicketClassId;

                var reEnterTicketRull =
                    await _reEnterTicketRullRepository.AsNoTrackingAndInclude(o => o.ReEnterEnrollRule).FirstOrDefaultAsync(
                        p => p.TicketClassId == ticketClassId);

                var reEnterEnroll = new ReEnterEnroll
                {
                    Barcode = barcode,
                    Customer = new Customer()
                    {
                        FpImage1 = fingerData
                    },
                    ReEnterEnrollRuleId = reEnterTicketRull.ReEnterEnrollRuleId,
                    TerminalId = terminal,
                    ReEnterEnrollState = ReEnterEnrollState.Registered
                };

                var reEnterEnrollId = await _reEnterEnrollRepository.InsertAndGetIdAsync(reEnterEnroll);


                var fingerDataModel = new FingerDataItem
                {
                    EnrollId = reEnterEnrollId,
                    FingerData = fingerData,
                };
                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                listFinger.Add(fingerDataModel);

                fingerCache.GetDicFingerCache().Set(barcode, listFinger, TimeSpan.FromDays(7));

                //var fingerlist = fingerCache.GetDicFingerCache().GetOrDefault(barcode);
                //if (fingerlist != null)
                //{
                //    listFinger = fingerlist;
                //    for (int j = 0; j < listFinger.Count; j++)
                //    {
                //        if (listFinger[j].EnrollId == fingerDataModel.EnrollId)
                //        {
                //            listFinger.RemoveAt(j);
                //            listFinger.Add(fingerDataModel);
                //        }
                //        else
                //        {
                //            listFinger.Add(fingerDataModel);
                //        }
                //    }
                //    fingerCache.GetDicFingerCache().Set(barcode, listFinger, TimeSpan.FromDays(7));
                //}
                //else
                //{
                //    listFinger = new List<FingerDataItem> { fingerDataModel };
                //    fingerCache.GetDicFingerCache().Set(barcode, listFinger, TimeSpan.FromDays(7));
                //}



                // 先找出验票缓存数据 更改缓存 二次入园
                var ticketCheckData =
                    await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(checkData.VerifyCode);

                if (ticketCheckData == null)
                {
                    return new Result(ResultCode.Fail, "未查找到该票入园缓存");
                }

                var verifySecondTicketDto = new VerifySecondTicketDto
                {
                    NeedCheckFp = reEnterTicketRull.ReEnterEnrollRule.NeedCheckFp,
                    NeedCheckPhoto = reEnterTicketRull.ReEnterEnrollRule.NeedCheckPhoto,
                    Id = checkData.VerifyCode,
                    EnrollId = reEnterEnrollId.ToString(),
                    Persons = 1,
                    DisplayName = checkData.GetTicketClassName(),
                    InParkTimeEnd = System.DateTime.Now.AddMinutes(reEnterTicketRull.ReEnterEnrollRule.LimitedTime),
                };

                ticketCheckData.SecondTicketDto = verifySecondTicketDto;

                ticketCheckData.AllowPersons = verifySecondTicketDto.Persons;

                //更改票缓存为二次入园状态
                ticketCheckData.CheckState = CheckState.SecondInPark;

                ticketCheckData.VerifyType = VerifyType.SecondTicket;

                _checkTicketManager.GetTicketCheckDataCache().Set(checkData.VerifyCode, ticketCheckData);
            }
            return Result.Ok();
        }

        /// <summary>
        /// TXW登记指纹
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result> TxwRegisterFinger(TicketCheckData checkData, byte[] fingerData, int terminal)
        {
            //判断指纹是否已存在
            var barcode = checkData.VerifyCode;

            //条码和卡号
            var fingerlist = fingerCache.GetDicFingerCache().GetOrDefault(barcode);
            if (fingerlist != null)
            {
                List<FingerCache.FingerDataItem> fts = fingerCache.GetDicFingerCache().GetOrDefault(barcode);

                for (int i = 0; i < fts.Count; i++)
                {
                    if (TxwFPClass.TwxCompareByte((byte[])fts[i].FingerData, fingerData))
                    {
                        return new Result(ResultCode.Fail, "指纹重复");
                    }
                }
            }
            return await RigisterFingerDB(checkData, fingerData, terminal);
        }

        /// <summary>
        /// ZK指纹机 指纹登记
        /// </summary>
        /// <param name="checkData"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result> ZkRegisterFinger(TicketCheckData checkData, byte[] fingerData, int terminal)
        {
            //char[] charImageData;
            //charImageData = new char[(fingerData.Length) / 2];
            //Buffer.BlockCopy(fingerData, 0, charImageData, 0, fingerData.Length);

            //string strImage = new string(charImageData);
            ////验证指纹
            //object obj = null;
            //int extractResult = ((ZKFPClass)FingerCache.ZkFPclass).zkfp.ExtractImageFromURU(strImage, fingerData.Length, true, ref obj);

            //if (extractResult > 3)
            //{
            //    return new Result(ResultCode.Fail, "指纹重复");
            //}

            //FingerCache.ZkFPclass.BeginEnroll();

            //extractResult = ((ZKFPClass)FingerCache.ZkFPclass).zkfp.ExtractImageFromURU(strImage, fingerData.Length, true, ref obj);

            ////表示指纹有效
            //if (extractResult == 0)
            //{
            //    FingerCache.ZkFPclass.CancelEnroll();
            //    FingerCache.ZkFPclass.BeginEnroll();
            //    object tp = FingerCache.ZkFPclass.GetTemplate();

            //    FingerCache.ZkFPclass.CancelEnroll();

            //    if (FingerCache.MDcFinger[terminal] != null)
            //    {
            //        bool b1 = false;
            //        object prefinger = FingerCache.MDcFinger[terminal];
            //        bool bResult = FingerCache.ZkFPclass.VerFinger(ref prefinger, tp, ref b1);
            //        if (bResult) //和上一次按的指纹一样
            //        {
            //            return new Result(ResultCode.Fail, "指纹重复");
            //        }
            //    }

            //    return await RigisterFingerDB(checkData, fingerData, terminal);

            //}

            //if (extractResult > 3)
            //{
            //    return new Result(ResultCode.Fail, "指纹重复");
            //}
            //else
            //{
            //    return new Result(ResultCode.Fail, "登记失败");
            //}

            return new Result(ResultCode.Fail, "登记失败");
        }

        /// <summary>
        /// 检查条码是否可以二次入园
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<Result<VerifySecondTicketDto>> CheckBarcodeSecond(string barcode)
        {
            int ticketClassId;

            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(barcode);

            if (checkData != null)
            {
                ticketClassId = checkData.GetTicketClassCacheItem().TicketClassId;
            }
            else
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "不符合二次入园条件");
            }

            if (checkData.CheckState != CheckState.Used)
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "已入园票才可以二次入园");
            }

            //是否登记
            var hasReEnterEnroll = await _reEnterEnrollRepository.GetAll().AnyAsync(p => p.Barcode == barcode && p.ReEnterEnrollState == ReEnterEnrollState.Registered);

            if (hasReEnterEnroll)
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "此条码已登记");
            }

            //取二次入园规则
            var reEnterTicketRull = await _reEnterTicketRullRepository.GetAll().Where(p => p.TicketClassId == ticketClassId)
                .Select(o => new
                {
                    o.ReEnterEnrollRule.ValidCount,
                    o.ReEnterEnrollRule.EnrollStartTime,
                    o.ReEnterEnrollRule.EnrollEndTime,
                    o.ReEnterEnrollRule.NeedCheckFp,
                    o.ReEnterEnrollRule.NeedCheckPhoto
                })
                .FirstOrDefaultAsync();

            if (reEnterTicketRull == null)
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "此票类未设置二次入园规则");
            }

            //判断可二次入园次数是否超过限制
            var count = await _reEnterEnrollRepository.CountAsync(p => p.Barcode == barcode);

            if (count >= reEnterTicketRull.ValidCount)
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "已超出二次入园次数");
            }

            //判断是否在可二次入园登记时间
            var nowTime = DateTime.Now.TimeOfDay;
            if (reEnterTicketRull.EnrollStartTime.TimeOfDay > nowTime || nowTime > reEnterTicketRull.EnrollEndTime.TimeOfDay)
            {
                return new Result<VerifySecondTicketDto>(ResultCode.Fail, "当前时间不允许二次入园");
            }

            return Result.FromData(new VerifySecondTicketDto()
            {
                NeedCheckPhoto = reEnterTicketRull.NeedCheckPhoto,
                NeedCheckFp = reEnterTicketRull.NeedCheckFp
            });
        }

        /// <summary>
        /// 获取指纹缓存
        /// </summary>
        /// <param name="icno"></param>
        /// <returns></returns>
        public async Task<List<FingerDataItem>> GetFingerData(string icno)
        {
            var vipCard = await _vipCardRepository.FirstOrDefaultAsync(p => p.IcBasicInfo.IcNo == icno.ToLower());
            if (vipCard != null)
            {
                List<FingerDataItem> listFinger = new List<FingerDataItem>();
                var customers = _userICRepository.GetAll().Where(p => p.VIPCardId == vipCard.Id).Select(o => new FingerDataItem
                {
                    EnrollId = o.Customer.Id,
                    FingerData = o.Customer.Fp1
                }).ToList();

                customers.ForEach(o => listFinger.Add(new FingerDataItem { EnrollId = o.EnrollId, FingerData = o.FingerData }));
                return listFinger;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 通过Id找到之前保存的指纹，与客户端的指纹数据比对，返回比对结果成功或失败
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public async Task<Result> CheckFinger(string verifyCode, string id, ZWJType fingerType, byte[] fingerData, int terminal)
        {
            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(verifyCode);

            if (checkData == null)
            {
                return new Result(ResultCode.NoRecord, "无此验票记录");
            }

            //验证TWX指纹 
            if (fingerType == ZWJType.TXW)
            {
                if (checkData.VerifyCodeType == VerifyType.ICNo || checkData.VerifyCodeType == VerifyType.Barcode)
                {

                    var fingerlist = fingerCache.GetDicFingerCache().GetOrDefault(verifyCode.ToUpper());
                    if (fingerlist == null)
                    {
                        //无缓存时添加缓存
                        fingerlist =await GetFingerData(verifyCode);
                        if (fingerlist != null)
                        {
                            IocManager.Instance.Resolve<FingerCache>().GetDicFingerCache().Set(verifyCode.ToUpper(), fingerlist, TimeSpan.MaxValue);
                        }
                    }

                    //条码和卡号
                    if (fingerlist != null)
                    {
                        List<FingerCache.FingerDataItem> fts = fingerlist;
                        for (int i = 0; i < fts.Count; i++)
                        {

                            if (TxwFPClass.TwxCompareByte(fts[i].FingerData, fingerData))
                            {
                                if (checkData.VerifyCodeType == VerifyType.Barcode && checkData.VerifyType == VerifyType.SecondTicket)
                                {
                                    fingerCache.GetDicFingerCache().Remove(verifyCode.ToUpper());
                                }
                                return Result.Ok();
                            }
                        }
                    }
                    else
                    {
                        return Result.FromError("指纹对应码不存在");
                    }
                }
            }

            return Result.FromError("指纹无效");
        }
    }
}
