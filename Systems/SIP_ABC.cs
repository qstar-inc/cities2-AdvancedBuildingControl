using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Components.Vehicles;
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
        public static BldgPropertiesInfo bldgPropertiesInfo = new();
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
            //CreateTrigger<string>("ToggleResource", ToggleResource);
            //CreateTrigger("ResetStorage", ResetStorage);
            //CreateTrigger<int>("ChangeLevel", ChangeLevel);
            //CreateTrigger("ResetLevel", ResetLevel);
            //CreateTrigger<int>("ChangeHousehold", ChangeHousehold);
            //CreateTrigger("ResetHousehold", ResetHousehold);
            CreateTrigger<int>("ChangeMaxWorkplace", ChangeMaxWorkplace);
            CreateTrigger("ResetMaxWorkplace", ResetMaxWorkplace);
            //CreateTrigger<int>("ChangeWaterPumpCapacity", ChangeWaterPumpCapacity);
            //CreateTrigger("ResetWaterPumpCapacity", ResetWaterPumpCapacity);
            //CreateTrigger<int>("ChangeSewageDumpCapacity", ChangeSewageDumpCapacity);
            //CreateTrigger("ResetSewageDumpCapacity", ResetSewageDumpCapacity);
            //CreateTrigger<int>("ChangeSewageDumpPurification", ChangeSewageDumpPurification);
            //CreateTrigger("ResetSewageDumpPurification", ResetSewageDumpPurification);
            //CreateTrigger<int>("ChangePowerProdCapacity", ChangePowerProdCapacity);
            //CreateTrigger("ResetPowerProdCapacity", ResetPowerProdCapacity);
            CreateTrigger<string, RefChangerSystem.UpdateValueType>("ChangeValue", ChangeValue);
            CreateTrigger<RefChangerSystem.UpdateValueType>("ResetValue", ResetValue);

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

            writer.PropertyName("bldgPropertiesInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgPropertiesInfo);

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
            bldgPropertiesInfo = new();
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
            bldgPropertiesInfo.HasLevel = false;
            if (
                !EntityManager.HasComponent<SignatureBuildingData>(prefabRef.m_Prefab)
                //&& !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(prefabRef.m_Prefab)
                && EntityManager.HasComponent<SpawnableBuildingData>(prefabRef.m_Prefab)
            )
            {
                bldgPropertiesInfo.HasLevel = true;
                UpdateUILevelBinding();
            }
        }

        public void CheckHousehold()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgPropertiesInfo.HasHousehold = false;
            if (EntityManager.HasComponent<BuildingData>(prefabRef.m_Prefab))
            {
                if (
                    EntityManager.TryGetComponent(
                        prefabRef.m_Prefab,
                        out BuildingPropertyData buildingPropertyData
                    )
                    && buildingPropertyData.m_ResidentialProperties > 0
                )
                {
                    bldgPropertiesInfo.HasHousehold = true;
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
            bldgPropertiesInfo.IsWorkplace = false;
            if (
                //EntityManager.HasComponent<Building>(selectedEntity)
                //&&
                EntityManager.HasComponent<WorkProvider>(selectedEntity)
            )
            {
                bldgPropertiesInfo.IsWorkplace = true;
                UpdateUIWorkplaceBinding();
            }
        }

        public void CheckUtility()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
                return;
            bldgUtilityInfo.IsWaterPump = false;
            if (
                EntityManager.HasComponent<Game.Buildings.WaterPumpingStation>(selectedEntity)
                && EntityManager.HasComponent<WaterPumpingStationData>(prefabRef.m_Prefab)
            )
            {
                bldgUtilityInfo.IsWaterPump = true;
            }

            bldgUtilityInfo.IsSewageOutlet = false;
            if (
                EntityManager.HasComponent<Game.Buildings.SewageOutlet>(selectedEntity)
                && EntityManager.HasComponent<SewageOutletData>(prefabRef.m_Prefab)
            )
            {
                bldgUtilityInfo.IsSewageOutlet = true;
            }

            bldgUtilityInfo.IsPowerPlant = false;
            if (
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

            if (EntityManager.HasComponent<TransportDepotData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsDepot = true;

            if (EntityManager.HasComponent<GarbageFacilityData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsGarbageFacility = true;

            if (EntityManager.HasComponent<HospitalData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsHospital = true;

            if (EntityManager.HasComponent<DeathcareFacilityData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsDeathcare = true;

            if (EntityManager.HasComponent<PoliceStationData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsPoliceStation = true;

            if (EntityManager.HasComponent<PrisonData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsPrison = true;
            if (EntityManager.HasComponent<FireStationData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsFireStation = true;
            if (EntityManager.HasComponent<EmergencyShelterData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsEmergencyShelter = true;
            if (EntityManager.HasComponent<PostFacilityData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsPostFacility = true;
            if (EntityManager.HasComponent<MaintenanceDepotData>(prefabRef.m_Prefab))
                bldgVehicleInfo.IsMaintenanceDepot = true;

            if (
                bldgVehicleInfo.IsDepot
                || bldgVehicleInfo.IsGarbageFacility
                || bldgVehicleInfo.IsHospital
                || bldgVehicleInfo.IsDeathcare
                || bldgVehicleInfo.IsPoliceStation
                || bldgVehicleInfo.IsPrison
                || bldgVehicleInfo.IsFireStation
                || bldgVehicleInfo.IsEmergencyShelter
                || bldgVehicleInfo.IsPostFacility
                || bldgVehicleInfo.IsMaintenanceDepot
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
                EntityManager.TryGetComponent(selectedEntity, out PostFacilityData postFacilityData)
            )
            {
                bldgGeneralInfo.HasPostVan = postFacilityData.m_PostVanCapacity > 0;
                bldgGeneralInfo.HasPostTruck = postFacilityData.m_PostTruckCapacity > 0;
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
                bldgPropertiesInfo.Level.Current = spawnableBuildingData.m_Level;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_Level ABC_Level)
                    && !ABC_Level.IsDefault()
                )
                    bldgPropertiesInfo.Level.Original = ABC_Level.Modified;
                bldgPropertiesInfo.Upkeep = consumptionData.m_Upkeep;
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
                bldgPropertiesInfo.HasHousehold = true;
            else
            {
                bldgPropertiesInfo.HasHousehold = false;
                return;
            }

            if (futureRent != -1)
                bldgPropertiesInfo.Rent = futureRent;
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
                    bldgPropertiesInfo.AreaType = zoneData.m_AreaType.ToString();
                    EconomyParameterData economyParameterData =
                        SystemAPI.GetSingleton<EconomyParameterData>();
                    var landValue = EntityManager
                        .GetComponentData<LandValue>(building.m_RoadEdge)
                        .m_LandValue;
                    bldgPropertiesInfo.Household.Current =
                        buildingPropertyData.m_ResidentialProperties;
                    (float totalRent, float props) = utils.CheckMaxRent(
                        buildingPropertyData,
                        bldgPropertiesInfo.Level.Current,
                        buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                        landValue,
                        zoneData.m_AreaType,
                        ref economyParameterData,
                        zonePropData.m_IgnoreLandValue
                    );
                    bldgPropertiesInfo.Rent = Mathf.RoundToInt(totalRent / props);

                    var isMixed = PropertyUtils.IsMixedBuilding(buildingPropertyData);
                    var maxTextValue = isMixed
                        ? totalRent
                            * (1 - economyParameterData.m_MixedBuildingCompanyRentPercentage)
                        : totalRent;
                    bldgPropertiesInfo.MaxHousehold = (int)Math.Floor(Math.Min(999, maxTextValue));
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
                bldgPropertiesInfo.Workplace.Current = workProvider.m_MaxWorkers;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_Workplace ABC_Workplace)
                    && !ABC_Workplace.IsDefault()
                )
                    bldgPropertiesInfo.Workplace.Original = ABC_Workplace.Modified;
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
                    bldgUtilityInfo.OriginalSewageDumpCap = ABC_SewageDump.Original;

                if (
                    EntityManager.TryGetComponent(
                        selectedEntity,
                        out ABC_SewagePurification ABC_SewagePurification
                    ) && !ABC_SewagePurification.IsDefault()
                )
                    bldgUtilityInfo.OriginalSewagePurification = ABC_SewagePurification.Original;
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
                        out ABC_DepotVehicle ABC_DepotVehicle
                    ) && !ABC_DepotVehicle.IsDefault()
                )
                {
                    bldgVehicleInfo.DepotVehicle.Original = ABC_DepotVehicle.Original;
                    bldgVehicleInfo.DepotVehicle.Enabled = ABC_DepotVehicle.Enabled;
                }
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
                {
                    bldgVehicleInfo.GarbageTruck.Original = ABC_GarbageTruck.Original;
                    bldgVehicleInfo.GarbageTruck.Enabled = ABC_GarbageTruck.Enabled;
                }
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
                {
                    bldgVehicleInfo.Ambulance.Original = ABC_Ambulance.Original;
                    bldgVehicleInfo.Ambulance.Enabled = ABC_Ambulance.Enabled;
                }
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_MediHeli ABC_MediHeli)
                    && !ABC_MediHeli.IsDefault()
                )
                {
                    bldgVehicleInfo.MediHeli.Original = ABC_MediHeli.Original;
                    bldgVehicleInfo.MediHeli.Enabled = ABC_MediHeli.Enabled;
                }
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
                {
                    bldgVehicleInfo.Hearse.Original = ABC_Hearse.Original;
                    bldgVehicleInfo.Hearse.Enabled = ABC_Hearse.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsDeathcare = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out PoliceStationData policeStationData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out PoliceStationData policeStationDataComb
                );

                bldgVehicleInfo.PatrolCar.Current = policeStationData.m_PatrolCarCapacity;
                bldgVehicleInfo.PatrolCar.Combined = policeStationDataComb.m_PatrolCarCapacity;

                bldgVehicleInfo.PoliceHeli.Current = policeStationData.m_PoliceHelicopterCapacity;
                bldgVehicleInfo.PoliceHeli.Combined =
                    policeStationDataComb.m_PoliceHelicopterCapacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PatrolCar ABC_PatrolCar)
                    && !ABC_PatrolCar.IsDefault()
                )
                {
                    bldgVehicleInfo.PatrolCar.Original = ABC_PatrolCar.Original;
                    bldgVehicleInfo.PatrolCar.Enabled = ABC_PatrolCar.Enabled;
                }
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PoliceHeli ABC_PoliceHeli)
                    && !ABC_PoliceHeli.IsDefault()
                )
                {
                    bldgVehicleInfo.PoliceHeli.Original = ABC_PoliceHeli.Original;
                    bldgVehicleInfo.PoliceHeli.Enabled = ABC_PoliceHeli.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsPoliceStation = false;

            if (EntityManager.TryGetComponent(prefabRef.m_Prefab, out PrisonData prisonData))
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out PrisonData policeStationDataComb
                );

                bldgVehicleInfo.PrisonVan.Current = prisonData.m_PrisonVanCapacity;
                bldgVehicleInfo.PrisonVan.Combined = policeStationDataComb.m_PrisonVanCapacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PrisonVan ABC_PrisonVan)
                    && !ABC_PrisonVan.IsDefault()
                )
                {
                    bldgVehicleInfo.PrisonVan.Original = ABC_PrisonVan.Original;
                    bldgVehicleInfo.PrisonVan.Enabled = ABC_PrisonVan.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsPrison = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out FireStationData fireStationData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out FireStationData fireStationDataComb
                );

                bldgVehicleInfo.FireTruck.Current = fireStationData.m_FireEngineCapacity;
                bldgVehicleInfo.FireTruck.Combined = fireStationDataComb.m_FireEngineCapacity;

                bldgVehicleInfo.FireHeli.Current = fireStationData.m_FireHelicopterCapacity;
                bldgVehicleInfo.FireHeli.Combined = fireStationDataComb.m_FireHelicopterCapacity;

                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_FireTruck ABC_FireTruck)
                    && !ABC_FireTruck.IsDefault()
                )
                {
                    bldgVehicleInfo.FireTruck.Original = ABC_FireTruck.Original;
                    bldgVehicleInfo.FireTruck.Enabled = ABC_FireTruck.Enabled;
                }
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_FireHeli ABC_FireHeli)
                    && !ABC_FireHeli.IsDefault()
                )
                {
                    bldgVehicleInfo.FireHeli.Original = ABC_FireHeli.Original;
                    bldgVehicleInfo.FireHeli.Enabled = ABC_FireHeli.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsFireStation = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out EmergencyShelterData emergencyShelterData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out EmergencyShelterData emergencyShelterDataComb
                );
                bldgVehicleInfo.EvacBus.Current = emergencyShelterData.m_VehicleCapacity;
                bldgVehicleInfo.EvacBus.Combined = emergencyShelterDataComb.m_VehicleCapacity;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_EvacBus ABC_EvacBus)
                    && !ABC_EvacBus.IsDefault()
                )
                {
                    bldgVehicleInfo.EvacBus.Original = ABC_EvacBus.Original;
                    bldgVehicleInfo.EvacBus.Enabled = ABC_EvacBus.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsEmergencyShelter = false;

            if (
                EntityManager.TryGetComponent(prefabRef.m_Prefab, out PostFacilityData postFacility)
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out PostFacilityData postFacilityComb
                );
                bldgVehicleInfo.PostTruck.Current = postFacility.m_PostTruckCapacity;
                bldgVehicleInfo.PostTruck.Combined = postFacilityComb.m_PostTruckCapacity;

                bldgVehicleInfo.PostVan.Current = postFacility.m_PostVanCapacity;
                bldgVehicleInfo.PostVan.Combined = postFacilityComb.m_PostVanCapacity;
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PostTruck ABC_PostTruck)
                    && !ABC_PostTruck.IsDefault()
                )
                {
                    bldgVehicleInfo.PostTruck.Original = ABC_PostTruck.Original;
                    bldgVehicleInfo.PostTruck.Enabled = ABC_PostTruck.Enabled;
                }
                if (
                    EntityManager.TryGetComponent(selectedEntity, out ABC_PostVan ABC_PostVan)
                    && !ABC_PostVan.IsDefault()
                )
                {
                    bldgVehicleInfo.PostVan.Original = ABC_PostVan.Original;
                    bldgVehicleInfo.PostVan.Enabled = ABC_PostVan.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsPostFacility = false;

            if (
                EntityManager.TryGetComponent(
                    prefabRef.m_Prefab,
                    out MaintenanceDepotData maintenanceDepotData
                )
            )
            {
                TryGetComponentWithUpgrades(
                    selectedEntity,
                    prefabRef.m_Prefab,
                    out MaintenanceDepotData maintenanceDepotDataComb
                );
                bldgVehicleInfo.MaintenanceVehicle.Current = maintenanceDepotData.m_VehicleCapacity;
                bldgVehicleInfo.MaintenanceVehicle.Combined =
                    maintenanceDepotDataComb.m_VehicleCapacity;
                if (
                    EntityManager.TryGetComponent(
                        selectedEntity,
                        out ABC_MaintenanceVehicle ABC_MaintenanceVehicle
                    ) && !ABC_MaintenanceVehicle.IsDefault()
                )
                {
                    bldgVehicleInfo.MaintenanceVehicle.Original = ABC_MaintenanceVehicle.Original;
                    bldgVehicleInfo.MaintenanceVehicle.Enabled = ABC_MaintenanceVehicle.Enabled;
                }
            }
            else
                bldgVehicleInfo.IsMaintenanceDepot = false;
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

        public void ChangeValue(string newCapacity, RefChangerSystem.UpdateValueType valueType)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                newCapacity,
                ProcessType.Update,
                valueType
            );
            UpdateBinding(valueType);
        }

        public void ResetValue(RefChangerSystem.UpdateValueType valueType)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessType.Reset,
                valueType
            );
            UpdateBinding(valueType);
        }

        public void UpdateBinding(RefChangerSystem.UpdateValueType valueType)
        {
            switch (valueType)
            {
                case RefChangerSystem.UpdateValueType.Storage:
                    UpdateUIResourceBinding();
                    break;
                case RefChangerSystem.UpdateValueType.Level:
                    UpdateUILevelBinding();
                    break;
                case RefChangerSystem.UpdateValueType.Household:
                    UpdateUIHouseholdBinding();
                    break;
                case RefChangerSystem.UpdateValueType.WaterPump:
                case RefChangerSystem.UpdateValueType.SewageCap:
                case RefChangerSystem.UpdateValueType.SewagePurification:
                case RefChangerSystem.UpdateValueType.PowerPlant:
                    UpdateUIUtilityBinding();
                    break;
                case RefChangerSystem.UpdateValueType.DepotVehicle:
                case RefChangerSystem.UpdateValueType.GarbageTruck:
                case RefChangerSystem.UpdateValueType.Ambulance:
                case RefChangerSystem.UpdateValueType.MediHeli:
                case RefChangerSystem.UpdateValueType.Hearse:
                case RefChangerSystem.UpdateValueType.PatrolCar:
                case RefChangerSystem.UpdateValueType.PoliceHeli:
                case RefChangerSystem.UpdateValueType.PrisonVan:
                case RefChangerSystem.UpdateValueType.FireTruck:
                case RefChangerSystem.UpdateValueType.FireHeli:
                case RefChangerSystem.UpdateValueType.EvacBus:
                case RefChangerSystem.UpdateValueType.PostVan:
                case RefChangerSystem.UpdateValueType.PostTruck:
                case RefChangerSystem.UpdateValueType.MaintenanceVehicle:
                    UpdateUIVehicleBinding();
                    break;
                default:
                    return;
            }
            RequestUpdate();
        }
    }
}
