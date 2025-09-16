using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Systems;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIPAdvancedBuildingControl : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return "SIPAdvancedBuildingControl"; }
        }

# nullable disable
        public static string CurrentBrandName { get; set; } = string.Empty;
        public static string CurrentBrandIcon { get; set; } = string.Empty;
        public static string CurrentCompanyName { get; set; } = string.Empty;

        public static int CurrentLevel { get; set; } = 0;
        public static int CurrentUpkeep { get; set; } = 0;
        public static string CurrentZoneName { get; set; } = string.Empty;

        public static string CurrentVariant { get; set; } = string.Empty;

        public static ResourceDataInfo[] BuildingResources { get; set; } = new ResourceDataInfo[0];
        public static ResourceDataInfo[] BuildingResourcesAll { get; set; } =
            new ResourceDataInfo[0];
        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;
        private StorageChangerSystem storageChangerSystem;

        private LevelChangerSystem levelChangerSystem;

#nullable enable

        private Entity companyEntity = Entity.Null;
        private bool isBrandDataSet = false;
        private bool hasBrand = false;
        private bool hasLevel = false;
        private bool hasStorage = false;

        private Resource BuildingResourcesLocal = Resource.NoResource;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<NameSystem>();
            prefabSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
            storageChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StorageChangerSystem>();
            levelChangerSystem = World.GetOrCreateSystemManaged<LevelChangerSystem>();
            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger<int>("ChangeLevel", ChangeLevel);
            CreateTrigger("CreateVariants", CreateVariants);
            CreateTrigger<string>("ToggleResource", ToggleResource);
            CreateTrigger("ResetStorage", ResetStorage);

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            visible = Visible();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
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

            writer.PropertyName("h_level");
            writer.Write(hasLevel);

            writer.PropertyName("w_level");
            writer.Write(CurrentLevel);

            writer.PropertyName("w_upkeep");
            writer.Write(CurrentUpkeep);

            writer.PropertyName("w_zone");
            writer.Write(CurrentZoneName);

            writer.PropertyName("w_zonelist");
            ZoneDataInfoJsonWriterExtensions.Write(writer, DataRetriever.zoneDataInfos.ToArray());

            writer.PropertyName("w_variant");
            writer.Write(CurrentVariant);

            writer.PropertyName("h_brand");
            writer.Write(hasBrand);

            writer.PropertyName("w_brand");
            writer.Write(CurrentBrandName);

            writer.PropertyName("w_brandicon");
            writer.Write(CurrentBrandIcon);

            writer.PropertyName("w_company");
            writer.Write(CurrentCompanyName);

            writer.PropertyName("w_brandlist");
            BrandDataInfoJsonWriterExtensions.Write(writer, DataRetriever.brandDataInfos.ToArray());

            writer.PropertyName("h_storage");
            writer.Write(hasStorage);

            writer.PropertyName("w_resources");
            writer.Write(BuildingResources);

            writer.PropertyName("w_resourceslist");
            writer.Write(BuildingResourcesAll);
        }

        protected override void Reset()
        {
            CurrentBrandName = string.Empty;
            CurrentBrandIcon = string.Empty;
            CurrentCompanyName = string.Empty;

            CurrentLevel = 0;
            CurrentUpkeep = 0;
            CurrentZoneName = string.Empty;

            CurrentVariant = string.Empty;

            BuildingResources = new ResourceDataInfo[0];
            BuildingResourcesAll = new ResourceDataInfo[0];

            companyEntity = Entity.Null;
            isBrandDataSet = false;
            hasBrand = false;
            hasLevel = false;
            hasStorage = false;

            BuildingResourcesLocal = Resource.NoResource;
        }

        private bool Visible()
        {
            bool isVisible = false;
            if (EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
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
            CheckLevels();
            CheckBrands();
            CheckStorage();
        }

        public void CheckLevels()
        {
            hasLevel = false;
            if (
                !EntityManager.HasComponent<SignatureBuildingData>(selectedPrefab)
                && !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<Renter>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(selectedPrefab)
                && EntityManager.HasComponent<SpawnableBuildingData>(selectedPrefab)
                && EntityManager.TryGetComponent(
                    selectedPrefab,
                    out SpawnableBuildingData spawnableBuildingData
                )
                && EntityManager.TryGetComponent(
                    selectedPrefab,
                    out ConsumptionData consumptionData
                )
            )
            {
                hasLevel = true;
                CurrentLevel = spawnableBuildingData.m_Level;
                CurrentUpkeep = consumptionData.m_Upkeep;
                CurrentZoneName = prefabSystem.GetPrefabName(spawnableBuildingData.m_ZonePrefab);
                CurrentVariant = prefabSystem.GetPrefabName(selectedPrefab);
                //CurrentVariant = levelSystem.UnPrefixify(
                //    prefabSystem.GetPrefabName(selectedPrefab)
                //);
            }
        }

        public void CheckBrands()
        {
            hasBrand = false;
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
                        CurrentBrandName = nameSystem.GetRenderedLabelName(brand);
                        CurrentCompanyName = nameSystem
                            .GetRenderedLabelName(companyEntity)
                            .Replace("Assets.NAME[", "")
                            .Replace("]", "");
                        hasBrand = true;
                        var brandInData = DataRetriever
                            .brandDataInfos.Where(v => v.Entity == brand)
                            .First();
                        CurrentBrandIcon = brandInData.Icon;
                    }
                }
            }
        }

        public void CheckStorage()
        {
            hasStorage = false;
            if (
                EntityManager.TryGetComponent(selectedPrefab, out StorageLimitData _)
                && EntityManager.TryGetComponent(
                    selectedPrefab,
                    out StorageCompanyData storageCompanyData
                )
            )
            {
                Resource rs = storageCompanyData.m_StoredResources;
                if (rs != Resource.Money)
                {
                    hasStorage = true;
                    UpdateUIResourceBinding(rs);
                }
            }
        }

        private void UpdateUIResourceBinding(Resource rs)
        {
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
            BuildingResources = list.ToArray();
            BuildingResourcesAll = list2.ToArray();
            BuildingResourcesLocal = rs;
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

        public void ChangeLevel(int level)
        {
            //levelSystem.SetLevel(
            //    EntityManager,
            //    prefabSystem,
            //    selectedEntity,
            //    selectedPrefab,
            //    level
            //);
            //SetDirty();
        }

        public void CreateVariants()
        {
            //prefabSystem.TryGetPrefab(selectedPrefab, out PrefabBase currentPrefabBase);
            //levelSystem.CreateVariants(
            //    EntityManager,
            //    prefabSystem,
            //    selectedEntity,
            //    currentPrefabBase
            //);
        }

        public void ToggleResource(string resId)
        {
            // ----
            ulong resIdX = ulong.Parse(resId);

            Resource resTempX = storageChangerSystem.ReplaceEntity(
                selectedEntity,
                StorageChangerSystem.ProcessMode.Update,
                (Resource)resIdX
            );
            UpdateUIResourceBinding(resTempX);
            SetDirty();
        }

        public void ResetStorage()
        {
            Resource resTempX = storageChangerSystem.ReplaceEntity(
                selectedEntity,
                StorageChangerSystem.ProcessMode.Reset
            );
            UpdateUIResourceBinding(resTempX);
            SetDirty();
        }
    }
}
