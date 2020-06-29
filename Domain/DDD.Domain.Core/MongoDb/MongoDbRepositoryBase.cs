using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Web.Query;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Core.MongoDb
{
    /// <summary>
    /// 关联MongoDb的仓储基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public abstract class MongoDbRepositoryBase<TEntity, TPrimaryKey> : RepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IAggregateRoot<TPrimaryKey>
    {

        //public IMongoCollection<TEntity> Collection { get; private set; }

        private readonly IMongoDatabaseProvider _databaseProvider;

        public virtual IMongoCollection<TEntity> Collection
        {
            get
            {
                return _databaseProvider.Database.GetCollection<TEntity>(typeof(TEntity).Name);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="databaseProvider"></param>
        public MongoDbRepositoryBase(IMongoDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }


        #region 新增
        public override TEntity Insert(TEntity entity)
        {
            Collection.InsertOne(entity);
            return entity;
        }

        public void InsertMany(IEnumerable<TEntity> docs, InsertManyOptions options = null)
        {
            Collection.InsertMany(docs, options);
        }
        #endregion

        public override IQueryable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }

        public IFindFluent<TEntity, TDto> FirstOrDefault<TDto>(Expression<Func<TEntity, bool>> filter)
        {
            var projection = GetTDtoReturnProperties<TEntity, TDto>();
            return Collection.Find(filter).Project(projection);
        }

        public void Update(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> updateFields, UpdateOptions options = null)
        {
            Collection.UpdateOne(filter, updateFields, options);
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            Collection.DeleteOne(filter, options);
        }

        /// <summary>
        /// 获取指定的返回列
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        private ProjectionDefinition<TEntity, TDto> GetTDtoReturnProperties<TEntity, TDto>()
            where TEntity : class
        {
            var returnType = typeof(TDto);

            var fieldList = new List<ProjectionDefinition<TEntity>>();
            foreach (var property in returnType.GetProperties())
            {
                fieldList.Add(Builders<TEntity>.Projection.Include(property.Name));
            }

            return Builders<TEntity>.Projection.Combine(fieldList);
        }
    }
}
