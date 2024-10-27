using Api.Interfaces;
using Api.SpaceEntities.Entities;
using Api.SpaceEntities.Interfaces;

namespace Api
{
    public class Sector : ISector
    {
        private readonly IModule _iModule;
        private readonly List<ISpaceEntity> _spaceEntities = [];
        private int _entityCounter;

        public Sector(IModule iModule)
        {
            _iModule = iModule;
            _iModule.OnSessionEnd += OnSessionEnd;
        }

        private void OnSessionEnd(int key)
        {
            foreach (var ship in _spaceEntities)
            {
                if (ship is PlayerEntity playerEntity && playerEntity.SessionId == key)
                {
                    playerEntity.StartLogout();
                }
                return;
            }
        }

        public void CreateEntity(ISpaceEntity entity)
        {
            if (entity is PlayerEntity spaceEntity)
            {
                foreach (var ship in _spaceEntities)
                {
                    if (ship is PlayerEntity playerEntity && playerEntity.UserId == spaceEntity.UserId)
                    {
                        if (spaceEntity.SessionId.HasValue)
                        {
                            playerEntity.StartReconciliation(spaceEntity.SessionId.Value);
                        }
                        return;
                    }
                }
            }
            _entityCounter++;
            entity.ObjectId = _entityCounter;
            _spaceEntities.Add(entity);
        }
    }
}