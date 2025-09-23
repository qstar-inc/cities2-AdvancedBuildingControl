using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.IO.AssetDatabase.Internal;
using Game;
using Game.Buildings;
using Game.Economy;
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
        public Utils utils;
#nullable enable
        public EntityQuery alteredEntitiesQuery;

        protected override void OnCreate()
        {
            alteredEntitiesQuery = SystemAPI
                .QueryBuilder()
                .WithAny<AltStorage, AltLevel, AltHousehold>()
                .Build();
            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
            createdEntitiesManagementSystem =
                Mod.world.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
            utils = Mod.world.GetOrCreateSystemManaged<Utils>();
            plopTheGrowableSystem = Mod.world.GetOrCreateSystemManaged<PlopTheGrowableSystem>();
            RequireForUpdate(alteredEntitiesQuery);
        }

        protected override void OnUpdate() { }

        public void InitOnGameStart()
        {
            var queryArray = alteredEntitiesQuery.ToEntityArray(Allocator.Temp);
            LogHelper.SendLog(
                $"Found {queryArray.Length} entities on InitOnGameStart",
                LogLevel.DEV
            );

            foreach (var entity in queryArray)
            {
                LogHelper.SendLog($"Initing {entity}", LogLevel.DEV);
                ReplaceEntity(entity, "", ProcessMode.Loading, ValueType.All);
            }
        }

        public class DataSLR
        {
            public int Level { get; set; }
            public int Household { get; set; }
            public Resource ResId { get; set; }
        }

        public class DoSLR
        {
            public bool Level { get; set; } = false;
            public bool Household { get; set; } = false;
            public bool Storage { get; set; } = false;
        }

        public void AddToNew(
            Entity entity,
            Entity currentPrefabRef,
            DataSLR dataSLR,
            DoSLR doSLR,
            Dictionary<string, object> components
        )
        {
            var ogComponent = (OriginalEntity)components["originalEntity"];
            string newName;
            if (ogComponent.OGEntity.ToString() != string.Empty)
            {
                newName = ogComponent.OGEntity.ToString();
            }
            else
            {
                newName = prefabSystem.GetPrefabName(currentPrefabRef);
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase _
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
                return;
            }

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            (bool hasStorage, bool hasSpawnable, bool hasProperty) = (false, false, false);
            if (EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd))
                hasStorage = true;

            if (EntityManager.TryGetComponent(newPrefabEntity, out SpawnableBuildingData sbd))
                hasSpawnable = true;

            if (
                EntityManager.TryGetComponent(newPrefabEntity, out BuildingPropertyData bpd)
                && EntityManager.HasComponent<ResidentialProperty>(entity)
            )
                hasProperty = true;

            if (hasProperty)
            {
                bool skip = false;
                var component = (AltHousehold)components["alteredHousehold"];
                int resiPropTemp = 0;
                if (component.IsDefault())
                {
                    if (doSLR.Household)
                    {
                        resiPropTemp = dataSLR.Household;
                    }
                    else
                    {
                        skip = true;
                    }
                }
                else
                {
                    resiPropTemp = component.Household;
                }

                if (!skip && resiPropTemp != 0)
                {
                    bpd.m_ResidentialProperties = resiPropTemp;
                    //bpd.m_SpaceMultiplier = 1;

                    component.Household = resiPropTemp;
                    component.Enabled = true;
                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, bpd);
                }
            }

            if (hasSpawnable)
            {
                bool skip = false;
                var component = (AltLevel)components["alteredLevel"];
                byte levelTemp = 0;
                if (component.IsDefault())
                {
                    if (doSLR.Level)
                    {
                        levelTemp = (byte)dataSLR.Level;
                    }
                    else
                    {
                        skip = true;
                    }
                }
                else
                {
                    levelTemp = (byte)component.Level;
                }

                if (!skip && levelTemp != 0)
                {
                    sbd.m_Level = levelTemp;

                    component.Level = levelTemp;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, sbd);
                    plopTheGrowableSystem.LockLevelWithPTG(entity);
                    utils.UpdateUpkeep(newPrefabEntity, dataSLR.Level, sbd.m_ZonePrefab);
                }
            }

            if (hasStorage)
            {
                bool skip = false;
                var component = (AltStorage)components["alteredStorage"];
                Resource resTemp = scd.m_StoredResources;
                if (component.IsDefault())
                {
                    if (doSLR.Storage)
                    {
                        if ((resTemp & dataSLR.ResId) != 0)
                        {
                            resTemp &= ~dataSLR.ResId;
                        }
                        else
                        {
                            resTemp |= dataSLR.ResId;
                        }
                    }
                    else
                    {
                        skip = true;
                    }
                }
                else
                {
                    resTemp = (Resource)component.Resourse;
                }

                if (!skip)
                {
                    scd.m_StoredResources = resTemp;

                    utils.RemoveCurrentResource(entity, scd.m_StoredResources);

                    component.Resourse = (ulong)scd.m_StoredResources;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, scd);
                }
            }

            OriginalEntity originalEntity = new() { OGEntity = newName };
            EntityManager.AddComponentData(entity, originalEntity);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);
        }

        public void UpdateExisting(
            Entity entity,
            Entity currentPrefabRef,
            DataSLR dataSLR,
            DoSLR doSLR,
            Dictionary<string, object> components
        )
        {
            var ogComponent = (OriginalEntity)components["originalEntity"];
            string newName;
            if (ogComponent.OGEntity.ToString() != string.Empty)
            {
                newName = ogComponent.OGEntity.ToString();
            }
            else
            {
                newName = prefabSystem.GetPrefabName(currentPrefabRef);
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase prefabBase
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
                return;
            }

            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            (bool hasStorage, bool hasSpawnable, bool hasProperty) = (false, false, false);
            if (EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd))
                hasStorage = true;

            if (EntityManager.TryGetComponent(newPrefabEntity, out SpawnableBuildingData sbd))
                hasSpawnable = true;

            if (
                EntityManager.TryGetComponent(newPrefabEntity, out BuildingPropertyData bpd)
                && EntityManager.HasComponent<ResidentialProperty>(entity)
            )
                hasProperty = true;

            if (
                hasProperty
                && doSLR.Household
                && components.TryGetValue("alteredHousehold", out AltHousehold alteredHousehold)
            )
            {
                //var component = (AltHousehold)components["alteredHousehold"];

                bpd.m_ResidentialProperties = Math.Max(1, dataSLR.Household);
                //bpd.m_SpaceMultiplier = 1;
                alteredHousehold.Household = bpd.m_ResidentialProperties;
                alteredHousehold.Enabled = true;

                EntityManager.AddComponentData(entity, alteredHousehold);
                EntityManager.SetComponentData(newPrefabEntity, bpd);
            }

            if (
                hasSpawnable
                && doSLR.Level
                && components.TryGetValue("alteredLevel", out AltLevel alteredLevel)
            )
            {
                //var component = (AltLevel)components["alteredLevel"];
                sbd.m_Level = (byte)dataSLR.Level;
                alteredLevel.Level = sbd.m_Level;
                alteredLevel.Enabled = true;

                EntityManager.AddComponentData(entity, alteredLevel);
                EntityManager.SetComponentData(newPrefabEntity, sbd);

                plopTheGrowableSystem.LockLevelWithPTG(entity);
                utils.UpdateUpkeep(newPrefabEntity, dataSLR.Level, sbd.m_ZonePrefab);
            }

            if (
                hasStorage
                && doSLR.Storage
                && components.TryGetValue("alteredStorage", out AltStorage alteredStorage)
            )
            {
                //var component = (AltStorage)components["alteredStorage"];
                Resource resTemp = (Resource)alteredStorage.Resourse;

                if ((resTemp & dataSLR.ResId) != 0)
                {
                    resTemp &= ~dataSLR.ResId;
                }
                else
                {
                    resTemp |= dataSLR.ResId;
                }

                scd.m_StoredResources = resTemp;
                alteredStorage.Resourse = (ulong)resTemp;
                alteredStorage.Enabled = true;

                Resource resTempX = scd.m_StoredResources;

                utils.RemoveCurrentResource(entity, resTempX);

                EntityManager.AddComponentData(entity, alteredStorage);
                EntityManager.SetComponentData(newPrefabEntity, scd);
            }

            OriginalEntity originalEntity = new() { OGEntity = newName };
            EntityManager.AddComponentData(entity, originalEntity);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
            {
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);
            }

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog(
                $"Done processing {entity}, replacing {newPrefabEntity}",
                LogLevel.DEV
            );
        }

        public void ResetToOG(
            Entity entity,
            Entity currentPrefabRef,
            DoSLR doSLR,
            Dictionary<string, object> components,
            bool keepComponent
        )
        {
            var ogComponent = (OriginalEntity)components["originalEntity"];
            string newName;
            if (ogComponent.OGEntity.ToString() != string.Empty)
            {
                newName = ogComponent.OGEntity.ToString();
            }
            else
            {
                newName = prefabSystem.GetPrefabName(currentPrefabRef);
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase prefabBase
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
                return;
            }

            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            //(bool hasStorage, bool hasSpawnable, bool hasProperty) = (false, false, false);

            if (doSLR.Household && doSLR.Storage && doSLR.Level)
            {
                if (!keepComponent)
                {
                    EntityManager.RemoveComponent<AltStorage>(entity);
                    EntityManager.RemoveComponent<AltLevel>(entity);
                    EntityManager.RemoveComponent<AltHousehold>(entity);
                    EntityManager.RemoveComponent<OriginalEntity>(entity);
                }
                //EntityManager.SetComponentData(
                //    entity,
                //    new PrefabRef() { m_Prefab = ogPrefabEntity }
                //);
            }
            else
            {
                DataSLR dataSLR = new();
                if (
                    //var component = (AltStorage)components["alteredStorage"];
                    components.TryGetValue("alteredStorage", out AltStorage alteredStorage)
                    //EntityManager.TryGetComponent(entity, out AltStorage alteredStorage)

                    && alteredStorage.Enabled
                    && doSLR.Storage
                )
                {
                    dataSLR.ResId = (Resource)alteredStorage.Resourse;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<AltStorage>(entity);
                        components.Remove("alteredStorage");
                    }
                }

                if (
                    components.TryGetValue("alteredLevel", out AltLevel alteredLevel)
                    //EntityManager.TryGetComponent(entity, out AltLevel alteredLevel)
                    && alteredLevel.Enabled
                )
                {
                    doSLR.Level = true;
                    dataSLR.Level = alteredLevel.Level;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<AltLevel>(entity);
                        components.Remove("alteredLevel");
                    }
                }

                if (
                    components.TryGetValue("alteredHousehold", out AltHousehold alteredHousehold)
                    //EntityManager.TryGetComponent(entity, out AltHousehold alteredHousehold)
                    && alteredHousehold.Enabled
                    && doSLR.Household
                )
                {
                    dataSLR.Household = alteredHousehold.Household;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<AltHousehold>(entity);
                        components.Remove("alteredHousehold");
                    }
                }

                //UpdateExisting(entity, ogPrefabEntity, dataSLR, doSLR, components);
            }

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
            {
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);
            }

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);
        }

        public void RemoveSingle(
            Entity entity,
            Entity currentPrefabRef,
            ValueType valueType,
            Dictionary<string, object> components
        )
        {
            var ogComponent = (OriginalEntity)components["originalEntity"];
            string newName;
            if (ogComponent.OGEntity.ToString() != string.Empty)
            {
                newName = ogComponent.OGEntity.ToString();
            }
            else
            {
                newName = prefabSystem.GetPrefabName(currentPrefabRef);
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase prefabBase
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
                return;
            }

            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            (bool hasStorage, bool hasSpawnable, bool hasProperty) = (false, false, false);

            if (EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd))
                hasStorage = true;

            if (EntityManager.TryGetComponent(newPrefabEntity, out SpawnableBuildingData sbd))
                hasSpawnable = true;

            if (
                EntityManager.TryGetComponent(newPrefabEntity, out BuildingPropertyData bpd)
                && EntityManager.HasComponent<ResidentialProperty>(entity)
            )
                hasProperty = true;

            EntityManager.TryGetComponent(ogPrefabEntity, out SpawnableBuildingData sbd0);
            EntityManager.TryGetComponent(ogPrefabEntity, out BuildingPropertyData bpd0);
            EntityManager.TryGetComponent(ogPrefabEntity, out StorageCompanyData scd0);

            components.TryGetValue("alteredLevel", out AltLevel alteredLevel);
            components.TryGetValue("alteredHousehold", out AltHousehold alteredHousehold);
            components.TryGetValue("alteredStorage", out AltStorage alteredStorage);

            (bool skipLevel, bool skipHousehold, bool skipStorage) = (false, false, false);
            switch (valueType)
            {
                case ValueType.Level:
                    if (!alteredLevel.IsDefault() && alteredLevel.Enabled && hasSpawnable)
                    {
                        sbd.m_Level = sbd0.m_Level;
                        EntityManager.RemoveComponent<AltLevel>(entity);
                        //alteredLevel.Enabled = false;
                        //alteredLevel.Level = sbd0.m_Level;
                        //EntityManager.AddComponentData(entity, alteredLevel);
                        EntityManager.AddComponentData(newPrefabEntity, sbd);
                    }
                    skipLevel = true;
                    break;
                case ValueType.Household:
                    if (!alteredHousehold.IsDefault() && alteredHousehold.Enabled && hasProperty)
                    {
                        bpd.m_ResidentialProperties = bpd0.m_ResidentialProperties;
                        EntityManager.RemoveComponent<AltHousehold>(entity);
                        //alteredHousehold.Enabled = false;
                        //alteredHousehold.Household = bpd0.m_ResidentialProperties;
                        //EntityManager.AddComponentData(entity, alteredHousehold);
                        EntityManager.AddComponentData(newPrefabEntity, bpd);
                    }
                    skipHousehold = true;
                    break;
                case ValueType.Storage:
                    if (!alteredStorage.IsDefault() && alteredStorage.Enabled && hasStorage)
                    {
                        scd.m_StoredResources = scd0.m_StoredResources;
                        EntityManager.RemoveComponent<AltStorage>(entity);
                        //alteredStorage.Enabled = false;
                        //alteredStorage.Resourse = (ulong)scd0.m_StoredResources;
                        //EntityManager.AddComponentData(entity, alteredStorage);
                        EntityManager.AddComponentData(newPrefabEntity, scd);
                    }
                    skipStorage = true;
                    break;
                default:
                    LogHelper.SendLog($"Failed removing one of {entity}!", LogLevel.DEV);
                    return;
            }

            if (!skipLevel && !alteredLevel.IsDefault() && alteredLevel.Enabled && hasSpawnable)
                sbd.m_Level = (byte)alteredLevel.Level;

            if (
                !skipHousehold
                && !alteredHousehold.IsDefault()
                && alteredHousehold.Enabled
                && hasProperty
            )
                bpd.m_ResidentialProperties = alteredHousehold.Household;

            if (!skipStorage && !alteredStorage.IsDefault() && alteredStorage.Enabled && hasStorage)
                scd.m_StoredResources = (Resource)alteredStorage.Resourse;

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
            {
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);
            }

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);
        }

        public enum ValueType
        {
            None,
            Storage,
            Level,
            Household,
            All,
        }

        public void ReplaceEntity(
            Entity entity,
            string value,
            ProcessMode processMode = ProcessMode.None,
            ValueType valueType = ValueType.All
        )
        {
            Entity currentPrefabRef = Entity.Null;
            if (!utils.CheckPrefab(entity, ref currentPrefabRef))
                return;

            EntityManager.TryGetComponent(entity, out OriginalEntity originalEntity);
            EntityManager.TryGetComponent(entity, out AltStorage alteredStorage);
            EntityManager.TryGetComponent(entity, out AltLevel alteredLevel);
            EntityManager.TryGetComponent(entity, out AltHousehold alteredHousehold);

            var components = new Dictionary<string, object>
            {
                { "originalEntity", originalEntity },
                { "alteredStorage", alteredStorage },
                { "alteredLevel", alteredLevel },
                { "alteredHousehold", alteredHousehold },
            };

            DataSLR dataSLR = new();
            DoSLR doSLR = new();
            switch (valueType)
            {
                case ValueType.Storage:
                    //doSLR.Level = alteredLevel.Enabled;
                    //dataSLR.Level = alteredLevel.Level;
                    //doSLR.Household = alteredHousehold.Enabled;
                    //dataSLR.Household = alteredHousehold.Household;
                    //doSLR.Storage = true;
                    //dataSLR.ResId = (Resource)(utils.UlongFromString(value));
                    dataSLR.ResId = (Resource)(utils.UlongFromString(value));
                    doSLR.Storage = true;

                    if (!alteredLevel.IsDefault())
                    {
                        dataSLR.Level = alteredLevel.Level;
                        doSLR.Level = true;
                    }
                    if (!alteredHousehold.IsDefault())
                    {
                        dataSLR.Household = alteredHousehold.Household;
                        doSLR.Household = true;
                    }
                    break;
                case ValueType.Level:
                    //doSLR.Level = true;
                    //dataSLR.Level = utils.IntFromString(value);
                    //doSLR.Household = alteredHousehold.Enabled;
                    //dataSLR.Household = alteredHousehold.Household;
                    //doSLR.Storage = alteredStorage.Enabled;
                    //dataSLR.ResId = (Resource)alteredStorage.NewRes;
                    if (!alteredStorage.IsDefault())
                    {
                        dataSLR.ResId = (Resource)alteredStorage.Resourse;
                        doSLR.Storage = true;
                    }
                    dataSLR.Level = utils.IntFromString(value);
                    doSLR.Level = true;

                    if (!alteredHousehold.IsDefault())
                    {
                        dataSLR.Household = alteredHousehold.Household;
                        doSLR.Household = true;
                    }
                    break;
                case ValueType.Household:
                    //doSLR.Level = alteredLevel.Enabled;
                    //dataSLR.Level = alteredLevel.Level;
                    //doSLR.Household = true;
                    //dataSLR.Household = utils.IntFromString(value);
                    //doSLR.Storage = alteredStorage.Enabled;
                    //dataSLR.ResId = (Resource)alteredStorage.NewRes;
                    if (!alteredStorage.IsDefault())
                    {
                        dataSLR.ResId = (Resource)alteredStorage.Resourse;
                        doSLR.Storage = true;
                    }
                    if (!alteredLevel.IsDefault())
                    {
                        dataSLR.Level = alteredLevel.Level;
                        doSLR.Level = true;
                    }
                    dataSLR.Household = utils.IntFromString(value);
                    doSLR.Household = true;

                    break;
                case ValueType.All:
                    //doSLR.Level = alteredLevel.Enabled;
                    //dataSLR.Level = alteredLevel.Level;
                    //doSLR.Household = alteredHousehold.Enabled;
                    //dataSLR.Household = alteredHousehold.Household;
                    //doSLR.Storage = alteredStorage.Enabled;
                    //dataSLR.ResId = (Resource)alteredStorage.NewRes;
                    if (!alteredStorage.IsDefault())
                    {
                        dataSLR.ResId = (Resource)alteredStorage.Resourse;
                        doSLR.Storage = true;
                    }
                    if (!alteredLevel.IsDefault())
                    {
                        dataSLR.Level = alteredLevel.Level;
                        doSLR.Level = true;
                    }
                    if (!alteredHousehold.IsDefault())
                    {
                        dataSLR.Household = alteredHousehold.Household;
                        doSLR.Household = true;
                    }
                    break;
                default:
                    break;
            }

            //if (!alteredStorage.CompareTo(new AlteredStorage()))
            //{
            //    dataSLR.ResId = (Resource)alteredStorage.NewRes;
            //}
            //if (!alteredLevel.CompareTo(new AlteredLevel()))
            //{
            //    dataSLR.Level = alteredLevel.Level;
            //}
            //if (!alteredHousehold.CompareTo(new AlteredHousehold()))
            //{
            //    dataSLR.Household = alteredHousehold.Household;
            //}

            bool isDefault = false;
            if (
                alteredStorage.IsDefault()
                && alteredLevel.IsDefault()
                && alteredHousehold.IsDefault()
            )
                isDefault = true;

            switch (processMode)
            {
                case ProcessMode.None:
                    return;
                case ProcessMode.Loading:
                    AddToNew(entity, currentPrefabRef, dataSLR, doSLR, components);
                    return;
                case ProcessMode.Update:
                    if (isDefault)
                    {
                        AddToNew(entity, currentPrefabRef, dataSLR, doSLR, components);
                        return;
                    }
                    UpdateExisting(entity, currentPrefabRef, dataSLR, doSLR, components);
                    return;
                case ProcessMode.Reset:
                    if (valueType == ValueType.All)
                    {
                        ResetToOG(entity, currentPrefabRef, doSLR, components, false);
                        return;
                    }
                    if (valueType != ValueType.All && valueType != ValueType.None)
                    {
                        RemoveSingle(entity, currentPrefabRef, valueType, components);
                        return;
                    }
                    return;
                case ProcessMode.Saving:
                    ResetToOG(entity, currentPrefabRef, doSLR, components, true);
                    return;
                default:
                    return;
            }
        }
    }
}
