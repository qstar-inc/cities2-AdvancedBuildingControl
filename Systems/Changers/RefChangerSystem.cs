using System;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Components.Vehicles;
using AdvancedBuildingControl.Variables;
using Game;
using Game.Economy;
using Game.Notifications;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems.Changers
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
                ReplaceEntity(entity, "", ProcessType.Loading, UpdateValueType.All);
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
            public int DepotVehicleCap { get; set; }
            public int GarbageTruckCap { get; set; }
            public int AmbulanceCap { get; set; }
            public int MediHeliCap { get; set; }
            public int HearseCap { get; set; }
            public int PatrolCarCap { get; set; }
            public int PoliceHeliCap { get; set; }
            public int PrisonVanCap { get; set; }
            public int FireTruckCap { get; set; }
            public int FireHeliCap { get; set; }
            public int EvacBusCap { get; set; }
            public int PostVanCap { get; set; }
            public int PostTruckCap { get; set; }
            public int MaintenanceVehicleCap { get; set; }
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
            public bool DepotVehicle { get; set; } = false;
            public bool GarbageFacility { get; set; } = false;
            public bool Ambulance { get; set; } = false;
            public bool MediHeli { get; set; } = false;
            public bool Hearse { get; set; } = false;
            public bool PatrolCar { get; set; }
            public bool PoliceHeli { get; set; }
            public bool PrisonVan { get; set; }
            public bool FireTruck { get; set; }
            public bool FireHeli { get; set; }
            public bool EvacBus { get; set; }
            public bool PostVan { get; set; }
            public bool PostTruck { get; set; }
            public bool MaintenanceVehicle { get; set; }
        }

        public struct EntityAltComponents
        {
            public OriginalEntity Original;
            public ABC_Storage Storage;
            public ABC_Level Level;
            public ABC_Household Household;
            public ABC_WaterPump WaterPump;
            public ABC_SewageDump SewageDump;
            public ABC_SewagePurification SewagePurification;
            public ABC_PowerPlant PowerProd;
            public ABC_DepotVehicle DepotVehicle;
            public ABC_GarbageTruck GarbageTruck;
            public ABC_Ambulance Ambulance;
            public ABC_MediHeli MediHeli;
            public ABC_Hearse Hearse;
            public ABC_PatrolCar PatrolCar;
            public ABC_PoliceHeli PoliceHeli;
            public ABC_PrisonVan PrisonVan;
            public ABC_FireTruck FireTruck;
            public ABC_FireHeli FireHeli;
            public ABC_EvacBus EvacBus;
            public ABC_PostVan PostVan;
            public ABC_PostTruck PostTruck;
            public ABC_MaintenanceVehicle MaintenanceVehicle;

            public bool HasOriginal;
            public bool HasStorage;
            public bool HasLevel;
            public bool HasHousehold;
            public bool HasWaterPump;
            public bool HasSewageDump;
            public bool HasSewagePurification;
            public bool HasPowerProd;
            public bool HasDepotVehicle;
            public bool HasGarbageTruck;
            public bool HasAmbu;
            public bool HasMediHeli;
            public bool HasHearse;
            public bool HasPatrolCar;
            public bool HasPoliceHeli;
            public bool HasPrisonVan;
            public bool HasFireTruck;
            public bool HasFireHeli;
            public bool HasEvacBus;
            public bool HasPostVan;
            public bool HasPostTruck;
            public bool HasMaintenanceVehicle;
        }

        public enum UpdateValueType
        {
            None,
            Storage,
            Level,
            Household,
            WaterPump,
            SewageCap,
            SewagePurification,
            PowerPlant,
            DepotVehicle,
            GarbageTruck,
            Ambulance,
            MediHeli,
            Hearse,
            PatrolCar,
            PoliceHeli,

            // NEW
            PrisonVan,
            FireTruck,
            FireHeli,
            EvacBus,
            PostVan,
            PostTruck,
            MaintenanceVehicle,

            // NEW END
            All,
        }

        private void CollectAvailableData(
            ABC_Storage ABC_Storage,
            ABC_Level ABC_Level,
            ABC_Household ABC_Household,
            ABC_WaterPump ABC_WaterPump,
            ABC_SewageDump ABC_SewageDump,
            ABC_SewagePurification ABC_SewagePurification,
            ABC_PowerPlant ABC_PowerPlant,
            ABC_DepotVehicle ABC_DepotVehicle,
            ABC_GarbageTruck ABC_GarbageTruck,
            ABC_Ambulance ABC_Ambulance,
            ABC_MediHeli ABC_MediHeli,
            ABC_Hearse ABC_Hearse,
            ABC_PatrolCar ABC_PatrolCar,
            ABC_PoliceHeli ABC_PoliceHeli,
            ABC_PrisonVan ABC_PrisonVan,
            ABC_FireTruck ABC_FireTruck,
            ABC_FireHeli ABC_FireHeli,
            ABC_EvacBus ABC_EvacBus,
            ABC_PostVan ABC_PostVan,
            ABC_PostTruck ABC_PostTruck,
            ABC_MaintenanceVehicle ABC_MaintenanceVehicle,
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
                entityChangeData.Level = ABC_Level.Modified;
                entityChangeFlags.Level = true;
            }
            if (!ABC_Household.IsDefault())
            {
                entityChangeData.Household = ABC_Household.Modified;
                entityChangeFlags.Household = true;
            }
            if (!ABC_WaterPump.IsDefault())
            {
                entityChangeData.WaterPumpCap = ABC_WaterPump.Modified;
                entityChangeFlags.WaterPump = true;
            }
            if (!ABC_SewageDump.IsDefault())
            {
                entityChangeData.SewageCap = ABC_SewageDump.Modified;
                entityChangeFlags.SewageCap = true;
            }
            if (!ABC_SewagePurification.IsDefault())
            {
                entityChangeData.SewagePurification = ABC_SewagePurification.Modified;
                entityChangeFlags.SewagePurification = true;
            }
            if (!ABC_PowerPlant.IsDefault())
            {
                entityChangeData.PowerProdCap = ABC_PowerPlant.Modified;
                entityChangeFlags.PowerProd = true;
            }
            if (!ABC_DepotVehicle.IsDefault())
            {
                entityChangeData.DepotVehicleCap = ABC_DepotVehicle.Modified;
                entityChangeFlags.DepotVehicle = true;
            }
            if (!ABC_GarbageTruck.IsDefault())
            {
                entityChangeData.GarbageTruckCap = ABC_GarbageTruck.Modified;
                entityChangeFlags.GarbageFacility = true;
            }
            if (!ABC_Ambulance.IsDefault())
            {
                entityChangeData.AmbulanceCap = ABC_Ambulance.Modified;
                entityChangeFlags.Ambulance = true;
            }
            if (!ABC_MediHeli.IsDefault())
            {
                entityChangeData.MediHeliCap = ABC_MediHeli.Modified;
                entityChangeFlags.MediHeli = true;
            }
            if (!ABC_Hearse.IsDefault())
            {
                entityChangeData.HearseCap = ABC_Hearse.Modified;
                entityChangeFlags.Hearse = true;
            }
            if (!ABC_PatrolCar.IsDefault())
            {
                entityChangeData.PatrolCarCap = ABC_PatrolCar.Modified;
                entityChangeFlags.PatrolCar = true;
            }
            if (!ABC_PoliceHeli.IsDefault())
            {
                entityChangeData.PoliceHeliCap = ABC_PoliceHeli.Modified;
                entityChangeFlags.PoliceHeli = true;
            }
            if (!ABC_PrisonVan.IsDefault())
            {
                entityChangeData.PrisonVanCap = ABC_PrisonVan.Modified;
                entityChangeFlags.PrisonVan = true;
            }
            if (!ABC_FireTruck.IsDefault())
            {
                entityChangeData.FireTruckCap = ABC_FireTruck.Modified;
                entityChangeFlags.FireTruck = true;
            }
            if (!ABC_FireHeli.IsDefault())
            {
                entityChangeData.FireHeliCap = ABC_FireHeli.Modified;
                entityChangeFlags.FireHeli = true;
            }
            if (!ABC_EvacBus.IsDefault())
            {
                entityChangeData.EvacBusCap = ABC_EvacBus.Modified;
                entityChangeFlags.EvacBus = true;
            }
            if (!ABC_PostVan.IsDefault())
            {
                entityChangeData.PostVanCap = ABC_PostVan.Modified;
                entityChangeFlags.PostVan = true;
            }
            if (!ABC_PostTruck.IsDefault())
            {
                entityChangeData.PostTruckCap = ABC_PostTruck.Modified;
                entityChangeFlags.PostTruck = true;
            }
            if (!ABC_MaintenanceVehicle.IsDefault())
            {
                entityChangeData.MaintenanceVehicleCap = ABC_MaintenanceVehicle.Modified;
                entityChangeFlags.MaintenanceVehicle = true;
            }
        }

        public void ReplaceEntity(
            Entity entity,
            string value,
            ProcessType processMode = ProcessType.None,
            UpdateValueType valueType = UpdateValueType.All
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
                ABC_SewagePurification ABC_SewagePurification =
                    GetComponentOrDefault<ABC_SewagePurification>(entity);
                ABC_PowerPlant ABC_PowerPlant = GetComponentOrDefault<ABC_PowerPlant>(entity);
                ABC_DepotVehicle ABC_DepotVehicle = GetComponentOrDefault<ABC_DepotVehicle>(entity);
                ABC_GarbageTruck ABC_GarbageTruck = GetComponentOrDefault<ABC_GarbageTruck>(entity);
                ABC_Ambulance ABC_Ambulance = GetComponentOrDefault<ABC_Ambulance>(entity);
                ABC_MediHeli ABC_MediHeli = GetComponentOrDefault<ABC_MediHeli>(entity);
                ABC_Hearse ABC_Hearse = GetComponentOrDefault<ABC_Hearse>(entity);
                ABC_PatrolCar ABC_PatrolCar = GetComponentOrDefault<ABC_PatrolCar>(entity);
                ABC_PoliceHeli ABC_PoliceHeli = GetComponentOrDefault<ABC_PoliceHeli>(entity);
                ABC_PrisonVan ABC_PrisonVan = GetComponentOrDefault<ABC_PrisonVan>(entity);
                ABC_FireTruck ABC_FireTruck = GetComponentOrDefault<ABC_FireTruck>(entity);
                ABC_FireHeli ABC_FireHeli = GetComponentOrDefault<ABC_FireHeli>(entity);
                ABC_EvacBus ABC_EvacBus = GetComponentOrDefault<ABC_EvacBus>(entity);
                ABC_PostVan ABC_PostVan = GetComponentOrDefault<ABC_PostVan>(entity);
                ABC_PostTruck ABC_PostTruck = GetComponentOrDefault<ABC_PostTruck>(entity);
                ABC_MaintenanceVehicle ABC_MaintenanceVehicle =
                    GetComponentOrDefault<ABC_MaintenanceVehicle>(entity);

                if (valueType == UpdateValueType.Level)
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
                if (EntityManager.HasComponent<ABC_SewageDump>(entity))
                {
                    comps.SewagePurification = ABC_SewagePurification;
                    comps.HasSewagePurification = true;
                }
                if (EntityManager.HasComponent<ABC_PowerPlant>(entity))
                {
                    comps.PowerProd = ABC_PowerPlant;
                    comps.HasPowerProd = true;
                }
                if (EntityManager.HasComponent<ABC_DepotVehicle>(entity))
                {
                    comps.DepotVehicle = ABC_DepotVehicle;
                    comps.HasDepotVehicle = true;
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
                if (EntityManager.HasComponent<ABC_PatrolCar>(entity))
                {
                    comps.PatrolCar = ABC_PatrolCar;
                    comps.HasPatrolCar = true;
                }
                if (EntityManager.HasComponent<ABC_PoliceHeli>(entity))
                {
                    comps.PoliceHeli = ABC_PoliceHeli;
                    comps.HasPoliceHeli = true;
                }
                if (EntityManager.HasComponent<ABC_PrisonVan>(entity))
                {
                    comps.PrisonVan = ABC_PrisonVan;
                    comps.HasPrisonVan = true;
                }
                if (EntityManager.HasComponent<ABC_FireTruck>(entity))
                {
                    comps.FireTruck = ABC_FireTruck;
                    comps.HasFireTruck = true;
                }
                if (EntityManager.HasComponent<ABC_FireHeli>(entity))
                {
                    comps.FireHeli = ABC_FireHeli;
                    comps.HasFireHeli = true;
                }
                if (EntityManager.HasComponent<ABC_EvacBus>(entity))
                {
                    comps.EvacBus = ABC_EvacBus;
                    comps.HasEvacBus = true;
                }
                if (EntityManager.HasComponent<ABC_PostVan>(entity))
                {
                    comps.PostVan = ABC_PostVan;
                    comps.HasPostVan = true;
                }
                if (EntityManager.HasComponent<ABC_PostTruck>(entity))
                {
                    comps.PostTruck = ABC_PostTruck;
                    comps.HasPostTruck = true;
                }
                if (EntityManager.HasComponent<ABC_MaintenanceVehicle>(entity))
                {
                    comps.MaintenanceVehicle = ABC_MaintenanceVehicle;
                    comps.HasMaintenanceVehicle = true;
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
                    ABC_SewagePurification,
                    ABC_PowerPlant,
                    ABC_DepotVehicle,
                    ABC_GarbageTruck,
                    ABC_Ambulance,
                    ABC_MediHeli,
                    ABC_Hearse,
                    ABC_PatrolCar,
                    ABC_PoliceHeli,
                    ABC_PrisonVan,
                    ABC_FireTruck,
                    ABC_FireHeli,
                    ABC_EvacBus,
                    ABC_PostVan,
                    ABC_PostTruck,
                    ABC_MaintenanceVehicle,
                    ref entityChangeData,
                    ref entityChangeFlags
                );

                switch (valueType)
                {
                    case UpdateValueType.Storage:
                        entityChangeData.ResId = (Resource)(utils.UlongFromString(value));
                        entityChangeFlags.Storage = true;
                        break;
                    case UpdateValueType.Level:
                        entityChangeData.Level = utils.IntFromString(value);
                        entityChangeFlags.Level = true;
                        break;
                    case UpdateValueType.Household:
                        entityChangeData.Household = utils.IntFromString(value);
                        entityChangeFlags.Household = true;
                        break;
                    case UpdateValueType.WaterPump:
                        entityChangeData.WaterPumpCap = utils.IntFromString(value);
                        entityChangeFlags.WaterPump = true;
                        break;
                    case UpdateValueType.SewageCap:
                        entityChangeData.SewageCap = utils.IntFromString(value);
                        entityChangeFlags.SewageCap = true;
                        break;
                    case UpdateValueType.SewagePurification:
                        entityChangeData.SewagePurification = utils.IntFromString(value);
                        entityChangeFlags.SewagePurification = true;
                        break;
                    case UpdateValueType.PowerPlant:
                        entityChangeData.PowerProdCap = utils.IntFromString(value);
                        entityChangeFlags.PowerProd = true;
                        break;
                    case UpdateValueType.DepotVehicle:
                        entityChangeData.DepotVehicleCap = utils.IntFromString(value);
                        entityChangeFlags.DepotVehicle = true;
                        break;
                    case UpdateValueType.GarbageTruck:
                        entityChangeData.GarbageTruckCap = utils.IntFromString(value);
                        entityChangeFlags.GarbageFacility = true;
                        break;
                    case UpdateValueType.Ambulance:
                        entityChangeData.AmbulanceCap = utils.IntFromString(value);
                        entityChangeFlags.Ambulance = true;
                        break;
                    case UpdateValueType.MediHeli:
                        entityChangeData.MediHeliCap = utils.IntFromString(value);
                        entityChangeFlags.MediHeli = true;
                        break;
                    case UpdateValueType.Hearse:
                        entityChangeData.HearseCap = utils.IntFromString(value);
                        entityChangeFlags.Hearse = true;
                        break;
                    case UpdateValueType.PatrolCar:
                        entityChangeData.PatrolCarCap = utils.IntFromString(value);
                        entityChangeFlags.PatrolCar = true;
                        break;
                    case UpdateValueType.PoliceHeli:
                        entityChangeData.PoliceHeliCap = utils.IntFromString(value);
                        entityChangeFlags.PoliceHeli = true;
                        break;
                    case UpdateValueType.PrisonVan:
                        entityChangeData.PrisonVanCap = utils.IntFromString(value);
                        entityChangeFlags.PrisonVan = true;
                        break;
                    case UpdateValueType.FireTruck:
                        entityChangeData.FireTruckCap = utils.IntFromString(value);
                        entityChangeFlags.FireTruck = true;
                        break;
                    case UpdateValueType.FireHeli:
                        entityChangeData.FireHeliCap = utils.IntFromString(value);
                        entityChangeFlags.FireHeli = true;
                        break;
                    case UpdateValueType.EvacBus:
                        entityChangeData.EvacBusCap = utils.IntFromString(value);
                        entityChangeFlags.EvacBus = true;
                        break;
                    case UpdateValueType.PostVan:
                        entityChangeData.PostVanCap = utils.IntFromString(value);
                        entityChangeFlags.PostVan = true;
                        break;
                    case UpdateValueType.PostTruck:
                        entityChangeData.PostTruckCap = utils.IntFromString(value);
                        entityChangeFlags.PostTruck = true;
                        break;
                    case UpdateValueType.MaintenanceVehicle:
                        entityChangeData.MaintenanceVehicleCap = utils.IntFromString(value);
                        entityChangeFlags.MaintenanceVehicle = true;
                        break;
                    case UpdateValueType.All:
                    default:
                        break;
                }

                bool isDefault =
                    ABC_Storage.IsDefault()
                    && ABC_Level.IsDefault()
                    && ABC_Household.IsDefault()
                    && ABC_WaterPump.IsDefault()
                    && ABC_SewageDump.IsDefault()
                    && ABC_SewagePurification.IsDefault()
                    && ABC_PowerPlant.IsDefault()
                    && ABC_DepotVehicle.IsDefault()
                    && ABC_GarbageTruck.IsDefault()
                    && ABC_Ambulance.IsDefault()
                    && ABC_MediHeli.IsDefault()
                    && ABC_Hearse.IsDefault()
                    && ABC_PatrolCar.IsDefault()
                    && ABC_PoliceHeli.IsDefault()
                    && ABC_PrisonVan.IsDefault()
                    && ABC_FireTruck.IsDefault()
                    && ABC_FireHeli.IsDefault()
                    && ABC_EvacBus.IsDefault()
                    && ABC_PostVan.IsDefault()
                    && ABC_PostTruck.IsDefault()
                    && ABC_MaintenanceVehicle.IsDefault()
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
                        if (valueType == UpdateValueType.All)
                        {
                            ResetToOG(entity, currentPrefabRef, entityChangeFlags, comps, false);
                            return;
                        }
                        if (valueType != UpdateValueType.All && valueType != UpdateValueType.None)
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
