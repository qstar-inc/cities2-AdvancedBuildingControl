using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Economy;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI;
using Game.UI.InGame;
using Game.Zones;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

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

    //public class ZoneDataInfo
    //{
    //    public string Name { get; set; } = "UnknownName";
    //    public string PrefabName { get; set; } = "UnknownPrefab";
    //    public string Color1 { get; set; } = "RGBA(0,0,0)";
    //    public string Color2 { get; set; } = "RGBA(0,0,0)";
    //    public float Upkeep { get; set; } = 0f;
    //    public Entity Entity { get; set; } = Entity.Null;

    //    //public string Icon { get; set; } = "";
    //    public AreaType AreaType { get; set; } = AreaType.None;
    //    public string AreaTypeString { get; set; } = "";
    //}

    //public enum ResourceGroup
    //{
    //    None = 0,
    //    Raw = 1,
    //    Processed = 2,
    //    Mail = 3,
    //    Others = 4,
    //    Money = 5,
    //}

    //public class ResourceDataInfo
    //{
    //    public Resource Resource { get; set; } = Resource.NoResource;
    //    public ResourceGroup Group { get; set; } = ResourceGroup.None;
    //    public ulong Id { get; set; } = 0;
    //    public string Name { get; set; } = "UnknownName";
    //}

    public partial class DataRetriever : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public PrefabUISystem prefabUISystem;
        public NameSystem nameSystem;
        public ImageSystem imageSystem;

        public static Entity integratedHelipad = Entity.Null;
