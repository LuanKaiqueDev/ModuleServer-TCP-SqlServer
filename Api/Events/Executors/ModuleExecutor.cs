using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;

namespace Api.Events.Executors;

public class ModuleExecutor(IEventDistributor distributor) : AbstractExecutorSessionHandler
{
    public delegate void DisconnectCallback(int key);
    public event DisconnectCallback? OnDisconnect;
    
    protected override void Received(ByteReader reader)
    {
        distributor.ParseMessageToHandler(((ISessionHandler)this).Key, reader);
    }
    
    protected override void OnDisconnected(int key)
    {
        OnDisconnect?.Invoke(key);
    }
}