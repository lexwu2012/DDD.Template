using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步配置
    /// </summary>
    public class TicketSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(TicketSetting.GroupValid, null, L("团体票有效天数")),
                new SettingDefinition(TicketSetting.NonGroupValid,null, L("散客票有效天数")),
                new SettingDefinition(TicketSetting.GroupPriceSet,null, L("团体打印价格设置")),
                new SettingDefinition(TicketSetting.NonGroupPriceSet,null, L("散客打印价格设置")),
                new SettingDefinition(TicketSetting.BarcodeMd5,null, L("条码MD5秘钥")),
                new SettingDefinition(TicketSetting.ParkAreaServer,null, L("公园窗口发布地址"),null,null,SettingScopes.Tenant),
                new SettingDefinition(VendorSetting.ValidTicketTypeSetting,null, L("自助售票机有效票类")),
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}


