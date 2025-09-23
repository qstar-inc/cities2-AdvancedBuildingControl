using System.Linq;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using StarQ.Shared.Extensions;
using Unity.Entities;
using UnityEngine;
using Resources = Game.Economy.Resources;

namespace AdvancedBuildingControl.Systems
{
    public partial class Utils : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;

#nullable enable
        protected override void OnCreate()
        {
            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnUpdate() { }

        public bool CheckPrefab(Entity entity, ref Entity currentPrefabRef)
        {
            if (entity == Entity.Null)
            {
                LogHelper.SendLog($"entity null for {entity}");
                return false;
            }

            if (!EntityManager.TryGetComponent(entity, out PrefabRef prefabRef))
            {
                LogHelper.SendLog($"prefabRef null for {entity}");
                return false;
            }

            currentPrefabRef = prefabRef.m_Prefab;

            if (currentPrefabRef == Entity.Null)
            {
                LogHelper.SendLog($"currentPrefabRef null for {entity}");
                return false;
            }
            return true;
        }

        public void UpdateUpkeep(Entity prefabRef, int level, Entity zonePrefab)
        {
            EntityManager.TryGetComponent(prefabRef, out BuildingData buildingData);
            EntityManager.TryGetComponent(prefabRef, out BuildingPropertyData buildingPropertyData);

            bool isStorage =
                buildingPropertyData.m_AllowedStored > Game.Economy.Resource.NoResource;

            var match = DataRetriever.zoneDataInfos.FirstOrDefault(b =>
                b.PrefabName == prefabSystem.GetPrefabName(zonePrefab)
            );

            EconomyParameterData economyParameterData =
                SystemAPI.GetSingleton<EconomyParameterData>();
            int newUpkeep = PropertyRenterSystem.GetUpkeep(
                level,
                match.Upkeep,
                buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                match.AreaType,
                ref economyParameterData,
                isStorage
            );

            EntityManager.TryGetComponent(prefabRef, out ConsumptionData consumptionData);
            consumptionData.m_Upkeep = newUpkeep;
            EntityManager.AddComponentData(prefabRef, consumptionData);
        }

        public void RemoveCurrentResource(Entity entity, Resource res)
        {
            EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Resources> resources);

            for (int i = resources.Length - 1; i >= 0; i--)
            {
                var r = resources[i].m_Resource;

                if (r != Resource.Money && !res.HasFlag(r))
                {
                    resources.RemoveAt(i);
                }
            }
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
                num3 =
                    num
                    * (float)buildingLevel
                    * (float)lotSize
                    * buildingPropertyData.m_SpaceMultiplier;
            }
            else
            {
                num3 =
                    (landValueBase * num2 + num * (float)buildingLevel)
                    * (float)lotSize
                    * buildingPropertyData.m_SpaceMultiplier;
            }
            float num4;
            if (PropertyUtils.IsMixedBuilding(buildingPropertyData))
            {
                num4 = (float)
                    Mathf.RoundToInt(
                        (float)buildingPropertyData.m_ResidentialProperties
                            / (1f - economyParameterData.m_MixedBuildingCompanyRentPercentage)
                    );
            }
            else
            {
                num4 = (float)buildingPropertyData.CountProperties();
            }
            return (num3, num4);
        }

        public int IntFromString(string value)
        {
            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }

        public ulong UlongFromString(string value)
        {
            return string.IsNullOrEmpty(value) ? 0 : ulong.Parse(value);
        }
    }
}
