using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步配置
    /// </summary>
    public class DataSyncSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(DataSyncSetting.DataSyncSecretKey, null, L("签名密钥")),
                new SettingDefinition(DataSyncSetting.FtpServerIp,null, L("FTP服务器IP")),

                new SettingDefinition(DataSyncSetting.FtpDownloadPath, null, L("FTP下载路径")),
                new SettingDefinition(DataSyncSetting.FtpUploadPath,null, L("FTP上传路径")),
                new SettingDefinition(DataSyncSetting.FtpUserId, null, L("FTP登录用户名")),
                new SettingDefinition(DataSyncSetting.FtpPassword,null, L("FTP登录密码")),

                new SettingDefinition(DataSyncSetting.DownloadPath, null, L("下载文件保存路径")),
                new SettingDefinition(DataSyncSetting.UploadFilePath,null, L("上传文件保存路径")),

                new SettingDefinition(DataSyncSetting.UploadSumInfoMinute,null, L("每隔多少分钟上传汇总数据")),
                new SettingDefinition(DataSyncSetting.UploadSumInfoStartHour, null, L("汇总数据上传开始时间(小时)")),
                new SettingDefinition(DataSyncSetting.UploadSumInfoEndHour,null, L("汇总数据上传结束时间(小时)")),

                new SettingDefinition(DataSyncSetting.LocalParkId, "1", L("本地公园编号")),

            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}


