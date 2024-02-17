namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-DISCONTINUITY-SEQUENCE", typeof(DiscontinuitySequnceAttributeReader))]
    internal class DiscontinuitySequnceAttributeReader : DiscontinuityAttributeReader {
    }
}