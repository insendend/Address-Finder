using System.Linq;
using System.Text;
using Classes.Parser;
using Classes.Server.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Classes.Server
{
    public class AddressServer : ServerBase
    {
        private readonly AddressParser _parser;

        public AddressServer(AddressServerSettings settings, AddressParser parser, ILogger logger) : base(settings, logger)
        {
            _parser = parser;
        }

        protected override byte[] ProcessData(byte[] data, int offset, int count)
        {
            var postCode = Encoding.UTF8.GetString(data, offset, count);

            var resCollection = _parser.FilterContent(postCode).Result?.ToList();

            if (resCollection is null)
                return new byte[0];

            var responseString = JsonConvert.SerializeObject(resCollection);
            var responseBytes = Encoding.UTF8.GetBytes(responseString);

            return responseBytes;
        }
    }
}
