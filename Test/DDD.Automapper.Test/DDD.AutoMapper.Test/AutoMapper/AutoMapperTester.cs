using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.AutoMapper.Attributes;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Common.Reflection;
using Shouldly;
using Xunit;

namespace DDD.AutoMapper.Test.AutoMapper
{
    public class AutoMapperTester : TestBaseWithLocalIocManager
    {

        [Fact]
        public void AutoMapperTest()
        {
            Action<IMapperConfigurationExpression> action = expression =>
            {
                expression.CreateMap<Model, DestModel>().ConvertUsing(o => new DestModel() { NameStr = o.Name });

                expression.CreateMissingTypeMaps = true;
            };

            Mapper.Initialize(action);

            var model = new Model() { Name = "aaa" };
            var dest = Mapper.Map<DestModel>(model);
            dest.NameStr.ShouldBe("aaa");

            dynamic model1 = new { Name = "www" };
            Model result = Mapper.Map<Model>(model1);
            result.Name.ShouldBe("www");
        }

        [Fact]
        public void ConfigAllMapAutoMapperTest()
        {
            var list = new List<Action<IMapperConfigurationExpression>>();
            list.Add(ConfigAllMap);

            Action<IMapperConfigurationExpression> action = mapperConfigurationExpression =>
            {
                FindAndAutoMapTypes(mapperConfigurationExpression);
                foreach (var configurator in list)
                {
                    configurator(mapperConfigurationExpression);
                }
            };

            Mapper.Initialize(action);

            var userInput = new AddUserInput
            {
                Name = "lex"
            };
            var user = userInput.MapTo<User>();

            user.Name.ShouldBe("lex");
        }

        private void action(IMapperConfigurationExpression expression)
        {
            FindAndAutoMapTypes(expression);
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var _typeFinder = LocalIocManager.IocContainer.Resolve<ITypeFinder>();
            //var types1 = _typeFinder.GetAllTypes();

            var types = _typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapToAttribute));
                }
            );

            //var type2 = new List<Type>();
            //type2.Add(typeof(Model));
            //type2.Add(typeof(DestModel));

            //foreach (var type in type2)
            //{
            //    configuration.CreateAutoAttributeMaps(type);
            //}

            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void ConfigAllMap(IMapperConfigurationExpression configuration)
        {
            configuration.ForAllMaps((map, c) =>
            {
                foreach (var property in map.DestinationType.GetProperties())
                {
                    var attr = property.GetAttribute<MapFromAttribute>();
                    if (attr?.PropertyPath?.Length > 0)
                    {
                        c.ForMember(property.Name, m =>
                        {
                            var path = string.Join(".", attr.PropertyPath);
                            var exp = System.Linq.Dynamic.DynamicExpression.ParseLambda(map.SourceType, null, path);

                            var m0 = m.GetType()
                                .GetProperty("PropertyMapActions", BindingFlags.Instance | BindingFlags.NonPublic)?
                                .GetValue(m) as List<Action<PropertyMap>>;
                            var m1 = typeof(PropertyMap).GetMethod(nameof(PropertyMap.MapFrom));
                            m0?.Add(t => m1.Invoke(t, new object[] { exp }));
                        });
                    }
                }
            });
        }
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
