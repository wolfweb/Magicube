using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;

namespace Magicube.Data.Abstractions.Loggings {
    public class SqlProfileLoggerProvider : ILoggerProvider {
        private readonly LoggerExternalScopeProvider _scopeProvider ;
        public SqlProfileLoggerProvider() {
            _scopeProvider = new LoggerExternalScopeProvider();
        }

        public ILogger CreateLogger(string categoryName) => new SqlLogger(categoryName, _scopeProvider);

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        class SqlLogger : ILogger {
            private readonly string _name;
            private readonly LoggerExternalScopeProvider _scopeProvider;
            public SqlLogger(string name, LoggerExternalScopeProvider scopeProvider) {
                _name = name;
                _scopeProvider = scopeProvider;
            }

            public IDisposable BeginScope<TState>(TState state) => _scopeProvider.Push(state);

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
                var sb = new StringBuilder();
                sb.Append(GetLogLevelString(logLevel))
                  .Append(" [").Append(_name).Append("] ")
                  .Append(formatter(state, exception));

                if (exception != null) {
                    sb.Append('\n').Append(exception);
                }

                _scopeProvider.ForEachScope((scope, state) => {
                    state.Append("\n => ");
                    state.Append(scope);
                }, sb);

                Trace.WriteLine(sb.ToString());
            }

            private static string GetLogLevelString(LogLevel logLevel) {
                return logLevel switch {
                    LogLevel.Trace => "trce",
                    LogLevel.Debug => "dbug",
                    LogLevel.Information => "info",
                    LogLevel.Warning => "warn",
                    LogLevel.Error => "fail",
                    LogLevel.Critical => "crit",
                    _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
                };
            }
        }
    }
}