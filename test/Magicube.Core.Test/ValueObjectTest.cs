using Magicube.Core.Models;
using MapsterMapper;
using System;
using Xunit;

namespace Magicube.Core.Test {
    public class ValueObjectTest {
        const string Template = "yyyy/MM/dd'T'HH:mm:ss";

        [Fact]
        public void Func_ValueObject_Null_Test() {
            ValueObject valueObject = default;
            var result = (string)valueObject;
            Assert.Null(result);
        }

        [Fact]
        public void Func_ValueObject_Mapper_Test() {
            var foo = new FooSource {
                Id   = 1,
                Name = "wolfweb"
            };

            var mapper = new Mapper();
            var viewModel = mapper.Map<FooSourceViewModel>(foo);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void Func_ValueObject_Test() {
            var date = DateTimeOffset.UtcNow;
            ValueObject v = date.ToString("s");
            long a = v;  
            Assert.Equal(a, date.ToUnixTimeSeconds());
            ValueObject x = a;
            Assert.Equal(a, x.Value<DateTimeOffset>().ToUnixTimeSeconds());
            Assert.Equal(a, x.Value<DateTime>().ToUnixTimeSeconds());

            ValueObject y = date.ToUnixTimeSeconds();
            DateTime d = y;
            Assert.Equal(d.ToString(Template), date.ToString(Template));
            DateTimeOffset c = y;
            Assert.Equal(c.ToString(Template), date.ToString(Template));
        }
    }

    public class FooSource {
        public int    Id   { get; set; }
        public string Name { get; set; }
        public long CreateAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public class FooSourceViewModel {
        public ValueObject Id   { get; set; }
        public ValueObject Name { get; set; }
        public ValueObject CreateAt { get; set; }
    }
}
