using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using ThemePark.Core;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.Application.Authorization
{
    /// <summary>
    /// Class RoleChangedHandler.
    /// </summary>
    /// <seealso cref="Abp.Events.Bus.Handlers.IEventHandler{Abp.Events.Bus.Entities.EntityChangedEventData{ThemePark.Core.Authorization.Users.User}}" />
    /// <seealso cref="Abp.Dependency.ISingletonDependency" />
    public class UserChangedHandler : IEventHandler<EntityChangedEventData<User>>, ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public UserChangedHandler(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        public void HandleEvent(EntityChangedEventData<User> eventData)
        {
            _cacheManager.GetUserNameCache().Remove(eventData.Entity.Id.ToString());
            _cacheManager.GetUserPermissionCache().Remove(eventData.Entity.Id.ToString());
        }
    }
}
