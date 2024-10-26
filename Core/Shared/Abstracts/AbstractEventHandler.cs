using Core.Shared.ByteHandler;

namespace Core.Shared.Abstracts;

public abstract class AbstractEventHandler(ushort handlerType)
{
    public readonly ushort HandlerType = handlerType;
    
    protected ByteWriter CreateMessage()
    {
        ByteWriter messageWriter = new ByteWriter();
        messageWriter.Write(HandlerType);
        return messageWriter;
    }
    public abstract void ParseMessage(int key, ByteReader mr);
}