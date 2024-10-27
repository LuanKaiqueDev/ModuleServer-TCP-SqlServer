using Api.Events.Executors;
using Api.Events.Handlers;
using Api.Interfaces;
using Core.Logging;
using Core.Shared;
using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;
using Microsoft.Extensions.Options;

namespace Api
{
    public class Module(IEventDistributor distributor, IOptions<Networking> networkingOptions) : AbstractBaseSession, IModule
    {
        #region Fields

        private readonly Networking _networking = networkingOptions.Value;
        public event IModule.DisconnectCallback? OnSessionEnd;
        #endregion

        #region ISession Implementation

        void IModule.Start()
        {
            try
            {
                StartSession();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to start session: {ex.Message}");
                throw;
            }
        }

        void IModule.SendBytesTo(int key, ByteWriter reader)
        {
            base.SendBytesTo(key,reader);
        }

        #endregion

        #region Private Methods

        private void StartSession()
        {
            var uri = new Uri(_networking.ApiEndpoint);
            base.InitSession(uri, _networking.MaxBacklog, _networking.NoDelay, _networking.BufferSize);
        }

        #endregion

        #region Protected Methods

        protected override IServerSessionHandler GetSessionHandler() => new ModuleExecutor(distributor);

        protected override void OnSessionCreated(int key)
        {
            LoginEventHandler.GetEventHandler().Connected(key);
        }

        protected override void OnSessionClosed(int key)
        {
            OnSessionEnd?.Invoke(key);
        }

        #endregion
    }
}