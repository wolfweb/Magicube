using CsvHelper;
using CsvHelper.Configuration;
using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Magicube.Csv.Abstractions {
    public class CsvParse<T> : IEnumerable<T> where T : class {
        private readonly CsvReader _csvReader;
        private readonly Action<T> _callback;
        
        public CsvParse(string filePath, CsvParseOption csvParseOption = null, Action<T> callback = null) 
            : this(File.OpenRead(filePath), csvParseOption, callback) { }

        public CsvParse(Stream stream, CsvParseOption csvParseOption = null, Action<T> callback = null) {
            stream.Seek(0,SeekOrigin.Begin);

            _callback = callback;
            _csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

            if(csvParseOption != null && csvParseOption.CsvMaps.Any()) {
                foreach (var type in csvParseOption.CsvMaps) {
                    _csvReader.Context.RegisterClassMap(type);
                }
            }

            _csvReader.Read();
            _csvReader.ReadHeader();
        }

        public IEnumerator<T> GetEnumerator() {
            while (_csvReader.Read()) {
                var record = _csvReader.GetRecord<T>();

                _callback?.Invoke(record);

                yield return record;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}