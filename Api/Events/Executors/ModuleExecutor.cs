using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;

namespace Api.Events.Executors;

public class ModuleExecutor(IEventDistributor distributor) : AbstractExecutorSessionHandler
{
    protected override void Received(ByteReader reader)
    {
        distributor.ParseMessageToHandler(((ISessionHandler)this).Key, reader);
    }
}