using System;

namespace DDD.Infrastructure.Domain.BaseEntities
{
    public class EntityTypeInfo
    {
        /// <summary>
        /// Type of the entity.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// DbContext type that has DbSet property.
        /// </summary>
        public Type DeclaringType { get; private set; }

        public EntityTypeInfo(Type entityType, Type declaringType)
        {
            EntityType = entityType;
            DeclaringType = declaringType;
        }
    }
}
