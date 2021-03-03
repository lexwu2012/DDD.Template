using Abp.Application.Services.Dto;

namespace ThemePark.ApplicationDto
{
    public class DeleteInput<T> : IEntityDto<T>
    {
        public T Id { get; set; }
    }

    public class DeleteInput : DeleteInput<int>
    {

    }
}
