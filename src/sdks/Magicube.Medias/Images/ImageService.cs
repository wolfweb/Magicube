using System;
using System.IO;
using Magicube.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Net.Http;

namespace Magicube.Media.Images {
    public class ImageService{
        public void AutoOrient(Stream source, Stream dest) {
            using (var image = Image.Load(source)) {
                image.Mutate(x => x.AutoOrient());
                image.Save(dest, image.Metadata.DecodedImageFormat);

                dest.Position = 0;
            }
        }

        public void CropScale(Stream source, Stream dest, int width, int height) {
            using (var image = Image.Load(source)) {
                var oldRatio = (float)image.Height / image.Width;
                var newRatio = (float)height / width;
                var cropWidth = image.Width;
                var cropHeight = image.Height;

                if (newRatio < oldRatio) {
                    // We making the image lower
                    cropHeight = (int)Math.Round(image.Width * newRatio);
                } else {
                    // We're making the image thinner
                    cropWidth = (int)Math.Round(image.Height / newRatio);
                }

                image.Mutate(x => x.Crop(new Rectangle {
                    Width = cropWidth,
                    Height = cropHeight,
                    X = cropWidth < image.Width ? (image.Width - cropWidth) / 2 : 0,
                    Y = cropHeight < image.Height ? (image.Height - cropHeight) / 2 : 0
                }));
                image.Mutate(x => x.Resize(new ResizeOptions {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                image.Save(dest, image.Metadata.DecodedImageFormat);
            }
        }

        public void Scale(Stream source, Stream dest, int width) {
            using (var image = Image.Load(source)) {
                int height = (int)Math.Round(width * ((float)image.Height / image.Width));

                image.Mutate(x => x.Resize(new ResizeOptions {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                image.Save(dest, image.Metadata.DecodedImageFormat);
            }
        }

        public void Crop(Stream source, Stream dest, int width, int height) {
            using (var image = Image.Load(source)) {
                image.Mutate(x => x.Crop(new Rectangle {
                    Width = width,
                    Height = height,
                    X = width < image.Width ? (image.Width - width) / 2 : 0,
                    Y = height < image.Height ? (image.Height - height) / 2 : 0
                }));

                image.Save(dest, image.Metadata.DecodedImageFormat);
            }
        }

        public static Image FromBase64(string base64Str) {
            var imgArr = base64Str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var imgBytes = Convert.FromBase64String(imgArr[1]);
            using (var mem = new MemoryStream(imgBytes)) {
                return Image.Load(mem);
            }
        }

        public static Image GetImageFromUrl(string url) {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                throw new MediaException($"image {url} get data incomplete!");
            }

            var mem = new MemoryStream();
            using var httpClient = new HttpClient();
            using var resp = httpClient.GetAsync(url).GetAwaiter().GetResult();
            resp.Content.ReadAsStream().CopyTo(mem);            
            mem.Seek(0L, SeekOrigin.Begin);

            Image image = Image.Load(mem);
            return image;
        }

        public static void RotateImage(Image img) {
            var orientation = GetExif(img);

            switch(orientation ) {
                case ExifOrientationMode.TopRight:
                    img.Mutate(x => x.Flip(FlipMode.Horizontal));
                    break;
                case ExifOrientationMode.BottomRight:
                    img.Mutate(x => x.Rotate(RotateMode.Rotate180));
                    break;
                case ExifOrientationMode.BottomLeft:
                    img.Mutate(x=>x.Flip(FlipMode.Vertical));
                    break;
                case ExifOrientationMode.LeftTop:
                    img.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));
                    break;
                case ExifOrientationMode.RightTop:
                    img.Mutate(x => x.Rotate(RotateMode.Rotate90));
                    break;
                case ExifOrientationMode.RightBottom:
                    img.Mutate(x => x.RotateFlip(RotateMode.Rotate270, FlipMode.Vertical));
                    break;
                case ExifOrientationMode.LeftBottom:
                    img.Mutate(x => x.Rotate(RotateMode.Rotate270));
                    break;
            }          
        }

        public static string ToBase64(Image img, IImageFormat format) {
            const string template = "data:{0};base64,";
            string dataPrefix = template.Format(format.DefaultMimeType);
            using (var ms = new MemoryStream()) {
                img.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                string base64String = "{0}{1}".Format(dataPrefix, Convert.ToBase64String(imageBytes));
                return base64String;
            }
        }

        public static Image Resize(Image image, Size size, bool preserveAspectRatio = true) {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio) {
                float gscale = size.Width / size.Height, imgScale = image.Width / (float)image.Height;
                if (gscale > imgScale) {
                    newWidth  = size.Width;
                    newHeight = (int)(size.Width / (float)image.Width * image.Height);
                } else {
                    newWidth  = (int)(size.Height / (float)image.Height * image.Width);
                    newHeight = size.Height;
                }
            } else {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            var newImage = image.Clone(x=>x.ApplyProcessor(new ResizeProcessor(new ResizeOptions { 
                Size = new Size(newWidth, newHeight),
                Mode = ResizeMode.Stretch
            }, x.GetCurrentSize())));

            return newImage;
        }

        public static Image ZoomCrop(Image image, Size size) {
            int width = size.Width, height = size.Height;
            var ctxRate = (float)width / height;
            
            var imgRate = (float)image.Width / image.Height;
            Rectangle rectangle;
            if (ctxRate > imgRate) {
                var rate    = (float)width / image.Width;
                var zoomH   = image.Height * rate;
                var actualH = (int)((zoomH - height) / rate);
                rectangle   = new Rectangle(0, actualH / 2, image.Width, image.Height - actualH);
            } else {
                var rate    = (float)height / image.Height;
                var zoomW   = image.Width * rate;
                var actualW = (int)((zoomW - width) / rate);
                rectangle   = new Rectangle(actualW / 2, 0, image.Width - actualW, image.Height);
            }

            return image.Clone(x => x.ApplyProcessor(new CropProcessor(rectangle, x.GetCurrentSize())));
        }

        private static ushort GetExif(Image img) {
            if (img.Metadata.ExifProfile is null) {
                return ExifOrientationMode.Unknown;
            }

            if (!img.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var value)) {
                return ExifOrientationMode.Unknown;
            }

            ushort orientation;
            if (value.DataType == ExifDataType.Short) {
                orientation = value.Value;
            } else {
                orientation = Convert.ToUInt16(value.Value);
                img.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);
            }

            img.Metadata.ExifProfile.SetValue(ExifTag.Orientation, ExifOrientationMode.TopLeft);

            return orientation;
        }
    }
}
