using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Systems.Changers;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using Unity.Entities;
using UnityEngine;
using static Game.Prefabs.TriggerPrefabData;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIP_ABC : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return nameof(AdvancedBuildingControl); }
        }

#nullable disable
        public static BldgGeneralInfo bldgGeneralInfo = new();
        public static BldgBrandInfo bldgBrandInfo = new();
        public static BldgZoningInfo bldgZoningInfo = new();
        public static BldgStorageInfo bldgStorageInfo = new();
        public static BldgUtilityInfo bldgUtilityInfo = new();
        public static BldgVehicleInfo bldgVehicleInfo = new();

        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;

        private RefChangerSystem refChangerSystem;
        private EntityComponentChanger entityComponentChangerSystem;
        private Utils utils;

#nullable enable

        private Entity companyEntity = Entity.Null;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem = WorldHelper.NameSystem;
            prefabSystem = WorldHelper.PrefabSystem;

            refChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RefChangerSystem>();
            entityComponentChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityComponentChanger>();
            utils = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Utils>();

            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger<string>("ToggleResource", ToggleResource);
            CreateTrigger("ResetStorage", ResetStorage);
            CreateTrigger<int>("ChangeLevel", ChangeLevel);
            CreateTrigger("ResetLevel", ResetLevel);
            CreateTrigger<int>("ChangeHousehold", ChangeHousehold);
            CreateTrigger("ResetHousehold", ResetHousehold);
            CreateTrigger<int>("ChangeMaxWorkplace", ChangeMaxWorkplace);
            CreateTrigger("ResetMaxWorkplace", ResetMaxWorkplace);
            CreateTrigger<int>("ChangeWaterPumpCapacity", ChangeWaterPumpCapacity);
            CreateTrigger("ResetWaterPumpCapacity", ResetWaterPumpCapacity);
            CreateTrigger<int>("ChangeSewageDumpCapacity", ChangeSewageDumpCapacity);
            CreateTrigger("ResetSewageDumpCapacity", ResetSewageDumpCapacity);
            CreateTrigger<int>("ChangeSewageDumpPurification", ChangeSewageDumpPurification);
            CreateTrigger("ResetSewageDumpPurification", ResetSewageDumpPurification);
            CreateTrigger<int>("ChangePowerProdCapacity", ChangePowerProdCapacity);
            CreateTrigger("ResetPowerProdCapacity", ResetPowerProdCapacity);
            CreateTrigger<int, RefChangerSystem.ValueType>(
                "ChangeVehicleCapacity",
                ChangeVehicleCapacity
            );
            CreateTrigger<RefChangerSystem.ValueType>("ResetVehicleCapacity", ResetVehicleCapacity);

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = Visible();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("bldgGeneralInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgGeneralInfo);

            writer.PropertyName("bldgZoningInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgZoningInfo);

            writer.PropertyName("bldgBrandInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgBrandInfo);

            writer.PropertyName("bldgStorageInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgStorageInfo);

            writer.PropertyName("bldgUtilityInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgUtilityInfo);

            writer.PropertyName("bldgVehicleInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgVehicleInfo);
        }

        protected override void Reset()
        {
            bldgGeneralInfo = new();
            bldgZoningInfo = new();
            bldgStorageInfo = new();
            bldgBrandInfo = new();
            bldgUtilityInfo = new();
            bldgVehicleInfo = new();

            companyEntity = Entity.Null;
        }

        private bool Visible()
        {
            bool isVisible = false;
            if (EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
            {
                if (!prefabSystem.TryGetPrefab(prefabRef.m_Prefab, out PrefabBase _))
                    return false;

                if (EntityManager.HasComponent<BuildingData>(prefabRef.m_Prefab))
                    isVisible = true;

                if (!isVisible)
                    return false;

                return true;
            }
            return false;
        }

        protected override void OnProcess()
        {
            if (
                EntityManager.HasComponent<Abandoned>(selectedEntity)
                || EntityManager.HasComponent<Destroyed>(selectedEntity)
            )
                return;

            CheckLevel();
            CheckHousehold();
            CheckBrand();
            CheckStorage();
            CheckWorkplace();
            CheckUtility();
            CheckVehicle();
            CheckGeneral();
        }

        public void CheckLevel()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgZoningInfo.HasLevel = false;
            if (
                !EntityManager.HasComponent<SignatureBuildingData>(prefabRef.m_Prefab)
                //&& !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(prefabRef.m_Prefab)
                && EntityManager.HasComponent<SpawnableBuildingData>(prefabRef.m_Prefab)
            )
            {
                bldgZoningInfo.HasLevel = true;
                UpdateUILevelBinding();
            }
        }

        public void CheckHousehold()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgZoningInfo.HasHousehold = false;
            if (
                //!EntityManager.HasComponent<Abandoned>(selectedEntity)
                //&&
                EntityManager.HasComponent<BuildingData>(prefabRef.m_Prefab)
            )
            {
                if (
                    EntityManager.TryGetComponent(
                        prefabRef.m_Prefab,
                        out BuildingPropertyData buildingPropertyData
                    )
                    && buildingPropertyData.m_ResidentialProperties > 0
                )
                {
                    bldgZoningInfo.HasHousehold = true;
                }
                UpdateUIHouseholdBinding();
            }
        }

        public void CheckBrand()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgBrandInfo.HasBrand = false;
            if (
                CompanyUIUtils.HasCompany(
                    EntityManager,
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out companyEntity
                )
            )
            {
                if (EntityManager.TryGetComponent(companyEntity, out CompanyData companyData))
                {
                    Entity brand = companyData.m_Brand;
                    if (!brand.Equals(Entity.Null))
                    {
                        bldgBrandInfo.BrandList = DataRetriever.brandDataInfos.ToArray();
                        bldgBrandInfo.BrandName = nameSystem.GetRenderedLabelName(brand);
                        bldgBrandInfo.CompanyName = nameSystem
                            .GetRenderedLabelName(companyEntity)
                            .Replace("Assets.NAME[", "")
                            .Replace("]", "");
                        bldgBrandInfo.HasBrand = true;
                        var brandInData = DataRetriever
                            .brandDataInfos.Where(v => v.Entity == brand)
                            ?.First();
                        bldgBrandInfo.BrandIcon = brandInData?.Icon ?? "";
                    }
                }
            }
        }

        public void CheckStorage()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgStorageInfo.HasStorage = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<StorageLimitData>(prefabRef.m_Prefab)
                && TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out StorageLimitData storageLimitData
                )
                && storageLimitData.m_Limit > 0
                && EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out StorageCompanyData storageCompanyData
                )
            )
            {
                Resource rs = storageCompanyData.m_StoredResources;
                if (rs != Resource.Money)
                {
                    bldgStorageInfo.HasStorage = true;
                    UpdateUIResourceBinding(rs);
                }
            }
        }

        public void CheckWorkplace()
        {
            bldgZoningInfo.HasWorkplace = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<WorkProvider>(selectedEntity)
            )
            {
                bldgZoningInfo.HasWorkplace = true;
                UpdateUIWorkplaceBinding();
            }
        }

        public void CheckUtility()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgUtilityInfo.IsWaterPump = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<Game.Buildings.WaterPumpingStation>(selectedEntity)
                && EntityManager.HasComponent<WaterPumpingStationData>(prefabRef.m_Prefab)
            )
            {
                bldgUtilityInfo.IsWaterPump = true;
            }

            bldgUtilityInfo.IsSewageOutlet = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<Game.Buildings.SewageOutlet>(selectedEntity)
                && EntityManager.HasComponent<SewageOutletData>(prefabRef.m_Prefab)
            )
            {
                bldgUtilityInfo.IsSewageOutlet = true;
            }

            bldgUtilityInfo.IsPowerPlant = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<ElectricityProducer>(selectedEntity)
                && EntityManager.HasComponent<PowerPlantData>(prefabRef.m_Prefab)
            )
            {
                bldgUtilityInfo.IsPowerPlant = true;
            }
            if (
                bldgUtilityInfo.IsWaterPump
                || bldgUtilityInfo.IsSewageOutlet
                || bldgUtilityInfo.IsPowerPlant
            )
                UpdateUIUtilityBinding();
        }

        public void CheckVehicle()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgVehicleInfo.IsDepot = false;
            if (EntityManager.HasComponent<TransportDepotData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsDepot = true;

            bldgVehicleInfo.IsGarbageFacility = false;
            if (EntityManager.HasComponent<GarbageFacilityData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsGarbageFacility = true;

            bldgVehicleInfo.IsHospital = false;
            if (EntityManager.HasComponent<HospitalData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsHospital = true;

            bldgVehicleInfo.IsDeathcare = false;
            if (EntityManager.HasComponent<DeathcareFacilityData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsDeathcare = true;

            if (
                bldgVehicleInfo.IsDepot
                || bldgVehicleInfo.IsGarbageFacility
                || bldgVehicleInfo.IsHospital
                || bldgVehicleInfo.IsDeathcare
            )
                UpdateUIVehicleBinding();
        }

        public void CheckGeneral()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            if (
                (
                    bldgUtilityInfo.IsWaterPump
                    || bldgUtilityInfo.IsSewageOutlet
                    || bldgUtilityInfo.IsPowerPlant
                )
                && EntityManager.HasComponent<Building>(selectedEntity)
                && EntityManager.HasComponent<Efficiency>(selectedEntity)
                //&& !EntityManager.HasComponent<Abandoned>(selectedEntity)
                //&& !EntityManager.HasComponent<Destroyed>(selectedEntity)
                && (
                    !CompanyUIUtils.HasCompany(
                        EntityManager,
                        selectedEntity,
                        prefabRef.m_Prefab,
                        out Entity entity
                    )
                    || entity != Entity.Null
                )
            )
            {
                if (
                    EntityManager.TryGetBuffer(
                        selectedEntity,
                        true,
                        out DynamicBuffer<Efficiency> efficiencyBuffer
                    )
                )
                    bldgGeneralInfo.Efficiency = (int)
                        Unity.Mathematics.math.round(
                            100f * BuildingUtils.GetEfficiency(efficiencyBuffer)
                        );
            }

            if (
                EntityManager.TryGetBuffer(
                    selectedEntity,
                    true,
                    out DynamicBuffer<SpawnLocationElement> spawnLocationElement
                )
            )
            {
                for (int i = 0; i < spawnLocationElement.Length; i++)
                {
                    var sl = spawnLocationElement[i];
                    if (
                        sl.m_Type == SpawnLocationType.SpawnLocation
                        && EntityManager.TryGetComponent(
                            sl.m_SpawnLocation,
                            out PrefabRef prefabData
                        )
                        && prefabData.m_Prefab == DataRetriever.integratedHelipad
                    )
                    {
                        bldgGeneralInfo.HasHeli = true;
                        break;
                    }
                }
            }
        }

        private void UpdateUILevelBinding()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out SpawnableBuildingData spawnableBuildingData
                )
                && EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out ConsumptionData consumptionData
                )
            )
            {
                bldgZoningInfo.Level = spawnableBuildingData.m_Level;
                bldgZoningInfo.Upkeep = consumptionData.m_Upkeep;
                //CurrentZoneName = prefabSystem.GetPrefabName(spawnableBuildingData.m_ZonePrefab);
            }
        }

        private void UpdateUIHouseholdBinding(int futureRent = -1)
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out BuildingPropertyData buildingPropertyData
                )
            )
                bldgZoningInfo.HasHousehold = true;
            else
            {
                bldgZoningInfo.HasHousehold = false;
                return;
            }

            if (futureRent != -1)
                bldgZoningInfo.Rent = futureRent;
            else
            {
                if (
                    EntityManager.TryGetComponent(prefabRef.m_Prefab, out BuildingData buildingData)
                    //&& EntityManager.TryGetComponent(selectedEntity, out PropertyRenter property)
                    && EntityManager.TryGetComponent(selectedEntity, out Building building)
                    && EntityManager.HasComponent<LandValue>(building.m_RoadEdge)
                    && EntityManager.TryGetComponent(
                        prefabRef.m_Prefab,
                        out SpawnableBuildingData spawnableBuildingData
                    )
                    && EntityManager.TryGetComponent(
                        spawnableBuildingData.m_ZonePrefab,
                        out ZoneData zoneData
                    )
                    && EntityManager.TryGetComponent(
                        spawnableBuildingData.m_ZonePrefab,
                        out ZonePropertiesData zonePropData
                    )
                )
                {
                    bldgZoningInfo.AreaType = zoneData.m_AreaType.ToString();
                    EconomyParameterData economyParameterData =
                        SystemAPI.GetSingleton<EconomyParameterData>();
                    var landValue = EntityManager
                        .GetComponentData<LandValue>(building.m_RoadEdge)
                        .m_LandValue;
                    bldgZoningInfo.Household = buildingPropertyData.m_ResidentialProperties;
                    (float totalRent, float props) = utils.CheckMaxRent(
                        buildingPropertyData,
                        bldgZoningInfo.Level,
                        buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                        landValue,
                        zoneData.m_AreaType,
                        ref economyParameterData,
                        zonePropData.m_IgnoreLandValue
                    );
                    bldgZoningInfo.Rent = Mathf.RoundToInt(totalRent / props);

                    var isMixed = PropertyUtils.IsMixedBuilding(buildingPropertyData);
                    var maxTextValue = isMixed
                        ? totalRent
                            * (1 - economyParameterData.m_MixedBuildingCompanyRentPercentage)
                        : totalRent;
                    bldgZoningInfo.MaxHousehold = (int)Math.Floor(Math.Min(999, maxTextValue));
                }
            }
        }

        private void UpdateUIResourceBinding(Resource rs = 0)
        {
            if (rs == 0)
            {
                if (
                    EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef)
                    && EntityManager.TryGetComponent(
                        prefabRef.m_Prefab,
                        out StorageCompanyData storageCompanyData
                    )
                )
                    rs = storageCompanyData.m_StoredResources;
            }
            List<ResourceDataInfo> list = new();
            List<ResourceDataInfo> list2 = new();
            foreach (var item in DataRetriever.resourceDataInfos)
            {
                if (item.Group == ResourceGroup.None || item.Group == ResourceGroup.Money)
                    continue;
                Resource flag = item.Resource;

                if ((rs & flag) == flag)
                    list.Add(new() { Id = (ulong)flag });

                list2.Add(item);
            }
            bldgStorageInfo.BuildingResources = list.ToArray();
            bldgStorageInfo.BuildingResourcesAll = list2.ToArray();
        }

        private void UpdateUIWorkplaceBinding()
        {
            if (EntityManager.TryGetComponent(selectedEntity, out WorkProvider workProvider))
            {
                bldgZoningInfo.CurrentMaxWorkplaceCount = workProvider.m_MaxWorkers;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_Workplace ABC_Workplace)
                    && !ABC_Workplace.IsDefault()
                )
                    bldgZoningInfo.OriginalMaxWorkplaceCount = ABC_Workplace.Workplace;
            }
        }

        private void UpdateUIUtilityBinding()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out WaterPumpingStationData waterPumpingStationData
                )
                && waterPumpingStationData.m_Capacity > 0
            )
            {
                bldgUtilityInfo.CurrentWaterPumpCap = waterPumpingStationData.m_Capacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_WaterPump ABC_WaterPump)
                    && !ABC_WaterPump.IsDefault()
                )
                    bldgUtilityInfo.OriginalWaterPumpCap = ABC_WaterPump.Original;
            }
            else
            {
                bldgUtilityInfo.IsWaterPump = false;
            }

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out SewageOutletData sewageOutletData
                )
                && sewageOutletData.m_Capacity > 0
            )
            {
                bldgUtilityInfo.CurrentSewageDumpCap = sewageOutletData.m_Capacity;
                bldgUtilityInfo.CurrentSewagePurification = sewageOutletData.m_Purification;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_SewageDump ABC_SewageDump)
                    && !ABC_SewageDump.IsDefault()
                )
                {
                    bldgUtilityInfo.OriginalSewageDumpCap = ABC_SewageDump.OriginalCap;
                    bldgUtilityInfo.OriginalSewagePurification =
                        ABC_SewageDump.OriginalPurification;
                }
            }
            else
                bldgUtilityInfo.IsSewageOutlet = false;

            if (
                EntityManager.TryGetComponent(prefabRef.m_Prefab, out PowerPlantData powerPlantData)
                && powerPlantData.m_ElectricityProduction > 0
            )
            {
                bldgUtilityInfo.CurrentPowerProdCap = powerPlantData.m_ElectricityProduction;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PowerPlant ABC_PowerPlant)
                    && !ABC_PowerPlant.IsDefault()
                )
                    bldgUtilityInfo.OriginalPowerProdCap = ABC_PowerPlant.Original;
            }
            else
                bldgUtilityInfo.IsPowerPlant = false;
        }

        private void UpdateUIVehicleBinding()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out TransportDepotData transportDepotData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out TransportDepotData transportDepotDataComb
                );

                bldgVehicleInfo.TransportType = transportDepotData.m_TransportType.ToString();
                bldgVehicleInfo.DepotVehicle.Current = transportDepotData.m_VehicleCapacity;
                bldgVehicleInfo.DepotVehicle.Combined = transportDepotDataComb.m_VehicleCapacity;

                if (
                    EntityManager.TryGetComponent(
                        selectedEntity,
                        out ABC_TransportDepot ABC_TransportDepot
                    ) && !ABC_TransportDepot.IsDefault()
                )
                    bldgVehicleInfo.DepotVehicle.Original = ABC_TransportDepot.Original;
            }
            else
                bldgVehicleInfo.IsDepot = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out GarbageFacilityData garbageFacilityData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out GarbageFacilityData garbageFacilityDataComb
                );

                bldgVehicleInfo.GarbageTruck.Current = garbageFacilityData.m_VehicleCapacity;
                bldgVehicleInfo.GarbageTruck.Combined = garbageFacilityDataComb.m_VehicleCapacity;
                if (
                    EntityManager.TryGetComponent(
                        selectedEntity,
                        out ABC_GarbageTruck ABC_GarbageTruck
                    ) && !ABC_GarbageTruck.IsDefault()
                )
                    bldgVehicleInfo.GarbageTruck.Original = ABC_GarbageTruck.Original;
            }
            else
                bldgVehicleInfo.IsGarbageFacility = false;

            if (EntityManager.TryGetComponent(prefabRef.m_Prefab, out HospitalData hospitalData))
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out HospitalData hospitalDataComb
                );

                bldgVehicleInfo.Ambulance.Current = hospitalData.m_AmbulanceCapacity;
                bldgVehicleInfo.Ambulance.Combined = hospitalDataComb.m_AmbulanceCapacity;

                bldgVehicleInfo.MediHeli.Current = hospitalData.m_MedicalHelicopterCapacity;
                bldgVehicleInfo.MediHeli.Combined = hospitalDataComb.m_MedicalHelicopterCapacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_Ambulance ABC_Ambulance)
                    && !ABC_Ambulance.IsDefault()
                )
                    bldgVehicleInfo.Ambulance.Original = ABC_Ambulance.Original;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_MediHeli ABC_MediHeli)
                    && !ABC_MediHeli.IsDefault()
                )
                    bldgVehicleInfo.MediHeli.Original = ABC_MediHeli.Original;
            }
            else
                bldgVehicleInfo.IsHospital = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out DeathcareFacilityData deathcareFacilityData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out DeathcareFacilityData deathcareFacilityDataComb
                );

                bldgVehicleInfo.Hearse.Current = deathcareFacilityData.m_HearseCapacity;
                bldgVehicleInfo.Hearse.Combined = deathcareFacilityDataComb.m_HearseCapacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_Hearse ABC_Hearse)
                    && !ABC_Hearse.IsDefault()
                )
                    bldgVehicleInfo.Hearse.Original = ABC_Hearse.Original;
            }
            else
                bldgVehicleInfo.IsDeathcare = false;
        }

        public void RandomizeStyle()
        {
            entityComponentChangerSystem.RandomizeStyle(selectedEntity);
            RequestUpdate();
        }

        public void SetBrand(string replaceBrand)
        {
            entityComponentChangerSystem.ChangeBrand(selectedEntity, replaceBrand);
            RequestUpdate();
        }

        public void ToggleResource(string resId)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                resId,
                ProcessType.Update,
                RefChangerSystem.ValueType.Storage
            );
            UpdateUIResourceBinding();
            RequestUpdate();
        }

        public void ResetStorage()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.Storage
            );
            UpdateUIResourceBinding();
            RequestUpdate();
        }

        public void ChangeLevel(int level)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                level.ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.Level
            );
            UpdateUILevelBinding();
            RequestUpdate();
        }

        public void ResetLevel()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.Level
            );
            UpdateUILevelBinding();
            RequestUpdate();
        }

        public void ChangeHousehold(int household)
        {
            int householdX = Math.Min(household, bldgZoningInfo.MaxHousehold);
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                householdX.ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.Household
            );
            UpdateUIHouseholdBinding();
            RequestUpdate();
        }

        public void ResetHousehold()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.Household
            );
            UpdateUIHouseholdBinding();
            RequestUpdate();
        }

        public void ChangeMaxWorkplace(int newMaxWorkplace)
        {
            entityComponentChangerSystem.ChangeMaxWorkplace(selectedEntity, newMaxWorkplace);
            RequestUpdate();
        }

        public void ResetMaxWorkplace()
        {
            entityComponentChangerSystem.ResetMaxWorkplace(selectedEntity);
            RequestUpdate();
        }

        public void ChangeWaterPumpCapacity(int newCapacity)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                newCapacity.ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.WaterPump
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ResetWaterPumpCapacity()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.WaterPump
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ChangeSewageDumpCapacity(int newCapacity)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                newCapacity.ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.SewageCap
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ResetSewageDumpCapacity()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.SewageCap
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ChangeSewageDumpPurification(int newCapacity)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                newCapacity.ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.SewagePurification
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ResetSewageDumpPurification()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.SewagePurification
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ChangePowerProdCapacity(int newCapacity)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                (newCapacity * 10000).ToString(),
                ProcessType.Update,
                RefChangerSystem.ValueType.PowerPlant
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ResetPowerProdCapacity()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                RefChangerSystem.ValueType.PowerPlant
            );
            UpdateUIUtilityBinding();
            RequestUpdate();
        }

        public void ChangeVehicleCapacity(
            int newCapacity,
            RefChangerSystem.ValueType vehicleChangeType
        )
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                newCapacity.ToString(),
                ProcessType.Update,
                vehicleChangeType
            );
            UpdateUIVehicleBinding();
            RequestUpdate();
        }

        public void ResetVehicleCapacity(RefChangerSystem.ValueType vehicleChangeType)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                vehicleChangeType
            );
            UpdateUIVehicleBinding();
            RequestUpdate();
        }

        //public void ChangeTransportDepotCapacity(int newCapacity)
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        newCapacity.ToString(),
        //        ProcessType.Update,
        //        RefChangerSystem.ValueType.Depot
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ResetTransportDepotCapacity()
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        string.Empty,
        //        ProcessType.Reset,
        //        RefChangerSystem.ValueType.Depot
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ChangeGarbageTruckCapacity(int newCapacity)
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        newCapacity.ToString(),
        //        ProcessType.Update,
        //        RefChangerSystem.ValueType.GarbageTruck
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ResetGarbageTruckCapacity()
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        string.Empty,
        //        ProcessType.Reset,
        //        RefChangerSystem.ValueType.GarbageTruck
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ChangeAmbulanceCapacity(int newCapacity)
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        newCapacity.ToString(),
        //        ProcessType.Update,
        //        RefChangerSystem.ValueType.Ambulance
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ResetAmbulanceCapacity()
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        string.Empty,
        //        ProcessType.Reset,
        //        RefChangerSystem.ValueType.Ambulance
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ChangeMediHeliCapacity(int newCapacity)
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        newCapacity.ToString(),
        //        ProcessType.Update,
        //        RefChangerSystem.ValueType.MediHeli
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}

        //public void ResetMediHeliCapacity()
        //{
        //    refChangerSystem.ReplaceEntity(
        //        selectedEntity,
        //        string.Empty,
        //        ProcessType.Reset,
        //        RefChangerSystem.ValueType.MediHeli
        //    );
        //    UpdateUIVehicleBinding();
        //    RequestUpdate();
        //}
    }
}
