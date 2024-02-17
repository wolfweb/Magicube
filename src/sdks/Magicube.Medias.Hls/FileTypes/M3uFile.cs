using FileTypeChecker.Abstracts;

namespace Magicube.Media.Hls.FileTypes {
    public class M3uFile : FileType, IFileType {
        public const string TypeName = "m3u file";
        public const string TypeExtension = "m3u8";
        private static readonly byte[] MagicBytes = {
            0x23,0x45,0x58,0x54,0x4D,0x33,0x55
        };

        public M3uFile() : base(TypeName, TypeExtension, MagicBytes) {
        }
    }
}
