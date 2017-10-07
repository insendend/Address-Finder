using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Classes.Http;
using System.Threading.Tasks;

namespace Classes.Parser
{
    public class AddressParser
    {
        private readonly IHttpProvider _provider;

        public AddressParser(IHttpProvider provider)
        {
            _provider = provider;
        }

        private async Task<string> GetSourceAsync(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var res = await _provider.ProcessRequestByEncodingAsync(request, Encoding.GetEncoding("Windows-1251"));

            return res.Content;
        }

        private async Task<IHtmlDocument> GetDocumentAsync(string htmlPage)
        {
            var parser = new HtmlParser();
            return await parser.ParseAsync(htmlPage);
        }

        public async Task<IEnumerable<Address>> FilterContent(string postCode)
        {
            var url = $"http://uaindex.info/zip/{postCode}/?lang=rus";
            var src = await GetSourceAsync(url);
            var doc = await GetDocumentAsync(src);

            var city = doc.QuerySelectorAll("big").Skip(1).FirstOrDefault()?.InnerHtml;

            if (city is null)
                return null;

            var addressTable = doc.GetElementById("main")?.QuerySelectorAll("table")?.LastOrDefault();

            return addressTable
                ?.QuerySelectorAll("tr")
                .Skip(1)
                .Select(rowAddress => new Address
                {
                    City = city,
                    Street = rowAddress.QuerySelector("td:nth-child(2) a")?.InnerHtml,
                    FullAddress = rowAddress.QuerySelector("td:nth-child(3)")?.InnerHtml
                });

        }
    }
}
