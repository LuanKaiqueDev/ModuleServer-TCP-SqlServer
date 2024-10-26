using System.Net.Sockets;

namespace Core.Shared.Interfaces;

public interface IServerSessionHandler : ISessionHandler
{
    void Start(int key, Socket socket, int bufferSize);
}