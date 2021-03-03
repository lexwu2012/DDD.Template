using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.InPark.Dto;
using ThemePark.Application.InPark.Interfaces;
using ThemePark.Common;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Core.DataSync;
using ThemePark.Core.InPark;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.Core;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.InPark
{
    /// <summary>
    /// 入园单
    /// </summary>
    public class EnterBillService : ThemeParkAppServiceBase, IEnterBillService
    {
        private readonly IRepository<InParkBill, string> _inPrakBillRepository;
        private readonly IUniqueCode _uniqueCode;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<User, long> _useRepository;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<VisitorInPark, long> _visitorInParkRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inPrakBillRepository"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="settingManager"></param>
        public EnterBillService(IRepository<InParkBill, string> inPrakBillRepository, IUniqueCode uniqueCode, ISettingManager settingManager,
            IRepository<Park> parkRepository, IRepository<User, long> useRepository, IDataSyncManager dataSyncManager, IRepository<VisitorInPark, long> visitorInParkRepository)
        {
            _inPrakBillRepository = inPrakBillRepository;
            _uniqueCode = uniqueCode;
            _settingManager = settingManager;
            _parkRepository = parkRepository;
            _useRepository = useRepository;
            _dataSyncManager = dataSyncManager;
            _visitorInParkRepository = visitorInParkRepository;
        }

        /// <summary>
        /// 入园凭证保存
        /// </summary>
        /// <param name="enterBillInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SaveEnterBill(EnterBillInput enterBillInput, int parkId, int terminalId)
        {
            var inPrakBill = await _inPrakBillRepository.FirstOrDefaultAsync(p => p.BillNo == enterBillInput.BillNo);
            if (inPrakBill != null)
            {
                return Result.FromError<string>("入园单编号重复，请刷新页面");
            }

            var entity = new InParkBill();
            entity.Id = await _uniqueCode.CreateAsync(CodeType.Barcode, parkId, terminalId);
            entity.ApplyBy = enterBillInput.ApplyBy;
            entity.ApplyDept = enterBillInput.ApplyDept;
            entity.ApprovedBy = enterBillInput.ApprovedBy;
            entity.Company = enterBillInput.Company;
            entity.CreationTime = System.DateTime.Now;
            entity.InParkChannel = enterBillInput.InParkChannel;
            entity.InparkNotice = enterBillInput.InparkNotice;
            entity.InParkTime = enterBillInput.InParkTime;
            entity.InParkType = enterBillInput.InParkType;
            entity.ParkId = parkId;
            entity.PersonNum = enterBillInput.PersonNum;
            entity.Reasons = enterBillInput.Reasons;
            entity.Remark = enterBillInput.Remark;
            entity.ValidDays = enterBillInput.ValidDays;
            entity.InParkBillState = InParkBillState.Valid;
            entity.InparkCounts = 0;
            entity.BillNo = enterBillInput.BillNo;

            entity.ValidStartDate = enterBillInput.ValidStartDate;
            //entity.WorkType = enterBillInput.WorkType==null? WorkType.Other : enterBillInput.WorkType;
            entity.WorkType = enterBillInput.WorkType;
            await _inPrakBillRepository.InsertAsync(entity);

            return new Result<string>(entity.Id);
        }


        /// <summary>
        /// 获取最大入园单编号
        /// </summary>
        /// <returns></returns>
        public async Task<Result<string>> GetMaxBillNo()
        {
            var maxBillNo = _inPrakBillRepository.GetAll().Max(t => t.BillNo);
            if (maxBillNo == null || maxBillNo == "")
            {
                maxBillNo = System.DateTime.Now.ToString("yyyyMMdd") + "0001";
            }
            else
            {
                if (maxBillNo.Length == 12 && maxBillNo.Substring(0, 8) == System.DateTime.Now.ToString("yyyyMMdd"))
                {
                    maxBillNo = Convert.ToString(Convert.ToInt64(maxBillNo) + 1);
                }
                else
                {
                    maxBillNo = System.DateTime.Now.ToString("yyyyMMdd") + "0001";
                }
            }
            return Result.FromData<string>(maxBillNo);
        }

        /// <summary>
        /// 手动入园
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> ManualInPark(string barcode)
        {
            //change state
            var ticket = await _inPrakBillRepository.GetAsync(barcode);

            string remark = "手动入园";
            //add inpark
            _visitorInParkRepository.Insert(new VisitorInPark
            {
                Barcode = barcode,
                ParkId = AbpSession.LocalParkId,
                TerminalId = AbpSession.TerminalId,
                Remark = remark,
                Persons = ticket.PersonNum

            });
            
            ticket.InParkBillState = InParkBillState.Entered;
            ticket.InparkCounts = ticket.PersonNum;

            //清掉验票的缓存
            var ticketCheckCacheDto = new TicketCheckCacheDto
            {
                Key = barcode
            };
            DataSyncInput dataSyncInput = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                SyncType = DataSyncType.TicketCheckCacheClear
            };
            _dataSyncManager.UploadDataToTargetPark(ticket.ParkId, dataSyncInput);

            return Result.Ok();
        }


        /// <summary>
        /// 根据条码获取入园单信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;</returns>
        public async Task<InParkBill> GetInParkBillByBarcodeAsync(string barcode)
        {
            if (!await _inPrakBillRepository.GetAll().AnyAsync(o => o.Id == barcode))
            {
                return null;
            }

            return await _inPrakBillRepository.AsNoTracking().FirstOrDefaultAsync(o => o.Id == barcode);
        }

        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CheckTicketUnusedAsync(string barcode)
        {
            var date = DateTime.Today.Date;

            return _inPrakBillRepository.GetAll().AnyAsync(o => o.Id == barcode && o.InParkBillState == InParkBillState.Valid
                && o.InparkCounts == 0 && DbFunctions.DiffDays(o.ValidStartDate, date) <= 0 && DbFunctions.AddDays(o.ValidStartDate, o.ValidDays) >= date);
        }

        /// <summary>
        /// 分页查询入园单信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageResult<InParkBillDto>> GetAllByPageAsync(SearchInParkBillModel query = null)
        {
            var allInParkBill = _inPrakBillRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(query.ApplyDept))
            {
                allInParkBill = allInParkBill.Where(p => p.ApplyDept.Contains(query.ApplyDept.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(query.ApplyBy))
            {
                allInParkBill = allInParkBill.Where(p => p.ApplyBy.Contains(query.ApplyBy.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(query.Company))
            {
                allInParkBill = allInParkBill.Where(p => p.Company.Contains(query.Company.Trim()));
            }


            if (query.EnterParkDate.HasValue)
            {
                var enterParkDateEnd = query.EnterParkDate.Value.AddDays(1);
                allInParkBill = allInParkBill.Where(p => p.ValidStartDate > query.EnterParkDate && p.ValidStartDate < enterParkDateEnd);
            }
            //入园单号或者条码
            if (!string.IsNullOrWhiteSpace(query.Id))
            {
                allInParkBill = allInParkBill.Where(p => p.Id == query.Id.Trim() || p.BillNo == query.Id.Trim());
            }
            if (query.InParkChannel >= 0)
            {
                allInParkBill = allInParkBill.Where(p => p.InParkChannel == query.InParkChannel);
            }
            if (query.InParkTime >= 0)
            {
                allInParkBill = allInParkBill.Where(p => p.InParkTime == query.InParkTime);
            }
            if (query.InParkType >= 0)
            {
                allInParkBill = allInParkBill.Where(p => p.InParkType == query.InParkType);
            }
            if (query.WorkType >= 0)
            {
                allInParkBill = allInParkBill.Where(p => p.WorkType == query.WorkType);
            }
            List<string> sorts = new List<string>(1);
            sorts.Add("CreationTime Desc");
            query.SortFields = sorts;

            var data = await allInParkBill.ProjectTo<InParkBillDto>().ToPageResultAsync(query);

            var parkIds = data.Data.SelectMany(o => o.BillVisitorInParks.Select(b => b.ParkId)).Distinct().ToList();
            var parks = await _parkRepository.GetAll()
                .Where(o => parkIds.Contains(o.Id))
                .Select(o => new { o.Id, o.ParkName }).ToListAsync();

            data.Data.SelectMany(o => o.BillVisitorInParks).ForEach(o => o.ParkName = parks.First(p => p.Id == o.ParkId).ParkName);

            return data;
        }

        /// <summary>
        /// 根据Id获取入园单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<InParkBillDto>> GetInParkBillInfoByIdAsync(string id)
        {
            var inParkBill = await _inPrakBillRepository.FirstOrDefaultAsync(p => p.Id == id);

            var data = inParkBill.MapTo<InParkBillDto>();

            if (inParkBill?.LastModifierUserId != null)
            {
                data.LastModifierUserName = await _useRepository.GetAll()
                        .Where(o => o.Id == inParkBill.LastModifierUserId.Value)
                        .Select(o => o.Name)
                        .SingleAsync();
            }

            return new Result<InParkBillDto>(data);
        }

        /// <summary>
        /// 入园单作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteInParkBillAsync(string id)
        {
            var inParkBill = await _inPrakBillRepository.FirstOrDefaultAsync(p => p.Id == id);

            inParkBill.InParkBillState = InParkBillState.Cancel;

            //清掉验票的缓存
            var ticketCheckCacheDto = new TicketCheckCacheDto
            {
                Key = id
            };
            DataSyncInput dataSyncInput = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(ticketCheckCacheDto),
                SyncType = DataSyncType.TicketCheckCacheClear
            };
            _dataSyncManager.UploadDataToTargetPark(inParkBill.ParkId, dataSyncInput);

            return Result.Ok();
        }

        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public string GetEnterBillSettingValue(string name, int parkId)
        {
            return _settingManager.GetSettingValueForTenant(name, parkId);
        }

    }
}
