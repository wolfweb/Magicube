using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.IO;

namespace Magicube.Media.Images {
    public static class ImageExtensions {
        public static Image ConvertToCMYK(this Image img, IImageEncoder format) {
            using (var mem = new MemoryStream()) {
                img.Save(mem, format);
                mem.Position = 0;
                using (MagickImage magick = new MagickImage(mem)) {
                    magick.ColorSpace = ColorSpace.CMYK;
                    magick.Density = new Density(300);
                    var imgStream = new MemoryStream();
                    magick.Write(imgStream);
                    return Image.Load(imgStream);
                }
            }
        }        
    }
}
