using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.Payment
{
    /// <summary>
    /// 收银台配置
    /// </summary>
    public class PaymentSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(PaymentSetting.ServerRoot, null, L("收银台服务器根目录")),
                new SettingDefinition(PaymentSetting.OuterRsaRublickKey,null, L("外部Rsa签名公钥")),

                new SettingDefinition(PaymentSetting.Ticket.PartnerCode, null, L("合作平台身份标识")),
                new SettingDefinition(PaymentSetting.Ticket.InnerRsaPrivateKey,null, L("内部Rsa签名私钥")),
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}