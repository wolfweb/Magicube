using System;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;

namespace Magicube.WebServer {
    public class RequestStream : Stream {
        public static long RequestLengthDiskThreshold = 81920;

        private bool _disableStreamSwitching;
        private readonly long _thresholdLength;
        private bool _isSafeToDisposeStream;
        private Stream _stream;
        private const int BufferSize = 4096;

        public RequestStream(Stream stream, long expectedLength, long thresholdLength, bool disableStreamSwitching) {
            _thresholdLength = thresholdLength;
            _disableStreamSwitching = disableStreamSwitching;
            _stream = stream ?? CreateDefaultMemoryStream(expectedLength);

            ThrowExceptionIfCtorParametersWereInvalid(_stream, expectedLength, _thresholdLength);

            if (!MoveStreamOutOfMemoryIfExpectedLengthExceedSwitchLength(expectedLength))
                MoveStreamOutOfMemoryIfContentsLengthExceedThresholdAndSwitchingIsEnabled();

            if (!_stream.CanSeek) {
                var task = MoveToWritableStream();
                task.Wait();

                if (task.IsFaulted)
                    throw new InvalidOperationException("Unable to copy stream", task.Exception);
            }

            _stream.Position = 0;
        }

        private Task<object> MoveToWritableStream() {
            var tcs = new TaskCompletionSource<object>();
            var sourceStream = _stream;
            _stream = new MemoryStream(BufferSize);

            CopyTo(sourceStream, this, (source, destination, ex) => {
                if (ex != null)
                    tcs.SetException(ex);
                else
                    tcs.SetResult(null);
            });

            return tcs.Task;
        }

        public static void CopyTo(Stream source, Stream destination, Action<Stream, Stream, Exception> onComplete) {
            var buffer = new byte[BufferSize];

            Action<Exception> done = e => {
                if (onComplete != null)
                    onComplete.Invoke(source, destination, e);
            };

            AsyncCallback rc = null;

            rc = readResult => {
                try {
                    var read = source.EndRead(readResult);

                    if (read <= 0) {
                        done.Invoke(null);
                        return;
                    }

                    destination.BeginWrite(buffer, 0, read, writeResult => {
                        try {
                            destination.EndWrite(writeResult);
                            source.BeginRead(buffer, 0, buffer.Length, rc, null);
                        } catch (Exception ex) {
                            done.Invoke(ex);
                        }

                    }, null);
                } catch (Exception ex) {
                    done.Invoke(ex);
                }
            };

            source.BeginRead(buffer, 0, buffer.Length, rc, null);
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanSeek {
            get { return _stream.CanSeek; }
        }

        public override bool CanTimeout {
            get { return false; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override long Length {
            get { return _stream.Length; }
        }

        public bool IsInMemory {
            get { return !(_stream.GetType() == typeof(FileStream)); }
        }

        public override long Position {
            get { return _stream.Position; }
            set {
                if (value < 0)
                    throw new InvalidOperationException("The position of the stream cannot be set to less than zero.");

                if (value > Length)
                    throw new InvalidOperationException("The position of the stream cannot exceed the length of the stream.");

                _stream.Position = value;
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return _stream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected override void Dispose(bool disposing) {
            if (_isSafeToDisposeStream) {
                ((IDisposable)_stream).Dispose();

                var fileStream = _stream as FileStream;
                if (fileStream != null)
                    DeleteTemporaryFile(fileStream.Name);
            }

            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult) {
            return _stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult) {
            _stream.EndWrite(asyncResult);
            ShiftStreamToFileStreamIfNecessary();
        }

        public override void Flush() {
            _stream.Flush();
        }

        private static long GetExpectedRequestLength(string contentLengthHeaderValue) {
            if (contentLengthHeaderValue == null)
                return 0;

            long contentLength;
            return !long.TryParse(contentLengthHeaderValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ? 0 : contentLength;
        }

        public static RequestStream FromStream(Stream stream, string contentLength) {
            long expectedLength = GetExpectedRequestLength(contentLength);
            return FromStream(stream, expectedLength, RequestLengthDiskThreshold, false);
        }

        public static RequestStream FromStream(Stream stream, long expectedLength, long thresholdLength, bool disableStreamSwitching) {
            return new RequestStream(stream, expectedLength, thresholdLength, disableStreamSwitching);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return _stream.Read(buffer, offset, count);
        }

        public override int ReadByte() {
            return _stream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            _stream.Write(buffer, offset, count);
            ShiftStreamToFileStreamIfNecessary();
        }

        private void ShiftStreamToFileStreamIfNecessary() {
            if (_disableStreamSwitching)
                return;

            if (_stream.Length >= _thresholdLength) {
                var old = _stream;
                MoveStreamContentsToFileStream();
                old.Close();
            }
        }

        private static FileStream CreateTemporaryFileStream() {
            var filePath = Path.GetTempFileName();
            return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 8192, true);
        }

        private Stream CreateDefaultMemoryStream(long expectedLength) {
            _isSafeToDisposeStream = true;

            if (_disableStreamSwitching || expectedLength < _thresholdLength)
                return new MemoryStream((int)expectedLength);

            _disableStreamSwitching = true;
            return CreateTemporaryFileStream();
        }

        private static void DeleteTemporaryFile(string fileName) {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return;

            try {
                File.Delete(fileName);
            } catch {
            }
        }

        private void MoveStreamOutOfMemoryIfContentsLengthExceedThresholdAndSwitchingIsEnabled() {
            if (!_stream.CanSeek)
                return;

            try {
                if ((_stream.Length > _thresholdLength) && !_disableStreamSwitching)
                    MoveStreamContentsToFileStream();
            } catch (NotSupportedException) {
            }
        }

        private bool MoveStreamOutOfMemoryIfExpectedLengthExceedSwitchLength(long expectedLength) {
            if ((expectedLength < _thresholdLength) || _disableStreamSwitching)
                return false;

            MoveStreamContentsToFileStream();
            return true;
        }

        private void MoveStreamContentsToFileStream() {
            var targetStream = CreateTemporaryFileStream();
            _isSafeToDisposeStream = true;

            if (_stream.CanSeek && _stream.Length == 0) {
                _stream.Close();
                _stream = targetStream;
                return;
            }

            if (_stream.CanSeek)
                _stream.Position = 0;

            _stream.CopyTo(targetStream, 8196);

            if (_stream.CanSeek)
                _stream.Flush();

            _stream = targetStream;
            _disableStreamSwitching = true;
        }

        private static void ThrowExceptionIfCtorParametersWereInvalid(Stream stream, long expectedLength, long thresholdLength) {
            if (!stream.CanRead)
                throw new InvalidOperationException("The stream must support reading.");

            if (expectedLength < 0)
                throw new ArgumentOutOfRangeException("expectedLength", expectedLength, "The value of the expectedLength parameter cannot be less than zero.");

            if (thresholdLength < 0)
                throw new ArgumentOutOfRangeException("thresholdLength", thresholdLength, "The value of the threshHoldLength parameter cannot be less than zero.");
        }
    }

}
