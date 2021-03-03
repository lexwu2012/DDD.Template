using Abp.Configuration;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using ThemePark.Core.Authorization.Configuration;

namespace ThemePark.Application.Authorization
{
    /// <summary>
    /// Class RoleChangedHandler.
    /// </summary>
    /// <seealso cref="Abp.Events.Bus.Handlers.IEventHandler{Abp.Events.Bus.Entities.EntityChangedEventData{ThemePark.Core.Authorization.Configuration.Setting}}" />
    /// <seealso cref="Abp.Dependency.ISingletonDependency" />
    public class SettingChangedHandler : IEventHandler<EntityChangedEventData<Setting>>, ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public SettingChangedHandler(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        public void HandleEvent(EntityChangedEventData<Setting> eventData)
        {
            _cacheManager.GetApplicationSettingsCache().Clear();
            _cacheManager.GetTenantSettingsCache().Clear();
            _cacheManager.GetUserSettingsCache().Clear();
        }
    }
}
