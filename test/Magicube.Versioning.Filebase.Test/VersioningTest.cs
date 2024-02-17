using Magicube.Versioning.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Magicube.Versioning.Filebase.Test {
    public class VersioningTest {
        private readonly IServiceProvider ServiceProvider;

        public VersioningTest() {
            ServiceProvider = new ServiceCollection()
                .AddVersioning(new Abstractions.VersioningOption {
                    Folder = @"d:\temp",
                    UserName = "user",
                    UserEmail = "user",
                })
                .BuildServiceProvider();
        }

        [Theory]
        [InlineData("69bf44bd48c4481795cce58698110ea3", "hello", "hello world")]
        public void Test1(string key, string content, string updateContent) {
            var provider = ServiceProvider.GetService<IVersioningProvider>();
            var foo = new FooVersioningContent {
                Key = key,
                Content = content,
                CreateAt = DateTime.Now
            };

            provider.AddOrUpdate(foo);

            foo.Content = updateContent;
            foo.UpdateAt = DateTime.Now;
            provider.AddOrUpdate(foo);

            var archives = provider.Query(foo);
            Assert.True(archives.Count() == 2);

            provider.Remove(foo);

            archives = provider.Query(foo);
            Assert.True(archives.Count() == 0);
        }
    }

    class FooVersioningContent : IVersioningContent {
        public string    Key      { get; set; }
        public string    Content  { get; set; }
        public DateTime  CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}