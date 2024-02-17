namespace Magicube.Data.Abstractions {
    public class DatabaseProvider {
        public string Name                    { get; set; }
        public string Value                   { get; set; }
        public bool   IsDefault               { get; set; }
        public bool   RequireConnection       { get; set; }
        public string ExampleConnectionString { get; set; }
    }
}
