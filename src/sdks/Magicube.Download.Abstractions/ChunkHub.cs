using System;
using System.Linq;

namespace Magicube.Download.Abstractions {
    public class ChunkHub {
        private readonly DownloadOptions _options;
        private int _chunkCount = 0;
        private long _chunkSize = 0;
        private long _startOffset = 0;

        public ChunkHub(DownloadOptions option) {
            _options = option;
        }

        public void CalcChunks(DownloadChunkPackage package) {
            Validate(package);

            if (package.Chunks is null) {
                package.Chunks = new Chunk[_chunkCount];
                for (int i = 0; i < _chunkCount; i++) {
                    long startPosition = _startOffset + (i * _chunkSize);
                    long endPosition = startPosition + _chunkSize - 1;
                    package.Chunks[i] = GetChunk(i.ToString(), startPosition, endPosition);
                }
                package.Chunks.Last().End += package.TotalFileSize % _chunkCount; 
            }
            else {
                package.Validate();
            }
        }

        private void Validate(DownloadChunkPackage package) {
            _chunkCount  = package.ChunkCount;
            _startOffset = package.RangeStart;

            if (_startOffset < 0) {
                _startOffset = 0;
            }

            if (package.TotalFileSize < _chunkCount) {
                _chunkCount = (int)package.TotalFileSize;
            }

            if (_chunkCount < 1) {
                _chunkCount = 1;
            }

            _chunkSize = package.TotalFileSize / _chunkCount;
        }

        private Chunk GetChunk(string id, long start, long end) {
            var chunk = new Chunk(start, end) {
                Id = id,
                MaxTryAgainOnFailover = _options.MaxTryAgainOnFailover,
                Timeout = _options.Timeout
            };

            return chunk;
        }
    }
}