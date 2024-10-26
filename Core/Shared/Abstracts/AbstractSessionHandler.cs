using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.Channels;
using Core.Logging;
using Core.Shared.ByteHandler;
using Core.Shared.ByteHandler.Interfaces;
using Core.Shared.Interfaces;

namespace Core.Shared.Abstracts
{
    public abstract class AbstractSessionHandler : ISessionHandler
    {
        #region Fields
        
        int ISessionHandler.Key { get; set; } = -1;
        private Socket? _socket;
        private int _bufferSize;
        private bool _disposed = false;

        #endregion

        #region Task Fields

        private Task? _receiverTask;
        private Task? _senderTask;
        private Channel<IByteReader>? _receiverChannel;
        private Channel<IByteWriter>? _senderChannel;

        #endregion

        #region Protected Methods

        protected ValueTask<bool> WaitToReadAsync()
        {
            return _receiverChannel!.Reader.WaitToReadAsync();
        }

        protected bool TryRead([NotNullWhen(true)] out IByteReader? read)
        {
            return _receiverChannel!.Reader.TryRead(out read);
        }

        protected abstract void OnDisconnected(int key);

        #endregion

        #region Private Methods

        private async Task SenderCallback()
        {
            while (!_disposed && _socket?.Connected == true)
            {
                if (!await _senderChannel!.Reader.WaitToReadAsync()) continue;
                if (_senderChannel.Reader.TryRead(out IByteWriter? message))
                {
                    await ((ISessionHandler)this).SendBytesAsync(message.FetchByteWriter());
                }
            }
        }

        private async Task ReceiverCallback()
        {
            var buffer = new byte[_bufferSize];

            while (!_disposed && _socket is { Connected: true })
            {
                try
                {
                    var messageLength = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (messageLength == 0)
                    {
                        OnDisconnected(((ISessionHandler)this).Key);
                        return;
                    }

                    var receivedData = new byte[messageLength];
                    Array.Copy(buffer, receivedData, messageLength);
                    ByteReader reader = new ByteReader(receivedData);
                    await _receiverChannel!.Writer.WriteAsync(reader);
                }
                catch (SocketException ex)
                {
                    OnDisconnected(((ISessionHandler)this).Key);
                    Logger.Error($"key: {((ISessionHandler)this).Key} Error: {ex.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    OnDisconnected(((ISessionHandler)this).Key);
                    Logger.Error($"key: {((ISessionHandler)this).Key} Error: {ex.Message}");
                    return;
                }
            }

            OnDisconnected(((ISessionHandler)this).Key);
        }

        #endregion

        #region Public Methods

        void ISessionHandler.SendBytes(ByteWriter writer)
        {
            if (_socket is null || !_socket.Connected) return;
            _socket.Send(writer.GetBuffer(), 0, writer.GetLength(), SocketFlags.None);
        }

        async Task ISessionHandler.SendBytesAsync(ByteWriter writer)
        {
            if (_socket is null) return;

            var buffer = new ArraySegment<byte>(writer.GetBuffer(), 0, writer.GetLength());
            await _socket.SendAsync(buffer, SocketFlags.None);
        }

        public virtual void Start(int key, Socket socket, int bufferSize)
        {
            ((ISessionHandler)this).Key = key;
            _socket = socket;
            _bufferSize = bufferSize;

            _receiverChannel = Channel.CreateBounded<IByteReader>(100);
            _senderChannel = Channel.CreateBounded<IByteWriter>(100);

            _receiverTask = Task.Run(ReceiverCallback);
            _senderTask = Task.Run(SenderCallback);
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _socket?.Shutdown(SocketShutdown.Both);
                _socket?.Dispose();
                _receiverChannel?.Writer.Complete();
                _senderChannel?.Writer.Complete();

                if (_receiverTask != null)
                {
                    try { _receiverTask.Wait(); } catch { /* Handle task cancellation */ }
                }

                if (_senderTask != null)
                {
                    try { _senderTask.Wait(); } catch { /* Handle task cancellation */ }
                }
            }

            _disposed = true;
        }

        #endregion
    }
}