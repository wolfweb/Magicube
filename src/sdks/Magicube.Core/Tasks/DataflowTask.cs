using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Magicube.Core.Tasks {
    public class DataflowTask<T> {
		private readonly BufferBlock<T> _buffer;
		private readonly Func<Task<DataflowContext<T>>> _produce;
		private readonly Func<T, Task> _consume;
		
		private int Size { get; set; }

		public DataflowTask(Func<Task<DataflowContext<T>>> produce, Func<T, Task> consume) {
			_buffer  = new BufferBlock<T>();
			_produce = produce;
			_consume = consume;
		}

		public async Task ExecuteOnceAsync() {
			Size = 1;
			await StartAsync();
		}

		public async Task ExecuteAsync(int size) {
			Size = size;
			await StartAsync();
		}

		public async Task StartAsync() {
			var consume = Consume(_buffer);
			var produce = Produce(_buffer);
			await consume;
		}

		private async Task Produce(ITargetBlock<T> target) {
			int count = 0;
			while (true) {
				var v = await _produce();
				if (v == null || v.Break) break;
				target.Post(v.Data);
				if (Size != -1) {
					count++;
					if (count >= Size) break;
				}
			}
			target.Complete();
		}

		private async Task Consume(IReceivableSourceBlock<T> source) {
			while (await source.OutputAvailableAsync()) {
				while (source.TryReceive(out T data)) {
					await _consume(data);
				}
			}
		}
	}

	public class DataflowContext<T> {
		public DataflowContext(T v) {
			Data = v;
		}
		public T    Data  { get; set; }
		public bool Break { get; set; } = false;
	}

	#region
	public class ChannelOption {
		public int MessageSize { get; set; } = 10;
		public ConcurrentDictionary<Type, object> Channels = new ConcurrentDictionary<Type, object>();
		public Channel<T> As<T>() {
			var type = typeof(T);
			if (Channels.ContainsKey(type)) return (Channel<T>)Channels[type];
			return null;
		}
	}

	public class ChannelProducer<T> {
		public ChannelProducer(ChannelOption option) {
			var type = typeof(T);
			if (!option.Channels.ContainsKey(type)) {
				var channel = Channel.CreateBounded<T>(option.MessageSize);
				Writer = channel.Writer;
				option.Channels.TryAdd(type, channel as object);
			}

		}
		public ChannelWriter<T> Writer { get; }

		public void Send(T message) {
			Writer.TryWrite(message);
		}

		public void Done() => Writer.Complete();
	}

	public class ChannelConsumer<T> {
		public ChannelConsumer(ChannelOption option) {
			Reader = option.As<T>()?.Reader;
		}

		public ChannelReader<T> Reader { get; }

		public async Task Read() {
			if (Reader == null) return;

			while (await Reader.WaitToReadAsync()) {
				while (Reader.TryRead(out T msg)) {
					Trace.WriteLine($"receive message");
				}
			}
		}
	}

	#endregion
}
