using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Magicube.Media.Images {
    public enum ImageType {
        Bmp,
        Jpeg,
        Gif,
        Tiff,
        Png,
        Unknown
    }

    public class ImageTypeFactory {
        private static List<IImageType> _imageTypes = new List<IImageType>();
        static ImageTypeFactory() {
            _imageTypes.Add(new ImageBmpType());
            _imageTypes.Add(new ImageGifType());
            _imageTypes.Add(new ImagePngType());
            _imageTypes.Add(new ImageTiffType());
            _imageTypes.Add(new ImageTiff2Type());
            _imageTypes.Add(new ImageJpegType());
            _imageTypes.Add(new ImageJpeg2Type());
        }

        public static ImageType GetFormatType(Stream stream) {
            foreach (var item in _imageTypes) {
                if (item.CheckImageType(stream)) return item.FormatType;
            }
            return ImageType.Unknown;
        }

        public interface IImageType {
            byte[] Identity { get; }
            ImageType FormatType { get; }
            bool CheckImageType(Stream stream);
        }
        public abstract class AbstractImageType : IImageType {
            private static byte[] bytes = new byte[10];
            public abstract byte[] Identity { get; }
            public abstract ImageType FormatType { get; }
            public bool CheckImageType(Stream stream) {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);
                return Identity.SequenceEqual(bytes.Take(Identity.Length));
            }
        }
        sealed class ImageBmpType : AbstractImageType {
            public override ImageType FormatType => ImageType.Bmp;
            public override byte[] Identity => Encoding.ASCII.GetBytes("BM");
        }
        sealed class ImageGifType : AbstractImageType {
            public override ImageType FormatType => ImageType.Gif;
            public override byte[] Identity => Encoding.ASCII.GetBytes("GIF");
        }
        sealed class ImagePngType : AbstractImageType {
            public override ImageType FormatType => ImageType.Png;
            public override byte[] Identity => new byte[] { 137, 80, 78, 71 };
        }
        sealed class ImageTiffType : AbstractImageType {
            public override ImageType FormatType => ImageType.Tiff;
            public override byte[] Identity => new byte[] { 73, 73, 42 };
        }
        sealed class ImageTiff2Type : AbstractImageType {
            public override ImageType FormatType => ImageType.Tiff;
            public override byte[] Identity => new byte[] { 77, 77, 42 };
        }
        sealed class ImageJpegType : AbstractImageType {
            public override ImageType FormatType => ImageType.Jpeg;
            public override byte[] Identity => new byte[] { 255, 216, 255, 224 };
        }
        sealed class ImageJpeg2Type : AbstractImageType {
            public override ImageType FormatType => ImageType.Jpeg;
            public override byte[] Identity => new byte[] { 255, 216, 255, 225 };
        }
    }
}
