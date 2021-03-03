using System;
using System.ComponentModel;
using Abp.Dependency;
using Abp.Domain.Entities.Auditing;
using Nito.AsyncEx;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Infrastructure.Web.Models
{
    /// <summary>
    /// Class CreationModel.
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    /// <seealso cref="BaseModel{T}" />
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto{TPrimaryKey}" />
    /// <seealso cref="Abp.Domain.Entities.Auditing.ICreationAudited" />
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasCreationTime" />
    public class CreationModel<TPrimaryKey> : BaseModel<TPrimaryKey>, ICreationAudited, IHasCreationTime
    {
        #region Fields

        /// <summary>
        /// The _user application service
        /// </summary>
        private readonly IUserInfoProvider _userinfoProvider;

        private IUserInfoService _userInfoService;

        /// <summary>
        /// Gets the user information service.
        /// </summary>
        /// <value>The user information service.</value>
        protected IUserInfoService UserInfoService => _userInfoService ?? (_userInfoService = _userinfoProvider.GetUserInfoService());

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationModel{TPrimaryKey}"/> class.
        /// </summary>
        public CreationModel()
        {
            //TODOCuizj: achieve this function by AutoMapper?
            _userinfoProvider = IocManager.Instance.IocContainer.Resolve<IUserInfoProvider>();
        }

        #endregion Constructors

        #region Properties

        private string _creatorUserName;

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Id of the creator user of this entity.
        /// </summary>
        [DisplayName("创建用户")]
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the creator user.
        /// </summary>
        /// <value>The name of the creator user.</value>
        [DisplayName("创建用户")]
        public string CreatorUserName
        {
            get
            {
                if (CreatorUserId.HasValue)
                {
                    if (string.IsNullOrEmpty(_creatorUserName))
                    {
                        _creatorUserName = AsyncContext.Run(() => UserInfoService.GetUserNameByIdAsync(CreatorUserId.Value));
                    }

                    return _creatorUserName;
                }

                return string.Empty;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// Class CreationModel.
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto{TPrimaryKey}"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.ICreationAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasCreationTime"/>
    public class CreationModel : CreationModel<int>
    {
    }
}