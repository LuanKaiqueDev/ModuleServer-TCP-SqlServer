using Api.Events.Enums;
using Api.Interfaces;
using Core.Logging;
using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;
using Database.Entities;
using Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Events.Handlers;

public class LoginEventHandler : AbstractEventHandler
{
    private enum Sender : ushort
    {
        Connected = 0,
    }

    private enum Receive : ushort
    {
        Login = 0,
    }
    
    private static IEventDistributor? _eventDistributor;
    private readonly IModule _module;
    private readonly CharacterRepository _characterRepository;
    
    public LoginEventHandler(IServiceProvider provider)
        : base((ushort)EventHandlerType.Login)
    {
        _eventDistributor = provider.GetRequiredService<IEventDistributor>()!;
        _module = provider.GetRequiredService<IModule>();
        _characterRepository = provider.GetRequiredService<CharacterRepository>();
    }
    
    public static LoginEventHandler GetEventHandler()
    {
        return _eventDistributor?.GetHandler((ushort)EventHandlerType.Login) as LoginEventHandler?? 
               throw new Exception($"No handler registered for {EventHandlerType.Login}");
    }
    
    public override void ParseMessage(int key, ByteReader reader)
    {
        var receiver = (Receive)reader.ReadUShort();
        switch (receiver)
        {
            case Receive.Login:
                _ = HandleLogin(key, reader);
                // use void ex: Function(key, reader)
                break;
                
            default:
                Logger.Error("Unknown type");
                break;
        }
    }

    private async Task HandleLogin(int key, ByteReader reader)
    {
        int characterId = reader.ReadInt();
        Character? character = await _characterRepository.GetCharacterById(characterId);
        if (character is not null)
        {
            //Make server logic to save character in another class.
        }
    }

    public void Connected(int key)
    {
        ByteWriter writer = CreateMessage();
        writer.Write((ushort)Sender.Connected);
        _module.SendBytes(key, writer);
    }
}