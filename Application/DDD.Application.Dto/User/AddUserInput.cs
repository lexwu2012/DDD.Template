using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Application.Dto.User
{
    [AutoMapTo(typeof(Domain.Core.Model.User))]
    public class AddUserInput
    {
        public string Name { get; set; }
    }
}
