namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-START", typeof(StartAttributeReader))]
    internal class StartAttributeReader : DiscontinuityAttributeReader {
    }
}