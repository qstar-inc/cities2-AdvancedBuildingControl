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
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
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

        public static BldgBrandInfo bldgBrandInfo = new();
        public static BldgZoningInfo bldgZoningInfo = new();
        public static BldgStorageInfo bldgStorageInfo = new();

        //public static string CurrentBrandName { get; set; } = string.Empty;
        //public static string CurrentBrandIcon { get; set; } = string.Empty;
        //public static string CurrentCompanyName { get; set; } = string.Empty;

        //public static int CurrentLevel { get; set; } = 0;
        //public static int CurrentUpkeep { get; set; } = 0;

        //public static int CurrentHousehold { get; set; } = 0;
        //public static int CurrentRent { get; set; } = 0;

        //public static int MaxHousehold { get; set; } = 0;
        //public static string CurrentAreaType { get; set; } = string.Empty;
        //public static float CurrentSpaceMultiplier { get; set; } = 0;
        //public static float ZoneTypeBase { get; set; } = 0;
        //public static float TotalRent { get; set; } = 0;
        //public static float PropertiesCount { get; set; } = 0;
        //public static float MixedPercent { get; set; } = 0;
        //public static float LandValueBase { get; set; } = 0;
        //public static float LandValueModifier { get; set; } = 0;
        //public static bool IgnoreLandValue { get; set; } = false;
        //public static int LotSize { get; set; } = 0;
        //public static bool IsMixed { get; set; } = false;

        //public static string CurrentZoneName { get; set; } = string.Empty;

        //public static string CurrentVariant { get; set; } = string.Empty;

        //public static ResourceDataInfo[] BuildingResources { get; set; } = new ResourceDataInfo[0];
        //public static ResourceDataInfo[] BuildingResourcesAll { get; set; } =
        //    new ResourceDataInfo[0];

        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;

        //private StorageChangerSystem storageChangerSystem;
        //private LevelChangerSystem levelChangerSystem;
        //private HouseholdChangerSystem householdChangerSystem;
        private RefChangerSystem refChangerSystem;
        private Utils utils;

