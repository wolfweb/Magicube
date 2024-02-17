using Magicube.Core.Tasks;
using Magicube.TestBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Magicube.Core.Test {
    public class DataflowTaskTest {
		private readonly ITestOutputHelper _outputHelper;
		public DataflowTaskTest(ITestOutputHelper outputHelper) {
			_outputHelper = outputHelper;
		}

		[Fact]
		public async Task Func_Dataflow_Test() {
			var task = new DataflowTask<Foo>(async () => {
				await Task.Delay(1);
				return new DataflowContext<Foo>(new Foo());
			}, x => {
				_outputHelper.WriteLine(x.Name);
				return Task.CompletedTask;
			});

			await task.ExecuteAsync(10);
		}

		[Fact]
        public async Task Func_Dataflow_Pipeline_Test() {
			string res = null;
			await SimpleDataflow.Create().Transform(x => "f3e60651ce25d22149d7ee8ddd78f914")
				.Transform(x => Guid.Parse(x))
				.Transform(x => x.ToByteArray())
				.TransformMany(x => {
					res = string.Join("|", x);
					return res;
				})
				.ExecuteAsync();

			Assert.NotNull(res);
			Assert.True(res.Split("|".ToCharArray()).Length == 16);
		}

		[Fact]
		public async Task Func_Dataflow_Pipeline_Test1() {
			var url = "https://www.gutenberg.org/cache/epub/16452/pg16452.txt";

			await SimpleDataflow.Create()
				.Transform(async x => await new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip }).GetStringAsync(url))
				.Transform(text => {
					Trace.WriteLine("Creating word list...");

					char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
					text = new string(tokens);

					return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				})
				.Transform(words=> {
					Trace.WriteLine("Filtering word list...");
					return words
						.Where(word => word.Length > 3)
						.Distinct()
						.ToArray();
				})
				.TransformMany(words=> {
					Trace.WriteLine("Finding reversed words...");

					var wordsSet = new HashSet<string>(words);

					return from word in words.AsParallel()
						   let reverse = new string(word.Reverse().ToArray())
						   where word != reverse && wordsSet.Contains(reverse)
						   select word;
				})
				.ForAll(reversedWord => {
					Trace.WriteLine($"Found reversed words {reversedWord}/{new string(reversedWord.Reverse().ToArray())}");
				})
				.ExecuteAsync();
		}
	}
}
