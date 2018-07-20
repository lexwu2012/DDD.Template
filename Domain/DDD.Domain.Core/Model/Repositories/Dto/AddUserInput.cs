using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Domain.Core.Model.Repositories.Dto
{
    [AutoMapTo(typeof(Domain.Core.Model.User))]
    public class AddUserInput
    {
        public string Name { get; set; }

        public Address Address { get; set; }
    }
}
