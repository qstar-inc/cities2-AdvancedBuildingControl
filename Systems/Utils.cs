using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Zones;
using StarQ.Shared.Extensions;
using Unity.Entities;
using UnityEngine;

namespace AdvancedBuildingControl.Systems
{
    public partial class Utils : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;

#nullable enable

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

        public (float, float) CheckMaxRent(
            BuildingPropertyData buildingPropertyData,
            int buildingLevel,
            int lotSize,
            float landValueBase,
            AreaType areaType,
            ref EconomyParameterData economyParameterData,
            bool ignoreLandValue = false
        )
        {
            float num = economyParameterData.m_RentPriceBuildingZoneTypeBase.x;
            float num2 = economyParameterData.m_LandValueModifier.x;
            if (areaType == AreaType.Commercial)
            {
                num = economyParameterData.m_RentPriceBuildingZoneTypeBase.y;
                num2 = economyParameterData.m_LandValueModifier.y;
            }
            else if (areaType == AreaType.Industrial)
            {
                num = economyParameterData.m_RentPriceBuildingZoneTypeBase.z;
                num2 = economyParameterData.m_LandValueModifier.z;
            }
            float num3;
            if (ignoreLandValue)
            {
                num3 = num * buildingLevel * lotSize * buildingPropertyData.m_SpaceMultiplier;
            }
            else
            {
                num3 =
                    (landValueBase * num2 + num * buildingLevel)
                    * lotSize
                    * buildingPropertyData.m_SpaceMultiplier;
            }
            float num4;
            if (PropertyUtils.IsMixedBuilding(buildingPropertyData))
            {
                num4 = Mathf.RoundToInt(
                    buildingPropertyData.m_ResidentialProperties
                        / (1f - economyParameterData.m_MixedBuildingCompanyRentPercentage)
                );
            }
            else
            {
                num4 = buildingPropertyData.CountProperties();
            }
            return (num3, num4);
        }

        public static bool TryGetModBuffer(
            EntityManager EntityManager,
            Entity city,
            out DynamicBuffer<ModifiedPrefab_T7> buffer
        )
        {
            buffer = new();
            if (!EntityManager.HasBuffer<ModifiedPrefab_T7>(city))
                return false;

            if (EntityManager.TryGetBuffer(city, false, out buffer))
                return true;
            return false;
        }
    }
}
