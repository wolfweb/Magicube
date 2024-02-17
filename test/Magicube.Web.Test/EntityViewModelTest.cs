using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.TestBase;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Magicube.Web.Test {
    public class EntityViewModelTest {
        [Fact]
        public void EntityViewModel_Entity_Test() {
            var dateTime = DateTimeOffset.UtcNow;
            dynamic viewModel = new FooViewModel();
            viewModel.Name     = "wolfweb";
            viewModel.Address  = "wolfweb's address";
            viewModel.CreateAt = dateTime;
            viewModel.Password = "123456";

            Assert.Throws<ValidationException>(() => viewModel.Build());
            viewModel.VerifyPwd = "1234567";
            Assert.Throws<ValidationException>(() => viewModel.Build());
            viewModel.VerifyPwd = "123456";

            var fooEntity = viewModel.Build();
            Assert.NotNull(fooEntity);
            Assert.True(fooEntity is FooEntity);
            Assert.Equal(fooEntity.Password, "123456");
            Assert.Equal(fooEntity.CreateAt, dateTime.ToUnixTimeSeconds());

            var entity = new FooEntity {
                Name     = Guid.NewGuid().ToString("N"),
                Address  = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
            };
            viewModel = new FooViewModel(entity);
            viewModel.VerifyPwd = entity.Password;

            Assert.NotEmpty(viewModel.Name);
            Assert.NotEmpty(viewModel.Address);
            Assert.NotEmpty(viewModel.Password);

            var expected = viewModel.Build();
            Assert.Equal(entity.Name, expected.Name);
            Assert.Equal(entity.Address, expected.Address);
            Assert.Equal(entity.Password, expected.Password);
        }

        [Fact]
        public void EntityViewModel_DynamicEntity_Test() {
            DynamicEntity entity = typeof(FooEntity);

            var validations = TypeAccessor.Get<RequiredAttribute>().ScanTypes<ValidationAttribute>();
            Assert.NotNull(validations);

            entity = new DynamicEntity(nameof(FooEntity));
            Assert.NotNull(entity);
            dynamic viewModel = new FooDynamicViewModel(entity);
            viewModel.Name = Guid.NewGuid().ToString("N");
            viewModel.Address = Guid.NewGuid().ToString("N");
            viewModel.Password = Guid.NewGuid().ToString("N");

            var expected = viewModel.Build();
            Assert.Equal(viewModel.Name, expected.Name);
            Assert.Equal(viewModel.Address, expected.Address);
            Assert.Equal(viewModel.Password, expected.Password);
        }
    }
}
