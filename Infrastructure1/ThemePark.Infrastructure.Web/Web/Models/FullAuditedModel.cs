using System;
using System.ComponentModel;
using Abp.Domain.Entities.Auditing;
using Nito.AsyncEx;

namespace ThemePark.Infrastructure.Web.Models
{
    /// <summary>
    /// Class FullAuditedModel.
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    /// <seealso cref="IFullAudited"/>
    /// <seealso creaf="Abp.Domain.Entities.AuditingllAudited"/>
    public class FullAuditedModel<TPrimaryKey> : AuditedModel<TPrimaryKey>, IFullAudited
    {
        #region Properties

        private string _deleterUserName;

        /// <summary>
        /// Which user deleted this entity?
        /// </summary>
        [DisplayName("删除用户")]
        public long? DeleterUserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the deleter user.
        /// </summary>
        /// <value>The name of the deleter user.</value>
        [DisplayName("删除用户")]
        public string DeleterUserName
        {
            get
            {
                if (LastModifierUserId.HasValue)
                {
                    if (string.IsNullOrEmpty(_deleterUserName))
                    {
                        _deleterUserName =
                            AsyncContext.Run(() => UserInfoService.GetUserNameByIdAsync(LastModifierUserId.Value));
                    }

                    return _deleterUserName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Deletion time of this entity.
        /// </summary>
        [DisplayName("删除时间")]
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// Used to mark an Entity as 'Deleted'.
        /// </summary>
        [DisplayName("是否已被删除")]
        public bool IsDeleted { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class FullAuditedModel.
    /// </summary>
    /// <seealso cref="IFullAudited"/>
    /// <seealso creaf="Abp.Domain.Entities.AuditingllAudited"/>
    public class FullAuditedModel : FullAuditedModel<int>
    {
    }
}