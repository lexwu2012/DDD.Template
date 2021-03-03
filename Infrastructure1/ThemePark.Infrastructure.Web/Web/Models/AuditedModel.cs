using System;
using System.ComponentModel;
using Abp.Domain.Entities.Auditing;
using Nito.AsyncEx;

namespace ThemePark.Infrastructure.Web.Models
{
    /// <summary>
    /// Class AuditedModel.
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    /// <seealso cref="CreationModel{TPrimaryKey}"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.ICreationAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasCreationTime"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IModificationAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasModificationTime"/>
    public class AuditedModel<TPrimaryKey> : CreationModel<TPrimaryKey>, IAudited
    {
        #region Properties

        private string _lastModifierUserName;

        /// <summary>
        /// The last modified time for this entity.
        /// </summary>
        [DisplayName("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user for this entity.
        /// </summary>
        [DisplayName("修改用户")]
        public long? LastModifierUserId { get; set; }

        /// <summary>
        /// Gets or sets the last name of the modifier user.
        /// </summary>
        /// <value>The last name of the modifier user.</value>
        [DisplayName("修改用户")]
        public string LastModifierUserName
        {
            get
            {
                if (LastModifierUserId.HasValue)
                {
                    if (string.IsNullOrEmpty(_lastModifierUserName))
                    {
                        _lastModifierUserName =
                            AsyncContext.Run(() => UserInfoService.GetUserNameByIdAsync(LastModifierUserId.Value));
                    }

                    return _lastModifierUserName;
                }

                return string.Empty;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// Class AuditedModel.
    /// </summary>
    /// <seealso cref="CreationModel{TPrimaryKey}"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.ICreationAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasCreationTime"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IModificationAudited"/>
    /// <seealso cref="Abp.Domain.Entities.Auditing.IHasModificationTime"/>
    public class AuditedModel : AuditedModel<int>
    {
    }
}