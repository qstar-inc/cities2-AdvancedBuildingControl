using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Enums;
using Colossal.Entities;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class ModifiedPrefabBufferSystem : GameSystemBase
    {
        protected override void OnUpdate() { }

        private bool TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer)
        {
            buffer = default;
            if (!SystemAPI.TryGetSingletonEntity<Game.City.City>(out Entity city))
            {
                LogHelper.SendLog(
                    "Something went wrong. Failed to get singleton entity for Game.City.City"
                );
                return false;
            }

            if (!EntityManager.HasBuffer<ModifiedPrefab>(city))
                EntityManager.AddBuffer<ModifiedPrefab>(city);

            buffer = EntityManager.GetBuffer<ModifiedPrefab>(city);

            return true;
        }

        public bool TryGetBufferCopy(out NativeArray<ModifiedPrefab> bufferCopy)
        {
            bufferCopy = default;

            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            bufferCopy = buffer.ToNativeArray(Allocator.Temp);
            return true;
        }

        private bool Matches(ModifiedPrefab entry, Entity prefab) =>
            EntityManager.Exists(entry.ModEntity)
            && EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
            && prefabRef.m_Prefab == prefab;

        public bool TryGetEntryFromBuffer(
            Entity selectedPrefab,
            UpdateValueType valueType,
            out ModifiedPrefab mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab entry = buffer[i];

                if (entry.ValueType == valueType && Matches(entry, selectedPrefab))
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
            out List<ModifiedPrefab> mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab entry = buffer[i];

                if (Matches(entry, selectedPrefab))
                {
                    LogHelper.SendLog($"Found existing buffer entry", LogLevel.DEVD);
                    mods.Add(entry);
                }
            }

            return mods.Count > 0;
        }

        public bool TryRemoveEntriesFromBuffer(Entity selectedPrefab)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            int removed = 0;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab entry = buffer[i];

                if (Matches(entry, selectedPrefab))
                {
                    buffer.RemoveAt(i);
                    removed++;
                }
            }

            return removed > 0;
        }

        public bool TryAddOrReplaceToBuffer(Entity selectedPrefab, ModifiedPrefab mods)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab entry = buffer[i];

                if (entry.ValueType == mods.ValueType && Matches(entry, selectedPrefab))
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
            if (!TryGetBuffer(out DynamicBuffer<ModifiedPrefab> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedPrefab entry = buffer[i];

                if (entry.ValueType == valueType && Matches(entry, selectedPrefab))
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
