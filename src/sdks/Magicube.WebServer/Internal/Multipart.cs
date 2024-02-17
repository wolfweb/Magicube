using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Magicube.WebServer.Internal {
    internal class Multipart {
        internal class HttpMultipart {
            private const byte Lf = (byte)'\n';
            private readonly HttpMultipartBuffer _readBuffer;
            private readonly Stream _requestStream;

            public HttpMultipart(Stream requestStream, string boundary) {
                _requestStream = requestStream;
                var boundaryAsBytes = GetBoundaryAsBytes(boundary, false);
                var closingBoundaryAsBytes = GetBoundaryAsBytes(boundary, true);
                _readBuffer = new HttpMultipartBuffer(boundaryAsBytes, closingBoundaryAsBytes);
            }

            public IEnumerable<HttpMultipartBoundary> GetBoundaries(int requestQueryFormMultipartLimit) {
                var list = new List<HttpMultipartBoundary>();

                foreach (var boundaryStream in GetBoundarySubStreams(requestQueryFormMultipartLimit))
                    list.Add(new HttpMultipartBoundary(boundaryStream));

                return list;
            }

            private IEnumerable<HttpMultipartSubStream> GetBoundarySubStreams(int requestQueryFormMultipartLimit) {
                var boundarySubStreams = new List<HttpMultipartSubStream>();
                var boundaryStart = GetNextBoundaryPosition();

                var found = 0;
                while (MultipartIsNotCompleted(boundaryStart) && found < requestQueryFormMultipartLimit) {
                    var boundaryEnd = GetNextBoundaryPosition();
                    boundarySubStreams.Add(new HttpMultipartSubStream(_requestStream, boundaryStart, GetActualEndOfBoundary(boundaryEnd)));
                    boundaryStart = boundaryEnd;
                    found++;
                }

                return boundarySubStreams;
            }

            private bool MultipartIsNotCompleted(long boundaryPosition) {
                return boundaryPosition > -1 && !_readBuffer.IsClosingBoundary;
            }

            private long GetActualEndOfBoundary(long boundaryEnd) {
                if (CheckIfFoundEndOfStream())
                    return _requestStream.Position - (_readBuffer.Length + 2);

                return boundaryEnd - (_readBuffer.Length + 2);
            }

            private bool CheckIfFoundEndOfStream() {
                return _requestStream.Position.Equals(_requestStream.Length);
            }

            private static byte[] GetBoundaryAsBytes(string boundary, bool closing) {
                var boundaryBuilder = new StringBuilder();

                boundaryBuilder.Append("--");
                boundaryBuilder.Append(boundary);

                if (closing)
                    boundaryBuilder.Append("--");
                else {
                    boundaryBuilder.Append('\r');
                    boundaryBuilder.Append('\n');
                }

                var bytes = Encoding.ASCII.GetBytes(boundaryBuilder.ToString());
                return bytes;
            }

            private long GetNextBoundaryPosition() {
                _readBuffer.Reset();
                while (true) {
                    var byteReadFromStream = _requestStream.ReadByte();

                    if (byteReadFromStream == -1)
                        return -1;

                    _readBuffer.Insert((byte)byteReadFromStream);

                    if (_readBuffer.IsFull && (_readBuffer.IsBoundary || _readBuffer.IsClosingBoundary))
                        return _requestStream.Position;

                    if (byteReadFromStream.Equals(Lf) || _readBuffer.IsFull)
                        _readBuffer.Reset();
                }
            }
        }

        internal class HttpMultipartBuffer {
            private readonly byte[] _boundaryAsBytes;
            private readonly byte[] _closingBoundaryAsBytes;
            private readonly byte[] _buffer;
            private int _position;

            public HttpMultipartBuffer(byte[] boundaryAsBytes, byte[] closingBoundaryAsBytes) {
                _boundaryAsBytes = boundaryAsBytes;
                _closingBoundaryAsBytes = closingBoundaryAsBytes;
                _buffer = new byte[_boundaryAsBytes.Length];
            }

            public bool IsBoundary {
                get { return _buffer.SequenceEqual(_boundaryAsBytes); }
            }

            public bool IsClosingBoundary {
                get { return _buffer.SequenceEqual(_closingBoundaryAsBytes); }
            }

            public bool IsFull {
                get { return _position.Equals(_buffer.Length); }
            }

            public int Length {
                get { return _buffer.Length; }
            }

            public void Reset() {
                _position = 0;
            }

            public void Insert(byte value) {
                _buffer[_position++] = value;
            }
        }

        internal class HttpMultipartBoundary {
            private const byte Lf = (byte)'\n';
            private const byte Cr = (byte)'\r';

            public HttpMultipartBoundary(HttpMultipartSubStream boundaryStream) {
                Value = boundaryStream;
                ExtractHeaders();
            }

            public string ContentType { get; private set; }

            public string Filename { get; private set; }

            public string Name { get; private set; }

            public HttpMultipartSubStream Value { get; private set; }

            private void ExtractHeaders() {
                while (true) {
                    var header = ReadLineFromStream(Value);

                    if (string.IsNullOrEmpty(header))
                        break;

                    if (header.StartsWith("Content-Disposition", StringComparison.CurrentCultureIgnoreCase)) {
                        Name = Regex.Match(header, @"name=""?(?<name>[^\""]*)", RegexOptions.IgnoreCase).Groups["name"].Value;
                        Filename = Regex.Match(header, @"filename=""?(?<filename>[^\""]*)", RegexOptions.IgnoreCase).Groups["filename"].Value;
                    }

                    if (header.StartsWith("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        ContentType = header.Split(' ').Last().Trim();
                }

                Value.PositionStartAtCurrentLocation();
            }

            private static string ReadLineFromStream(Stream stream) {
                var readBuffer = new List<byte>();

                while (true) {
                    var byteReadFromStream = stream.ReadByte();

                    if (byteReadFromStream == -1)
                        return null;

                    if (byteReadFromStream.Equals(Lf))
                        break;

                    readBuffer.Add((byte)byteReadFromStream);
                }

                return Encoding.UTF8.GetString(readBuffer.ToArray()).Trim((char)Cr);
            }
        }

        internal class HttpMultipartSubStream : Stream {
            private readonly Stream _stream;
            private long _start;
            private readonly long _end;
            private long _position;

            public HttpMultipartSubStream(Stream stream, long start, long end) {
                _stream = stream;
                _start = start;
                _position = start;
                _end = end;
            }

            public override bool CanRead {
                get { return true; }
            }

            public override bool CanSeek {
                get { return true; }
            }

            public override bool CanWrite {
                get { return false; }
            }

            public override long Length {
                get { return (_end - _start); }
            }

            public override long Position {
                get { return _position - _start; }
                set { _position = Seek(value, SeekOrigin.Begin); }
            }

            private long CalculateSubStreamRelativePosition(SeekOrigin origin, long offset) {
                var subStreamRelativePosition = 0L;

                switch (origin) {
                    case SeekOrigin.Begin:
                        subStreamRelativePosition = _start + offset;
                        break;
                    case SeekOrigin.Current:
                        subStreamRelativePosition = _position + offset;
                        break;
                    case SeekOrigin.End:
                        subStreamRelativePosition = _end + offset;
                        break;
                }
                return subStreamRelativePosition;
            }

            public void PositionStartAtCurrentLocation() {
                _start = _stream.Position;
            }

            public override void Flush() {
            }

            public override int Read(byte[] buffer, int offset, int count) {
                if (count > (_end - _position))
                    count = (int)(_end - _position);

                if (count <= 0)
                    return 0;

                _stream.Position = _position;
                var bytesReadFromStream = _stream.Read(buffer, offset, count);
                RepositionAfterRead(bytesReadFromStream);
                return bytesReadFromStream;
            }

            public override int ReadByte() {
                if (_position >= _end)
                    return -1;

                _stream.Position = _position;
                var byteReadFromStream = _stream.ReadByte();
                RepositionAfterRead(1);
                return byteReadFromStream;
            }

            private void RepositionAfterRead(int bytesReadFromStream) {
                if (bytesReadFromStream == -1)
                    _position = _end;
                else
                    _position += bytesReadFromStream;
            }

            public override long Seek(long offset, SeekOrigin origin) {
                var subStreamRelativePosition =
                    CalculateSubStreamRelativePosition(origin, offset);

                if (subStreamRelativePosition < 0 || subStreamRelativePosition > _end)
                    throw new InvalidOperationException();

                _position = _stream.Seek(subStreamRelativePosition, SeekOrigin.Begin);
                return _position;
            }

            public override void SetLength(long value) {
                throw new InvalidOperationException();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                throw new InvalidOperationException();
            }
        }
    }
}
