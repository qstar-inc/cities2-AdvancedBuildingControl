using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game;
using Game.Economy;
using Game.Prefabs;
using Game.UI.Menu;
using Game.Zones;
using Newtonsoft.Json;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public class SPData
    {
        public string prefabName;
        public string guid;
    }

    public class SPDataFile
    {
        public List<SPData> SpData { get; set; } = new();
    }

    public partial class StaticPloppableData : GameSystemBase
    {
        internal static string fileFolder = $"{EnvPath.kUserDataPath}/ModsData/{Mod.Id}/";
        internal static string fileName = "StaticPloppableBuilder.json";
        internal static string fullPath = Path.Combine(fileFolder, fileName);
        static SPDataFile spDataFile;

        public PrefabSystem prefabSystem;

        static StaticPloppableBuilder staticPloppableBuilder;

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = WorldHelper.PrefabSystem;

            staticPloppableBuilder = WorldHelper.GetSystem<StaticPloppableBuilder>();

            Directory.CreateDirectory(fileFolder);
        }

        protected override void OnUpdate() { }

        public void ReadFile()
        {
            SPDataFile dataFile;

            if (File.Exists(fullPath))
            {
                Mod.m_Setting.NoSPCache = false;
                string json = File.ReadAllText(fullPath);

                if (!string.IsNullOrWhiteSpace(json))
                    dataFile =
                        JsonConvert.DeserializeObject<SPDataFile>(json, GetSetting())
                        ?? new SPDataFile();
                else
                    dataFile = new SPDataFile();
            }
            else
                dataFile = new SPDataFile();

            spDataFile = dataFile;
        }

        public void SaveFile()
        {
            string output = JsonConvert.SerializeObject(spDataFile, GetSetting());
            File.WriteAllText(fullPath, output);
        }

        public void DeleteFile()
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                spDataFile = new();
                Mod.m_Setting.NoSPCache = true;
            }
        }

        public void AddToFile(string newPrefabName, Colossal.Hash128 guid)
        {
            SPDataFile dataFile = spDataFile;

            SPData data = new() { prefabName = newPrefabName, guid = guid.ToString() };

            if (FindData(newPrefabName, guid, out int index) && (index >= 0))
                dataFile.SpData[index] = data;
            else
                dataFile.SpData.Add(data);

            spDataFile = dataFile;
            SaveFile();
        }

        public void RemoveFromFile(string prefabName, Colossal.Hash128 guid)
        {
            if (FindData(prefabName, guid, out int index) && (index >= 0))
                spDataFile.SpData.RemoveAt(index);

            SaveFile();
        }

        public bool FindData(string prefabName, Colossal.Hash128 guid, out int index)
        {
            index = -1;
            if (spDataFile == null)
                return false;

            var existing = spDataFile.SpData.FirstOrDefault(x =>
                x.prefabName == prefabName && x.guid == guid.ToString()
            );

            if (existing != null)
            {
                index = spDataFile.SpData.IndexOf(existing);
                return true;
            }
            return false;
        }

        internal JsonSerializerSettings GetSetting()
        {
            JsonSerializerSettings settings = new()
            {
                Formatting = Formatting.Indented,
                Error = (sender, args) =>
                {
                    LogHelper.SendLog(
                        $"Serialization error on property '{args.ErrorContext.Member}': {args.ErrorContext.Error.Message}"
                    );
                    args.ErrorContext.Handled = true;
                },
            };
            return settings;
        }

        public void CreateSPFromFile()
        {
            ReadFile();
            if (spDataFile == null || spDataFile.SpData.Count <= 0)
                return;

            var spData = spDataFile.SpData;

            LogHelper.SendLog($"{spData.Count} SP prefabs being created", LogLevel.DEVD);
            for (int i = 0; i < spData.Count; i++)
                staticPloppableBuilder.MakeSP(spData[i]);
            LocaleHelper.OnActiveDictionaryChanged();
        }

        public bool TryGetTab(Entity buildingEntity, out UIAssetCategoryPrefab tab)
        {
            tab = null;

            if (
                EntityManager.TryGetBuffer(
                    buildingEntity,
                    true,
                    out DynamicBuffer<ServiceUpgradeBuilding> sub
                )
                && sub.Length > 0
                && sub[0].m_Building != Entity.Null
            )
                buildingEntity = sub[0].m_Building;

            if (
                EntityManager.TryGetComponent(
                    buildingEntity,
                    out SpawnableBuildingData spawnableBuildingData
                )
            )
            {
                Entity zoneEntity = spawnableBuildingData.m_ZonePrefab;

                if (
                    !EntityManager.TryGetComponent(
                        buildingEntity,
                        out BuildingPropertyData bldgPropertyData
                    )
                    || !EntityManager.TryGetComponent(
                        zoneEntity,
                        out ZonePropertiesData zonePropertiesData
                    )
                    || !EntityManager.TryGetComponent(zoneEntity, out ZoneData zoneData)
                )
                    return false;

                ZoneDensity zd = Game.Buildings.PropertyUtils.GetZoneDensity(
                    zoneData,
                    zonePropertiesData
                );

                if (zoneData.m_AreaType == AreaType.Residential)
                {
                    switch (zd)
                    {
                        case ZoneDensity.Low:
                            if (TryGetTabPrefab("Zoned_ResiLow", out PrefabBase z_rl))
                            {
                                tab = z_rl as UIAssetCategoryPrefab;
                                return true;
                            }
                            break;
                        case ZoneDensity.Medium:
                            if (
                                !(
                                    bldgPropertyData.m_AllowedSold
                                        == Game.Economy.Resource.NoResource
                                    && bldgPropertyData.m_AllowedManufactured
                                        == Game.Economy.Resource.NoResource
                                    && bldgPropertyData.m_AllowedStored
                                        == Game.Economy.Resource.NoResource
                                )
                            )
                            {
                                if (TryGetTabPrefab("Mixed", out PrefabBase z_mx))
                                {
                                    tab = z_mx as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            if (
                                EntityManager.TryGetComponent(
                                    buildingEntity,
                                    out BuildingData bldgData
                                )
                                && bldgData.m_LotSize.x == 1
                            )
                            {
                                if (TryGetTabPrefab("Zoned_ResiRow", out PrefabBase z_rr))
                                {
                                    tab = z_rr as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            if (TryGetTabPrefab("Zoned_ResiMedium", out PrefabBase z_rm))
                            {
                                tab = z_rm as UIAssetCategoryPrefab;
                                return true;
                            }
                            break;
                        case ZoneDensity.High:
                            if (bldgPropertyData.m_SpaceMultiplier > 1)
                            {
                                if (TryGetTabPrefab("Zoned_ResiHigh", out PrefabBase z_rh))
                                {
                                    tab = z_rh as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }

                            if (TryGetTabPrefab("Zoned_ResiLowRent", out PrefabBase z_rlr))
                            {
                                tab = z_rlr as UIAssetCategoryPrefab;
                                return true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (zoneData.m_AreaType == AreaType.Commercial)
                {
                    switch (zd)
                    {
                        case ZoneDensity.Low:
                            if (TryGetTabPrefab("Zoned_CommLow", out PrefabBase z_cl))
                            {
                                tab = z_cl as UIAssetCategoryPrefab;
                                return true;
                            }
                            break;
                        case ZoneDensity.High:
                            if (TryGetTabPrefab("Zoned_CommHigh", out PrefabBase z_ch))
                            {
                                tab = z_ch as UIAssetCategoryPrefab;
                                return true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (zoneData.m_AreaType == AreaType.Industrial)
                {
                    if (zoneData.IsOffice())
                    {
                        switch (zd)
                        {
                            case ZoneDensity.Low:
                                if (TryGetTabPrefab("Zoned_OfficeLow", out PrefabBase z_ol))
                                {
                                    tab = z_ol as UIAssetCategoryPrefab;
                                    return true;
                                }
                                break;
                            case ZoneDensity.High:
                                if (TryGetTabPrefab("Zoned_OfficeHigh", out PrefabBase z_oh))
                                {
                                    tab = z_oh as UIAssetCategoryPrefab;
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (
                            EconomyUtils.IsExtractorResource(bldgPropertyData.m_AllowedManufactured)
                        )
                        {
                            if (bldgPropertyData.m_AllowedManufactured == Resource.Wood)
                            {
                                if (TryGetTabPrefab("Zoned_IndForest", out PrefabBase z_i))
                                {
                                    tab = z_i as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            else if (bldgPropertyData.m_AllowedManufactured == Resource.Oil)
                            {
                                if (TryGetTabPrefab("Zoned_IndOil", out PrefabBase z_i))
                                {
                                    tab = z_i as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            else if (bldgPropertyData.m_AllowedManufactured == Resource.Fish)
                            {
                                if (TryGetTabPrefab("Zoned_IndFishing", out PrefabBase z_i))
                                {
                                    tab = z_i as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            else if (
                                (
                                    bldgPropertyData.m_AllowedManufactured
                                    & (
                                        Resource.Grain
                                        | Resource.Vegetables
                                        | Resource.Livestock
                                        | Resource.Cotton
                                    )
                                )
                                == (
                                    Resource.Grain
                                    | Resource.Vegetables
                                    | Resource.Livestock
                                    | Resource.Cotton
                                )
                            )
                            {
                                if (TryGetTabPrefab("Zoned_IndAgri", out PrefabBase z_i))
                                {
                                    tab = z_i as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                            else if (
                                (
                                    bldgPropertyData.m_AllowedManufactured
                                    & (Resource.Coal | Resource.Stone | Resource.Ore)
                                ) == (Resource.Coal | Resource.Stone | Resource.Ore)
                            )
                            {
                                if (TryGetTabPrefab("Zoned_IndOre", out PrefabBase z_i))
                                {
                                    tab = z_i as UIAssetCategoryPrefab;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (TryGetTabPrefab("Zoned_IndManu", out PrefabBase z_if))
                            {
                                tab = z_if as UIAssetCategoryPrefab;
                                return true;
                            }
                        }
                    }
                }
            }

            if (
                EntityManager.TryGetComponent(buildingEntity, out UIObjectData uiObjectData)
                && EntityManager.TryGetComponent(
                    uiObjectData.m_Group,
                    out UIAssetCategoryData uIAssetCategoryData
                )
            )
            {
                string menuName = prefabSystem.GetPrefabName(uIAssetCategoryData.m_Menu);

                switch (menuName)
                {
                    case "Roads":
                        if (TryGetTabPrefab("Service_Roads", out PrefabBase s_Roads))
                        {
                            tab = s_Roads as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Electricity":
                        if (TryGetTabPrefab("Service_Electricity", out PrefabBase s_Electricity))
                        {
                            tab = s_Electricity as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Water & Sewage":
                        if (TryGetTabPrefab("Service_Water", out PrefabBase s_Water))
                        {
                            tab = s_Water as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Health & Deathcare":
                        if (TryGetTabPrefab("Service_Healthcare", out PrefabBase s_Healthcare))
                        {
                            tab = s_Healthcare as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Garbage Management":
                        if (TryGetTabPrefab("Service_Garbage", out PrefabBase s_Garbage))
                        {
                            tab = s_Garbage as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Education & Research":
                        if (TryGetTabPrefab("Service_Education", out PrefabBase s_Education))
                        {
                            tab = s_Education as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Fire & Rescue":
                        if (TryGetTabPrefab("Service_Fire", out PrefabBase s_Fire))
                        {
                            tab = s_Fire as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Police & Administration":
                        if (TryGetTabPrefab("Service_Police", out PrefabBase s_Police))
                        {
                            tab = s_Police as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Transportation":
                        if (
                            TryGetTabPrefab(
                                "Service_Transportation",
                                out PrefabBase s_Transportation
                            )
                        )
                        {
                            tab = s_Transportation as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Parks & Recreation":
                        if (TryGetTabPrefab("Service_Park", out PrefabBase s_Park))
                        {
                            tab = s_Park as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Communications":
                        if (
                            TryGetTabPrefab("Service_Communication", out PrefabBase s_Communication)
                        )
                        {
                            tab = s_Communication as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                    case "Landscaping":
                        if (TryGetTabPrefab("Service_Landscaping", out PrefabBase s_Landscaping))
                        {
                            tab = s_Landscaping as UIAssetCategoryPrefab;
                            return true;
                        }
                        break;
                }
                LogHelper.SendLog(menuName);
            }

            if (TryGetTabPrefab("Service_Landscaping", out PrefabBase any))
            {
                tab = any as UIAssetCategoryPrefab;
                return true;
            }

            return false;
        }

        private bool TryGetTabPrefab(string name, out PrefabBase prefabBase)
        {
            prefabBase = null;

            if (name == null)
                return false;

            string pname = $"StarQ_SP_{name}";
            string guid = "";
            switch (name)
            {
                case "Service_Communication":
                    guid = "7082f45853a45ef45a2f6d544f1a4b09";
                    break;
                case "Service_Education":
                    guid = "06fa933c8ca05be56b1495b5da513cfd";
                    break;
                case "Service_Electricity":
                    guid = "3b317f8d46e130952fed42ea94f23358";
                    break;
                case "Service_Fire":
                    guid = "5ecda3ca0b568b4e675f3a93bd43aca6";
                    break;
                case "Service_Garbage":
                    guid = "849cb13c4d7dc5184b32db5ff14c1c66";
                    break;
                case "Service_Healthcare":
                    guid = "aa12822734be3938ff80e2d652f28eba";
                    break;
                case "Service_Landscaping":
                    guid = "2f058b873c515ef55f25ff1ef9251643";
                    break;
                case "Service_Park":
                    guid = "3347feb0d4bfeb07230dc6da16621a4c";
                    break;
                case "Service_Police":
                    guid = "37d54129cf9a0baaefb47bd74392d9d4";
                    break;
                case "Service_Roads":
                    guid = "8419f54051cdb819145c94cc926faf5e";
                    break;
                case "Service_Transportation":
                    guid = "8259f5c8a27010097451a491e1e03a98";
                    break;
                case "Service_Water":
                    guid = "7abf55236992e29e3db8b046c230ffbb";
                    break;
                case "Zoned_CommHigh":
                    guid = "103e6548607bec6e096a3ba76af5674e";
                    break;
                case "Zoned_CommLow":
                    guid = "6d683baf98a5819559b8d5672d33dfdb";
                    break;
                case "Zoned_IndAgri":
                    guid = "cd5973951221c6156484be5fceef261c";
                    break;
                case "Zoned_IndFishing":
                    guid = "eb0df7be311ebc195a420982104fdf19";
                    break;
                case "Zoned_IndForest":
                    guid = "c2b044ad52b9fd91d88ca7d54883c50e";
                    break;
                case "Zoned_IndManu":
                    guid = "ee6415ed1209cc99d483691fdfa809d7";
                    break;
                case "Zoned_IndOil":
                    guid = "7f0f00a29820fd7dd08b3adf7cfd428c";
                    break;
                case "Zoned_IndOre":
                    guid = "7763066d11e3a8b7c24dd7167a4acf64";
                    break;
                case "Zoned_Mixed":
                    guid = "45fb6bdf5cf76a43ba17bb9e2c5fae31";
                    break;
                case "Zoned_OfficeHigh":
                    guid = "8341b919ad1885cbee78b0ad45f1c82a";
                    break;
                case "Zoned_OfficeLow":
                    guid = "d2eda98feaed5cd3524194842998bfd1";
                    break;
                case "Zoned_ResiHigh":
                    guid = "2e66961e86687444fb47677e24483304";
                    break;
                case "Zoned_ResiLow":
                    guid = "a615db47c60d717d9f701bb6db9f19bd";
                    break;
                case "Zoned_ResiLowRent":
                    guid = "afce99696f473a3e2f6e930f0c1cbaf8";
                    break;
                case "Zoned_ResiMedium":
                    guid = "3daf9d6959d0d9ec0af25e5d699ddf89";
                    break;
                case "Zoned_ResiMediumRow":
                    guid = "838bad7ac15c18916d7e5d39c247fc79";
                    break;
            }

            if (
                prefabSystem.TryGetPrefab(
                    new PrefabID(
                        nameof(UIAssetCategoryPrefab),
                        pname,
                        Colossal.Hash128.Parse(guid)
                    ),
                    out prefabBase
                )
                && prefabBase != null
            )
                return true;

            return false;
        }
    }
}
