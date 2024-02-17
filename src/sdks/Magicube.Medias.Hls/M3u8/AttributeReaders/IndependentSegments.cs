namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-INDEPENDENT-SEGMENTS", typeof(IndependentSegments))]
    internal class IndependentSegments : DiscontinuityAttributeReader {
    }
}