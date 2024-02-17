using Microsoft.Extensions.Options;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Magicube.Media.Fonts {
    public class FontProvider {
        private static FontCollection Fonts { get; } = new();
        private readonly FontOption _fontOption;

        public FontProvider(IOptionsMonitor<FontOption> options) {
            _fontOption = options.CurrentValue;

            Initialize();
        }

        private void Initialize() {
            foreach(var file in _fontOption.FontFiles) {
                using (var stream = File.OpenRead(file))
                    Fonts.Add(stream);
            }
        }

        public FontFamily DefaultFont => Fonts.Families.First();
    }
    
    public class FontOption {
        public FontOption() {
            FontFiles = new List<string>();
        }

        public List<string> FontFiles { get; set; }
    }
}