#nullable enable

        private Entity companyEntity = Entity.Null;

        //private bool hasBrand = false;

        //private bool hasLevel = false;
        //private bool hasHousehold = false;
        //private bool hasStorage = false;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem = Mod.world.GetOrCreateSystemManaged<NameSystem>();
            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();

            //storageChangerSystem = Mod.world.GetOrCreateSystemManaged<StorageChangerSystem>();
            //levelChangerSystem = Mod.world.GetOrCreateSystemManaged<LevelChangerSystem>();
            //householdChangerSystem = Mod.world.GetOrCreateSystemManaged<HouseholdChangerSystem>();
            refChangerSystem = Mod.world.GetOrCreateSystemManaged<RefChangerSystem>();
            utils = Mod.world.GetOrCreateSystemManaged<Utils>();

            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger<string>("ToggleResource", ToggleResource);
            CreateTrigger("ResetStorage", ResetStorage);
            CreateTrigger<int>("ChangeLevel", ChangeLevel);
            CreateTrigger("ResetLevel", ResetLevel);
            CreateTrigger<int>("ChangeHousehold", ChangeHousehold);
            CreateTrigger("ResetHousehold", ResetHousehold);

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            visible = Visible();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            //if (bldgZoningInfo.HasLevel || bldgZoningInfo.HasHousehold)
            //{
            writer.PropertyName("bldgZoningInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgZoningInfo);
            //}
            //if (bldgBrandInfo.HasBrand)
            //{
            writer.PropertyName("bldgBrandInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgBrandInfo);
            //}
            //if (bldgStorageInfo.HasStorage)
            //{
            writer.PropertyName("bldgStorageInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgStorageInfo);
            //}

            //Mod.log.Info("start");
            //Mod.log.Info(hasLevel);
            //Mod.log.Info(CurrentLevel);
            //Mod.log.Info(CurrentUpkeep);
            //Mod.log.Info(CurrentZoneName);
            //Mod.log.Info(DataRetriever.zoneDataInfos.Count);
            //Mod.log.Info(CurrentVariant);
            //Mod.log.Info(hasBrand);
            //Mod.log.Info(CurrentBrandName);
            //Mod.log.Info(CurrentBrandIcon);
            //Mod.log.Info(CurrentCompanyName);
            //Mod.log.Info(DataRetriever.brandDataInfos.Count);
            //Mod.log.Info(hasStorage);
            //Mod.log.Info(BuildingResources.Length);
            //Mod.log.Info(DataRetriever.resourceDataInfos.Count);
            //Mod.log.Info("end");

            //writer.PropertyName("h_level");
            //writer.Write(hasLevel);

            //writer.PropertyName("w_level");
            //writer.Write(CurrentLevel);

            //writer.PropertyName("w_upkeep");
            //writer.Write(CurrentUpkeep);

            //writer.PropertyName("h_household");
            //writer.Write(hasHousehold);

            //writer.PropertyName("w_household");
            //writer.Write(CurrentHousehold);

            //writer.PropertyName("w_rent");
            //writer.Write(CurrentRent);

            //writer.PropertyName("w_maxhousehold");
            //writer.Write(MaxHousehold);

            //writer.PropertyName("w_areatype");
            //writer.Write(CurrentAreaType);

            //writer.PropertyName("w_spacemult");
            //writer.Write(CurrentSpaceMultiplier);

            //writer.PropertyName("w_zonetypebase");
            //writer.Write(ZoneTypeBase);

            //writer.PropertyName("w_landvaluemodifier");
            //writer.Write(LandValueModifier);

            //writer.PropertyName("w_ignorelandvalue");
            //writer.Write(IgnoreLandValue);

            //writer.PropertyName("w_lotsize");
            //writer.Write(LotSize);

            //writer.PropertyName("w_landvaluebase");
            //writer.Write(LandValueBase);

            //writer.PropertyName("w_totalrent");
            //writer.Write(TotalRent);

            //writer.PropertyName("w_propertiescount");
            //writer.Write(PropertiesCount);

            //writer.PropertyName("w_mixedpercent");
            //writer.Write(MixedPercent);

            //writer.PropertyName("w_ismixed");
            //writer.Write(IsMixed);

            ////writer.PropertyName("w_zone");
            ////writer.Write(CurrentZoneName);

            ////writer.PropertyName("w_zonelist");
            ////ZoneDataInfoJsonWriterExtensions.Write(writer, DataRetriever.zoneDataInfos.ToArray());

            ////writer.PropertyName("w_variant");
            ////writer.Write(CurrentVariant);

            //writer.PropertyName("h_brand");
            //writer.Write(hasBrand);

            //writer.PropertyName("w_brand");
            //writer.Write(CurrentBrandName);

            //writer.PropertyName("w_brandicon");
            //writer.Write(CurrentBrandIcon);

            //writer.PropertyName("w_company");
            //writer.Write(CurrentCompanyName);

            //writer.PropertyName("w_brandlist");
            //BrandDataInfoJsonWriterExtensions.Write(writer, DataRetriever.brandDataInfos.ToArray());

            //writer.PropertyName("h_storage");
            //writer.Write(hasStorage);

            //writer.PropertyName("w_resources");
            //writer.Write(BuildingResources);

            //writer.PropertyName("w_resourceslist");
            //writer.Write(BuildingResourcesAll);
        }

        protected override void Reset()
        {
            bldgZoningInfo = new();
            bldgStorageInfo = new();
            bldgBrandInfo = new();

            //CurrentBrandName = string.Empty;
            //CurrentBrandIcon = string.Empty;
            //CurrentCompanyName = string.Empty;

            ////CurrentLevel = 0;
            ////CurrentUpkeep = 0;
            ////CurrentHousehold = 0;
            ////CurrentRent = 0;
            ////MaxHousehold = 0;
            //CurrentSpaceMultiplier = 0;
            //CurrentAreaType = string.Empty;

            //ZoneTypeBase = 0;
            //LandValueModifier = 0;
            //IgnoreLandValue = false;
            //LotSize = 0;
            //LandValueBase = 0;
            //TotalRent = 0;
            //PropertiesCount = 0;
            //MixedPercent = 0;
            //IsMixed = false;
            ////CurrentZoneName = string.Empty;

            ////CurrentVariant = string.Empty;

            //BuildingResources = new ResourceDataInfo[0];
            //BuildingResourcesAll = new ResourceDataInfo[0];

            //hasBrand = false;
            ////hasLevel = false;
            ////hasHousehold = false;
            //hasStorage = false;
            companyEntity = Entity.Null;
        }

        private bool Visible()
        {
            bool isVisible = false;
            if (EntityManager.TryGetComponent(selectedEntity, out PrefabRef _))
            {
                if (!prefabSystem.TryGetPrefab(selectedPrefab, out PrefabBase _))
                    return false;

                if (EntityManager.TryGetComponent(selectedPrefab, out BuildingData _))
                    isVisible = true;

                if (!isVisible)
                    return false;
                return true;
            }
            return false;
        }

        protected override void OnProcess()
        {
            CheckLevel();
            CheckHousehold();
            CheckBrand();
            CheckStorage();
        }

        public void CheckLevel()
        {
            bldgZoningInfo.HasLevel = false;
            if (
                !EntityManager.HasComponent<SignatureBuildingData>(selectedPrefab)
                && !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(selectedPrefab)
                && EntityManager.HasComponent<SpawnableBuildingData>(selectedPrefab)
            )
            {
                bldgZoningInfo.HasLevel = true;
                UpdateUILevelBinding();
            }
        }

        public void CheckHousehold()
        {
            bldgZoningInfo.HasHousehold = false;
            if (
                !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(selectedPrefab)
            )
            {
                if (
                    EntityManager.TryGetComponent(selectedPrefab, out BuildingPropertyData bpd)
                    && bpd.m_ResidentialProperties > 0
                )
                {
                    bldgZoningInfo.HasHousehold = true;
                }
                UpdateUIHouseholdBinding();
            }
        }

        public void CheckBrand()
        {
            bldgBrandInfo.HasBrand = false;
            if (
                CompanyUIUtils.HasCompany(
                    EntityManager,
                    selectedEntity,
                    selectedPrefab,
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
                            .First();
                        bldgBrandInfo.BrandIcon = brandInData.Icon;
                    }
                }
            }
        }

        public void CheckStorage()
        {
            bldgStorageInfo.HasStorage = false;
            if (
                EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef)
                && EntityManager.TryGetComponent(prefabRef.m_Prefab, out StorageLimitData _)
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

        private void UpdateUILevelBinding()
        {
            if (
                EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef)
                && EntityManager.TryGetComponent(
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
            if (
                EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef)
                && EntityManager.TryGetComponent(prefabRef.m_Prefab, out BuildingPropertyData bpd)
            )
            {
                bldgZoningInfo.HasHousehold = true;
            }
            else
            {
                bldgZoningInfo.HasHousehold = false;
                return;
            }

            if (futureRent != -1)
            {
                bldgZoningInfo.Rent = futureRent;
            }
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
                    bldgZoningInfo.Rent = PropertyUtils.GetRentPricePerRenter(
                        bpd,
                        bldgZoningInfo.Level,
                        buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                        landValue,
                        zoneData.m_AreaType,
                        ref economyParameterData,
                        zonePropData.m_IgnoreLandValue
                    );
                    bldgZoningInfo.Household = bpd.m_ResidentialProperties;
                    bldgZoningInfo.SpaceMultiplier = bpd.m_SpaceMultiplier;
                    switch (zoneData.m_AreaType)
                    {
                        case Game.Zones.AreaType.Residential:
                            bldgZoningInfo.ZoneTypeBase = economyParameterData
                                .m_RentPriceBuildingZoneTypeBase
                                .x;
                            bldgZoningInfo.LandValueModifier = economyParameterData
                                .m_LandValueModifier
                                .x;
                            break;
                        case Game.Zones.AreaType.Commercial:
                            bldgZoningInfo.ZoneTypeBase = economyParameterData
                                .m_RentPriceBuildingZoneTypeBase
                                .y;
                            bldgZoningInfo.LandValueModifier = economyParameterData
                                .m_LandValueModifier
                                .y;
                            break;
                        case Game.Zones.AreaType.Industrial:
                            bldgZoningInfo.ZoneTypeBase = economyParameterData
                                .m_RentPriceBuildingZoneTypeBase
                                .z;
                            bldgZoningInfo.LandValueModifier = economyParameterData
                                .m_LandValueModifier
                                .z;
                            break;
                    }
                    bldgZoningInfo.IgnoreLandValue = zonePropData.m_IgnoreLandValue;
                    (float a, float b) = utils.CheckMaxRent(
                        bpd,
                        bldgZoningInfo.Level,
                        buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                        landValue,
                        zoneData.m_AreaType,
                        ref economyParameterData,
                        zonePropData.m_IgnoreLandValue
                    );
                    bldgZoningInfo.LandValueBase = landValue;
                    bldgZoningInfo.LotSize = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
                    bldgZoningInfo.TotalRent = a;
                    bldgZoningInfo.PropertiesCount = b;
                    bldgZoningInfo.MixedPercent =
                        economyParameterData.m_MixedBuildingCompanyRentPercentage;
                    bldgZoningInfo.IsMixed = PropertyUtils.IsMixedBuilding(bpd);

                    //BuildingPropertyData bpdX;
                    //if (
                    //    EntityManager.TryGetComponent(
                    //        selectedEntity,
                    //        out OriginalEntity originalEntity
                    //    )
                    //    && prefabSystem.TryGetPrefab(
                    //        new PrefabID("BuildingPrefab", originalEntity.OGEntity.ToString()),
                    //        out PrefabBase prefabBase
                    //    )
                    //    && prefabSystem.TryGetEntity(prefabBase, out Entity ogEntity)
                    //    && EntityManager.TryGetComponent(ogEntity, out BuildingPropertyData bpd2)
                    //)
                    //{
                    //    bpdX = bpd2;
                    //}
                    //else if (
                    //    EntityManager.TryGetComponent(selectedPrefab, out PrefabData prefaData)
                    //    && prefabSystem.TryGetPrefab(prefaData, out PrefabBase prefabBase2)
                    //    && prefabSystem.TryGetEntity(prefabBase2, out Entity ogEntity2)
                    //    && EntityManager.TryGetComponent(ogEntity2, out BuildingPropertyData bpd3)
                    //)
                    //{
                    //    bpdX = bpd3;
                    //}
                    //else
                    //{
                    //    return;
                    //}
                    //(float rent, float propertiesCount) = utils.CheckMaxRent(
                    //    bpdX,
                    //    CurrentLevel,
                    //    buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                    //    landValue,
                    //    zoneData.m_AreaType,
                    //    ref economyParameterData,
                    //    zonePropData.m_IgnoreLandValue
                    //);
                    //HouseholdText = $"Total Rent ${rent} for {propertiesCount} properties";
                    var maxTextValue = bldgZoningInfo.IsMixed
                        ? bldgZoningInfo.TotalRent * (1 - bldgZoningInfo.MixedPercent)
                        : bldgZoningInfo.TotalRent;
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
                //Mod.log.Info($"Now testing {item.DisplayName} ({item.Id}), type {item.Group}");
                if (item.Group == ResourceGroup.None || item.Group == ResourceGroup.Money)
                    continue;
                Resource flag = item.Resource;

                if ((rs & flag) == flag)
                {
                    list.Add(new() { Id = (ulong)flag });
                }
                list2.Add(item);
            }
            bldgStorageInfo.BuildingResources = list.ToArray();
            bldgStorageInfo.BuildingResourcesAll = list2.ToArray();
        }

        public void SetDirty()
        {
            m_Dirty = true;
        }

        public void RandomizeStyle()
        {
            RandomizeStyleSystem.RandomizeStyle(EntityManager, selectedEntity);
            SetDirty();
        }

        public void SetBrand(string replaceBrand)
        {
            BrandChangerSystem.ChangeBrand(EntityManager, selectedEntity, replaceBrand);
            SetDirty();
        }

        public void ToggleResource(string resId)
        {
            // ----
            //ulong resIdX = ulong.Parse(resId);

            refChangerSystem.ReplaceEntity(
                selectedEntity,
                resId,
                ProcessMode.Update,
                RefChangerSystem.ValueType.Storage
            );
            UpdateUIResourceBinding();
            //Resource resTempX = storageChangerSystem.ReplaceEntity(
            //    selectedEntity,
            //    ProcessMode.Update,
            //    (Resource)resIdX
            //);
            //UpdateUIResourceBinding(resTempX);
            SetDirty();
        }

        public void ResetStorage()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessMode.Reset,
                RefChangerSystem.ValueType.Storage
            );
            UpdateUIResourceBinding();
            //Resource resTempX = storageChangerSystem.ReplaceEntity(
            //    selectedEntity,
            //    ProcessMode.Reset
            //);
            //UpdateUIResourceBinding(resTempX);
            SetDirty();
        }

        public void ChangeLevel(int level)
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                level.ToString(),
                ProcessMode.Update,
                RefChangerSystem.ValueType.Level
            );
            UpdateUILevelBinding();
            SetDirty();
        }

        public void ResetLevel()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessMode.Reset,
                RefChangerSystem.ValueType.Level
            );
            UpdateUILevelBinding();
            SetDirty();
        }

        public void ChangeHousehold(int household)
        {
            int householdX = Math.Min(household, bldgZoningInfo.MaxHousehold);
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                householdX.ToString(),
                ProcessMode.Update,
                RefChangerSystem.ValueType.Household
            );
            UpdateUIHouseholdBinding();
            SetDirty();
        }

        public void ResetHousehold()
        {
            refChangerSystem.ReplaceEntity(
                selectedEntity,
                string.Empty,
                ProcessMode.Reset,
                RefChangerSystem.ValueType.Household
            );
            UpdateUIHouseholdBinding();
            SetDirty();
        }
    }
}
