using Magicube.Media.Emojis;
using Magicube.Net;
using Magicube.Text.TrieTree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Magicube.Text.Unicode;
using System.IO;

namespace Magicube.Media.Test {
    public class EmojiServiceTest {
        private readonly IServiceProvider ServiceProvider;
        public EmojiServiceTest(){
            var services = new ServiceCollection()
                .AddHttpServices()
                .Configure<EmojiOptions>(x => {
                    x.CachePath = Path.GetTempPath();
                    x.EmojiHost = "file:///full-emoji-list.html";
                })
                .AddTransient(typeof(EmojiService));
            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Func_EmojiService_Test() {
            var text = "三小时飞机✈️➕两小时公交地铁🚇➕四小时大巴>🦹‍♂️<>🦹<";
            var service = ServiceProvider.GetRequiredService<EmojiService>();
            Assert.NotNull(service);

            var result = await service.GetEmojis(true);
            Assert.True(result.Count > 0);

            Trie trie = new Trie();
            foreach (var item in result) {
                trie.Add(item.Code);
            }

            var points = text.Codepoints();

            var checkStr = string.Join("", points.Select(x => x.ToString().ToLower().Substring(2)));

            var res = trie.ContainsWords(checkStr);
            Assert.True(res.Count == 6);

            foreach (var item in res) {
                Assert.NotNull(result.Find(x => x.Code == item.Word));
            }
        }
    }
}
