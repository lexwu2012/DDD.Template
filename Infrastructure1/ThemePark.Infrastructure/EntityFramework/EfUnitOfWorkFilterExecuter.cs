using System;
using System.Data.Entity;
using Abp.Domain.Uow;
using Abp.EntityFramework.Uow;
using Abp.Extensions;
using EntityFramework.DynamicFilters;

namespace ThemePark.Infrastructure.EntityFramework
{
    public class EfUnitOfWorkFilterExecuter : IEfUnitOfWorkFilterExecuter
    {
        public void ApplyDisableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
            {
                activeDbContext.DisableFilter(filterName);
            }
        }

        public void ApplyEnableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            if (DynamicFilterConditions.Instance.NotExistsOrConditionIsEnable(filterName))
            {
                foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
                {
                    activeDbContext.EnableFilter(filterName);
                }
            }
        }

        public void ApplyFilterParameterValue(IUnitOfWork unitOfWork, string filterName, string parameterName, object value)
        {
            foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
            {
                if (IsFunc<object>(value))
                {
                    activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, (Func<object>)value);
                }
                else
                {
                    activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, value);
                }
            }
        }

        public void ApplyCurrentFilters(IUnitOfWork unitOfWork, DbContext dbContext)
        {
            foreach (var filter in unitOfWork.Filters)
            {
                if (filter.IsEnabled && DynamicFilterConditions.Instance.NotExistsOrConditionIsEnable(filter.FilterName))
                {
                    dbContext.EnableFilter(filter.FilterName);
                }
                else
                {
                    dbContext.DisableFilter(filter.FilterName);
                }

                foreach (var filterParameter in filter.FilterParameters)
                {
                    if (IsFunc<object>(filterParameter.Value))
                    {
                        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, (Func<object>)filterParameter.Value);
                    }
                    else
                    {
                        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, filterParameter.Value);
                    }
                }
            }
        }

        private static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }
    }
}
