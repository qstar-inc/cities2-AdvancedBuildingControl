using System.Collections.Generic;
using AdvancedBuildingControl.Components.Effects;
using AdvancedBuildingControl.Enums;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class ModifiedLocalEffectBufferSystem : GameSystemBase
    {
        protected override void OnUpdate() { }

        private bool TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer)
        {
            buffer = default;
            if (!SystemAPI.TryGetSingletonEntity<Game.City.City>(out Entity city))
            {
                LogHelper.SendLog(
                    "Something went wrong. Failed to get singleton entity for Game.City.City"
                );
                return false;
            }

            if (!EntityManager.HasBuffer<ModifiedLocalEffect>(city))
                EntityManager.AddBuffer<ModifiedLocalEffect>(city);

            buffer = EntityManager.GetBuffer<ModifiedLocalEffect>(city);

            return true;
        }

        public bool TryGetBufferCopy(out NativeArray<ModifiedLocalEffect> bufferCopy)
        {
            bufferCopy = default;

            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            bufferCopy = buffer.ToNativeArray(Allocator.Temp);
            return true;
        }

        private bool Matches(ModifiedLocalEffect entry, Entity prefab) =>
            EntityManager.Exists(entry.ModEntity)
            && EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
            && prefabRef.m_Prefab == prefab;

        public bool TryGetEntryFromBuffer(
            Entity selectedPrefab,
            LocalModifierType modifierType,
            out ModifiedLocalEffect mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedLocalEffect entry = buffer[i];

                if (entry.LocalModifier.m_Type == modifierType && Matches(entry, selectedPrefab))
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
            out List<ModifiedLocalEffect> mods
        )
        {
            mods = new();
            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedLocalEffect entry = buffer[i];

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
            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            int removed = 0;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedLocalEffect entry = buffer[i];

                if (Matches(entry, selectedPrefab))
                {
                    buffer.RemoveAt(i);
                    removed++;
                }
            }

            return removed > 0;
        }

        public bool TryAddOrReplaceToBuffer(Entity selectedPrefab, ModifiedLocalEffect mods)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedLocalEffect entry = buffer[i];

                if (
                    entry.LocalModifier.m_Type == mods.LocalModifier.m_Type
                    && Matches(entry, selectedPrefab)
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

        public bool TryRemoveFromBuffer(Entity selectedPrefab, LocalModifierType modifierType)
        {
            if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buffer))
                return false;

            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                ModifiedLocalEffect entry = buffer[i];

                if (entry.LocalModifier.m_Type == modifierType && Matches(entry, selectedPrefab))
                {
                    LogHelper.SendLog($"Removing existing buffer entry", LogLevel.DEVD);
                    buffer.RemoveAt(i);
                    return true;
                }
            }

            LogHelper.SendLog($"No buffer entry to remove", LogLevel.DEVD);
            return false;
        }

        //public Entity GetOrCreateModEntity(Entity prefab)
        //{
        //    if (TryGetAllEntityEntriesFromBuffer(prefab, out List<ModifiedLocalEffect> list))
        //        return list[0].ModEntity;

        //    Entity e = EntityManager.CreateEntity();
        //    EntityManager.AddComponentData(e, new PrefabRef(prefab));
        //    return e;
        //}

        //public void RemoveEntryAndCleanup(Entity prefab, LocalModifierType type)
        //{
        //    if (!TryGetBuffer(out DynamicBuffer<ModifiedLocalEffect> buf))
        //        return;
        //    Entity mod = Entity.Null;
        //    for (int i = buf.Length - 1; i >= 0; i--)
        //        if (buf[i].LocalModifier.m_Type == type && Matches(buf[i], prefab))
        //        {
        //            mod = buf[i].ModEntity;
        //            buf.RemoveAt(i);
        //            break;
        //        }

        //    if (mod == Entity.Null)
        //        return;
        //    for (int i = 0; i < buf.Length; i++)
        //        if (buf[i].ModEntity == mod)
        //            return;
        //    EntityManager.DestroyEntity(mod);
        //}
    }
}
