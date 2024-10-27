using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Core.Logging;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;

namespace Core.Shared.Abstracts
{
    public abstract class AbstractBaseSession
    {
        #region Fields

        private int _bufferSize;
        private readonly Socket _sessionSocket = new(SocketType.Stream, ProtocolType.Tcp);
        private Task? _acceptorTask;
        private readonly ConcurrentDictionary<int, ISessionHandler> _sessionHandlers = new();
        private int _nextSessionKey;

        #endregion

        #region Properties

        protected void InitSession(Uri uri, int maxBackLog, bool noDelay, int bufferSize)
        {
            try
            {
                _sessionSocket.Bind(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port));
                _sessionSocket.Listen(maxBackLog);
                _sessionSocket.NoDelay = noDelay;
                _bufferSize = bufferSize;

                _acceptorTask = Task.Run(AcceptorCallback);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing session: {ex.Message}");
                throw; 
            }
        }

        #endregion
        
        #region Private Methods

        private async Task AcceptorCallback()
        {
            while (_acceptorTask is { IsCompleted: false })
            {
                try
                {
                    Socket sessionSocket = await _sessionSocket.AcceptAsync();
                    int index = _nextSessionKey++; // Use and increment the counter

                    IServerSessionHandler handler = GetSessionHandler();
                    if (handler is AbstractExecutorSessionHandler ash)
                    {
                        ash.OnDisconnect += OnDisconnected;
                    }
                    handler.Start(index, sessionSocket, _bufferSize);
                    _sessionHandlers.TryAdd(index, handler);
                    OnSessionCreated(index);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error accepting socket: {ex.Message}");
                }
            }
        }

        #endregion

        #region Protected Methods

        private void OnDisconnected(int key)
        {
            OnSessionClosed(key);
            _sessionHandlers.TryRemove(key, out var sessionHandler);
            Logger.Information($"Session {key} has disconnected");
        }
        
        protected abstract IServerSessionHandler GetSessionHandler();
        
        protected abstract void OnSessionCreated(int key);    
        protected abstract void OnSessionClosed(int key);
        protected void SendBytesTo(int key, ByteWriter writer)
        {
            try
            {
                if (_sessionHandlers.TryGetValue(key, out ISessionHandler? value))
                {
                    value.SendBytes(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected void SendBytesToAllExcept(int excludedKey, ByteWriter writer)
        {
            try
            {
                foreach (var keyValuePair in _sessionHandlers)
                {
                    int key = keyValuePair.Key;
                    ISessionHandler value = keyValuePair.Value;
                    if (key == excludedKey) continue;

                    value.SendBytes(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected void SendBytesToAll(ByteWriter writer)
        {
            try
            {
                foreach (var keyValuePair in _sessionHandlers)
                {
                    ISessionHandler value = keyValuePair.Value;
                    value.SendBytes(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        protected async Task SendBytesAsync(int key, ByteWriter writer)
        {
            try
            {
                if (_sessionHandlers.TryGetValue(key, out ISessionHandler? value))
                {
                    await value.SendBytesAsync(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        #endregion
    }
}