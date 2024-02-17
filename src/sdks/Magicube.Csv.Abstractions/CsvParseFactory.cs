using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Magicube.Csv.Abstractions {
    public class CsvParseFactory {
        private readonly CsvParseOption _csvParseOption;
        public CsvParseFactory(IOptions<CsvParseOption> options) {
            _csvParseOption = options.Value;
        }

        public CsvParse<T> GetParse<T>(string filePath, Action<T> callback = null) where T : class {
             return new CsvParse<T>(filePath, _csvParseOption, callback);
        }

        public CsvParse<T> GetParse<T>(Stream stream, Action<T> callback = null) where T : class {
            return new CsvParse<T>(stream, _csvParseOption, callback);
        }
    }
}