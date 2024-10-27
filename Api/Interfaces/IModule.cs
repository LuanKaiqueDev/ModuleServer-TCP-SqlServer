using Core.Shared.ByteHandler;

namespace Api.Interfaces;

public interface IModule
{
    void Start();
    void SendBytesTo(int key, ByteWriter writer);
    delegate void DisconnectCallback(int key);
    event DisconnectCallback? OnSessionEnd;
}