#nullable enable
        public bool NeedUpdate = true;

        public int prevBrandEntityCount = 0;
        public static readonly List<BrandDataInfo> brandDataInfos = new();
        public static bool hasNewBrandData = false;

        //public int prevZoneEntityCount = 0;
        //public static readonly List<ZoneDataInfo> zoneDataInfos = new();
        //public static bool hasNewZoneData = false;

        //public int prevResourceEntityCount = 0;
        //public static readonly List<ResourceDataInfo> resourceDataInfos = new();
        //public static bool hasNewResourceData = false;

        //public static bool dataRetrieved = false;
        public static bool firstTime = true;

        private static readonly object _lock1 = new();
        private static readonly object _lock2 = new();

        //private static readonly object _lock3 = new();

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = WorldHelper.PrefabSystem;
            prefabUISystem = WorldHelper.PrefabUISystem;
            nameSystem = WorldHelper.NameSystem;
            imageSystem = WorldHelper.ImageSystem;

            //Mod.m_Setting.onSettingsApplied += OnSettingsChanged;
        }

        protected override void OnUpdate() { }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (firstTime)
            {
                GameManager.instance.localizationManager.onActiveDictionaryChanged +=
                    CleanAndGetData;
                CleanAndGetData();
                firstTime = false;
            }

            lock (_lock1)
                CleanAndGetData();
        }

        //private void OnSettingsChanged(Game.Settings.Setting setting) => NeedUpdate = true;

        void CleanAndGetData()
        {
            if (!NeedUpdate)
                return;

            lock (_lock2)
            {
                brandDataInfos.Clear();
                prevBrandEntityCount = 0;
                hasNewBrandData = false;

                //prevZoneEntityCount = 0;
                //zoneDataInfos.Clear();
                //hasNewZoneData = false;

                //prevResourceEntityCount = 0;
                //resourceDataInfos.Clear();
                //hasNewResourceData = false;

                //dataRetrieved = false;
                GetBrandData();
                //GetZoneData();
                //GetResourceData();

                if (
                    prefabSystem.TryGetPrefab(
                        new PrefabID("MarkerObjectPrefab", "Integrated Helipad"),
                        out PrefabBase intHeli
                    )
                )
                    prefabSystem.TryGetEntity(intHeli, out integratedHelipad);

                //dataRetrieved = true;
                LogHelper.SendLog(
                    $"Data retrieved:\nBrandDataInfos: {brandDataInfos.Count}", //, ZoneDataInfos: {zoneDataInfos.Count}, ResourceDataInfos: {resourceDataInfos.Count}",
                    LogLevel.DEV
                );
                NeedUpdate = false;
            }
        }

        void GetBrandData()
        {
            //if (dataRetrieved)
            //    return;
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

        //void GetZoneData()
        //{
        //    //if (dataRetrieved)
        //    //    return;
        //    try
        //    {
        //        EntityQuery zoneQuery = SystemAPI.QueryBuilder().WithAll<ZoneData>().Build();
        //        NativeArray<Entity> zoneEntitiesFromQuery = zoneQuery.ToEntityArray(Allocator.Temp);

        //        hasNewZoneData = false;
        //        int zCount = zoneEntitiesFromQuery.Length;
        //        if (zoneDataInfos.Count == 0 || prevZoneEntityCount != zCount)
        //        {
        //            zoneDataInfos.Clear();
        //            prevZoneEntityCount = zCount;
        //            foreach (var entity in zoneEntitiesFromQuery)
        //            {
        //                prefabSystem.TryGetPrefab(entity, out ZonePrefab zonePrefab);

        //                if (zonePrefab == null)
        //                {
        //                    EntityManager.TryGetComponent(entity, out PrefabData pd);
        //                    string missingName = EntityManager.GetName(entity);

        //                    Mod.log.Info($"Zone '{missingName}' ({entity} is missing");
        //                    continue;
        //                }

        //                //string icon =
        //                //    imageSystem.GetIconOrGroupIcon(entity)
        //                //    ?? string.Format(
        //                //        "{0}?width={1}&height={2}",
        //                //        zonePrefab.thumbnailUrl,
        //                //        32,
        //                //        32
        //                //    )
        //                //    ?? "Media/Misc/Error.svg";

        //                //zonePrefab.TryGetExactly(out UIObject uiObject);
        //                //string icon = "";
        //                //if (uiObject != null)
        //                //{
        //                //    icon = uiObject.m_Icon;
        //                //}

        //                string areaType = zonePrefab.m_AreaType.ToString();

        //                if (zonePrefab.m_Office)
        //                    areaType = "Office";

        //                float upkeep = 0f;
        //                zonePrefab.TryGetExactly(out ZoneServiceConsumption zoneServiceConsumption);
        //                if (zoneServiceConsumption != null)
        //                {
        //                    upkeep = zoneServiceConsumption.m_Upkeep;
        //                }

        //                prefabUISystem.GetTitleAndDescription(entity, out var titleId, out var _);

        //                zoneDataInfos.Add(
        //                    new ZoneDataInfo
        //                    {
        //                        Name = titleId,
        //                        PrefabName = zonePrefab.name,
        //                        Color1 = zonePrefab.m_Color.ToHexCode(),
        //                        Color2 = zonePrefab.m_Edge.ToHexCode(),
        //                        Upkeep = upkeep,
        //                        Entity = entity,
        //                        //Icon = icon,
        //                        AreaType = zonePrefab.m_AreaType,
        //                        AreaTypeString = areaType,
        //                    }
        //                );
        //            }
        //            zoneDataInfos.Sort(
        //                (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
        //            );
        //            hasNewZoneData = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Mod.log.Error(ex);
        //    }
        //}

        //public ResourceGroup GetResourceGroup(Resource res, ResourcePrefab resourcePrefab)
        //{
        //    switch (res)
        //    {
        //        case Resource.Money:
        //            return ResourceGroup.Money;
        //        case Resource.NoResource:
        //        case Resource.Last:
        //        case Resource.All:
        //            return ResourceGroup.None;
        //        default:
        //            break;
        //    }

        //    if ((res & (Resource)28672UL) != Resource.NoResource)
        //        return ResourceGroup.Mail;

        //    if (resourcePrefab.m_IsMaterial)
        //        return ResourceGroup.Raw;

        //    if (resourcePrefab.m_Weight == 0)
        //        return ResourceGroup.Others;

        //    return ResourceGroup.Processed;
        //}

        //void GetResourceData()
        //{
        //    //if (dataRetrieved)
        //    //    return;
        //    try
        //    {
        //        EntityQuery resourceQuery = SystemAPI
        //            .QueryBuilder()
        //            .WithAll<ResourceData>()
        //            .Build();
        //        NativeArray<Entity> resourceEntitiesFromQuery = resourceQuery.ToEntityArray(
        //            Allocator.Temp
        //        );

        //        int rCount = resourceEntitiesFromQuery.Length;
        //        hasNewResourceData = false;
        //        if (resourceDataInfos.Count == 0 || prevResourceEntityCount != rCount)
        //        {
        //            prevResourceEntityCount = rCount;
        //            resourceDataInfos.Clear();
        //            foreach (Entity entity in resourceEntitiesFromQuery)
        //            {
        //                prefabSystem.TryGetPrefab(entity, out ResourcePrefab resourcePrefab);

        //                if (resourcePrefab == null)
        //                {
        //                    EntityManager.TryGetComponent(entity, out PrefabData pd);
        //                    string missingName = EntityManager.GetName(entity);

        //                    Mod.log.Info($"Resource '{missingName}' ({entity} is missing");
        //                    continue;
        //                }

        //                string resourceSuffix = resourcePrefab.name.Replace("Resource", "");
        //                var res = EconomyUtils.GetResource(resourcePrefab.m_Resource);

        //                resourceDataInfos.Add(
        //                    new ResourceDataInfo
        //                    {
        //                        Resource = res,
        //                        Group = GetResourceGroup(res, resourcePrefab),
        //                        Id = (ulong)res,
        //                        Name = res.ToString(),
        //                    }
        //                );
        //            }
        //            resourceDataInfos.Sort(
        //                (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
        //            );
        //            hasNewResourceData = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Mod.log.Error(ex);
        //    }
        //}
    }
}
