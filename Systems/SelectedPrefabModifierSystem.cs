using System;
using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Agents;
using Game.Common;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public class PrefabChange
    {
        public UpdateValueType ValueType { get; set; }
        public ModifiedPrefab Modifications { get; set; }
    }

    public class CreatedEntitiesData
    {
        public Entity CreatedEntity { get; set; }
        public List<PrefabChange> PrefabChanges { get; set; }
    }

    public partial class SelectedPrefabModifierSystem : GameSystemBase
    {
        public BufferControlSystem bufferControlSystem;
        public Utils utils;

        public Dictionary<Entity, CreatedEntitiesData> localEntities;

        protected override void OnCreate()
        {
            base.OnCreate();
            bufferControlSystem = WorldHelper.GetSystem<BufferControlSystem>();
            utils = WorldHelper.GetSystem<Utils>();
            localEntities = new();
        }

        protected override void OnUpdate() { }

        public void Modify(
            Entity selectedPrefab,
            string value,
            UpdateValueType valueType,
            bool isReset = false
        )
        {
            try
            {
                LogHelper.SendLog(
                    $"Starting modify for {PrefabHelper.GetPrefabName(selectedPrefab)}: {valueType}, {value}",
                    LogLevel.DEVD
                );

                if (valueType == UpdateValueType._All)
                {
                    ResetAll(selectedPrefab);
                    return;
                }

                if (!UVTHelper.TryConvertValue(value, valueType, out long modifiedValue))
                {
                    LogHelper.SendLog($"Failed to convert `{value}` to long for {valueType}");
                    return;
                }

                LogHelper.SendLog(
                    $"Converted {valueType} {value} to (long){modifiedValue}",
                    LogLevel.DEVD
                );

                if (!TryApply(selectedPrefab, valueType, modifiedValue, out long originalValue))
                    return;

                ModifiedPrefab entry;
                if (
                    bufferControlSystem.TryGetEntryFromBuffer(
                        selectedPrefab,
                        valueType,
                        out var existingEntry
                    )
                )
                {
                    entry = existingEntry;
                    if (modifiedValue == entry.Original)
                    {
                        bufferControlSystem.TryRemoveFromBuffer(selectedPrefab, valueType);
                        RemoveFromDict(selectedPrefab, valueType);
                    }
                    else
                    {
                        entry.Modified = modifiedValue;
                        entry.IsEnabled = true;
                        bufferControlSystem.TryAddOrReplaceToBuffer(selectedPrefab, entry);
                        AddChangesToDict(selectedPrefab, entry);
                    }
                }
                else
                {
                    if (isReset)
                    {
                        RemoveFromDict(selectedPrefab, valueType);
                        LogHelper.SendLog(
                            $"Resetting dict for {PrefabHelper.GetPrefabName(selectedPrefab)}: {valueType}, {value}",
                            LogLevel.DEVD
                        );
                        return;
                    }
                    Entity newEntity = Entity.Null;

                    if (!TryAddOrCreateEntity(selectedPrefab, out newEntity))
                        return;

                    entry = default;
                    entry.ValueType = valueType;
                    entry.ModEntity = newEntity;
                    entry.Original = originalValue;
                    entry.Modified = modifiedValue;
                    entry.IsEnabled = true;
                    bufferControlSystem.TryAddOrReplaceToBuffer(selectedPrefab, entry);
                    AddChangesToDict(selectedPrefab, entry);
                }
                EntityManager.AddComponent<LocalModified>(selectedPrefab);
                LogHelper.SendLog(
                    $"Set {valueType} from (long){entry.Original} to (long){modifiedValue} for {PrefabHelper.GetPrefabName(selectedPrefab)}"
                );

                LogHelper.SendLog(
                    $"Ending modify for {PrefabHelper.GetPrefabName(selectedPrefab)}: {valueType}, {value}",
                    LogLevel.DEVD
                );
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public bool TryAddOrCreateEntity(Entity selectedPrefab, out Entity newEntity)
        {
            if (
                localEntities.TryGetValue(selectedPrefab, out CreatedEntitiesData ced)
                && ced.CreatedEntity != Entity.Null
            )
            {
                newEntity = ced.CreatedEntity;
                LogHelper.SendLog($"Existing entity found", LogLevel.DEVD);
                if (!EntityManager.HasComponent<LocalEntities>(newEntity))
                    EntityManager.AddComponent<LocalEntities>(newEntity);
                if (
                    !EntityManager.TryGetComponent(newEntity, out PrefabRef prefabRef)
                    || prefabRef.m_Prefab != selectedPrefab
                )
                    EntityManager.SetComponentData(newEntity, new PrefabRef(selectedPrefab));
                return true;
            }

            newEntity = EntityManager.CreateEntity();
            LogHelper.SendLog($"Creating new entity", LogLevel.DEVD);
            EntityManager.AddComponentData(newEntity, new PrefabRef(selectedPrefab));
            EntityManager.AddComponent<LocalEntities>(newEntity);
            localEntities[selectedPrefab] = new() { CreatedEntity = newEntity };
            return true;
        }

        public void AdditionalPostApply(
            Entity selectedPrefab,
            long modifiedValue,
            UpdateValueType valueType
        )
        {
            switch (valueType)
            {
                case UpdateValueType.ConsumptionData_Upkeep:

                    if (
                        !EntityManager.TryGetBuffer(
                            selectedPrefab,
                            false,
                            out DynamicBuffer<ServiceUpkeepData> serviceUpkeepBuffer
                        ) || serviceUpkeepBuffer.IsEmpty
                    )
                        return;
                    for (int i = 0; i < serviceUpkeepBuffer.Length; i++)
                    {
                        var entry = serviceUpkeepBuffer[i];
                        if (entry.m_Upkeep.m_Resource == Game.Economy.Resource.Money)
                        {
                            entry.m_Upkeep.m_Amount = UVTHelper.ConvertToInt(modifiedValue);
                            serviceUpkeepBuffer[i] = entry;
                            break;
                        }
                    }
                    return;

                case UpdateValueType.WorkplaceData_MaxWorkers:
                    EntityQuery workProviderQuery = SystemAPI
                        .QueryBuilder()
                        .WithAll<Game.Companies.WorkProvider>()
                        .Build();
                    NativeArray<Entity> workProviderEntities = workProviderQuery.ToEntityArray(
                        Allocator.Temp
                    );
                    foreach (Entity entity in workProviderEntities)
                    {
                        if (
                            !EntityManager.TryGetComponent(entity, out PrefabRef prefabRef)
                            || prefabRef.m_Prefab != selectedPrefab
                            || !EntityManager.TryGetComponent(
                                entity,
                                out Game.Companies.WorkProvider workProvider
                            )
                        )
                            continue;
                        workProvider.m_MaxWorkers = UVTHelper.ConvertToInt(modifiedValue);
                        utils.SetAndUpdate(entity, workProvider);
                    }

                    return;

                case UpdateValueType.ParkData_AllowHomeless:
                    bool allowHomeless = UVTHelper.ConvertToBool(modifiedValue);

                    if (!allowHomeless)
                    {
                        EntityQuery homelessShelterable = SystemAPI
                            .QueryBuilder()
                            .WithAll<Game.Buildings.Park, Game.Buildings.Renter>()
                            .Build();
                        NativeArray<Entity> homelessShelterableEntities =
                            homelessShelterable.ToEntityArray(Allocator.Temp);
                        foreach (Entity entity in homelessShelterableEntities)
                        {
                            if (
                                !EntityManager.TryGetComponent(entity, out PrefabRef prefabRef)
                                || prefabRef.m_Prefab != selectedPrefab
                            )
                                continue;

                            EntityManager.TryGetBuffer(
                                entity,
                                false,
                                out DynamicBuffer<Game.Buildings.Renter> renters
                            );
                            if (!renters.IsEmpty)
                            {
                                for (int i = renters.Length - 1; i >= 0; i--)
                                {
                                    MovingAway movingAway = new()
                                    {
                                        m_Reason = MoveAwayReason.NoSuitableProperty,
                                    };
                                    utils.SetAndUpdate(renters[i].m_Renter, movingAway);
                                    renters.RemoveAt(i);
                                }
                            }
                            //EntityManager.RemoveComponent<Game.Buildings.Renter>(entity);
                            EntityManager.RemoveComponent<Game.Buildings.PropertyOnMarket>(entity);
                            EntityManager.AddComponent<Updated>(entity);
                        }
                    }
                    return;
                default:
                    return;
            }
        }

        public void AddChangesToDict(Entity selectedPrefab, ModifiedPrefab mods)
        {
            LogHelper.SendLog(
                $"AddChangesToDict: {selectedPrefab}, {mods.ValueType}",
                LogLevel.DEVD
            );
            if (mods.ModEntity == Entity.Null)
                return;

            if (
                !localEntities.TryGetValue(
                    selectedPrefab,
                    out CreatedEntitiesData createdEntitiesData
                )
            )
            {
                createdEntitiesData = new() { CreatedEntity = mods.ModEntity };
            }
            createdEntitiesData.PrefabChanges ??= new();

            List<PrefabChange> list = createdEntitiesData.PrefabChanges;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ValueType == mods.ValueType)
                {
                    list[i].Modifications = mods;
                    createdEntitiesData.PrefabChanges = list;
                    localEntities[selectedPrefab] = createdEntitiesData;
                    return;
                }
            }

            list.Add(new PrefabChange { ValueType = mods.ValueType, Modifications = mods });
            createdEntitiesData.PrefabChanges = list;
            localEntities[selectedPrefab] = createdEntitiesData;
        }

        public void RemoveFromDict(Entity selectedPrefab, UpdateValueType updateValueType)
        {
            LogHelper.SendLog(
                $"RemoveFromDict: {selectedPrefab}, {updateValueType}",
                LogLevel.DEVD
            );

            if (
                !localEntities.TryGetValue(
                    selectedPrefab,
                    out CreatedEntitiesData createdEntitiesData
                )
            )
                return;

            if (createdEntitiesData?.PrefabChanges == null)
            {
                localEntities.Remove(selectedPrefab);
                return;
            }

            createdEntitiesData.PrefabChanges ??= new();
            List<PrefabChange> list = createdEntitiesData.PrefabChanges;
            var list2 = list;

            for (int i = list2.Count - 1; i >= 0; i--)
            {
                if (list2[i].ValueType == updateValueType)
                {
                    if (list2.Count == 1)
                    {
                        EntityManager.RemoveComponent<LocalModified>(selectedPrefab);
                        EntityManager.DestroyEntity(createdEntitiesData.CreatedEntity);
                        localEntities.Remove(selectedPrefab);

                        LogHelper.SendLog($"Removing data from localEntities", LogLevel.DEVD);
                        return;
                    }

                    list.RemoveAt(i);
                    LogHelper.SendLog(
                        $"Removing component data from localEntities.PrefabChanges",
                        LogLevel.DEVD
                    );
                    createdEntitiesData.PrefabChanges = list;
                    localEntities[selectedPrefab] = createdEntitiesData;
                }
            }
        }

        public void ResetAll(Entity selectedPrefab)
        {
            ResetAllFromBuffer(selectedPrefab);
            ResetAllFromDict(selectedPrefab);
        }

        public void ResetAllFromBuffer(Entity selectedPrefab)
        {
            if (
                !bufferControlSystem.TryGetAllEntityEntriesFromBuffer(
                    selectedPrefab,
                    out List<ModifiedPrefab> entries
                )
            )
                return;

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                Modify(selectedPrefab, $"(long){entry.Original}", entry.ValueType, true);
            }
            EntityManager.RemoveComponent<LocalModified>(selectedPrefab);
        }

        public void ResetAllFromDict(Entity selectedPrefab)
        {
            if (localEntities.ContainsKey(selectedPrefab))
                localEntities.Remove(selectedPrefab);
        }

        public void ResetAll()
        {
            var qb = SystemAPI.QueryBuilder().WithAll<LocalModified>().Build();
            var entities = qb.ToEntityArray(Allocator.Temp);

            foreach (var item in entities)
            {
                ResetAll(item);
            }
        }

        public void ResetFromDict()
        {
            var list = localEntities;
            localEntities.Clear();
            foreach (var item in list)
            {
                var entity = item.Key;
                var cd = item.Value;

                if (cd == null || cd.PrefabChanges == null || cd.PrefabChanges.Count <= 0)
                    continue;

                if (EntityManager.Exists(cd.CreatedEntity))
                    EntityManager.DestroyEntity(cd.CreatedEntity);

                foreach (var change in cd.PrefabChanges)
                {
                    Modify(
                        entity,
                        $"(long){change.Modifications.Original}",
                        change.ValueType,
                        true
                    );
                }
            }
        }
    }
}
