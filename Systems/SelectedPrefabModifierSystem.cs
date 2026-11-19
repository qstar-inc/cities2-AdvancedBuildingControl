using System;
using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Enums;
using AdvancedBuildingControl.Helpers;
using AdvancedBuildingControl.Models;
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
    public partial class SelectedPrefabModifierSystem : GameSystemBase
    {
        public ModifiedPrefabBufferSystem modifiedPrefabBufferSystem;

        public Dictionary<Entity, CreatedEntitiesData> localEntities;
        public HashSet<string> loggerLines = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            modifiedPrefabBufferSystem = WorldHelper.GetSystem<ModifiedPrefabBufferSystem>();
            localEntities = new();
        }

        protected override void OnUpdate() { }

        public void Modify(
            Entity selectedPrefab,
            string value,
            UpdateValueType valueType,
            bool isReset = false,
            bool useLogger = false
        )
        {
            try
            {
                string pName = PrefabHelper.GetPrefabName(selectedPrefab);
                LogHelper.SendLog(
                    $"Starting modify for {pName}: {valueType}, {value}",
                    LogLevel.DEVD
                );

                if (valueType == UpdateValueType._All)
                {
                    ResetAll(selectedPrefab);
                    return;
                }

                if (!UVTHelper.TryConvertValue(value, valueType, out long modifiedValue))
                {
                    LogHelper.SendLog(
                        $"Failed to convert `{value}` to long for {valueType} in {pName}"
                    );
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
                    modifiedPrefabBufferSystem.TryGetEntryFromBuffer(
                        selectedPrefab,
                        valueType,
                        out var existingEntry
                    )
                )
                {
                    entry = existingEntry;
                    if (modifiedValue == entry.Original)
                    {
                        modifiedPrefabBufferSystem.TryRemoveFromBuffer(selectedPrefab, valueType);
                        RemoveFromDict(selectedPrefab, valueType);
                        RemoveAndDestroy(selectedPrefab, entry.ModEntity);
                        return;
                    }
                    else
                    {
                        entry.Modified = modifiedValue;
                        entry.IsEnabled = true;
                        modifiedPrefabBufferSystem.TryAddOrReplaceToBuffer(selectedPrefab, entry);
                        AddChangesToDict(selectedPrefab, entry);
                    }
                }
                else
                {
                    if (isReset)
                    {
                        RemoveFromDict(selectedPrefab, valueType);
                        LogHelper.SendLog(
                            $"Resetting dict for {pName}: {valueType}, {value}",
                            LogLevel.DEVD
                        );
                        return;
                    }

                    if (!TryAddOrCreateEntity(selectedPrefab, out Entity newEntity))
                        return;

                    entry = default;
                    entry.ValueType = valueType;
                    entry.ModEntity = newEntity;
                    entry.Original = originalValue;
                    entry.Modified = modifiedValue;
                    entry.IsEnabled = true;
                    modifiedPrefabBufferSystem.TryAddOrReplaceToBuffer(selectedPrefab, entry);
                    AddChangesToDict(selectedPrefab, entry);
                }

                EntityManager.AddComponent<LocalModified>(selectedPrefab);

                string logLine =
                    $"{pName} ({valueType}) = {originalValue} => {modifiedValue} (default {entry.Original})";
                if (useLogger)
                    loggerLines.Add(logLine);
                else
                    LogHelper.SendLog(logLine);
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public bool TryAddOrCreateEntity(Entity selectedPrefab, out Entity newEntity)
        {
            if (
                localEntities.TryGetValue(selectedPrefab, out CreatedEntitiesData ceData)
                && ceData.CreatedEntity != Entity.Null
                && EntityManager.Exists(ceData.CreatedEntity)
            )
            {
                newEntity = ceData.CreatedEntity;
                LogHelper.SendLog($"Existing entity found", LogLevel.DEVD);
                if (!EntityManager.HasComponent<LocalEntities>(newEntity))
                    EntityManager.AddComponent<LocalEntities>(newEntity);
                if (
                    !EntityManager.TryGetComponent(newEntity, out PrefabRef prefabRef)
                    || prefabRef.m_Prefab != selectedPrefab
                )
                    EntityManager.SetOrAdd(newEntity, new PrefabRef(selectedPrefab));
                return true;
            }

            newEntity = EntityManager.CreateEntity();
            LogHelper.SendLog($"Creating new entity", LogLevel.DEVD);
            EntityManager.AddComponent<LocalEntities>(newEntity);
            EntityManager.SetOrAdd(newEntity, new PrefabRef(selectedPrefab));
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
                        EntityManager.SetAndUpdate(entity, workProvider);
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

                            if (
                                !EntityManager.TryGetBuffer(
                                    entity,
                                    false,
                                    out DynamicBuffer<Game.Buildings.Renter> renters
                                ) || renters.IsEmpty
                            )
                                continue;

                            NativeArray<Game.Buildings.Renter> toEvict = renters.ToNativeArray(
                                Allocator.Temp
                            );

                            foreach (Game.Buildings.Renter renter in toEvict)
                                EntityManager.SetAndUpdate(
                                    renter.m_Renter,
                                    new MovingAway { m_Reason = MoveAwayReason.NoSuitableProperty }
                                );

                            if (EntityManager.TryGetBuffer(entity, false, out renters))
                                renters.Clear();

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

            if (!localEntities.TryGetValue(selectedPrefab, out CreatedEntitiesData ceData))
            {
                ceData = new() { CreatedEntity = mods.ModEntity };
                localEntities[selectedPrefab] = ceData;
            }
            ceData.PrefabChanges ??= new();

            foreach (PrefabChange pChange in ceData.PrefabChanges)
            {
                if (pChange.ValueType == mods.ValueType)
                {
                    pChange.Modifications = mods;
                    return;
                }
            }

            ceData.PrefabChanges.Add(
                new PrefabChange { ValueType = mods.ValueType, Modifications = mods }
            );
        }

        public void RemoveAndDestroy(Entity selectedPrefab, Entity createdEntity)
        {
            EntityManager.RemoveComponent<LocalModified>(selectedPrefab);
            if (EntityManager.Exists(createdEntity))
                EntityManager.DestroyEntity(createdEntity);
            localEntities.Remove(selectedPrefab);
        }

        public void RemoveFromDict(Entity selectedPrefab, UpdateValueType updateValueType)
        {
            LogHelper.SendLog(
                $"RemoveFromDict: {selectedPrefab}, {updateValueType}",
                LogLevel.DEVD
            );

            if (!localEntities.TryGetValue(selectedPrefab, out CreatedEntitiesData ceData))
            {
                EntityManager.RemoveComponent<LocalModified>(selectedPrefab);
                return;
            }

            if (ceData?.PrefabChanges == null)
            {
                RemoveAndDestroy(selectedPrefab, ceData.CreatedEntity);
                LogHelper.SendLog(
                    $"Removing {updateValueType} data from localEntities because ceData or PrefabChanges is null"
                );
                return;
            }

            List<PrefabChange> list = ceData.PrefabChanges;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].ValueType != updateValueType)
                    continue;

                if (list.Count == 1)
                {
                    RemoveAndDestroy(selectedPrefab, ceData.CreatedEntity);
                    LogHelper.SendLog(
                        $"Removing {updateValueType} data from localEntities",
                        LogLevel.DEVD
                    );
                    return;
                }

                list.RemoveAt(i);
                LogHelper.SendLog(
                    $"Removing {updateValueType} component data from localEntities",
                    LogLevel.DEVD
                );
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
                !modifiedPrefabBufferSystem.TryGetAllEntityEntriesFromBuffer(
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
    }
}
