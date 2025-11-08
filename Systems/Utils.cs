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
            prefabSystem = WorldHelper.PrefabSystem;
        }

        protected override void OnUpdate() { }

        public bool TryGetPrefabEntity(string prefabName, out Entity prefabEntity)
        {
            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", prefabName),
                    out var prefabBase
                )
            )
            {
                LogHelper.SendLog($"Missing BuildingPrefab: {prefabName}");
                prefabEntity = Entity.Null;
                return false;
            }
            return prefabSystem.TryGetEntity(prefabBase, out prefabEntity);
        }

        public bool CheckPrefab(Entity entity, ref Entity currentPrefabRef)
        {
            if (EntityManager.HasComponent<Building>(entity) == false)
            {
                LogHelper.SendLog($"not a building, {entity}");
                return false;
            }
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

            if (EntityManager.TryGetComponent(entity, out OriginalEntity og))
            {
                var currentPrefabName = prefabSystem.GetPrefabName(currentPrefabRef);
                if (!og.OGEntity.Equals(string.Empty) && og.OGEntity != currentPrefabName)
                {
                    LogHelper.SendLog(
                        $"currentPrefabName '{currentPrefabName}' didn't match with '{og.OGEntity}' for {entity}"
                    );
                    return false;
                }
                return true;
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

        public void CheckPrefabData(
            Entity newPrefabEntity,
            Entity entity,
            out bool hasStorage,
            out StorageCompanyData storageCompanyData,
            out bool isSpawnable,
            out SpawnableBuildingData spawnableBuildingData,
            out bool hasProperty,
            out BuildingPropertyData buildingPropertyData,
            out bool isWaterPump,
            out WaterPumpingStationData waterPumpingStationData,
            out bool isSewageDump,
            out SewageOutletData sewageOutletData,
            out bool isPowerProd,
            out PowerPlantData powerPlantData,
            out bool isDepot,
            out TransportDepotData transportDepotData,
            out bool isGarbageFacility,
            out GarbageFacilityData garbageFacilityData,
            out bool isHospital,
            out HospitalData hospitalData,
            out bool isDeathcare,
            out DeathcareFacilityData deathcareFacilityData
        )
        {
            hasStorage = EntityManager.TryGetComponent(newPrefabEntity, out storageCompanyData);
            isSpawnable = EntityManager.TryGetComponent(newPrefabEntity, out spawnableBuildingData);
            hasProperty =
                EntityManager.TryGetComponent(newPrefabEntity, out buildingPropertyData)
                && EntityManager.HasComponent<ResidentialProperty>(entity);
            isWaterPump = EntityManager.TryGetComponent(
                newPrefabEntity,
                out waterPumpingStationData
            );
            isSewageDump = EntityManager.TryGetComponent(newPrefabEntity, out sewageOutletData);
            isPowerProd = EntityManager.TryGetComponent(newPrefabEntity, out powerPlantData);
            isDepot = EntityManager.TryGetComponent(newPrefabEntity, out transportDepotData);
            isGarbageFacility = EntityManager.TryGetComponent(
                newPrefabEntity,
                out garbageFacilityData
            );
            isHospital = EntityManager.TryGetComponent(newPrefabEntity, out hospitalData);
            isDeathcare = EntityManager.TryGetComponent(newPrefabEntity, out deathcareFacilityData);
        }

        public int IntFromString(string value) =>
            string.IsNullOrEmpty(value) ? 0 : int.Parse(value);

        public ulong UlongFromString(string value) =>
            string.IsNullOrEmpty(value) ? 0 : ulong.Parse(value);
    }
}
