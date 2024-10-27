using Api.SpaceEntities.Abstracts;

namespace Api.SpaceEntities.Entities;

public class PlayerEntity(int userId) : SpaceEntity
{
    public int UserId { get; set; } = userId;
    public int? SessionId { get; set; }

    public void StartLogout()
    {
        SessionId = null;
    }

    public void StartReconciliation(int sessionId)
    {
        SessionId = sessionId;
    }
}