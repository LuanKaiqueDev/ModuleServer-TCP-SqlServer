using Core.Shared.ByteHandler;

namespace Core.Shared.Abstracts;

public abstract class AbstractEventHandler(ushort eventHandlerType)
{
    public readonly ushort EventHandlerType = eventHandlerType;
    
    protected ByteWriter CreateMessage()
    {
        ByteWriter messageWriter = new ByteWriter();
        messageWriter.Write(EventHandlerType);
        return messageWriter;
    }
    public abstract void ParseMessage(int key, ByteReader mr);
}