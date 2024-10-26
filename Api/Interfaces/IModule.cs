using Core.Shared.ByteHandler;

namespace Api.Interfaces;

public interface IModule
{
    void Start();
    void SendBytes(int key, ByteWriter reader);
    Task SendBytesAsync(int key, ByteWriter reader);
}