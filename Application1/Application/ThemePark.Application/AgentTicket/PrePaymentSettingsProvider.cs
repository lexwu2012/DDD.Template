using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 预付款相关配置
    /// </summary>
    public class PrePaymentSettingsProvider: SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(PrePaymentSetting.InnerMessageTemplate, null, L("方特人员预付款预警金额模板"),null,null, SettingScopes.Application),
                new SettingDefinition(PrePaymentSetting.OuterMessageTemplate, null, L("旅游网预付款预警金额模板"),null,null, SettingScopes.Application)
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
