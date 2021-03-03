using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.OTA.V1
{
    /// <summary>
    /// OTA接口地址配置
    /// </summary>
    public class V1OTASettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(V1OTASetting.ServerRoot, null, L("旧版OTA接口地址")),
                new SettingDefinition(V1OTASetting.MessageTemplate, null, L("OTA接口短信模板")),
            };
        }


        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
