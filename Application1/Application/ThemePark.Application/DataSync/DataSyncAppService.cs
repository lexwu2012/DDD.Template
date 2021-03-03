using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.Uow;
using EntityFramework.DynamicFilters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Common;
using ThemePark.Core;
using ThemePark.Core.Agencies;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Core.SumInfo;
using ThemePark.Core.SumInfo.Repositories;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步服务类
    /// </summary>
    public class DataSyncAppService : ThemeParkAppServiceBase, IDataSyncAppService
    {
        #region Fields
        // public ILogger Logger;
        private readonly ISyncTableAppService _syncTableAppService;
        private readonly ISyncLogAppService _syncLogAppService;
        private readonly JsonSerializerSettings _serializeSetting;
        private readonly ISumParkInfoRepository _sumParkInfoRepository;
        private readonly IRepository<SyncLog, long> _syncLogRepository;
        private readonly ISyncParkAppService _syncParkAppService;

        private const string TypeNameHeader = "typename=";
        #endregion Fields

        /// <summary>
        /// 数据同步服务类
        /// </summary>
        public DataSyncAppService(ISyncTableAppService syncTableAppService, ISumParkInfoRepository sumParkInfoRepository,
            ISyncLogAppService syncLogAppService, IRepository<SyncLog, long> syncLogRepository,
            ISyncParkAppService syncParkAppService)
        {
            _syncTableAppService = syncTableAppService;
            _sumParkInfoRepository = sumParkInfoRepository;
            _syncLogAppService = syncLogAppService;
            _syncLogRepository = syncLogRepository;
            this._syncParkAppService = syncParkAppService;
            _serializeSetting = new JsonSerializerSettings() { Formatting = Formatting.None };
        }

        /// <summary>
        /// 增加数据同步日志
        /// </summary>
        /// <param name="fromParkId">From park identifier.</param>
        /// <param name="toParkId">To park identifier.</param>
        /// <param name="syncType">Type of the synchronize.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="syncInfo">The synchronize information.</param>
        /// <param name="result">The result.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="remark">The remark.</param>
        /// <returns>System.Threading.Tasks.Task&lt;ThemePark.Infrastructure.Application.Result&gt;.</returns>
        [DisableAuditing]
        public async Task<Result> AddSyncLog(int fromParkId, int toParkId, DataSyncType syncType, string requestUrl, string syncInfo, int result, Guid taskId, string remark = "")
        {
            SyncLog input = new SyncLog
            {
                FromParkId = fromParkId,
                ToParkId = toParkId,
                CreationTime = DateTime.Now,
                RequestUrl = requestUrl,
                SyncInfo = syncInfo,
                Result = result,
                Remark = remark,
                SyncType = syncType,
                TaskId = taskId
            };

            return await _syncLogAppService.AddSyncLogAsync(input);

        }

        /// <summary>
        /// 生成数据更新文件
        /// </summary>
        /// <param name="parkId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Result> MakeTableUpdateFile(int parkId, DateTime startTime, DateTime endTime, string fileName)
        {
            try
            {
                IList<SyncTable> syncTables;   // 要同步的数据表

                if (parkId == 0) // 中心
                {
                    // 取所有的下传数据表
                    syncTables = await _syncTableAppService.GetSyncTableListAsync(new Query<SyncTable>(x => x.SyncType == SyncType.Download));
                }
                else
                {
                    // 取所有的上传数据表
                    syncTables = await _syncTableAppService.GetSyncTableListAsync(new Query<SyncTable>(x => x.SyncType == SyncType.Upload));
                }

                // 根据表名找实体类型
                var metadata = ((IObjectContextAdapter)CurrentUnitOfWork.GetDbContext<ThemeParkDbContext>()).ObjectContext.MetadataWorkspace;
                var sspace = metadata.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.SSpace);
                var cspace = metadata.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace);
                var sets = sspace.GetItems<EntityContainer>().Single().EntitySets;
                var entityTypes = cspace.GetItems<EntityType>();

                var tables = new Dictionary<string, EntityType>();
                foreach (var table in syncTables)
                {
                    var key = table.EntityName;
                    var name = sets.First(o => o.Table == key).Name;
                    var entity = entityTypes.First(o => o.Name == name);
                    tables.Add(key, entity);
                }

                var allSetsType = entityTypes.Select(GetClrType).ToList();

                List<string> jsons = new List<string>();

                using (CurrentUnitOfWork.DisableFilter(DataFilters.ParkPermission, DataFilters.AgencyPermission, AbpDataFilters.SoftDelete))
                {
                    foreach (var table in tables)
                    {
                        var entityType = table.Value;

                        var clrType = GetClrType(entityType);
                        var types = !clrType.IsAbstract ? new[] { clrType } :
                            allSetsType.Where(o => o.BaseType != null && o.BaseType.FullName == clrType.FullName).ToArray();

                        // 构造查询SQL
                        string filter = string.Empty;
                        if (entityType.Members.Contains(nameof(ICreationAudited.CreationTime)))
                        {
                            filter = "CreationTime >= @startTime and CreationTime <= @endTime";
                        }
                        if (entityType.Members.Contains(nameof(IModificationAudited.LastModificationTime)))
                        {
                            filter = filter + (string.IsNullOrEmpty(filter) ? string.Empty : " or ") +
                                "LastModificationTime >= @startTime and LastModificationTime <= @endTime";
                        }
                        if (entityType.Members.Contains(nameof(IDeletionAudited.DeletionTime)))
                        {
                            filter = filter + (string.IsNullOrEmpty(filter) ? string.Empty : " or ") +
                                "DeletionTime >= @startTime and DeletionTime <= @endTime";
                        }

                        string sql = $"select * from {table.Key} {(string.IsNullOrEmpty(filter) ? "" : " where (" + filter + ")")}";

                        foreach (var type in types)
                        {
                            string sql1 = sql;
                            if (types.Length > 1)
                            {
                                sql1 = sql + (string.IsNullOrEmpty(filter) ? " where " : " and ") + "Discriminator='" + type.Name + "'";
                            }

                            // 利用DbContext查询修改记录
                            var dbContext = CurrentUnitOfWork.GetDbContext<ThemeParkDbContext>();

                            object[] sqlParameters = {
                                new SqlParameter { ParameterName = "startTime", Value = startTime },
                                new SqlParameter { ParameterName = "endTime", Value = endTime }
                            };

                            DbRawSqlQuery query = dbContext.Database.SqlQuery(type, sql1, sqlParameters);
                            var entities = await query.ToListAsync();

                            // 将修改记录加入jsons列表
                            if (entities.Any())
                            {
                                jsons.Add(TypeNameHeader + type.AssemblyQualifiedName);
                                jsons.AddRange(entities.Select(o => JsonConvert.SerializeObject(o, _serializeSetting)));
                            }
                        }
                    }
                }

                //如果有需要同步的记录，则生成压缩文件
                if (!jsons.Any())
                    return Result.FromCode(ResultCode.NoRecord);
                else
                {
                    GZHelper.Compress(fileName, jsons);
                    return Result.Ok();
                }
            }
            catch (Exception ex)
            {
                return Result.FromError(ex.Message);
            }
        }

        private Type GetClrType(EntityType entityType)
        {
            string typeKey = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType";
            return (Type)entityType.MetadataProperties.GetValue(typeKey, true).Value;
        }

        /// <summary>
        /// 上传数据更新文件
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> UploadTableUpdateFile(DateTime startTime, DateTime endTime)
        {
            try
            {
                int parkId = SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId);
                string uploadPath = parkId == ThemeParkConsts.CenterParkId ?
                    SettingManager.GetSettingValueForApplication(DataSyncSetting.DownloadPath) :
                    SettingManager.GetSettingValueForApplication(DataSyncSetting.UploadFilePath);

                var fileName = this.GetFileNameNew(parkId, startTime, endTime);
                string sourceFile = Path.Combine(uploadPath, fileName);
                var result = await MakeTableUpdateFile(parkId, startTime, endTime, sourceFile);
                if (result.Code == ResultCode.NoRecord)//如果没有需要同步的记录，返回空文件名
                {
                    return Result.Ok("");
                }
                else if (!result.Success)
                {
                    return result;
                }
                else if (parkId == ThemeParkConsts.CenterParkId)//当前是中心则返回生成的文件名
                {
                    return Result.Ok(fileName);
                }
                else//当前是公园则向中心Ftp上传生成的数据更新文件，并重命名触发中心数据更新操作
                {
                    string ftpServerIp = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpServerIp);
                    string ftpRemotePath = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpUploadPath);
                    string ftpUserId = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpUserId);
                    string ftpPassword = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpPassword);

                    FtpWeb ftp = new FtpWeb(ftpServerIp, ftpRemotePath, ftpUserId, ftpPassword);
                    string destFileName1 = fileName + ".tmp";
                    if (ftp.Upload(sourceFile, destFileName1))
                    {
                        // 上传成功，重命名文件，触发重命名事件让中心保存更新
                        ftp.ReName(destFileName1, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.FromError(ex.Message);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 下载中心的数据更新文件
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [DisableAuditing]
        public Result DownloadTableUpdateFile(DateTime startTime, DateTime endTime)
        {
            try
            {
                // 下载更新文件
                string fileName = GetFileName(0, startTime, endTime);

                string ftpServerIp = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpServerIp);
                string ftpRemotePath = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpDownloadPath);
                string ftpUserId = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpUserId);
                string ftpPassword = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpPassword);

                string downloadPath = SettingManager.GetSettingValueForApplication(DataSyncSetting.DownloadPath);

                FtpWeb ftp = new FtpWeb(ftpServerIp, ftpRemotePath, ftpUserId, ftpPassword);

                // 判断文件是否存在，存在就不下载 ？ 
                if (ftp.GetFileSize(fileName) <= 0)
                {
                    return Result.Ok(ResultCode.NoRecord.DisplayName());
                }


                ftp.Download(downloadPath, fileName);

                // 根据下载文件更新数据表
                var result = UpdateTables(Path.Combine(downloadPath, fileName));
                return result;
            }
            catch (Exception ex)
            {
                return Result.FromError(ex.Message);
            }

        }

        /// <summary>
        /// 下载中心的数据更新文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        [DisableAuditing]
        public Result DownloadTableUpdateFile(string fileName)
        {
            try
            {
                var ftpServerIp = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpServerIp);
                var ftpRemotePath = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpDownloadPath);
                var ftpUserId = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpUserId);
                var ftpPassword = SettingManager.GetSettingValueForApplication(DataSyncSetting.FtpPassword);
                var downloadPath = SettingManager.GetSettingValueForApplication(DataSyncSetting.DownloadPath);

                var ftpWeb = new FtpWeb(ftpServerIp, ftpRemotePath, ftpUserId, ftpPassword);
                if (ftpWeb.GetFileSize(fileName) <= 0)// 判断文件是否存在，存在就不下载 ？ 
                {
                    return Result.Ok(ResultCode.NoRecord.DisplayName());
                }

                //下载数据更新文件
                ftpWeb.Download(downloadPath, fileName);
                // 根据下载文件更新数据表
                var result = UpdateTables(Path.Combine(downloadPath, fileName));
                return result;
            }
            catch (Exception ex)
            {
                return Result.FromError(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        private string GetFileName(int parkId, DateTime startTime, DateTime endTime)
        {
            return $"{parkId:D3}{startTime:yyMMdd}_{endTime:yyMMdd}.gz";
        }

        /// <summary>
        /// 获取新的上传文件命名
        /// </summary>
        private string GetFileNameNew(int parkId, DateTime startTime, DateTime endTime)
        {
            return $"{parkId:D5}{startTime:yyMMddHHmm}_{endTime:yyMMddHHmm}.gz";
        }

        /// <summary>
        /// 根据下载文件更新数据表
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [DisableAuditing]
        [UnitOfWork(IsDisabled = true)]
        public Result UpdateTables(string fileName)
        {
            //create .bak file if not exist
            string bakName = fileName + ".bak";
            if (!File.Exists(bakName))
            {
                File.Copy(fileName, bakName);
            }

            Type entityType = null;
            var list = new List<object>();
            List<string> jsons = new List<string>();
            var log = new List<string>();

            try
            {
                // 解压数据文件
                GZHelper.Decompress(fileName, ref jsons);
                var unoperated = new List<string>(jsons);
                jsons.Add(TypeNameHeader);

                foreach (var json in jsons)
                {
                    if (json.StartsWith(TypeNameHeader))
                    {
                        if (list.Any())
                        {
                            var property = entityType.GetProperty(nameof(IEntity.Id));
                            if (property == null)
                            {
                                throw new Exception($"{entityType.Name} is not implement interface IEntity");
                            }

                            //property.PropertyType
                            var idType = typeof(List<>).MakeGenericType(property.PropertyType);
                            var ids = Activator.CreateInstance(idType) as IList;
                            list.ForEach(o => ids.Add(property.GetValue(o)));
                            using (var context = IocManager.Instance.Resolve<ThemeParkDbContext>())
                            {
                                context.Database.CommandTimeout = 2 * 60;
                                context.Database.Log = log.Add;
                                context.DisableAllFilters();

                                //select Id in (...)
                                var set = context.Set(entityType);
                                var addList = new List<object>();
                                var updateList = new List<object>();

                                var updateIds = Nito.AsyncEx.AsyncContext.Run(
                                        () => set.Where("@0.Contains(outerIt.Id)", ids).Select("Id").ToListAsync());
                                list.Cast<dynamic>().ForEach(o =>
                                {
                                    if (updateIds.Contains(o.Id))
                                    {
                                        updateList.Add(o);
                                    }
                                    else
                                    {
                                        addList.Add(o);
                                    }
                                });

                                //add range
                                //set.AddRange(addList);
                                context.BulkInsert(addList, entityType);

                                if (entityType != typeof(Account))
                                {
                                    context.Configuration.AutoDetectChangesEnabled = false;
                                    //foreach update
                                    updateList.ForEach(o =>
                                    {
                                        set.Attach(o);
                                        context.Entry(o).State = EntityState.Modified;
                                    });
                                    context.Configuration.AutoDetectChangesEnabled = true;
                                }

                                //savechanges
                                context.SaveChanges();
                            }

                            //write log for test
                            WriteDbExcutionLog(log, entityType?.Name);
                            log.Clear();

                            //rebuild gzip file for update table wrong, 
                            //but now this executed by uow with transaction
                            if (unoperated.Any())
                            {
                                GZHelper.Compress(fileName, unoperated);
                            }

                            list = new List<object>();
                            entityType = null;
                        }

                        // 根据实体类型名反射出实体类型
                        string typeName = json.Substring(TypeNameHeader.Length).Trim();
                        if (string.IsNullOrEmpty(typeName))
                        {
                            continue;
                        }

                        entityType = Type.GetType(typeName);
                        if (entityType == null)
                        {
                            throw new NullReferenceException(
                                $"entity type is null, wrong type description: \"{typeName}\" in file: {fileName}.");
                        }
                    }
                    else
                    {
                        // 更新数据
                        var item = JsonConvert.DeserializeObject(json, entityType, _serializeSetting);

                        list.Add(item);
                        unoperated.Remove(json);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.GetAllExceptionMessage().JoinAsString(Environment.NewLine);
                log.Add(error);
                WriteDbExcutionLog(log, entityType?.Name + "-error");
                return Result.FromError($"{error}{Environment.NewLine}{entityType?.FullName}");
            }

            return Result.Ok(fileName);
        }

        private void WriteDbExcutionLog(IList<string> messages, string name)
        {
            var content = string.Join("\r\n", messages);
            base.Logger.Info($"{name}\r\n{content}");
        }

        /// <summary>
        /// 地方公园上传了当天的汇总信息，中心在这里保存
        /// </summary>
        /// <param name="sumInfoDto"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<Result> UploadSumInfo(SumParkInfoDto sumInfoDto)
        {
            var entity = await this._sumParkInfoRepository.GetSumParkInfoAsync(sumInfoDto.ParkId, sumInfoDto.SummarizeDate);
            if (entity == null)//如果没有就新增
            {
                entity = sumInfoDto.MapTo<SumParkInfo>();
                this._sumParkInfoRepository.Insert(entity);
            }
            else
            {
                entity.InParkCount = sumInfoDto.InParkCount;
                entity.SaleAmount = sumInfoDto.SaleAmount;
                this._sumParkInfoRepository.Update(entity);
            }

            return Result.Ok();
        }

        /// <summary>
        /// 获取任务完成状态
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> GetTaskState(Guid taskId)
        {
            var taskResult = await _syncLogRepository.GetAll().Where(o => o.TaskId == taskId)
                .Select(o => new { o.Id, o.Result }).FirstOrDefaultAsync();

            if (taskResult != null && taskResult.Result == (int)ResultCode.Ok)
            {
                return Result.Ok();
            }

            return Result.FromCode(ResultCode.Fail);
        }
    }
}


