using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.Common.Reflection;
using Xunit;

namespace DDD.SqlServer.Test
{
    public class Tester: TestBaseWithLocalIocManager
    {

        private readonly ITypeFinder _typeFinder;
        public Tester()
        {
            _typeFinder = LocalIocManager.IocContainer.Resolve<ITypeFinder>();
        }

        [Fact]
        public void BookTest()
        {

        }

       

        [Fact]
        public void TypeFinder()
        {
            var types1 = _typeFinder.GetAllTypes();
        }

        #region Private Methods

       

        #endregion

    }

    
    class Model
    {
        public string Name { get; set; }
    }

    [AutoMap(typeof(Model))]
    class DestModel
    {
        [MapFrom(nameof(Model.Name))]
        public string NameStr { get; set; }
    }
}
