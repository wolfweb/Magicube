using Magicube.Core.Reflection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Magicube.Core.Test {
    public class TypeFactoryTest {
        [Theory]
        [InlineData("DynamicEntity1")]
        public void Func_TypeFactory_Property_Test(string name) {
            var typeBuilder = TypeFactory.Default.NewType(name);
            typeBuilder.NewProperty("Name", typeof(string)).SetCustomAttribute<RequiredAttribute>();
            typeBuilder.NewProperty("Address", typeof(string));
            var type = typeBuilder.CreateType();

            Assert.True(type.IsClass);
            Assert.Equal(type.Name, name);
            var properties = type.GetTypeInfo().DeclaredProperties;
            Assert.True(properties.Count() == 2);

            var model = New<object>.Creator(type);
            Assert.NotNull(model);
            model.SetValue("Name", "wolfweb");
            model.SetValue("Address", "wolfweb's host");
            var typeAccessor = TypeAccessor.Get(model.GetType(), model).Context;
            Assert.Equal(typeAccessor.Type.Name, name);
            Assert.True(typeAccessor.Properties.Count() == 2);
            Assert.Equal("wolfweb", model.GetValue("Name"));
        }

        [Theory]
        [InlineData("DynamicEntity2")]
        public void Func_TypeFactory_Method_Test(string name) {
            var typeBuilder = TypeFactory.Default.NewType(name);
            typeBuilder.NewProperty("Name", typeof(string));
            typeBuilder.NewProperty("Address", typeof(string));

            typeBuilder
                .NewMethod("Test")
                .Body()
                .LdStr("Hello World")
                .Call(typeof(Trace).GetMethod("WriteLine", new[] { typeof(string)}))
                .Ret();

            var myType = typeBuilder.CreateType();
            var objInstance = Activator.CreateInstance(myType);
            objInstance.GetType().GetMethod("Test").Invoke(objInstance, null);
        }

        [Theory]
        [InlineData("DynamicEntity3")]
        public void Func_TypeFactory_Method1_Test(string name) {
            var typeFactory = TypeFactory.Default.NewType(name);
            var type = typeFactory.NewMethod("Method1", builder =>
            {
                builder.Body(body =>
                {
                    body
                      .LdStr("Hello World")
                      .Call(typeof(Trace).GetMethod("WriteLine", new[] { typeof(string) }))
                      .Ret();
                });
            }).CreateType();

            var instance = Activator.CreateInstance(type);
            instance.GetType().GetMethod("Method1").Invoke(instance, null);
        }

        [Theory]
        [InlineData("DynamicEntity4")]
        public void Func_TypeFactory_Field_Test(string name) {
            var typeFactory = TypeFactory.Default.NewType(name);
            typeFactory.NewField<int>("Age").Public();
            var type = typeFactory.CreateType();

            Assert.True(type.IsClass);
            Assert.Equal(type.Name, name);
            var properties = type.GetTypeInfo().DeclaredFields;
            Assert.True(properties.Count() == 1);

            var model = New<object>.Creator(type);
            Assert.NotNull(model);
            model.SetValue("Age", 10);
            var typeAccessor = TypeAccessor.Get(model.GetType(), model).Context;
            Assert.Equal(typeAccessor.Type.Name, name);
            Assert.True(typeAccessor.Fields.Count() == 1);
            Assert.Equal(10, model.GetValue("Age"));
        }
    }
}
