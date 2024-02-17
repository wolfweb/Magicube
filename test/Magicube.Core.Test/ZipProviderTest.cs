using Magicube.Core.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Magicube.Core.Test {
    public class ZipProviderTest {
        [Fact]
        public void Func_Zip_Test() {
            var zip = new FileAssistorProvider();
            using (var stream = zip.ZipCompress(new Dictionary<string, Stream> {
                {"str",new MemoryStream("hello".ToByte()) }
            })) {
                var zipItems = zip.ZipUnCompress(stream);
                var item = zipItems.FirstOrDefault();
                Assert.NotNull(item);
                Assert.True(item.Name == "str");
                Assert.True(item.Content is MemoryStream);
				Assert.True((item.Content as MemoryStream).ToArray().ToString("utf-8") == "hello");
            }
        }
    }
}
