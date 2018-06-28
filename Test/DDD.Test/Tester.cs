using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Shouldly;
using Xunit;
using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Common.Reflection;

namespace DDD.Test
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
