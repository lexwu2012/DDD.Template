using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.SaleCard
{
    /// <summary>
    /// 年卡配置
    /// </summary>
    public class SaleCardSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(SaleCardSetting.FillCardPrice,null, L("年卡补卡价格配置"),null,null,SettingScopes.Tenant),
                new SettingDefinition(SaleCardSetting.FingerType,null, L("指纹机类型配置"),null,null,SettingScopes.Tenant),
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
