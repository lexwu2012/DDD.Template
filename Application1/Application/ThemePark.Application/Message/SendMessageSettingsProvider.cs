using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.Message
{
    /// <summary>
    /// 发送短信配置
    /// </summary>
    public class SendMessageSettingsProvider : SettingProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(SendMessageSetting.SDK_SN, null, L("发送短信账号")),
                new SettingDefinition(SendMessageSetting.SDK_PWD, null, L("发送短信密码")),
                new SettingDefinition(SendMessageSetting.MessageServiceUrl, null, L("短信服务器地址")),
                new SettingDefinition(SendMessageSetting.ApiTokenUrl, null, L("获取token地址")),
                new SettingDefinition(SendMessageSetting.DhstSendSmsUrl, null, L("发送短信地址")),
            };
        }


        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
