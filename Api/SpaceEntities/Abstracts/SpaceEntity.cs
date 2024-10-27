using Api.SpaceEntities.Interfaces;

namespace Api.SpaceEntities.Abstracts;

public abstract class SpaceEntity : ISpaceEntity
{
    public int ObjectId { get; set; }
}