using System;

namespace Classes.Client
{
    public interface IClient : IDisposable
    {
        bool Connect();

        bool IsConnected { get; }

        int Send(byte[] msg, int offset, int count);
        byte[] Receive();
    }
}
