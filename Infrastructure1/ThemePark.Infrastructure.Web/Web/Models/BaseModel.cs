using Abp.Application.Services.Dto;

namespace ThemePark.Infrastructure.Web.Models
{
    public abstract class BaseModel<T> : IEntityDto<T>
    {
        #region Properties

        /// <summary>
        /// Id of the entity.
        /// </summary>
        /// <value>The identifier.</value>
        public T Id { get; set; }

        #endregion Properties
    }

    public abstract class BaseModel : BaseModel<int>
    {
    }
}