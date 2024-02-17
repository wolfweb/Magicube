using Magicube.Media.Images;
using SixLabors.ImageSharp;
using System.IO;
using System.Linq;
using Xunit;

namespace Magicube.Media.Test {
    public class ImageServiceTest {
        private readonly string file = @".\000063a110f131dc925e6cbf596d2201.jpg";

        [Fact]
        public void Func_Image_Exif_Test() {
            var exifs = ImageExifService.Parse(file);
            Assert.True(exifs.Count() > 0);
        }

        [Fact]
        public void Func_Image_Type_Test() {
            using (var stream = File.Open(file, FileMode.Open)) {
                var type = ImageTypeFactory.GetFormatType(stream);
                Assert.True(type == ImageType.Jpeg);
            }
        }

        [Fact]
        public void Func_Image_Resize_Test() {
            using (var image = Image.Load(file)) {
                var result = ImageService.Resize(image, new Size(108,108));
                Assert.Equal(108, result.Width);
                Assert.Equal(108, result.Height);
            }
        }
    }
}
