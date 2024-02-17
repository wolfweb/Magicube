using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Magicube.Core.Tasks {
	public static class SimpleDataflow {
		public static SimpleDataflow<ValueTuple> Create(CancellationToken cancellationToken = default) {
			var input = new TransformBlock<ValueTuple, ValueTuple>(x => x);
			return new SimpleDataflow<ValueTuple>(input, input, CreateDefaultBlockOptions(cancellationToken), CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
		}

		public static SimpleDataflow<T> Create<T>(IEnumerable<T> initial, CancellationToken cancellationToken = default) {
			var input = new TransformManyBlock<ValueTuple, T>(_ => initial);
			return new SimpleDataflow<T>(input, input, CreateDefaultBlockOptions(cancellationToken), CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
		}

		internal static readonly DataflowLinkOptions DataflowLinkOptions = new() { PropagateCompletion = true };

		private static ExecutionDataflowBlockOptions CreateDefaultBlockOptions(CancellationToken cancellationToken) =>
			new() {
				BoundedCapacity = DataflowBlockOptions.Unbounded,
				CancellationToken = cancellationToken,
				EnsureOrdered = false,
				MaxDegreeOfParallelism = Math.Max(1, System.Environment.ProcessorCount / 2),
			};
	}

	public sealed class SimpleDataflow<T> {
		public SimpleDataflow<TNext> Transform<TNext>(Func<T, TNext> func) =>
			LinkTo(new TransformBlock<T, TNext>(value => {
				try {
					return func(value);
				} catch (Exception) when (CancelAndRethrow()) {
					throw;
				}
			}, m_nextBlockOptions));

		public SimpleDataflow<TNext> Transform<TNext>(Func<T, Task<TNext>> func) =>
			LinkTo(new TransformBlock<T, TNext>(
				async value => {
					try {
						return await func(value).ConfigureAwait(false);
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));

		public SimpleDataflow<TNext> Transform<TNext>(Func<T, CancellationToken, Task<TNext>> func) {
			var cancellationToken = m_cancellationTokenSource.Token;
			return LinkTo(new TransformBlock<T, TNext>(
				async value => {
					try {
						return await func(value, cancellationToken).ConfigureAwait(false);
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));
		}

		public SimpleDataflow<TNext> TransformMany<TNext>(Func<T, IEnumerable<TNext>> func) =>
			LinkTo(new TransformManyBlock<T, TNext>(value => {
				try {
					return func(value);
				} catch (Exception) when (CancelAndRethrow()) {
					throw;
				}
			}, m_nextBlockOptions));

		public SimpleDataflow<TNext> TransformMany<TNext>(Func<T, Task<IEnumerable<TNext>>> func) =>
			LinkTo(new TransformManyBlock<T, TNext>(
				async value => {
					try {
						return await func(value).ConfigureAwait(false);
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));

		public SimpleDataflow<TNext> TransformMany<TNext>(Func<T, CancellationToken, Task<IEnumerable<TNext>>> func) {
			var cancellationToken = m_cancellationTokenSource.Token;
			return LinkTo(new TransformManyBlock<T, TNext>(
				async value => {
					try {
						return await func(value, cancellationToken).ConfigureAwait(false);
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));
		}

		public SimpleDataflow<T[]> Batch(int batchSize) =>
			LinkTo(new BatchBlock<T>(batchSize));

		public SimpleDataflow<T> ForAll(Action<T> action) =>
			LinkTo(new TransformBlock<T, T>(
				value => {
					try {
						action(value);
						return value;
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));

		public SimpleDataflow<T> ForAll(Func<T, Task> action) =>
			LinkTo(new TransformBlock<T, T>(
				async value => {
					try {
						await action(value).ConfigureAwait(false);
						return value;
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));

		public SimpleDataflow<T> ForAll(Func<T, CancellationToken, Task> action) {
			var cancellationToken = m_cancellationTokenSource.Token;
			return LinkTo(new TransformBlock<T, T>(
				async value => {
					try {
						await action(value, cancellationToken).ConfigureAwait(false);
						return value;
					} catch (Exception) when (CancelAndRethrow()) {
						throw;
					}
				}, m_nextBlockOptions));
		}

		public SimpleDataflow<TNext> LinkTo<TNext>(IPropagatorBlock<T, TNext> newOutput) {
			m_output.LinkTo(newOutput, SimpleDataflow.DataflowLinkOptions);
			return new SimpleDataflow<TNext>(m_input, newOutput, m_nextBlockOptions, m_cancellationTokenSource);
		}

		public SimpleDataflow<T> BoundedCapacity(int value) {
			m_nextBlockOptions.BoundedCapacity = value;
			return this;
		}

		public SimpleDataflow<T> EnsureOrdered(bool value = true) {
			m_nextBlockOptions.EnsureOrdered = value;
			return this;
		}

		public SimpleDataflow<T> MaxDegreeOfParallelism(int value) {
			m_nextBlockOptions.MaxDegreeOfParallelism = value;
			return this;
		}

		public async Task ExecuteAsync() {
			m_output.LinkTo(DataflowBlock.NullTarget<T>());
			if (!await m_input.SendAsync(default).ConfigureAwait(false))
				throw new InvalidOperationException("Input rejected.");
			m_input.Complete();
			await m_output.Completion.ConfigureAwait(false);
			m_cancellationTokenSource.Dispose();
		}

		public async Task ExecuteAsync(Action finish) {
			await ExecuteAsync();
			finish();
        }

        public async Task ExecuteAsync<TR>(Func<Task> finish) {
            await ExecuteAsync();
            await finish();
        }

        private bool CancelAndRethrow() {
			m_cancellationTokenSource.Cancel();
			return false;
		}

		internal SimpleDataflow(ITargetBlock<ValueTuple> input, ISourceBlock<T> output, ExecutionDataflowBlockOptions nextBlockOptions, CancellationTokenSource cancellationTokenSource) {
			m_input = input;
			m_output = output;
			m_nextBlockOptions = nextBlockOptions;
			m_cancellationTokenSource = cancellationTokenSource;
		}

		private readonly ITargetBlock<ValueTuple> m_input;
		private readonly ISourceBlock<T> m_output;
		private readonly ExecutionDataflowBlockOptions m_nextBlockOptions;
		private readonly CancellationTokenSource m_cancellationTokenSource;
	}
}
