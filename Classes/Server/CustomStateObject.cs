using System.IO;
using System.Net.Sockets;

namespace Classes.Server
{
    public class CustomStateObject
    {
        public Socket Client { get; set; }
        public byte[] BuffBytes { get; set; }
        public int ReadBytes { get; set; }

        public CustomStateObject(int buffSize = 4096)
        {
            BuffBytes = new byte[buffSize];
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
