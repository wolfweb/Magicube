namespace Magicube.Resource {
    public class ResourceSettings {
        public ResourceDebugMode ResourceDebugMode      { get; set; }
        public bool              UseCdn                 { get; set; }
        public string            CdnBaseUrl             { get; set; }
        public bool              AppendVersion          { get; set; }
    }

    public enum ResourceDebugMode {
        FromConfiguration,
        Enabled,
        Disabled
    }
}
