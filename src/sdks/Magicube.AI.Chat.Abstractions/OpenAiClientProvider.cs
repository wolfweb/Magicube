using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Magicube.AI.Chat.Abstractions {
    public class OpenAiClientProvider : IDisposable {
        protected readonly HttpClient HttpClient;
        public OpenAiClientProvider(IHttpClientFactory httpClientFactory) {
            HttpClient = httpClientFactory.CreateClient();
        }

        protected async Task<T> ReadResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken) {
            if (!response.IsSuccessStatusCode) {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            try {
                var debug = await response.Content.ReadAsStringAsync();
                return (await response.Content.ReadFromJsonAsync<T>(options: null, cancellationToken))!;
            }
            catch (Exception e) when (e is NotSupportedException or JsonException) {
                throw new Exception($"未能将以下json转换为: {typeof(T).Name}: {await response.Content.ReadAsStringAsync()}", e);
            }
        }

        public void Dispose() {
            
        }
    }
}