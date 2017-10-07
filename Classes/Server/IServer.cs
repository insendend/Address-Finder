using System;

namespace Classes.Server
{
    public interface IServer : IDisposable
    {
        void Start();

        void Stop();
    }
}