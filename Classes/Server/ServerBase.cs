using System;
using System.Net;
using System.Net.Sockets;
using Classes.Server.Settings;
using Serilog;

namespace Classes.Server
{
    public class ServerBase : IServer
    {
        private readonly AddressServerSettings _settings;
        private readonly Socket _server;
        private readonly IPEndPoint _endPoint;
        private readonly ILogger _logger;

        public ServerBase(AddressServerSettings settings, ILogger logger)
        {
            _settings = settings;
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _endPoint = new IPEndPoint(IPAddress.Parse(settings.Ip), settings.Port);
            _logger = logger;
        }

        public void Start()
        {
            try
            {
                _server.Bind(_endPoint);
                _server.Listen(_settings.ClientCount);

                _logger.Information($"Awaiting for TCP requests at {_server.LocalEndPoint}");

                _server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Start failed");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var client = _server.EndAccept(ar);
                _logger.Information($"Client {client.RemoteEndPoint} connected.");

                var state = new CustomStateObject { Client = client };

                client
                    .BeginReceive(state.BuffBytes, 0, state.BuffBytes.Length, SocketFlags.None, ReceiveCallback, state);

                _server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Accept failed");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as CustomStateObject;

            try
            {
                if (state is null) throw new ArgumentNullException(nameof(state));

                state.ReadBytes = state.Client.EndReceive(ar);
                _logger.Information($"Received from {state.Client.RemoteEndPoint} {state.ReadBytes} bytes.");

                if (state.ReadBytes == state.BuffBytes.Length)
                {
                    _logger.Information(
                        $"To much data received from {state.Client.RemoteEndPoint}. Closing connection...");
                    state.Client?.Shutdown(SocketShutdown.Both);
                    state.Client?.Dispose();
                    return;
                }

                var newData = ProcessData(state.BuffBytes, 0, state.ReadBytes);

                state.Client
                    .BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, state.Client);

            }
            catch (SocketException)
            {
                state?.Client.Shutdown(SocketShutdown.Both);
                _logger.Warning($"Client {state?.Client.RemoteEndPoint} disconnected.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Receive data failed");
            }
        }

        protected virtual byte[] ProcessData(byte[] data, int offset, int count)
        {
            return data;
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;
            if (client is null) throw new ArgumentNullException(nameof(client));

            try
            {
                var sentBytes = client.EndSend(ar);
                _logger.Information($"Sent to {client.RemoteEndPoint} {sentBytes} bytes.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Send data failed");
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Disconnect(false);
                _logger.Warning($"Disconnection of the {client.RemoteEndPoint}.");
                client.Close();
            }
        }

        public void Stop()
        {
            _server.Shutdown(SocketShutdown.Both);
            Dispose();
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
