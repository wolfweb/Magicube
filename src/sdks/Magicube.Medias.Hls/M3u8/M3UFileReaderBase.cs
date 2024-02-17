using Magicube.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Magicube.Medias.Hls {
    public abstract class M3UFileReaderBase {
        private int currentIndex;
        protected string CurrentTitle => $"{++currentIndex}.tmp";
        protected Uri RequestUri = default!;

        public M3UFileReaderBase() {
        }

        public M3UFileReaderBase WithUri(Uri uri) {
            RequestUri = uri;
            return this;
        }

        protected M3UKeyInfo GetM3UKeyInfo(string method, string uri, string key, string iv) {
            if (uri == null && key == null && iv == null)
                return null;

            M3UKeyInfo m3UKeyInfo = new() {
                Method = method!,
                Uri = uri is not null ? RequestUri.Join(uri!) : default!,
                BKey = key is not null ? Encoding.UTF8.GetBytes(key!) : default!,
                IV = iv?.ToHex()!
            };
            return m3UKeyInfo;
        }


        protected M3UMediaInfo GetM3UMediaInfo(string uri, string? title) {
            M3UMediaInfo m3UMediaInfo = new() {
                Uri = RequestUri.Join(uri),
                Title = string.IsNullOrWhiteSpace(title) ? CurrentTitle : title
            };
            return m3UMediaInfo;
        }

        public abstract M3UFileInfo Read(Stream stream);
    }

    public class M3UFileReaderWithStream : M3UFileReaderBase {
        private readonly IDictionary<string, IAttributeReader> attributeReaders = default!;

        public M3UFileReaderWithStream(IDictionary<string, IAttributeReader> attributeReaders = default!) {
            this.attributeReaders = attributeReaders ?? AttributeReaderRoot.Instance.AttributeReaders;
        }


        public override M3UFileInfo Read(Stream stream) {
            return Read(RequestUri, stream);
        }

        internal M3UFileInfo Read(Uri baseUri, Stream stream) {
            M3UFileInfo m3UFileInfo = new();
            using var reader = new LineReader(stream);
            if (!reader.MoveNext())
                throw new InvalidDataException("无效得m3u8文件");
            if (!string.Equals(reader.Current?.Trim(), "#EXTM3U", StringComparison.Ordinal))
                throw new InvalidDataException("无效得m3u8文件头部");

            while (reader.MoveNext()) {
                var text = reader.Current?.Trim();
                if (string.IsNullOrEmpty(text))
                    continue;

                var keyValuePair = KV.Parse(text);
                var CompareKey = keyValuePair.Key;

                if (!attributeReaders.TryGetValue(CompareKey ?? text, out IAttributeReader attributeReader))
                    attributeReaders.TryGetValue("#EXT-X-DISCONTINUITY", out attributeReader);

                if (attributeReader is null)
                    throw new InvalidDataException($"{text} 无法识别的标签,可能是非标准的标签，你可以删除此行，然后拖拽m3u8文件到请求地址，再次尝试下载");

                attributeReader.Write(m3UFileInfo, keyValuePair.Value, reader, baseUri);
                if (attributeReader.ShouldTerminate) break;
            }

            stream?.Dispose();
            return m3UFileInfo;
        }

        sealed class LineReader : IEnumerator<string> {
            private readonly StreamReader _reader;

            public string Current { get; private set; } = default!;

            object IEnumerator.Current => Current;

            public LineReader(Stream stream) {
                _reader = new StreamReader(stream);
            }

            public void Dispose() {
                _reader.Dispose();
            }

            public bool MoveNext() {
                bool endOfStream = _reader.EndOfStream;
                Current = endOfStream ? string.Empty : _reader.ReadLine() ?? string.Empty;
                return !endOfStream;
            }

            public void Reset() {
                var baseStream = _reader.BaseStream;
                if (baseStream.CanSeek)
                    baseStream.Seek(0L, SeekOrigin.Begin);
                _reader.DiscardBufferedData();
                Current = string.Empty;
            }
        }
    }
}