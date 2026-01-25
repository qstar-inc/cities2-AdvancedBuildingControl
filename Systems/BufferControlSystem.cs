using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class BufferControlSystem : GameSystemBase
    {
        protected override void OnUpdate() { }

        private bool TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer)
        {
            buffer = new();
            if (!SystemAPI.TryGetSingletonEntity<Game.City.City>(out Entity city))
                return false;

            if (!EntityManager.HasBuffer<ModifiedPrefab_T7>(city))
                EntityManager.AddBuffer<ModifiedPrefab_T7>(city);

            buffer = EntityManager.GetBuffer<ModifiedPrefab_T7>(city);

            return true;
        }

        public bool TryGetBufferCopy(out NativeArray<ModifiedPrefab_T7> bufferCopy)
        {
            bufferCopy = new(0, Allocator.Temp);

            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            bufferCopy = buffer.ToNativeArray(Allocator.Temp);
            return true;
        }

        public bool TryGetEntryFromBuffer(
            Entity selectedPrefab,
            UpdateValueType valueType,
            out ModifiedPrefab_T7 mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab_T7 entry = buffer[i];

                if (
                    entry.ValueType == valueType
                    && EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                    && prefabRef.m_Prefab == selectedPrefab
                )
                {
                    LogHelper.SendLog($"Found existing buffer entry", LogLevel.DEVD);
                    mods = entry;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetAllEntityEntriesFromBuffer(
            Entity selectedPrefab,
            out List<ModifiedPrefab_T7> mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab_T7 entry = buffer[i];

                if (
                    EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                    && prefabRef.m_Prefab == selectedPrefab
                )
                {
                    LogHelper.SendLog($"Found existing buffer entry", LogLevel.DEVD);
                    mods.Add(entry);
                    return true;
                }
            }

            return false;
        }

        public bool TryRemoveEntriesFromBuffer(Entity selectedPrefab)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab_T7 entry = buffer[i];

                if (!EntityManager.Exists(entry.ModEntity))
                    continue;

                if (
                    EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                    && prefabRef.m_Prefab == selectedPrefab
                )
                    buffer.RemoveAt(i);
            }

            return true;
        }

        public bool TryAddOrReplaceToBuffer(Entity selectedPrefab, ModifiedPrefab_T7 mods)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab_T7 entry = buffer[i];

                if (!EntityManager.Exists(entry.ModEntity))
                    continue;

                if (
                    entry.ValueType == mods.ValueType
                    && EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                    && prefabRef.m_Prefab == selectedPrefab
                )
                {
                    LogHelper.SendLog($"Replacing existing buffer entry", LogLevel.DEVD);
                    buffer[i] = mods;
                    return true;
                }
            }

            LogHelper.SendLog($"Adding new buffer entry", LogLevel.DEVD);
            buffer.Add(mods);
            return true;
        }

        public bool TryRemoveFromBuffer(Entity selectedPrefab, UpdateValueType valueType)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab_T7> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab_T7 entry = buffer[i];

                if (!EntityManager.Exists(entry.ModEntity))
                    continue;

                if (
                    entry.ValueType == valueType
                    && EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                    && prefabRef.m_Prefab == selectedPrefab
                )
                {
                    LogHelper.SendLog($"Removing existing buffer entry", LogLevel.DEVD);
                    buffer.RemoveAt(i);
                    return true;
                }
            }

            LogHelper.SendLog($"No buffer entry to remove", LogLevel.DEVD);
            return false;
        }
    }
}
