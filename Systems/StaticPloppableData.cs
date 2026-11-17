using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.Entities;
using Colossal.PSI.Environment;
using Game;
using Game.Economy;
using Game.Prefabs;
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
        private static readonly Dictionary<string, string> TabGuids = new()
        {
            ["Service_Communication"] = "7082f45853a45ef45a2f6d544f1a4b09",
            ["Service_Education"] = "06fa933c8ca05be56b1495b5da513cfd",
            ["Service_Electricity"] = "3b317f8d46e130952fed42ea94f23358",
            ["Service_Fire"] = "5ecda3ca0b568b4e675f3a93bd43aca6",
            ["Service_Garbage"] = "849cb13c4d7dc5184b32db5ff14c1c66",
            ["Service_Healthcare"] = "aa12822734be3938ff80e2d652f28eba",
            ["Service_Landscaping"] = "2f058b873c515ef55f25ff1ef9251643",
            ["Service_Park"] = "3347feb0d4bfeb07230dc6da16621a4c",
            ["Service_Police"] = "37d54129cf9a0baaefb47bd74392d9d4",
            ["Service_Roads"] = "8419f54051cdb819145c94cc926faf5e",
            ["Service_Transportation"] = "8259f5c8a27010097451a491e1e03a98",
            ["Service_Water"] = "7abf55236992e29e3db8b046c230ffbb",
            ["Zoned_CommHigh"] = "103e6548607bec6e096a3ba76af5674e",
            ["Zoned_CommLow"] = "6d683baf98a5819559b8d5672d33dfdb",
            ["Zoned_IndAgri"] = "cd5973951221c6156484be5fceef261c",
            ["Zoned_IndFishing"] = "eb0df7be311ebc195a420982104fdf19",
            ["Zoned_IndForest"] = "c2b044ad52b9fd91d88ca7d54883c50e",
            ["Zoned_IndManu"] = "ee6415ed1209cc99d483691fdfa809d7",
            ["Zoned_IndOil"] = "7f0f00a29820fd7dd08b3adf7cfd428c",
            ["Zoned_IndOre"] = "7763066d11e3a8b7c24dd7167a4acf64",
            ["Zoned_Mixed"] = "45fb6bdf5cf76a43ba17bb9e2c5fae31",
            ["Zoned_OfficeHigh"] = "8341b919ad1885cbee78b0ad45f1c82a",
            ["Zoned_OfficeLow"] = "d2eda98feaed5cd3524194842998bfd1",
            ["Zoned_ResiHigh"] = "2e66961e86687444fb47677e24483304",
            ["Zoned_ResiLow"] = "a615db47c60d717d9f701bb6db9f19bd",
            ["Zoned_ResiLowRent"] = "afce99696f473a3e2f6e930f0c1cbaf8",
            ["Zoned_ResiMedium"] = "3daf9d6959d0d9ec0af25e5d699ddf89",
            ["Zoned_ResiMediumRow"] = "838bad7ac15c18916d7e5d39c247fc79",
        };

        private static readonly Dictionary<string, string> MenuToTab = new()
        {
            ["Roads"] = "Service_Roads",
            ["Electricity"] = "Service_Electricity",
            ["Water & Sewage"] = "Service_Water",
            ["Health & Deathcare"] = "Service_Healthcare",
            ["Garbage Management"] = "Service_Garbage",
            ["Education & Research"] = "Service_Education",
            ["Fire & Rescue"] = "Service_Fire",
            ["Police & Administration"] = "Service_Police",
            ["Transportation"] = "Service_Transportation",
            ["Parks & Recreation"] = "Service_Park",
            ["Communications"] = "Service_Communication",
            ["Landscaping"] = "Service_Landscaping",
        };

        internal static string fileFolder = $"{EnvPath.kUserDataPath}/ModsData/{Mod.Id}/";
        internal static string fileName = "StaticPloppableBuilder.json";
        internal static string fullPath = Path.Combine(fileFolder, fileName);
        static SPDataFile spDataFile;

        private bool createdFromFileThisSession = false;
        private bool dirty = false;
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
            spDataFile ??= new();
            string g = guid.ToString();

            int index = spDataFile.SpData.FindIndex(x =>
                x.prefabName == newPrefabName && x.guid == g
            );

            if (index >= 0)
                return;

            spDataFile.SpData.Add(new() { prefabName = newPrefabName, guid = g });

            dirty = true;
        }

        public void RemoveFromFile(string prefabName, Colossal.Hash128 guid)
        {
            if (spDataFile == null)
                return;
            string g = guid.ToString();

            if (spDataFile.SpData.RemoveAll(x => x.prefabName == prefabName && x.guid == g) > 0)
                dirty = true;
        }

        public void FlushFile()
        {
            if (!dirty)
                return;
            SaveFile();
            dirty = false;
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
            if (createdFromFileThisSession)
                return;

            ReadFile();
            createdFromFileThisSession = true;
            if (spDataFile == null || spDataFile.SpData.Count <= 0)
                return;

            var spData = spDataFile.SpData.ToList();

            LogHelper.SendLog($"{spData.Count} SP prefabs being created", LogLevel.DEVD);
            try
            {
                for (int i = 0; i < spData.Count; i++)
                    staticPloppableBuilder.MakeSP(spData[i]);
            }
            finally
            {
                FlushFile();
                LocaleHelper.OnActiveDictionaryChanged();
            }
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
                            if (TryGetTabByName("Zoned_ResiLow", out tab))
                                return true;
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
                                if (TryGetTabByName("Zoned_Mixed", out tab))
                                    return true;
                            }
                            if (
                                EntityManager.TryGetComponent(
                                    buildingEntity,
                                    out BuildingData bldgData
                                )
                                && bldgData.m_LotSize.x == 1
                            )
                            {
                                if (TryGetTabByName("Zoned_ResiMediumRow", out tab))
                                    return true;
                            }

                            if (TryGetTabByName("Zoned_ResiMedium", out tab))
                                return true;
                            break;
                        case ZoneDensity.High:
                            if (bldgPropertyData.m_SpaceMultiplier > 1)
                            {
                                if (TryGetTabByName("Zoned_ResiHigh", out tab))
                                    return true;
                            }

                            if (TryGetTabByName("Zoned_ResiLowRent", out tab))
                                return true;
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
                            if (TryGetTabByName("Zoned_CommLow", out tab))
                                return true;
                            break;
                        case ZoneDensity.High:
                            if (TryGetTabByName("Zoned_CommHigh", out tab))
                                return true;
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
                                if (TryGetTabByName("Zoned_OfficeLow", out tab))
                                    return true;
                                break;
                            case ZoneDensity.High:
                                if (TryGetTabByName("Zoned_OfficeHigh", out tab))
                                    return true;
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
                                if (TryGetTabByName("Zoned_IndForest", out tab))
                                    return true;
                            }
                            else if (bldgPropertyData.m_AllowedManufactured == Resource.Oil)
                            {
                                if (TryGetTabByName("Zoned_IndOil", out tab))
                                    return true;
                            }
                            else if (bldgPropertyData.m_AllowedManufactured == Resource.Fish)
                            {
                                if (TryGetTabByName("Zoned_IndFishing", out tab))
                                    return true;
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
                                ) != 0
                            )
                            {
                                if (TryGetTabByName("Zoned_IndAgri", out tab))
                                    return true;
                            }
                            else if (
                                (
                                    bldgPropertyData.m_AllowedManufactured
                                    & (Resource.Coal | Resource.Stone | Resource.Ore)
                                ) != 0
                            )
                            {
                                if (TryGetTabByName("Zoned_IndOre", out tab))
                                    return true;
                            }
                        }
                        else
                        {
                            if (TryGetTabByName("Zoned_IndManu", out tab))
                                return true;
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
                if (
                    MenuToTab.TryGetValue(menuName, out string tabName)
                    && TryGetTabByName(tabName, out tab)
                )
                    return true;
            }

            if (TryGetTabByName("Service_Landscaping", out tab))
                return true;

            return false;
        }

        private bool TryGetTabPrefab(string name, out PrefabBase prefabBase)
        {
            prefabBase = null;
            if (name == null || !TabGuids.TryGetValue(name, out string guid))
                return false;

            return prefabSystem.TryGetPrefab(
                    new PrefabID(
                        nameof(UIAssetCategoryPrefab),
                        $"StarQ_SP_{name}",
                        Colossal.Hash128.Parse(guid)
                    ),
                    out prefabBase
                )
                && prefabBase != null;
        }

        private bool TryGetTabByName(string name, out UIAssetCategoryPrefab tab)
        {
            tab = null;
            if (TryGetTabPrefab(name, out PrefabBase pb))
                tab = pb as UIAssetCategoryPrefab;
            return tab != null;
        }
    }
}
