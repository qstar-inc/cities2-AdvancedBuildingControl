using System;
using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Extensions;
using CliWrap;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Economy;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Settings;
using Game.UI;
using Game.UI.InGame;
using Game.Zones;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;
using static Game.UI.InGame.UIResource;

namespace AdvancedBuildingControl.Systems
{
    public class BrandDataInfo
    {
        public string Name { get; set; } = "UnknownName";
        public string PrefabName { get; set; } = "UnknownPrefab";
        public string Color1 { get; set; } = "RGBA(0,0,0)";
        public string Color2 { get; set; } = "RGBA(0,0,0)";
        public string Color3 { get; set; } = "RGBA(0,0,0)";
        public Entity Entity { get; set; } = Entity.Null;
        public string Icon { get; set; } = "";
        public string[] Companies { get; set; } = new string[0];
    }

    public class ZoneDataInfo
    {
        public string Name { get; set; } = "UnknownName";
        public string PrefabName { get; set; } = "UnknownPrefab";
        public string Color1 { get; set; } = "RGBA(0,0,0)";
        public string Color2 { get; set; } = "RGBA(0,0,0)";
        public float Upkeep { get; set; } = 0f;
        public Entity Entity { get; set; } = Entity.Null;
        public string Icon { get; set; } = "";
        public AreaType AreaType { get; set; } = AreaType.None;
        public string AreaTypeString { get; set; } = "";
    }

    public enum ResourceGroup
    {
        None = 0,
        Raw = 1,
        Processed = 2,
        Mail = 3,
        Others = 4,
        Money = 5,
    }

    public class ResourceDataInfo
    {
        public Resource Resource { get; set; } = Resource.NoResource;
        public ResourceGroup Group { get; set; } = ResourceGroup.None;
        public ulong Id { get; set; } = 0;
        public string Name { get; set; } = "UnknownName";
        //public string Icon { get; set; } = "";
        //public string DisplayName { get; set; } = "";
    }

    public partial class DataRetriever : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public PrefabUISystem prefabUISystem;
        public NameSystem nameSystem;
        public ImageSystem imageSystem;
        public StorageChangerSystem storageChangerSystem;

        //public ResourcePrefabs resourcePrefabs;

#nullable enable
        public static GameMode gameMode;

        public int prevBrandEntityCount = 0;
        public static readonly List<BrandDataInfo> brandDataInfos = new();
        public static bool hasNewBrandData = false;

        public int prevZoneEntityCount = 0;
        public static readonly List<ZoneDataInfo> zoneDataInfos = new();
        public static bool hasNewZoneData = false;

        public int prevResourceEntityCount = 0;
        public static readonly List<ResourceDataInfo> resourceDataInfos = new();
        public static bool hasNewResourceData = false;

        public static bool dataRetrieved = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
            prefabUISystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabUISystem>();
            nameSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<NameSystem>();
            imageSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ImageSystem>();
            storageChangerSystem = World.GetOrCreateSystemManaged<StorageChangerSystem>();
            //resourcePrefabs = World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();

