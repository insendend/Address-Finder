using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Classes.Http
{
    public class HttpProvider : IHttpProvider
    {
        public async Task<CustomHttpResult> ProcessRequestAsync(HttpRequestMessage requestMessage)
        {
            var client = new HttpClient();
            var responseMessage = await client.SendAsync(requestMessage);
            return new CustomHttpResult
            {
                IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
                HttpStatusCode = responseMessage.StatusCode,
                Content = await responseMessage.Content.ReadAsStringAsync()
            };
        }

        public async Task<CustomHttpResult> ProcessRequestByEncodingAsync(HttpRequestMessage requestMessage, Encoding encoding)
        {
            var client = new HttpClient();
            var responseMessage = await client.SendAsync(requestMessage);

            var responseBytes = await responseMessage.Content.ReadAsByteArrayAsync();

            return new CustomHttpResult
            {
                IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
                HttpStatusCode = responseMessage.StatusCode,
                Content = encoding.GetString(responseBytes)
            };
        }
    }
}
