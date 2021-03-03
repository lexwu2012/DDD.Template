using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using ThemePark.Application.VerifyTicket.Finger;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.VerifyTicketDto.Model;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Application.VerifyTicket.Strategy;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Core.MultiTicket;
using ThemePark.Core.ReEnter;
using ThemePark.Infrastructure.Application;
using System.Data.Entity;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.DataSync;
using ThemePark.Core.ReEnter.Repositories;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验票逻辑
    /// </summary>
    public class CheckTicketAppService : ThemeParkAppServiceBase, ICheckTicketAppService
    {
        /// <summary>
        /// 验票管理器
        /// </summary>
        private readonly ICheckTicketManager _checkTicketManager;

        private readonly ITOTicketDomainService _toTicketDomainService;

        private readonly IMultiTicketEnterEnrollRepository _multiTicketEnterEnrollRepository;

        private readonly IReEnterEnrollRepository _reEnterEnrollRepository;

        private readonly IRepository<ReEnterTicketRull> _reEnterTicketRullRepository;

        /// <summary>
        /// 验票服务构造函数
        /// </summary>
        public CheckTicketAppService(ICheckTicketManager checkTicketManager, ITOTicketDomainService toTicketDomainService, IMultiTicketEnterEnrollRepository multiTicketEnterEnrollRepository, IReEnterEnrollRepository reEnterEnrollRepository, IRepository<ReEnterTicketRull> reEnterTicketRullRepository)
        {
            _checkTicketManager = checkTicketManager;
            _toTicketDomainService = toTicketDomainService;
            _multiTicketEnterEnrollRepository = multiTicketEnterEnrollRepository;
            _reEnterEnrollRepository = reEnterEnrollRepository;
            _reEnterTicketRullRepository = reEnterTicketRullRepository;
        }

        /// <summary>
        /// 检查条码是否可二次入园
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public async Task<Result<VerifySecondTicketDto>> CheckBarcodeSecond(string barcode)
        {
            return await IocManager.Instance.Resolve<FingerService>().CheckBarcodeSecond(barcode);
        }

        /// <summary>
        /// 二次入园管理卡确认
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="confirmType"></param>
        /// <returns></returns>
        public async Task<Result> Confirm(string cardNo, int confirmType)
        {
            return await IocManager.Instance.Resolve<CheckICNoStrategy>().CheckBarcodeSecond(cardNo);
        }

        /// <summary>
        /// 取照片（base64字符串）
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<GetPhotoDto>> GetPhoto(string verifyCode, string id)
        {
            return await IocManager.Instance.Resolve<FingerService>().GetPhoto(verifyCode, id);
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
            return await IocManager.Instance.Resolve<FingerService>().RegisterFinger(verifyCode, id, fingerType, fingerData, terminal);
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
            return await IocManager.Instance.Resolve<FingerService>().RegisterPhoto(verifyCode, id, photoData, terminal);
        }

        /// <summary>
        /// 失败返回结果
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="verifyType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Result<VerifyDto> Failed(string verifyCode, VerifyType verifyType, string message)
        {
            var dto = new VerifyDto()
            {
                VerifyCode = verifyCode,
                VerifyType = verifyType,
                VerifyData = ""
            };
            var result = Result.FromData(dto);
            result.Message = message;
            result.Code = ResultCode.Fail;
            return result;
        }

        public async Task<Result<VerifyDto>> Verify(VerifyModel model)
        {
            if (_checkTicketManager.GetCheckingCheckTicketCache().GetOrDefault(model.VerifyCode) != null)
            {
                return Failed(model.VerifyCode, VerifyType.InvalidTicket, "请勿重刷");
            }

            _checkTicketManager.GetCheckingCheckTicketCache().Set(model.VerifyCode, "Checking");

            var result = await CheckingVerify(model);
            if (result != null)
            {
                _checkTicketManager.GetCheckingCheckTicketCache().Remove(model.VerifyCode);
            }

            return result;
        }

        /// <summary>
        /// 验票，返回验票结果（可入园人数、票类名称等）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<Result<VerifyDto>> CheckingVerify(VerifyModel model)
        {
            //先通过缓存检票
            Result<VerifyDto> result;
            using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckCacheStrategy>())
            {
                result = await strategy.Object.Verify(model.VerifyCode, model.Terminal);
            }
            if (result != null)
            {
                return result;
            }

            switch (model.VerifyType)
            {
                case VerifyType.Barcode:
                    if (model.VerifyCode.Length == 18 && model.VerifyCode.StartsWith("11"))
                    {
                        //取票码
                        //确保释放对象，防止内存泄漏
                        using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckQRCodeStrategy>())
                        {
                            return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                        }
                    }
                    else if (model.VerifyCode.Length == 18 && model.VerifyCode.StartsWith("68"))
                    {
                        //电子年卡
                        using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckECardStrategy>())
                        {
                            return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                        }
                    }
                    else if (model.VerifyCode.Length == 13)
                    {
                        return await CheckLeftTicketStrategy.Verify(model.VerifyCode, model.Terminal);
                    }
                    else if (model.VerifyCode.Length == 16)
                    {
                        //确保释放对象，防止内存泄漏
                        using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckBarcodeStrategy>())
                        {
                            return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                        }
                    }
                    break;

                case VerifyType.PID:
                    //确保释放对象，防止内存泄漏
                    using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckPIDStrategy>())
                    {
                        return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                    }
                case VerifyType.ICNo:
                    //确保释放对象，防止内存泄漏
                    using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckICNoStrategy>())
                    {
                        return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                    }
                case VerifyType.Face:
                    //确保释放对象，防止内存泄漏
                    using (var strategy = IocManager.Instance.ResolveAsDisposable<CheckBarcodeStrategy>())
                    {
                        return await strategy.Object.Verify(model.VerifyCode, model.Terminal);
                    }
            }
            return Result.FromError<VerifyDto>("无效条码");
        }

        /// <summary>
        /// 验指纹（中控）
        /// </summary>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal">闸机编号</param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> VerifyFinger(byte[] fingerData, int terminal, ZWJType fingerType)
        {
            return await IocManager.Instance.Resolve<CheckFingerStrategy>().Verify(fingerData, terminal, fingerType);
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
            return await IocManager.Instance.Resolve<FingerService>().CheckFinger(verifyCode.ToUpper(), id, fingerType, fingerData, terminal);
        }

        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="model">入园模型</param>
        /// <returns></returns>
        public async Task<Result> WriteInPark(InParkModel model)
        {
            return await _checkTicketManager.WriteInPark(model.VerifyCode, model.Id, model.NoPast, model.Remark, model.Terminal) ?
                Result.Ok() : Result.FromError("找不到验票记录");
        }

        /// <summary>
        /// 登记人脸
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result<long>> RegisterFace(RegisterFaceModel model)
        {
            long uid = 0;

            //从缓存查找验票记录
            var checkData = await _checkTicketManager.GetTicketCheckDataCache().GetOrDefaultAsync(model.VerifyCode);

            if (checkData == null)
            {
                return Result.FromCode<long>(ResultCode.NoRecord, "无此验票记录");
            }

            if (checkData.VerifyType == VerifyType.MultiTicket)
            {
                var multiTicketEnterEnroll = await _multiTicketEnterEnrollRepository
                    .FirstOrDefaultAsync(p => p.Barcode == model.VerifyCode) ?? new MultiTicketEnterEnroll();

                multiTicketEnterEnroll.Barcode = model.VerifyCode;
                multiTicketEnterEnroll.ParkId = AbpSession.LocalParkId;
                multiTicketEnterEnroll.TerminalId = model.Terminal;
                multiTicketEnterEnroll.State = MultiTicketEnterEnrollState.Registered;
                multiTicketEnterEnroll.Customer = new Customer()
                {
                    Photo = model.FaceData,
                    PhotoFeature = model.FaceFeat
                };

                var enterEnroll =
                    await _toTicketDomainService.BindFaceInfoAsync(model.VerifyCode, multiTicketEnterEnroll);

                uid = long.Parse(enterEnroll.Barcode);

                //同步套票数据
                var dto = new MultiTicketEnrollDto
                {
                    Barcode = enterEnroll.Barcode,
                    EnrollTime = DateTime.Now,
                    Finger = null,
                    FromParkid = checkData.ParkId,
                    Photo = enterEnroll.Customer.Photo,
                    TerminalId = model.Terminal
                };

                var dataSyncManager = IocManager.Instance.Resolve<IDataSyncManager>();
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
                        dataSyncManager.UploadDataToTargetPark(Convert.ToInt32(parkid), syncInput);
                    }
                }
            }
            else
            {
                //判断可二次入园次数是否超过限制
                var count = await _reEnterEnrollRepository.CountAsync(p => p.Barcode == checkData.VerifyCode && p.ReEnterEnrollState == ReEnterEnrollState.InParked);

                var classId = checkData.GetTicketClassCacheItem().TicketClassId;
                var reEnterTicketRull = await _reEnterTicketRullRepository.GetAll()
                    .Where(p => p.TicketClassId == classId).Select(o => new
                    {
                        o.ReEnterEnrollRule.ValidCount,
                        o.ReEnterEnrollRule.EnrollStartTime,
                        o.ReEnterEnrollRule.EnrollEndTime,
                        o.ReEnterEnrollRuleId,
                        o.ReEnterEnrollRule.LimitedTime,
                        o.ReEnterEnrollRule.NeedCheckFp,
                        o.ReEnterEnrollRule.NeedCheckPhoto
                    })
                    .FirstOrDefaultAsync();
                if (count >= reEnterTicketRull.ValidCount)
                {
                    return Result.FromCode<long>(ResultCode.Fail, "已超出二次入园次数");
                }

                //判断是否在可二次入园登记时间
                var nowTime = DateTime.Now.TimeOfDay;
                if (reEnterTicketRull.EnrollStartTime.TimeOfDay > nowTime || nowTime > reEnterTicketRull.EnrollEndTime.TimeOfDay)
                {
                    return Result.FromCode<long>(ResultCode.Fail, "当前时间不允许二次入园");
                }

                var reEnterEnroll = await _reEnterEnrollRepository.
                    FirstOrDefaultAsync(p => p.Barcode == checkData.VerifyCode && p.ReEnterEnrollState == ReEnterEnrollState.Registered)
                    ?? new ReEnterEnroll();

                reEnterEnroll.Barcode = model.VerifyCode;
                reEnterEnroll.TerminalId = model.Terminal;
                reEnterEnroll.ReEnterEnrollState = ReEnterEnrollState.Registered;
                reEnterEnroll.ReEnterEnrollRuleId = reEnterTicketRull.ReEnterEnrollRuleId;
                reEnterEnroll.Customer = new Customer()
                {
                    Photo = model.FaceData,
                    PhotoFeature = model.FaceFeat
                };

                await _toTicketDomainService.MakeReEnterAble(model.VerifyCode, reEnterEnroll);

                uid = long.Parse(reEnterEnroll.Barcode);

                var verifySecondTicketDto = new VerifySecondTicketDto
                {
                    NeedCheckFp = reEnterTicketRull.NeedCheckFp,
                    NeedCheckPhoto = reEnterTicketRull.NeedCheckPhoto,
                    Id = checkData.VerifyCode,
                    EnrollId = reEnterEnroll.Id.ToString(),
                    Persons = 1,
                    DisplayName = checkData.GetTicketClassName(),
                    InParkTimeEnd = System.DateTime.Now.AddMinutes(reEnterTicketRull.LimitedTime),
                    FaceId = uid
                };

                checkData.SecondTicketDto = verifySecondTicketDto;

                checkData.AllowPersons = verifySecondTicketDto.Persons;

                //更改票缓存为二次入园状态
                checkData.CheckState = CheckState.SecondInPark;

                checkData.VerifyType = VerifyType.SecondTicket;

                _checkTicketManager.GetTicketCheckDataCache().Set(checkData.VerifyCode, checkData);
            }

            return Result.FromData(uid);
        }

    }
}