            GameManager.instance.localizationManager.onActiveDictionaryChanged += CleanAndGetData;
            gameMode = GameMode.None;
        }

        protected override void OnUpdate() { }

        //protected override void OnSaveGameLoaded(Purpose purpose, GameMode mode)
        //{
        //    base.OnGamePreload(purpose, mode);

        //    EntityQuery entityQuery = SystemAPI.QueryBuilder().WithAll<AlteredStorage>().Build();
        //    NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        //    Mod.log.Info($"Found {entities.Length} Altered Storage");

        //    foreach (var item in entities)
        //    {
        //        Mod.log.Info(item);
        //    }
        //}

        //protected override void OnGamePreload(Purpose purpose, GameMode mode)
        //{
        //    base.OnGamePreload(purpose, mode);
        //    gameMode = mode;

        //    if (GameModeExtensions.IsGame(gameMode))
        //    {
        //        base.OnGameLoadingComplete(purpose, mode);
        //        LogHelper.SendLog($"Starting InitOnGameStart on OnGamePreload");
        //        storageChangerSystem.InitOnGameStart();
        //    }
        //    else
        //    {
        //        LogHelper.SendLog($"Game mode is {gameMode} for OnGameLoadingComplete");
        //    }
        //}

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            gameMode = mode;

            if (GameModeExtensions.IsGame(gameMode))
            {
                base.OnGameLoadingComplete(purpose, mode);
                LogHelper.SendLog($"Starting InitOnGameStart on OnGamePreload");
                storageChangerSystem.InitOnGameStart();
            }
            else
            {
                LogHelper.SendLog($"Game mode is {gameMode} for OnGameLoadingComplete");
            }

            if (GameModeExtensions.IsGame(mode))
                return;
            try
            {
                CleanAndGetData();
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }
            Enabled = false;
        }

        void CleanAndGetData()
        {
            brandDataInfos.Clear();
            prevBrandEntityCount = 0;
            hasNewBrandData = false;

            prevZoneEntityCount = 0;
            zoneDataInfos.Clear();
            hasNewZoneData = false;

            prevResourceEntityCount = 0;
            resourceDataInfos.Clear();
            hasNewResourceData = false;

            dataRetrieved = false;
            GetBrandData();
            GetZoneData();
            GetResourceData();
        }

        void GetBrandData()
        {
            if (dataRetrieved)
                return;
            try
            {
                EntityQuery brandQuery = SystemAPI.QueryBuilder().WithAll<BrandData>().Build();
                NativeArray<Entity> brandEntitiesFromQuery = brandQuery.ToEntityArray(
                    Allocator.Temp
                );

                hasNewBrandData = false;
                int bCount = brandEntitiesFromQuery.Length;
                if (brandDataInfos.Count == 0 || prevBrandEntityCount != bCount)
                {
                    brandDataInfos.Clear();
                    prevBrandEntityCount = bCount;
                    foreach (var entity in brandEntitiesFromQuery)
                    {
                        prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab);

                        if (brandPrefab == null)
                        {
                            EntityManager.TryGetComponent(entity, out PrefabData pd);
                            string missingName = EntityManager.GetName(entity);

                            Mod.log.Info($"Brand '{missingName}' ({entity} is missing");
                            continue;
                        }

                        string icon =
                            imageSystem.GetIconOrGroupIcon(entity)
                            ?? string.Format(
                                "{0}?width={1}&height={2}",
                                brandPrefab.thumbnailUrl,
                                32,
                                32
                            );
                        ;

                        //if (brandPrefab.TryGetExactly(out UIObject uiObject))
                        //{
                        //    icon = uiObject.m_Icon;
                        //}
                        //else
                        //{
                        //    icon =
                        //        $"thumbnail://ThumbnailCamera/BrandPrefab/{brandPrefab.name}?width=32&height=32";
                        //}

                        CompanyPrefab[] companyPrefabs = brandPrefab.m_Companies;
                        string[] companies = new string[companyPrefabs.Length];
                        for (int i = 0; i < companyPrefabs.Length; i++)
                        {
                            companies[i] = companyPrefabs[i].name;
                        }

                        brandDataInfos.Add(
                            new BrandDataInfo
                            {
                                Name = nameSystem.GetRenderedLabelName(entity),
                                PrefabName = brandPrefab.name,
                                Color1 = brandPrefab.m_BrandColors[0].ToHexCode(),
                                Color2 = brandPrefab.m_BrandColors[1].ToHexCode(),
                                Color3 = brandPrefab.m_BrandColors[2].ToHexCode(),
                                Entity = entity,
                                Icon = icon,
                                Companies = companies,
                            }
                        );
                    }
                    brandDataInfos.Sort(
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                    );
                    hasNewBrandData = true;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }
        }

        void GetZoneData()
        {
            if (dataRetrieved)
                return;
            try
            {
                EntityQuery zoneQuery = SystemAPI.QueryBuilder().WithAll<ZoneData>().Build();
                NativeArray<Entity> zoneEntitiesFromQuery = zoneQuery.ToEntityArray(Allocator.Temp);

                hasNewZoneData = false;
                int zCount = zoneEntitiesFromQuery.Length;
                if (zoneDataInfos.Count == 0 || prevZoneEntityCount != zCount)
                {
                    zoneDataInfos.Clear();
                    prevZoneEntityCount = zCount;
                    foreach (var entity in zoneEntitiesFromQuery)
                    {
                        prefabSystem.TryGetPrefab(entity, out ZonePrefab zonePrefab);

                        if (zonePrefab == null)
                        {
                            EntityManager.TryGetComponent(entity, out PrefabData pd);
                            string missingName = EntityManager.GetName(entity);

                            Mod.log.Info($"Zone '{missingName}' ({entity} is missing");
                            continue;
                        }

                        string icon =
                            imageSystem.GetIconOrGroupIcon(entity)
                            ?? string.Format(
                                "{0}?width={1}&height={2}",
                                zonePrefab.thumbnailUrl,
                                32,
                                32
                            )
                            ?? "Media/Misc/Error.svg";

                        //zonePrefab.TryGetExactly(out UIObject uiObject);
                        //string icon = "";
                        //if (uiObject != null)
                        //{
                        //    icon = uiObject.m_Icon;
                        //}

                        string areaType = zonePrefab.m_AreaType.ToString();

                        if (zonePrefab.m_Office)
                            areaType = "Office";

                        float upkeep = 0f;
                        zonePrefab.TryGetExactly(out ZoneServiceConsumption zoneServiceConsumption);
                        if (zoneServiceConsumption != null)
                        {
                            upkeep = zoneServiceConsumption.m_Upkeep;
                        }

                        prefabUISystem.GetTitleAndDescription(entity, out var titleId, out var _);

                        zoneDataInfos.Add(
                            new ZoneDataInfo
                            {
                                Name = titleId,
                                PrefabName = zonePrefab.name,
                                Color1 = zonePrefab.m_Color.ToHexCode(),
                                Color2 = zonePrefab.m_Edge.ToHexCode(),
                                Upkeep = upkeep,
                                Entity = entity,
                                Icon = icon,
                                AreaType = zonePrefab.m_AreaType,
                                AreaTypeString = areaType,
                            }
                        );
                    }
                    zoneDataInfos.Sort(
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                    );
                    hasNewZoneData = true;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }
        }

        public ResourceGroup GetResourceGroup(Resource res, ResourcePrefab resourcePrefab)
        {
            switch (res)
            {
                case Resource.Money:
                    return ResourceGroup.Money;
                case Resource.NoResource:
                case Resource.Last:
                case Resource.All:
                    return ResourceGroup.None;
                default:
                    break;
            }

            if ((res & (Resource)28672UL) != Resource.NoResource)
                return ResourceGroup.Mail;

            if (resourcePrefab.m_IsMaterial)
                return ResourceGroup.Raw;

            if (resourcePrefab.m_Weight == 0)
                return ResourceGroup.Others;

            return ResourceGroup.Processed;
        }

        void GetResourceData()
        {
            if (dataRetrieved)
                return;
            try
            {
                EntityQuery resourceQuery = SystemAPI
                    .QueryBuilder()
                    .WithAll<ResourceData>()
                    .Build();
                NativeArray<Entity> resourceEntitiesFromQuery = resourceQuery.ToEntityArray(
                    Allocator.Temp
                );

                //Resource allResource = EconomyUtils.GetAllResources();

                int rCount = resourceEntitiesFromQuery.Length;
                hasNewResourceData = false;
                if (resourceDataInfos.Count == 0 || prevResourceEntityCount != rCount)
                {
                    prevResourceEntityCount = rCount;
                    resourceDataInfos.Clear();
                    foreach (Entity entity in resourceEntitiesFromQuery)
                    //foreach (Resource res in Enum.GetValues(typeof(Resource)))
                    {
                        //Entity prefab = resourceSystem.GetPrefab(res);
                        //EntityManager.TryGetComponent(entity, out PrefabData prefabData);
                        //prefabSystem.TryGetPrefab(prefabData, out PrefabBase resourcePrefab);
                        prefabSystem.TryGetPrefab(entity, out ResourcePrefab resourcePrefab);

                        if (resourcePrefab == null)
                        {
                            EntityManager.TryGetComponent(entity, out PrefabData pd);
                            string missingName = EntityManager.GetName(entity);

                            Mod.log.Info($"Resource '{missingName}' ({entity} is missing");
                            continue;
                        }

                        string resourceSuffix = resourcePrefab.name.Replace("Resource", "");
                        //ResourcePrefab resourcePrefabX = (ResourcePrefab)resourcePrefab;
                        var res = EconomyUtils.GetResource(resourcePrefab.m_Resource);
                        //if (
                        //    res == Resource.NoResource
                        //    || res == Resource.All
                        //    || res == Resource.Money
                        //    || res == Resource.Last
                        //)
                        //    continue;

                        //ResourceGroup resourceGroup = ;

                        //string name = res.ToString();

                        //string icon = $"Media/Game/Resources/{resourceSuffix}.svg";

                        //if (resourcePrefab.name == "ResourceMoney") continue;

                        //string icon = "";
                        //if (resourcePrefab.TryGetExactly(out UIObject uiObject))
                        //{
                        //    icon = uiObject.m_Icon;
                        //}
                        //else
                        //{
                        //    icon =
                        //        $"thumbnail://ThumbnailCamera/BrandPrefab/{resourcePrefab.name}?width=32&height=32";
                        //}

                        //string displayName =
                        //    LocaleHelper.Translate($"Resources.TITLE[{resourceSuffix}]") ?? name;

                        //Mod.log.Info(
                        //    $"New Resource: {resourcePrefab.name} ({displayName}), ID: {(ulong)res}, Icon: {icon}"
                        //);

                        resourceDataInfos.Add(
                            new ResourceDataInfo
                            {
                                Resource = res,
                                Group = GetResourceGroup(res, resourcePrefab),
                                Id = (ulong)res,
                                Name = res.ToString(),
                                //Icon = icon,
                                //DisplayName = displayName,
                            }
                        );
                    }
                    resourceDataInfos.Sort(
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                    );
                    hasNewResourceData = true;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }

            dataRetrieved = true;
        }
    }
}
