using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;
using ThemePark.Core.Settings;

namespace ThemePark.Application.AliBusiness
{
    /// <summary>
    /// ali平台回话token
    /// </summary>
    public class AccessTokenSettingProvider : SettingProvider
    {
        /// <inheritdoc />
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            //if(context.Manager.GetSettingDefinition(AliAccessTokenSetting.AccessToken) != null)
            return new[]
            {
                new SettingDefinition(AliApplicationSetting.AccessToken, null, L("阿里平台会话凭证"),null,null,SettingScopes.Application)                
            };
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
