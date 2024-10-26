using System.Collections.Concurrent;
using Api.Events.Enums;
using Core.Logging;
using Core.Shared.Abstracts;
using Core.Shared.ByteHandler;
using Core.Shared.Interfaces;

namespace Api.Events.EventDistributors;

public class ModuleEventDistributor : IEventDistributor
{
    #region Fields

    private ConcurrentDictionary<EventHandlerType, AbstractEventHandler> Handlers { get; } = new();

    #endregion

    #region IEventDistributor Implementation

    void IEventDistributor.InitHandlerDistributor()
    {
        Logger.Information("Initializing EventDistributor...");
        RegisterHandlers(CreateDefaultHandlers());
        Logger.Information($"Initialized EventDistributor with {Handlers.Count} handlers.");
    }

    AbstractEventHandler IEventDistributor.GetHandler(ushort handlerType)
    {
        if (!Handlers.TryGetValue((EventHandlerType)handlerType, out var handler))
        {
            throw new InvalidOperationException($"Handler not found for type: {handlerType}");
        }
        return handler;
    }

    void IEventDistributor.ParseMessageToHandler(int key, ByteReader reader)
    {
        EventHandlerType handlerType = (EventHandlerType)reader.ReadUShort();
        ((IEventDistributor)this).GetHandler((ushort)handlerType).ParseMessage(key, reader);
    }

    #endregion

    #region Private Methods

    private void RegisterHandlers(params AbstractEventHandler[] handlers)
    {
        foreach (var current in handlers)
        {
            if (Handlers.TryAdd((EventHandlerType)current.EventHandlerType, current))
            {
                Logger.Information($"[{((EventHandlerType)current.EventHandlerType).ToString()}Handler] has been registered.");
            }
            else
            {
                Logger.Error($"Handler for type {current.EventHandlerType} is already registered.");
            }
        }
    }

    private AbstractEventHandler[] CreateDefaultHandlers()
    {
        return new AbstractEventHandler[]
        {
            //Add Handlers here
        };
    }

    #endregion
}