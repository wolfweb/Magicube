using ImageMagick;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Magicube.Media.Images {
    public class ExifItem {
        public string       Title { get; set; }
        public object       Value { get; set; }
        public ExifTag      Tag   { get; set; }
        public ExifDataType Type  { get; set; }
    }
    
    public class ImageExifService {
        public static IEnumerable<ExifItem> Parse(string path) {
            return Parse(File.OpenRead(path));
        }

        public static IEnumerable<ExifItem> Parse(Stream stream) {
            using (var image = new MagickImage(stream)) {
                var profile = image.GetExifProfile();
                if(profile == null) return Enumerable.Empty<ExifItem>();
                return profile.Values.Select(x => new ExifItem {
                    Title  = x.Tag.ToString(),
                    Value  = x.GetValue(),
                    Tag    = x.Tag,
                    Type   = x.DataType
                }) ;
            }            
        }
    }
}
