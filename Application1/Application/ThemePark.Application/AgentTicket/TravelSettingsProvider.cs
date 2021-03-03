using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.AgentTicket
{  /// <summary>
   /// 数据同步配置
   /// </summary>
    public class TravelSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(TravelSetting.MessageTemplate, null, L("旅行社短信模板"),null,null,SettingScopes.Tenant | SettingScopes.Application)
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
