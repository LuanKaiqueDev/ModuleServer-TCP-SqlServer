using System.Net.Sockets;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;

namespace Core.Shared.Abstracts
{
    public abstract class AbstractExecutorSessionHandler : AbstractSessionHandler, IServerSessionHandler
    {
        #region  Fields

        public delegate void DisconnectCallback(int key);
        public event DisconnectCallback? OnDisconnect;

        #endregion
        
        #region Override Methods
        public override void Start(int key, Socket socket, int bufferSize)
        {
            base.Start(key, socket, bufferSize);
            Task.Run(Execute);
        }

        #endregion

        #region Abstract Methods

        protected abstract void Received(ByteReader reader);

        #endregion

        #region Private Methods

        private async Task Execute()
        {
            while (await WaitToReadAsync())
            {
                if (TryRead(out var reader))
                {
                    Received(reader.FetchByteReader());
                }
            }
        }
        
        protected override void OnDisconnected(int key)
        {
            OnDisconnect?.Invoke(key);
        }

        #endregion
    }
}