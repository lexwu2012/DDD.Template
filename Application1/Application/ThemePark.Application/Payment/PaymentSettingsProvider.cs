using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.Payment
{
    /// <summary>
    /// ����̨����
    /// </summary>
    public class PaymentSettingsProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(PaymentSetting.ServerRoot, null, L("����̨��������Ŀ¼")),
                new SettingDefinition(PaymentSetting.OuterRsaRublickKey,null, L("�ⲿRsaǩ����Կ")),

                new SettingDefinition(PaymentSetting.Ticket.PartnerCode, null, L("����ƽ̨��ݱ�ʶ")),
                new SettingDefinition(PaymentSetting.Ticket.InnerRsaPrivateKey,null, L("�ڲ�Rsaǩ��˽Կ")),
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}