using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magicube.Medias.Hls {
    public class AttributeReaderRoot {
        private static readonly Lazy<AttributeReaderRoot> attributeReaderRootLazy = new(() => new AttributeReaderRoot());
        public static AttributeReaderRoot Instance => attributeReaderRootLazy.Value;

        private readonly IReadOnlyDictionary<string, Type> attributeReaders;
        public IDictionary<string, IAttributeReader> AttributeReaders {
            get => attributeReaders.ToDictionary(x => x.Key, x => (IAttributeReader)Activator.CreateInstance(x.Value)!);
        }

        public AttributeReaderRoot() {
            attributeReaders = InitAttributeReaders();
        }

        private static IReadOnlyDictionary<string, Type> InitAttributeReaders() {
            Assembly asm = typeof(M3U8ReaderAttribute).Assembly;
            return asm.GetTypes()
                .Where(t => t.IsDefined(typeof(M3U8ReaderAttribute), false))
                .Select(t => (M3U8ReaderAttribute)t.GetCustomAttribute(typeof(M3U8ReaderAttribute), false)!)
                .ToDictionary(x => x.Key!, x => x.Type!);
        }
    }
}