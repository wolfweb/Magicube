using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Magicube.Core.IO {
    public class ThrottledStream : Stream {
        private long _bandwidthLimit;
        private Bandwidth _bandwidth;

        private readonly Stream _baseStream;

        public ThrottledStream(Stream baseStream, long bandwidthLimit) {
            if (bandwidthLimit < 0) {
                throw new ArgumentOutOfRangeException(nameof(bandwidthLimit),
                    bandwidthLimit, "The maximum number of bytes per second can't be negative.");
            }

            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            BandwidthLimit = bandwidthLimit;
        }

        public static long Infinite => long.MaxValue;

        public long BandwidthLimit {
            get => _bandwidthLimit;
            set {
                _bandwidthLimit = value <= 0 ? Infinite : value;
                _bandwidth ??= new Bandwidth();
                _bandwidth.BandwidthLimit = _bandwidthLimit;
            }
        }

        public override bool CanRead  => _baseStream.CanRead;

        public override bool CanSeek  => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length   => _baseStream.Length;

        public override long Position {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public override void Flush() {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            Throttle(count).Wait();
            return _baseStream.Read(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken) {
            await Throttle(count).ConfigureAwait(false);
            return await _baseStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            Throttle(count).Wait();
            _baseStream.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
            await Throttle(count).ConfigureAwait(false);
            await _baseStream.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }

        public override void Close() {
            _baseStream.Close();
            base.Close();
        }

        private async Task Throttle(int transmissionVolume) {
            if (BandwidthLimit > 0 && transmissionVolume > 0) {
                _bandwidth.CalculateSpeed(transmissionVolume);
                await Sleep(_bandwidth.PopSpeedRetrieveTime()).ConfigureAwait(false);
            }
        }

        private async Task Sleep(int time) {
            if (time > 0) {
                await Task.Delay(time).ConfigureAwait(false);
            }
        }

        public override string ToString() {
            return _baseStream.ToString();
        }
    }

    public class Bandwidth {
        private const double OneSecond = 1000; // millisecond
        
        private long _count;
        private int  _speedRetrieveTime;
        private int  _lastSecondCheckpoint;
        private long _lastTransferredBytesCount;

        public double Speed          { get; private set; }
        public double AverageSpeed   { get; private set; }
        public long   BandwidthLimit { get; set; }

        public Bandwidth() {
            BandwidthLimit = long.MaxValue;
            Reset();
        }

        public void CalculateSpeed(long receivedBytesCount) {
            int elapsedTime = System.Environment.TickCount - _lastSecondCheckpoint + 1;
            receivedBytesCount = Interlocked.Add(ref _lastTransferredBytesCount, receivedBytesCount);
            double momentSpeed = receivedBytesCount * OneSecond / elapsedTime; // B/s

            if (OneSecond < elapsedTime) {
                Speed = momentSpeed;
                AverageSpeed = ((AverageSpeed * _count) + Speed) / (_count + 1);
                _count++;
                SecondCheckpoint();
            }

            if (momentSpeed >= BandwidthLimit) {
                var expectedTime = receivedBytesCount * OneSecond / BandwidthLimit;
                Interlocked.Add(ref _speedRetrieveTime, (int)expectedTime - elapsedTime);
            }
        }

        public int PopSpeedRetrieveTime() {
            return Interlocked.Exchange(ref _speedRetrieveTime, 0);
        }

        public void Reset() {
            SecondCheckpoint();
            _count = 0;
            Speed = 0;
            AverageSpeed = 0;
        }

        private void SecondCheckpoint() {
            Interlocked.Exchange(ref _lastSecondCheckpoint, System.Environment.TickCount);
            Interlocked.Exchange(ref _lastTransferredBytesCount, 0);
        }
    }
}
