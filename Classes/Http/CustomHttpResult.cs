using System.Net;

namespace Classes.Http
{
    public class CustomHttpResult
    {
        public bool IsSuccessStatusCode { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string Content { get; set; }
    }
}
