using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using Game;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class CreatedEntitiesManagementSystem : GameSystemBase
    {
        public static readonly Dictionary<string, List<CreatedEntitiesDetail>> createdEntities =
            new();

        //public static readonly Dictionary<
        //    string,
        //    List<CreatedEntitiesDetail>
        //> createdEntitiesStorage = new();
        //public static readonly Dictionary<
        //    string,
        //    List<CreatedEntitiesDetail>
        //> createdEntitiesLevel = new();
        //public static readonly Dictionary<
        //    string,
        //    List<CreatedEntitiesDetail>
        //> createdEntitiesHousing = new();

        public class CreatedEntitiesDetail
        {
            public Entity Entity = Entity.Null;
        }

        //public enum Arrays
        //{
        //    None,
        //    Storage,
        //    Level,
        //    Housing,
        //}

        protected override void OnUpdate() { }

        public void DoUpdate()
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem DoUpdate", LogLevel.DEV);
            //List<Dictionary<string, List<CreatedEntitiesDetail>>> arrays = new()
            //{
            //    createdEntitiesStorage,
            //    createdEntitiesLevel,
            //    createdEntitiesHousing,
            //};

            //foreach (var array in arrays)
            //{

            var array = createdEntities;
            foreach (var item in array)
            {
                foreach (var item1 in item.Value)
                {
                    if (EntityManager.Exists(item1.Entity))
                    {
                        LogHelper.SendLog($"Destroying {item1.Entity} from dict", LogLevel.DEV);
                        EntityManager.DestroyEntity(item1.Entity);
                    }
                }
            }
            array.Clear();

            EntityQuery createdQuery = SystemAPI.QueryBuilder().WithAll<CreatedEntities>().Build();
            var createEntities = createdQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var item in createEntities)
            {
                if (EntityManager.Exists(item))
                {
                    LogHelper.SendLog($"Destroying {item} from query", LogLevel.DEV);
                    EntityManager.DestroyEntity(item);
                }
            }

            LogHelper.SendLog($"Clearing createdEntities from {array.Count} items", LogLevel.DEV);
            //}
        }

        //private Dictionary<string, List<CreatedEntitiesDetail>>? GetArray(Arrays arrayEnum)
        //{
        //    return createdEntities;
        //    //return arrayEnum switch
        //    //{
        //    //    Arrays.None => null,
        //    //    Arrays.Storage => createdEntitiesStorage,
        //    //    Arrays.Level => createdEntitiesLevel,
        //    //    Arrays.Housing => createdEntitiesHousing,
        //    //    _ => null,
        //    //};
        //}

        public void AddEntity(string name, Entity entity) //, Arrays arrayEnum = Arrays.None)
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem AddEntity", LogLevel.DEV);

            var array = createdEntities; //GetArray(arrayEnum);

            if (array == null)
                return;

            if (!array.ContainsKey(name))
            {
                array.Add(name, new List<CreatedEntitiesDetail>());
            }

            List<CreatedEntitiesDetail> list = array[name];

            list.Add(new CreatedEntitiesDetail() { Entity = entity });
            LogHelper.SendLog($"Adding {entity} to createdEntities", LogLevel.DEV);
        }

        public void RemoveEntity(string name, Entity entity) //, Arrays arrayEnum = Arrays.None)
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem RemoveEntity", LogLevel.DEV);

            var array = createdEntities; //GetArray(arrayEnum);
            if (array == null)
                return;

            if (!array.ContainsKey(name))
            {
                return;
            }

            List<CreatedEntitiesDetail> list = array[name];

            var details = new CreatedEntitiesDetail() { Entity = entity };
            if (list.Contains(details))
            {
                list.Remove(details);
                LogHelper.SendLog($"Removing {entity} from createdEntities", LogLevel.DEV);
            }
        }

        public Entity CreateEntity(Entity ogEntity, string newName = "")
        {
            Entity newEntity = EntityManager.Instantiate(ogEntity);
            EntityManager.AddComponentData(newEntity, new CreatedEntities());
            if (!string.IsNullOrEmpty(newName))
            {
                AddEntity(newName, newEntity);
            }

            return newEntity;
        }

        public void DestroyEntity(string name, Entity entity)
        {
            LogHelper.SendLog("CreatedEntitiesManagementSystem DestroyEntity", LogLevel.DEV);

            var array = createdEntities; //GetArray(arrayEnum);
            if (array == null)
                return;

            if (!array.ContainsKey(name))
            {
                return;
            }

            List<CreatedEntitiesDetail> list = array[name];

            var details = new CreatedEntitiesDetail() { Entity = entity };
            bool contain = list.Contains(details);
            if (contain)
            {
                list.Remove(details);
                LogHelper.SendLog($"Removing {entity} from createdEntities", LogLevel.DEV);
            }
            else
            {
                LogHelper.SendLog($"{entity} not on list", LogLevel.DEV);
            }
            if (EntityManager.Exists(entity))
            {
                EntityManager.DestroyEntity(entity);
            }
        }
    }
}
