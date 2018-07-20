using System;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Infrastructure.Domain.Auditing
{
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                //没继承 IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default(DateTime))
            {
                entityWithCreationTime.CreationTime = DateTime.Now;
            }

            if (!(entityAsObj is ICreationAudited))
            {
                //没继承 ICreationAudited
                return;
            }

            if (!userId.HasValue)
            {
                //Unknown user
                return;
            }

            var entity = entityAsObj as ICreationAudited;
            if (entity.CreatorUserId != null)
            {
                return;
            }

            entity.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            if (entityAsObj is IHasModificationTime)
            {
                entityAsObj.As<IHasModificationTime>().LastModificationTime = DateTime.Now;
            }

            if (!(entityAsObj is IModificationAudited))
            {
                //没继承 IModificationAudited
                return;
            }

            var entity = entityAsObj.As<IModificationAudited>();

            if (userId == null)
            {
                //Unknown user
                entity.LastModifierUserId = null;
                return;
            }

            entity.LastModifierUserId = userId;
        }

        public static void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                if (entity.DeletionTime == null)
                {
                    entity.DeletionTime = DateTime.Now;
                }
            }

            if (entityAsObj is IDeletionAudited)
            {
                var entity = entityAsObj.As<IDeletionAudited>();

                if (entity.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    entity.DeleterUserId = null;
                    return;
                }

                entity.DeleterUserId = userId;

            }
        }
    }
}
