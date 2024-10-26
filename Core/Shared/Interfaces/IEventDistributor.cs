using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;

namespace Core.Shared.Interfaces;

public interface IEventDistributor
{
    void InitHandlerDistributor();
    AbstractEventHandler GetHandler(ushort handlerType);
    void ParseMessageToHandler(int key, ByteReader reader);
}