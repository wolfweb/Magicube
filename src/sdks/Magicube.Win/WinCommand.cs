using Magicube.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Magicube.Win {
    public class WinCommand {
        public static IReadOnlyList<int> AcceptableExitCodes { get; set; } = new[] { 0 };

        static bool IsInvalidExitCode(Process process) {
            return !AcceptableExitCodes.Any(x => x == process.ExitCode);
        }
        public static (Process Process, ProcessAsyncEnumerable StdOut, ProcessAsyncEnumerable StdError) DualRun(string fileName, string args, string workDirctroy = null) {
            var startInfo = Build(fileName, args, workDirctroy);

            var waitOutputDataCompleted = new TaskCompletionSource<object>();
            var (process, outputChannel) = StartAsync(startInfo, waitOutputDataCompleted);

            var errorChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true
            });

            var waitErrorDataCompleted = new TaskCompletionSource<object>();
            process.ErrorDataReceived += (sender, e) => {
                if (e.Data != null) {
                    errorChannel.Writer.TryWrite(e.Data);
                } else {
                    waitErrorDataCompleted.TrySetResult(null);
                }
            };

            process.Exited += async (sender, e) => {
                await waitErrorDataCompleted.Task.ConfigureAwait(false);
                await waitOutputDataCompleted.Task.ConfigureAwait(false);

                if (IsInvalidExitCode(process)) {
                    errorChannel.Writer.TryComplete();
                    outputChannel.Writer.TryComplete(new ProcessErrorException(process.ExitCode, Array.Empty<string>()));
                } else {
                    errorChannel.Writer.TryComplete();
                    outputChannel.Writer.TryComplete();
                }
            };

            if (!process.Start()) {
                throw new InvalidOperationException("Can't start process. FileName:" + fileName + ", Arguments:" + args);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return (process, new ProcessAsyncEnumerable(process, outputChannel.Reader), new ProcessAsyncEnumerable(null, errorChannel.Reader));
        }
        public static ProcessAsyncEnumerable Run(string fileName, string args, string workDirctroy) {
            var startInfo = Build(fileName, args, workDirctroy);
            startInfo.RedirectStandardInput = false;

            var errorList = new List<string>();
            
            var waitOutputDataCompleted = new TaskCompletionSource<object>();
            var waitErrorDataCompleted = new TaskCompletionSource<object>();
            
            var (process, outputChannel) = StartAsync(startInfo, waitOutputDataCompleted);

            process.ErrorDataReceived += (sender, e) => {
                if (e.Data != null) {
                    lock (errorList) {
                        errorList.Add(e.Data);
                    }
                } else {
                    waitErrorDataCompleted.TrySetResult(null);
                }
            };

            process.Exited += async (sender, e) => {
                await waitErrorDataCompleted.Task.ConfigureAwait(false);

                if (errorList.Count == 0) {
                    await waitOutputDataCompleted.Task.ConfigureAwait(false);
                }

                if (IsInvalidExitCode(process)) {
                    outputChannel.Writer.TryComplete(new ProcessErrorException(process.ExitCode, errorList.ToArray()));
                } else {
                    if (errorList.Count == 0) {
                        outputChannel.Writer.TryComplete();
                    } else {
                        outputChannel.Writer.TryComplete(new ProcessErrorException(process.ExitCode, errorList.ToArray()));
                    }
                }
            };

            if (!process.Start()) {
                throw new InvalidOperationException("Can't start process. FileName:" + fileName + ", Arguments:" + args);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return new ProcessAsyncEnumerable(process, outputChannel.Reader);
        }

        public static (Process,Channel<string>) StartAsync(ProcessStartInfo startInfo, TaskCompletionSource<object> waitOutputDataCompleted) {
            var process = new Process {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            var outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true
            });

            process.OutputDataReceived += (sender, e) => {
                if (e.Data != null) {
                    outputChannel.Writer.TryWrite(e.Data);
                } else {
                    waitOutputDataCompleted.TrySetResult(null);
                }
            };


            return (process,outputChannel);
        }

        private static ProcessStartInfo Build(string args, string fileName="cmd.exe", string workDirectory = null) {
            var startInfo = new ProcessStartInfo(fileName) {
                Arguments              = args,
                WindowStyle            = ProcessWindowStyle.Hidden,
                CreateNoWindow         = true,
                UseShellExecute        = false,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
            };


            if (!workDirectory.IsNullOrEmpty()) startInfo.WorkingDirectory = workDirectory;

            return startInfo;
        }
    }

    public class ProcessErrorException : Exception {
        public int ExitCode { get; }
        public string[] ErrorOutput { get; }

        public ProcessErrorException(int exitCode, string[] errorOutput)
            : base("Process returns error, ExitCode:" + exitCode + Environment.NewLine + string.Join(Environment.NewLine, errorOutput)) {
            ExitCode = exitCode;
            ErrorOutput = errorOutput;
        }
    }

    class ProcessAsyncEnumerator : IAsyncEnumerator<string> {
        private readonly Process _process;
        private readonly ChannelReader<string> _channel;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenRegistration _cancellationTokenRegistration;
        private string current;
        private bool disposed;

        public ProcessAsyncEnumerator(Process process, ChannelReader<string> channel, CancellationToken cancellationToken) {
            this._process = process;
            this._channel = channel;
            this._cancellationToken = cancellationToken;
            if (cancellationToken.CanBeCanceled) {
                _cancellationTokenRegistration = cancellationToken.Register(() => {
                    _ = DisposeAsync();
                });
            }
        }

        public string Current => current;

        public async ValueTask<bool> MoveNextAsync() {
            if (_channel.TryRead(out current)) {
                return true;
            } else {
                if (await _channel.WaitToReadAsync(_cancellationToken).ConfigureAwait(false)) {
                    if (_channel.TryRead(out current)) {
                        return true;
                    }
                }
                return false;
            }
        }

        public ValueTask DisposeAsync() {
            if (!disposed) {
                disposed = true;
                try {
                    _cancellationTokenRegistration.Dispose();
                    if (_process != null) {
                        _process.EnableRaisingEvents = false;
                        if (!_process.HasExited) {
                            _process.Kill();
                        }
                    }
                } finally {
                    if (_process != null) {
                        _process.Dispose();
                    }
                }
            }

            return default;
        }
    }

    public class ProcessAsyncEnumerable : IAsyncEnumerable<string> {
        private readonly Process _process;
        private readonly ChannelReader<string> _channel;

        internal ProcessAsyncEnumerable(Process process, ChannelReader<string> channel) {
            _process = process;
            _channel = channel;
        }

        public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
            return new ProcessAsyncEnumerator(_process, _channel, cancellationToken);
        }

        public async Task<string[]> ToTask(CancellationToken cancellationToken = default) {
            var list = new List<string>();
            await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false)) {
                list.Add(item);
            }
            return list.ToArray();
        }

        public async Task WriteLineAllAsync(CancellationToken cancellationToken = default) {
            await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false)) {
                Console.WriteLine(item);
            }
        }
    }
}
