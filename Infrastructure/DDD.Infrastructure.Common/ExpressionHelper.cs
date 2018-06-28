using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DDD.Infrastructure.Common
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// transform expression parameter type to <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static LambdaExpression CreatePropertySelectorLambda<T, TDelegate>(Expression<TDelegate> expression)
            where T : class, new()
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }

            if (expression.Parameters.Any() && expression.Parameters.Count > 1)
            {
                throw new ArgumentException(nameof(expression) + " must have one Parameter");
            }

            var paraType = expression.Parameters.First().Type;
            var entityType = typeof(T);

            if (paraType.IsAssignableFrom(entityType))
            {
                //todo: now only support one-level Property, eg. TEntity.Property
                var member = expression.Body as MemberExpression;
                var property = member?.Member as PropertyInfo;
                if (property != null)
                {
                    if (entityType.GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public) == null)
                    {
                        throw new ArgumentException(entityType.FullName + " no have property: " + property.Name);
                    }

                    return CreateLambda<T>(property);
                }
            }

            return null;
        }

        /// <summary>
        /// transform expression parameter type to <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TTargetType">parameter type</typeparam>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static LambdaExpression CreatePropertySelectorLambda<T, TTargetType, TResult>(
            Expression<Func<TTargetType, TResult>> expression) where T : class, new()
        {
            return CreatePropertySelectorLambda<T, Func<TTargetType, TResult>>(expression);
        }

        #region Private Methods

        private static LambdaExpression CreateLambda<T>(PropertyInfo propertyInfo)
        {
            return CreateLambda(typeof(T), propertyInfo);
        }

        private static LambdaExpression CreateLambda(Type paraType, PropertyInfo propertyInfo)
        {
            ParameterExpression para = Expression.Parameter(paraType, "o");
            MemberExpression body = Expression.Property(para, propertyInfo);
            LambdaExpression result = Expression.Lambda(body, para);

            return result;
        }
        #endregion
    }
}
