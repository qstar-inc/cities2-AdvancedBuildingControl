using System;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Game;
using Game.Economy;
using Game.Notifications;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class RefChangerSystem : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;
        public PlopTheGrowableSystem plopTheGrowableSystem;
        public IconCommandSystem iconCommandSystem;
        public Utils utils;
#nullable enable
        public EntityQuery alteredEntitiesQuery;
        public IconCommandBuffer m_IconCommandBuffer;
        public EntityQuery m_BuildingSettingsQuery;
        public BuildingConfigurationData m_BuildingConfigurationData;
        public ProcessType CurrentProcessMode = ProcessType.None;

        private T GetComponentOrDefault<T>(Entity entity)
            where T : unmanaged, IComponentData
        {
            return EntityManager.HasComponent<T>(entity)
                ? EntityManager.GetComponentData<T>(entity)
                : default;
        }

        protected override void OnCreate()
        {
            prefabSystem = WorldHelper.PrefabSystem;
            createdEntitiesManagementSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
            utils = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Utils>();
            plopTheGrowableSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlopTheGrowableSystem>();
            iconCommandSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<IconCommandSystem>();

            alteredEntitiesQuery = SystemAPI.QueryBuilder().WithAny<OriginalEntity>().Build();
            m_BuildingSettingsQuery = SystemAPI
                .QueryBuilder()
                .WithAll<BuildingConfigurationData>()
                .Build();
        }

        protected override void OnUpdate() { }

        public void InitOnGameStart()
        {
            var queryArray = alteredEntitiesQuery.ToEntityArray(Allocator.Temp);
            LogHelper.SendLog(
                $"Found {queryArray.Length} entities on InitOnGameStart",
                LogLevel.DEV
            );

            for (int i = 0; i < queryArray.Length; ++i)
            {
                var entity = queryArray[i];
                //LogHelper.SendLog($"Initing {entity}", LogLevel.DEV);
                ReplaceEntity(entity, "", ProcessType.Loading, ValueType.All);
            }
        }

        public class EntityChangeData
        {
            public int Level { get; set; }
            public int Household { get; set; }
            public Resource ResId { get; set; }
            public int WaterPumpCap { get; set; }
            public int SewageCap { get; set; }
            public float SewagePurification { get; set; }
            public int PowerProdCap { get; set; }
            public int DepotCap { get; set; }
            public int GarbageTruckCap { get; set; }
            public int AmbulanceCap { get; set; }
            public int MediHeliCap { get; set; }
            public int HearseCap { get; set; }
        }

        public class EntityChangeFlag
        {
            public bool Level { get; set; } = false;
            public bool Household { get; set; } = false;
            public bool Storage { get; set; } = false;
            public bool WaterPump { get; set; } = false;
            public bool SewageCap { get; set; } = false;
            public bool SewagePurification { get; set; } = false;
            public bool PowerProd { get; set; } = false;
            public bool Depot { get; set; } = false;
            public bool GarbageFacility { get; set; } = false;
            public bool Ambulance { get; set; } = false;
            public bool MediHeli { get; set; } = false;
            public bool Hearse { get; set; } = false;
        }

        public struct EntityAltComponents
        {
            public OriginalEntity Original;
            public ABC_Storage Storage;
            public ABC_Level Level;
            public ABC_Household Household;
            public ABC_WaterPump WaterPump;
            public ABC_SewageDump SewageDump;
            public ABC_PowerPlant PowerProd;
            public ABC_TransportDepot TransportDepot;
            public ABC_GarbageTruck GarbageTruck;
            public ABC_Ambulance Ambulance;
            public ABC_MediHeli MediHeli;
            public ABC_Hearse Hearse;

            public bool HasOriginal;
            public bool HasStorage;
            public bool HasLevel;
            public bool HasHousehold;
            public bool HasWaterPump;
            public bool HasSewageDump;
            public bool HasPowerProd;
            public bool HasDepot;
            public bool HasGarbageTruck;
            public bool HasAmbu;
            public bool HasMediHeli;
            public bool HasHearse;
        }

        public enum ValueType
        {
            None,
            Storage,
            Level,
            Household,
            WaterPump,
            SewageCap,
            SewagePurification,
            PowerPlant,
            Depot,
            GarbageTruck,
            Ambulance,
            MediHeli,
            Hearse,
            All,
        }

        private void CollectAvailableData(
            ABC_Storage ABC_Storage,
            ABC_Level ABC_Level,
            ABC_Household ABC_Household,
            ABC_WaterPump ABC_WaterPump,
            ABC_SewageDump ABC_SewageDump,
            ABC_PowerPlant ABC_PowerPlant,
            ABC_TransportDepot ABC_TransportDepot,
            ABC_GarbageTruck ABC_GarbageTruck,
            ABC_Ambulance ABC_Ambulance,
            ABC_MediHeli ABC_MediHeli,
            ABC_Hearse ABC_Hearse,
            ref EntityChangeData entityChangeData,
            ref EntityChangeFlag entityChangeFlags
        )
        {
            if (!ABC_Storage.IsDefault())
            {
                entityChangeData.ResId = (Resource)ABC_Storage.Resource;
                entityChangeFlags.Storage = true;
            }
            if (!ABC_Level.IsDefault())
            {
                entityChangeData.Level = ABC_Level.Level;
                entityChangeFlags.Level = true;
            }
            if (!ABC_Household.IsDefault())
            {
                entityChangeData.Household = ABC_Household.Household;
                entityChangeFlags.Household = true;
            }
            if (!ABC_WaterPump.IsDefault())
            {
                entityChangeData.WaterPumpCap = ABC_WaterPump.Capacity;
                entityChangeFlags.WaterPump = true;
            }
            if (!ABC_SewageDump.IsDefault())
            {
                entityChangeData.SewageCap = ABC_SewageDump.Capacity;
                entityChangeData.SewagePurification = ABC_SewageDump.Purification;
                entityChangeFlags.SewageCap = true;
                entityChangeFlags.SewagePurification = true;
            }
            if (!ABC_PowerPlant.IsDefault())
            {
                entityChangeData.PowerProdCap = ABC_PowerPlant.Capacity;
                entityChangeFlags.PowerProd = true;
            }
            if (!ABC_TransportDepot.IsDefault())
            {
                entityChangeData.DepotCap = ABC_TransportDepot.Capacity;
                entityChangeFlags.Depot = true;
            }
            if (!ABC_GarbageTruck.IsDefault())
            {
                entityChangeData.GarbageTruckCap = ABC_GarbageTruck.Capacity;
                entityChangeFlags.GarbageFacility = true;
            }
            if (!ABC_Ambulance.IsDefault())
            {
                entityChangeData.AmbulanceCap = ABC_Ambulance.Capacity;
                entityChangeFlags.Ambulance = true;
            }
            if (!ABC_MediHeli.IsDefault())
            {
                entityChangeData.MediHeliCap = ABC_MediHeli.Capacity;
                entityChangeFlags.MediHeli = true;
            }
            if (!ABC_Hearse.IsDefault())
            {
                entityChangeData.HearseCap = ABC_Hearse.Capacity;
                entityChangeFlags.Hearse = true;
            }
        }

        public void ReplaceEntity(
            Entity entity,
            string value,
            ProcessType processMode = ProcessType.None,
            ValueType valueType = ValueType.All
        )
        {
            try
            {
                CurrentProcessMode = processMode;
                Entity currentPrefabRef = Entity.Null;
                if (!utils.CheckPrefab(entity, ref currentPrefabRef))
                    return;

                //EntityManager.TryGetComponent(entity, out OriginalEntity originalEntity);
                ABC_Storage ABC_Storage = GetComponentOrDefault<ABC_Storage>(entity);
                ABC_Level ABC_Level = GetComponentOrDefault<ABC_Level>(entity);
                ABC_Household ABC_Household = GetComponentOrDefault<ABC_Household>(entity);
                ABC_WaterPump ABC_WaterPump = GetComponentOrDefault<ABC_WaterPump>(entity);
                ABC_SewageDump ABC_SewageDump = GetComponentOrDefault<ABC_SewageDump>(entity);
                ABC_PowerPlant ABC_PowerPlant = GetComponentOrDefault<ABC_PowerPlant>(entity);
                ABC_TransportDepot ABC_TransportDepot = GetComponentOrDefault<ABC_TransportDepot>(
                    entity
                );
                ABC_GarbageTruck ABC_GarbageTruck = GetComponentOrDefault<ABC_GarbageTruck>(entity);
                ABC_Ambulance ABC_Ambulance = GetComponentOrDefault<ABC_Ambulance>(entity);
                ABC_MediHeli ABC_MediHeli = GetComponentOrDefault<ABC_MediHeli>(entity);
                ABC_Hearse ABC_Hearse = GetComponentOrDefault<ABC_Hearse>(entity);

                if (valueType == ValueType.Level)
                {
                    m_IconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                    m_BuildingConfigurationData =
                        m_BuildingSettingsQuery.GetSingleton<BuildingConfigurationData>();
                }

                EntityAltComponents comps = new();

                if (EntityManager.HasComponent<OriginalEntity>(entity))
                {
                    comps.Original = EntityManager.GetComponentData<OriginalEntity>(entity);
                    comps.HasOriginal = true;
                }
                if (EntityManager.HasComponent<ABC_Storage>(entity))
                {
                    comps.Storage = ABC_Storage;
                    comps.HasStorage = true;
                }
                if (EntityManager.HasComponent<ABC_Level>(entity))
                {
                    comps.Level = ABC_Level;
                    comps.HasLevel = true;
                }
                if (EntityManager.HasComponent<ABC_Household>(entity))
                {
                    comps.Household = ABC_Household;
                    comps.HasHousehold = true;
                }
                if (EntityManager.HasComponent<ABC_WaterPump>(entity))
                {
                    comps.WaterPump = ABC_WaterPump;
                    comps.HasWaterPump = true;
                }
                if (EntityManager.HasComponent<ABC_SewageDump>(entity))
                {
                    comps.SewageDump = ABC_SewageDump;
                    comps.HasSewageDump = true;
                }
                if (EntityManager.HasComponent<ABC_PowerPlant>(entity))
                {
                    comps.PowerProd = ABC_PowerPlant;
                    comps.HasPowerProd = true;
                }
                if (EntityManager.HasComponent<ABC_TransportDepot>(entity))
                {
                    comps.TransportDepot = ABC_TransportDepot;
                    comps.HasDepot = true;
                }
                if (EntityManager.HasComponent<ABC_GarbageTruck>(entity))
                {
                    comps.GarbageTruck = ABC_GarbageTruck;
                    comps.HasGarbageTruck = true;
                }
                if (EntityManager.HasComponent<ABC_Ambulance>(entity))
                {
                    comps.Ambulance = ABC_Ambulance;
                    comps.HasAmbu = true;
                }
                if (EntityManager.HasComponent<ABC_MediHeli>(entity))
                {
                    comps.MediHeli = ABC_MediHeli;
                    comps.HasMediHeli = true;
                }
                if (EntityManager.HasComponent<ABC_Hearse>(entity))
                {
                    comps.Hearse = ABC_Hearse;
                    comps.HasHearse = true;
                }

                //Dictionary<string, object>? components = new(6)
                //{
                //    { ComponentKeys.Original, originalEntity },
                //    { ComponentKeys.Storage, altStorage },
                //    { ComponentKeys.Level, altLevel },
                //    { ComponentKeys.Household, altHousehold },
                //    { ComponentKeys.WaterPump, altWaterPump },
                //    { ComponentKeys.SewageDump, altSewageDump },
                //};

                EntityChangeData entityChangeData = new();
                EntityChangeFlag entityChangeFlags = new();

                CollectAvailableData(
                    ABC_Storage,
                    ABC_Level,
                    ABC_Household,
                    ABC_WaterPump,
                    ABC_SewageDump,
                    ABC_PowerPlant,
                    ABC_TransportDepot,
                    ABC_GarbageTruck,
                    ABC_Ambulance,
                    ABC_MediHeli,
                    ABC_Hearse,
                    ref entityChangeData,
                    ref entityChangeFlags
                );

                switch (valueType)
                {
                    case ValueType.Storage:
                        entityChangeData.ResId = (Resource)(utils.UlongFromString(value));
                        entityChangeFlags.Storage = true;
                        break;
                    case ValueType.Level:
                        entityChangeData.Level = utils.IntFromString(value);
                        entityChangeFlags.Level = true;
                        break;
                    case ValueType.Household:
                        entityChangeData.Household = utils.IntFromString(value);
                        entityChangeFlags.Household = true;
                        break;
                    case ValueType.WaterPump:
                        entityChangeData.WaterPumpCap = utils.IntFromString(value);
                        entityChangeFlags.WaterPump = true;
                        break;
                    case ValueType.SewageCap:
                        entityChangeData.SewageCap = utils.IntFromString(value);
                        entityChangeFlags.SewageCap = true;
                        break;
                    case ValueType.SewagePurification:
                        entityChangeData.SewagePurification = utils.IntFromString(value);
                        entityChangeFlags.SewagePurification = true;
                        break;
                    case ValueType.PowerPlant:
                        entityChangeData.PowerProdCap = utils.IntFromString(value);
                        entityChangeFlags.PowerProd = true;
                        break;
                    case ValueType.Depot:
                        entityChangeData.DepotCap = utils.IntFromString(value);
                        entityChangeFlags.Depot = true;
                        break;
                    case ValueType.GarbageTruck:
                        entityChangeData.GarbageTruckCap = utils.IntFromString(value);
                        entityChangeFlags.GarbageFacility = true;
                        break;
                    case ValueType.Ambulance:
                        entityChangeData.AmbulanceCap = utils.IntFromString(value);
                        entityChangeFlags.Ambulance = true;
                        break;
                    case ValueType.MediHeli:
                        entityChangeData.MediHeliCap = utils.IntFromString(value);
                        entityChangeFlags.MediHeli = true;
                        break;
                    case ValueType.Hearse:
                        entityChangeData.HearseCap = utils.IntFromString(value);
                        entityChangeFlags.Hearse = true;
                        break;
                    case ValueType.All:
                    default:
                        break;
                }

                bool isDefault =
                    ABC_Storage.IsDefault()
                    && ABC_Level.IsDefault()
                    && ABC_Household.IsDefault()
                    && ABC_WaterPump.IsDefault()
                    && ABC_SewageDump.IsDefault()
                    && ABC_PowerPlant.IsDefault()
                    && ABC_TransportDepot.IsDefault()
                    && ABC_GarbageTruck.IsDefault()
                    && ABC_Ambulance.IsDefault()
                    && ABC_MediHeli.IsDefault()
                    && ABC_Hearse.IsDefault()
                    && 1 == 1;

                switch (processMode)
                {
                    case ProcessType.None:
                        return;
                    case ProcessType.Loading:
                        AddToNew(
                            entity,
                            currentPrefabRef,
                            entityChangeData,
                            entityChangeFlags,
                            comps
                        );
                        return;
                    case ProcessType.Update:
                        if (isDefault)
                        {
                            AddToNew(
                                entity,
                                currentPrefabRef,
                                entityChangeData,
                                entityChangeFlags,
                                comps
                            );
                            return;
                        }
                        UpdateExisting(
                            entity,
                            currentPrefabRef,
                            entityChangeData,
                            entityChangeFlags,
                            comps
                        );
                        return;
                    case ProcessType.Reset:
                        if (valueType == ValueType.All)
                        {
                            ResetToOG(entity, currentPrefabRef, entityChangeFlags, comps, false);
                            return;
                        }
                        if (valueType != ValueType.All && valueType != ValueType.None)
                        {
                            RemoveSingle(entity, currentPrefabRef, valueType, comps);
                            return;
                        }
                        return;
                    case ProcessType.Saving:
                        ResetToOG(entity, currentPrefabRef, entityChangeFlags, comps, true);
                        return;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void SendIconCommand(int levelOld, int levelNew, Entity entity)
        {
            if (
                CurrentProcessMode == ProcessType.Loading
                || CurrentProcessMode == ProcessType.Saving
            )
                return;
            int levelDiff = levelNew - levelOld;
            if (levelDiff > 0)
                m_IconCommandBuffer.Add(
                    entity,
                    m_BuildingConfigurationData.m_LevelUpNotification,
                    IconPriority.Info,
                    IconClusterLayer.Transaction,
                    0,
                    default,
                    false,
                    false,
                    false,
                    0f
                );
            else if (levelDiff < 0)
                m_IconCommandBuffer.Add(
                    entity,
                    m_BuildingConfigurationData.m_TurnedOffNotification,
                    IconPriority.Info,
                    IconClusterLayer.Transaction,
                    0,
                    default,
                    false,
                    false,
                    false,
                    0f
                );
        }
    }
}
