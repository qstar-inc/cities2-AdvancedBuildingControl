using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossal.Serialization.Entities;
using Game;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class CreatedEntitiesManagementSystem : GameSystemBase
    {
        //public StorageChangerSystem storageChangerSystem;

        public static readonly Dictionary<string, List<CreatedEntitiesDetail>> createdEntities =
            new();

        public class CreatedEntitiesDetail
        {
            public Entity entity = Entity.Null;
        }

        protected override void OnCreate()
        {
            //storageChangerSystem =
            //    World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StorageChangerSystem>();
        }

        protected override void OnUpdate()
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem OnUpdate", LogLevel.DEV);
            foreach (var item in createdEntities)
            {
                foreach (var item1 in item.Value)
                {
                    if (EntityManager.Exists(item1.entity))
                    {
                        LogHelper.SendLog($"Destroying {item1.entity}", LogLevel.DEV);
                        EntityManager.DestroyEntity(item1.entity);
                    }
                }
            }
            LogHelper.SendLog(
                $"Clearing createdEntities from {createdEntities.Count} items",
                LogLevel.DEV
            );
            createdEntities.Clear();
        }

        public void AddEntity(string name, Entity entity)
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem AddEntity", LogLevel.DEV);
            if (!createdEntities.ContainsKey(name))
            {
                createdEntities.Add(name, new List<CreatedEntitiesDetail>());
            }

            List<CreatedEntitiesDetail> list = createdEntities[name];

            list.Add(new CreatedEntitiesDetail() { entity = entity });
            LogHelper.SendLog($"Adding {entity} to createdEntities", LogLevel.DEV);
        }

        public void RemoveEntity(string name, Entity entity)
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem RemoveEntity", LogLevel.DEV);
            if (!createdEntities.ContainsKey(name))
            {
                return;
            }
            List<CreatedEntitiesDetail> list = createdEntities[name];

            var details = new CreatedEntitiesDetail() { entity = entity };
            if (list.Contains(details))
            {
                list.Remove(details);
                LogHelper.SendLog($"Removing {entity} from createdEntities", LogLevel.DEV);
            }
        }
    }
}
