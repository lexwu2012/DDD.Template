using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Domain.Core.Model.Repositories.Dto
{
    [AutoMap(typeof(User))]
    public class AddUserInput
    {
        public string Name { get; set; }

        public Address Address { get; set; }
    }
}
