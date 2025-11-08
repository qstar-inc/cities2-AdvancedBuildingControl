using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using Game;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class CreatedEntitiesManagementSystem : GameSystemBase
    {
        //private static readonly object _lock = new();
        public static readonly Dictionary<string, List<Entity>> createdEntities = new();

        protected override void OnUpdate() { }

        public void DoUpdate()
        {
            //lock (_lock)
            //{
            foreach (var item in createdEntities)
            {
                foreach (var entity in item.Value)
                {
                    if (EntityManager.Exists(entity))
                        EntityManager.DestroyEntity(entity);
                }
            }
            createdEntities.Clear();

            EntityQuery createdQuery = SystemAPI.QueryBuilder().WithAll<CreatedEntities>().Build();
            EntityManager.DestroyEntity(createdQuery);
            //var createEntities = createdQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            //foreach (var item in createEntities)
            //{
            //    if (EntityManager.Exists(item))
            //    {
            //        //LogHelper.SendLog($"Destroying {item} from query", LogLevel.DEV);
            //        EntityManager.DestroyEntity(item);
            //    }
            //}

            //LogHelper.SendLog($"Clearing createdEntities from {array.Count} items", LogLevel.DEV);
            //}
        }

        public void AddEntity(string name, Entity entity)
        {
            //lock (_lock)
            //{
            if (!createdEntities.TryGetValue(name, out var list))
            {
                list = new List<Entity>();
                createdEntities[name] = list;
            }
            list.Add(entity);
            //LogHelper.SendLog($"Adding {entity} to createdEntities", LogLevel.DEV);
            //}
        }

        public void RemoveEntity(string name, Entity entity)
        {
            //lock (_lock)
            //{
            if (!createdEntities.TryGetValue(name, out var list))
                return;
            list.Remove(entity);
            if (list.Count == 0)
                createdEntities.Remove(name);
            //LogHelper.SendLog($"Removing {entity} from createdEntities", LogLevel.DEV);
            //}
        }

        public Entity CreateEntity(Entity ogEntity, string newName = "")
        {
            Entity newEntity = EntityManager.Instantiate(ogEntity);
            EntityManager.AddComponentData(newEntity, new CreatedEntities());

            if (!string.IsNullOrEmpty(newName))
                AddEntity(newName, newEntity);

            return newEntity;
        }

        public void DestroyEntity(string name, Entity entity)
        {
            //lock (_lock)
            //{
            if (!createdEntities.TryGetValue(name, out var list))
                return;

            list.Remove(entity);
            if (list.Count == 0)
                createdEntities.Remove(name);

            if (EntityManager.Exists(entity))
                EntityManager.DestroyEntity(entity);
            //}
            //if (createdEntities == null)
            //    return;

            //if (!createdEntities.ContainsKey(name))
            //    return;

            //List<Entity> list = createdEntities[name];

            //var details = entity;
            //bool contain = list.Contains(details);
            //if (contain)
            //    list.Remove(details);
            ////LogHelper.SendLog($"Removing {entity} from createdEntities", LogLevel.DEV);

            //if (EntityManager.Exists(entity))
            //    EntityManager.DestroyEntity(entity);
        }
    }
}
