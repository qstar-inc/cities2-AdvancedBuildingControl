using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Common;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class Utils : GameSystemBase
    {
        public PrefabSystem prefabSystem;

        protected override void OnCreate()
        {
            prefabSystem = WorldHelper.PrefabSystem;
        }

        protected override void OnUpdate() { }

        public void SetOrAdd<T>(Entity entity, T toSet)
            where T : unmanaged, IComponentData
        {
            if (!EntityManager.HasComponent<T>(entity))
                EntityManager.AddComponentData(entity, toSet);
            else
                EntityManager.SetComponentData(entity, toSet);
        }

        public void SetAndUpdate<T>(Entity entity, T toSet)
            where T : unmanaged, IComponentData
        {
            if (!EntityManager.HasComponent<T>(entity))
                EntityManager.AddComponentData(entity, toSet);
            else
                EntityManager.SetComponentData(entity, toSet);
            EntityManager.AddComponent<Updated>(entity);
        }

        public void SetAndUpdate<T>(Entity toSetEntity, Entity toUpdateEntity, T toSet)
            where T : unmanaged, IComponentData
        {
            if (!EntityManager.HasComponent<T>(toSetEntity))
                EntityManager.AddComponentData(toSetEntity, toSet);
            else
                EntityManager.SetComponentData(toSetEntity, toSet);
            EntityManager.AddComponent<Updated>(toUpdateEntity);
        }

        public void AddAndUpdate<TAdd>(Entity entity, TAdd toAdd)
            where TAdd : unmanaged, IComponentData
        {
            EntityManager.AddComponentData(entity, toAdd);
            EntityManager.AddComponent<Updated>(entity);
        }

        public void AddAndSet<TAdd, TSet>(Entity entity, TAdd toAdd, TSet toSet)
            where TAdd : unmanaged, IComponentData
            where TSet : unmanaged, IComponentData
        {
            EntityManager.AddComponentData(entity, toAdd);
            SetAndUpdate(entity, toSet);
        }

        public void RemoveAndSet<TRemove, TSet>(Entity entity, TSet toSet)
            where TRemove : unmanaged, IComponentData
            where TSet : unmanaged, IComponentData
        {
            EntityManager.RemoveComponent<TRemove>(entity);
            SetAndUpdate(entity, toSet);
        }

        public static bool TryGetModBuffer(
            EntityManager EntityManager,
            Entity city,
            out DynamicBuffer<ModifiedPrefab> buffer
        )
        {
            buffer = new();
            if (!EntityManager.HasBuffer<ModifiedPrefab>(city))
                return false;

            if (EntityManager.TryGetBuffer(city, false, out buffer))
                return true;
            return false;
        }
    }
}
