using System;
using System.IO;
using System.Net.Sockets;
using Classes.Client.Settings;
using System.Net;

namespace Classes.Client
{
    public class AddressClient : IClient
    {
        private readonly AddressClientSettings _settings;
        private Socket _client;

        public bool IsConnected => _client?.Connected ?? false;

        public AddressClient(AddressClientSettings settings)
        {
            _settings = settings;
        }

        public bool Connect()
        {
            if (_client != null && _client.Connected)
                Dispose();

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            IPAddress.TryParse(_settings.Ip, out var ip);
            _client.Connect(new IPEndPoint(ip, _settings.Port));
            return _client.Connected;
        }

        public int Send(byte[] buff, int offset, int count)
        {
            if (!_client.Connected)
                throw new SocketException();

            return _client.Send(buff, offset, count, SocketFlags.None);   
        }

        public byte[] Receive()
        {
            if (!_client.Connected)
                throw new SocketException();

            using (var ms = new MemoryStream())
            {
                var buff = new byte[4096];

                while (true)
                {
                    var recvBytes = _client.Receive(buff, 0, buff.Length, SocketFlags.None);
                    ms.Write(buff, 0, recvBytes);

                    if (recvBytes < buff.Length)
                        break;

                    Array.Clear(buff, 0, recvBytes);
                }

                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            if (_client?.Connected ?? false)
            {
                _client?.Shutdown(SocketShutdown.Both);
                _client?.Disconnect(false);
            }
            _client?.Dispose();
        }
    }
}
