using Abp.Configuration;
using Abp.Localization;
using System.Collections.Generic;
using ThemePark.Core.Settings;

namespace ThemePark.Application.InPark
{
    /// <summary>
    /// 默认配置
    /// </summary>
    public class EnterBillSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(EnterBillSetting.Notice,null, L("入园须知默认配置（参观）"),null,null,SettingScopes.Tenant),
                new SettingDefinition(EnterBillSetting.NoticeForWork,null, L("入园须知默认配置（工作）"),null,null,SettingScopes.Tenant),
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
