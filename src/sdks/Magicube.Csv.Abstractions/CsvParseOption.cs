using CsvHelper.Configuration;
using System;
using System.Collections.Generic;

namespace Magicube.Csv.Abstractions {
    public class CsvParseOption {
        private readonly List<Type> _parseMaps;
        public CsvParseOption() {
            _parseMaps = new();
        }

        public IReadOnlyList<Type> CsvMaps => _parseMaps;

        public CsvParseOption RetisterMap<T>() where T : ClassMap {
            _parseMaps.Add(typeof(T));
            return this;
        }
    }
}