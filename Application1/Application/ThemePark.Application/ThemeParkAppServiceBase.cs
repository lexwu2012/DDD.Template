using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using ThemePark.Core;
using ThemePark.Core.Authorization.Users;
using ThemePark.Infrastructure;

namespace ThemePark.Application
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ThemeParkAppServiceBase : ApplicationService
    {
        public UserManager UserManager { get; set; }

        /// <summary>
        /// Gets current session information.
        /// </summary>
        public new IThemeParkSession AbpSession
        {
            get
            {
                var session = base.AbpSession as IThemeParkSession;
                if (session != null)
                {
                    return session;
                }
                
                throw new Exception("not a correct session");
            }
            set
            {
                var session = value as IAbpSession;
                if (session == null)
                {
                    throw new Exception("not a correct session");
                }
                base.AbpSession = session;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ThemeParkAppServiceBase()
        {
            LocalizationSourceName = ThemeParkConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual User GetCurrentUser()
        {
            var user = UserManager.FindById(AbpSession.GetUserId());
            return user;
        }
    }
}