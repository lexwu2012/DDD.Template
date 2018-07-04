﻿using DDD.Domain.BaseEntities;
using DDD.Domain.Entities;

namespace DDD.Domain.Common.Repositories
{
    public interface IRepositoryWithEntity<TEntity> : IRepositoryWithTEntityAndTPrimaryKey<TEntity,int> 
        where TEntity:class, IAggregateRoot<int>
    {
    }
}