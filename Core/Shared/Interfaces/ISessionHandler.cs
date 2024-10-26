using Core.Shared.ByteHandler;

namespace Core.Shared.Interfaces;

public interface ISessionHandler : IDisposable
{
    int Key { get; set; }
    void SendBytes(ByteWriter writer);
    Task SendBytesAsync(ByteWriter writer);
}