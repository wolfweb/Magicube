namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-FIRST-SEGMENT-TIMESTAMP", typeof(FirstSegmentTimestampAttributeReader))]
    internal class FirstSegmentTimestampAttributeReader : DiscontinuityAttributeReader {
    }
}