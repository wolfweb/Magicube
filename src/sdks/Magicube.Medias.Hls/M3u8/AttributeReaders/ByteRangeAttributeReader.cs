namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-BYTERANGE", typeof(ByteRangeAttributeReader))]
    internal class ByteRangeAttributeReader : DiscontinuityAttributeReader {
    }
}