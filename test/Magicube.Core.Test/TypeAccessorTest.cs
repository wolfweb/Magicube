using Magicube.Core.Models;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.TestBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Magicube.Core.Test {
    public class TypeAccessorTest {
        [Fact]
        public void TypeAccessor_Copy_Test() {
            var foo = new Foo(Guid.NewGuid().ToString("n"), 1);
            var expected = TypeAccessor.Get(foo).Copy<Foo>();
            Assert.NotNull(expected);
            Assert.NotEqual(expected, foo);
            Assert.Equal(foo.Name, expected.Name);
        }

        [Fact]
        public void TypeAccess_ViewModel_Test() {
            var type = typeof(FooViewModel);
            var viewModel = type.GetBaseType(x => x.IsGenericType);
            var entity = viewModel.GetGenericArguments().First();
            var entityTypeAccessor    = TypeAccessor.Get(entity, null);
            var viewModelTypeAccessor = TypeAccessor.Get(type, null);           
        }

        [Fact]
        public void TypeAccessor_Test() {
            PageResult<FooViewModel, int> result = PageResult<FooViewModel, int>.Empty(-1);
            var typeAccessor = TypeAccessor.Get(result.GetType(),result).Context;
            Assert.Equal(result.GetValue("Items"), Array.Empty<FooViewModel>());
            Assert.Equal(typeof(FooViewModel), typeAccessor.Type.GetGenericArguments().First());

            IEnumerable<FooViewModel> result1 = new[] {
                new FooViewModel(new FooEntity())
            };
            typeAccessor = TypeAccessor.Get(result1.GetType(), result1).Context;
            Assert.Equal(typeof(FooViewModel), typeAccessor.Type.GetGenericEnumerableType().GenericTypeArguments.First());

            var entity = new FooEntity {
                Id       = 1,
                Age      = 20,
                Born     = DateTime.UtcNow,
                Name     = "wolfweb",
                Address  = "wolfweb's address",
                CreateAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Password = Guid.NewGuid().ToString("n")
            };
            var viewModel = new FooViewModel(entity);

            foreach(var property in viewModel.ExportProperties) {
                if (entity.GetType().GetProperty(property.Property.Name) == null) continue;
                var raw = property.Property.GetValue(entity);
                var value = property.Property.Value(viewModel);
                if (value!=null && value is ValueObject ov) {
                    Assert.Equal(raw, ov.ConvertTo(property.Property.PropertyType));
                } else
                    Assert.Equal(raw, value);
            }

            viewModel.CreateAt = DateTimeOffset.UtcNow.AddHours(1.5);
            viewModel.VerifyPwd = entity.Password;
            entity = viewModel.Build();
            Assert.Equal(entity.CreateAt, viewModel.CreateAt.ConvertTo(entity.CreateAt.GetType()));
        }
        
        [Fact]
        public void New_Instance_Test() {
            var foo = New<Foo>.Instance();
            Assert.NotNull(foo);

            foo = New<Foo>.Instance();
            Assert.NotNull(foo);

            foo = New<Foo, string>.Instance("ab");
            Assert.NotNull(foo);

            foo = New<Foo, string, int>.Instance("ab", 1);
            Assert.NotNull(foo);

            foo = New<Foo, string, int, DateTime>.Instance("ab", 1, DateTime.Now);
            Assert.NotNull(foo);

            foo = New<Foo, string, int, DateTime, TimeSpan>.Instance("ab", 1, DateTime.Now, TimeSpan.FromDays(1));
            Assert.NotNull(foo);

            foo = New<Foo, string, int, DateTime, TimeSpan, float>.Instance("ab", 1, DateTime.Now, TimeSpan.FromDays(1), 1f);
            Assert.NotNull(foo);
        }
    }
}
