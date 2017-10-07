using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Classes.Http
{
    public interface IHttpProvider
    {
        Task<CustomHttpResult> ProcessRequestAsync(HttpRequestMessage requestMessage);
        Task<CustomHttpResult> ProcessRequestByEncodingAsync(HttpRequestMessage requestMessage, Encoding encoding);
    }
}
