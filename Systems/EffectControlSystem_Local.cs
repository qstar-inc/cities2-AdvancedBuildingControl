using System.Collections.Generic;
using AdvancedBuildingControl.Components.Effects;
using Colossal.Entities;
using Colossal.Mathematics;
using Game;
using Game.Buildings;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class EffectControlSystem : GameSystemBase
    {
        private readonly HashSet<Entity> touchedLocal = new();

        private static int IndexOf(DynamicBuffer<LocalModifierCustomization> b, LocalModifierType t)
        {
            for (int i = 0; i < b.Length; i++)
                if (b[i].Current.m_Type == t)
                    return i;
            return -1;
        }

        private static int IndexOf(DynamicBuffer<LocalModifierData> b, LocalModifierType t)
        {
            for (int i = 0; i < b.Length; i++)
                if (b[i].m_Type == t)
                    return i;
            return -1;
        }

        private int IndexOf(
            DynamicBuffer<ModifiedLocalEffect> b,
            LocalModifierType type,
            Entity prefab
        )
        {
            for (int i = b.Length - 1; i >= 0; i--)
                if (b[i].LocalModifier.m_Type == type && Matches(b[i].ModEntity, prefab))
                    return i;
            return -1;
        }

        private static bool SameLocal(LocalModifierData a, LocalModifierData b) =>
            a.m_Type == b.m_Type
            && a.m_Mode == b.m_Mode
            && a.m_RadiusCombineMode == b.m_RadiusCombineMode
            && a.m_Delta.min == b.m_Delta.min
            && a.m_Delta.max == b.m_Delta.max
            && a.m_Radius.min == b.m_Radius.min
            && a.m_Radius.max == b.m_Radius.max;

        private void CheckLocalBuffers(Entity prefab)
        {
            if (!EntityManager.HasBuffer<LocalModifierData>(prefab))
                EntityManager.AddBuffer<LocalModifierData>(prefab);

            CheckPrefabSignature(prefab);

            if (!EntityManager.HasBuffer<LocalModifierCustomization>(prefab))
            {
                DynamicBuffer<LocalModifierCustomization> cust =
                    EntityManager.AddBuffer<LocalModifierCustomization>(prefab);
                DynamicBuffer<LocalModifierData> live = EntityManager.GetBuffer<LocalModifierData>(
                    prefab,
                    true
                );

                foreach (LocalModifierData d in live)
                    cust.Add(
                        new LocalModifierCustomization
                        {
                            IsEnabled = true,
                            HasOriginal = true,
                            Original = d,
                            Current = d,
                        }
                    );
            }
            touchedLocal.Add(prefab);
        }

        private void ApplyLocal(Entity prefab, LocalModifierType type)
        {
            DynamicBuffer<LocalModifierData> live = EntityManager.GetBuffer<LocalModifierData>(
                prefab
            );
            DynamicBuffer<LocalModifierCustomization> cust =
                EntityManager.GetBuffer<LocalModifierCustomization>(prefab, true);

            int liveIndex = IndexOf(live, type);
            int custIndex = IndexOf(cust, type);

            bool wanted = custIndex >= 0 && cust[custIndex].IsEnabled;
            if (wanted)
            {
                if (liveIndex >= 0)
                    live[liveIndex] = cust[custIndex].Current;
                else
                    live.Add(cust[custIndex].Current);
            }
            else if (liveIndex >= 0)
                live.RemoveAt(liveIndex);
        }

        private bool TryGetSavedLocalEffect(out DynamicBuffer<ModifiedLocalEffect> buf)
        {
            buf = default;
            if (!TryGetCity(out Entity city))
                return false;

            if (!EntityManager.TryGetBuffer(city, false, out buf))
                buf = EntityManager.AddBuffer<ModifiedLocalEffect>(city);
            return true;
        }

        private bool HasSavedLocalEffect(Entity prefab)
        {
            if (!TryGetSavedLocalEffect(out DynamicBuffer<ModifiedLocalEffect> buf))
                return false;
            for (int i = 0; i < buf.Length; i++)
                if (Matches(buf[i].ModEntity, prefab))
                    return true;
            return false;
        }

        private void PersistLocal(Entity prefab, LocalModifierType type)
        {
            LocalModifierCustomization c = default;
            bool found = false;

            if (
                EntityManager.TryGetBuffer(
                    prefab,
                    true,
                    out DynamicBuffer<LocalModifierCustomization> cust
                )
            )
            {
                int custIndex = IndexOf(cust, type);
                if (custIndex >= 0)
                {
                    c = cust[custIndex];
                    found = true;
                }
            }

            bool differs =
                found && (!c.HasOriginal || !c.IsEnabled || !SameLocal(c.Current, c.Original));

            if (!differs)
            {
                if (TryGetSavedLocalEffect(out DynamicBuffer<ModifiedLocalEffect> b))
                {
                    int idx = IndexOf(b, type, prefab);
                    if (idx >= 0)
                        b.RemoveAt(idx);
                }
                DeleteEffectEntityIfUnused(prefab);
                return;
            }

            Entity mod = GetOrCreateEffectEntity(prefab);
            if (!TryGetSavedLocalEffect(out DynamicBuffer<ModifiedLocalEffect> buf))
                return;

            ModifiedLocalEffect entry = new()
            {
                ModEntity = mod,
                Enabled = (byte)(c.IsEnabled ? 1 : 0),
                LocalModifier = c.Current,
            };

            int existing = IndexOf(buf, type, prefab);
            if (existing >= 0)
                buf[existing] = entry;
            else
                buf.Add(entry);
        }

        public void SetLocal(Entity prefab, LocalModifierData data)
        {
            CheckLocalBuffers(prefab);
            DynamicBuffer<LocalModifierCustomization> cust =
                EntityManager.GetBuffer<LocalModifierCustomization>(prefab);

            int custIndex = IndexOf(cust, data.m_Type);
            if (custIndex >= 0)
            {
                LocalModifierCustomization e = cust[custIndex];
                e.Current = data;
                cust[custIndex] = e;
            }
            else
                cust.Add(
                    new LocalModifierCustomization
                    {
                        IsEnabled = true,
                        HasOriginal = false,
                        Current = data,
                    }
                );

            ApplyLocal(prefab, data.m_Type);
            PersistLocal(prefab, data.m_Type);
            Refresh(prefab);
        }

        public void AddLocal(Entity prefab, LocalModifierType type) =>
            SetLocal(
                prefab,
                new LocalModifierData(
                    type,
                    ModifierValueMode.Relative,
                    ModifierRadiusCombineMode.Maximal,
                    new Bounds1(0f, 0.1f),
                    new Bounds1(0f, 500f)
                )
            );

        public void SetLocalEnabled(Entity prefab, LocalModifierType type, bool enabled)
        {
            CheckLocalBuffers(prefab);
            DynamicBuffer<LocalModifierCustomization> cust =
                EntityManager.GetBuffer<LocalModifierCustomization>(prefab);

            int custIndex = IndexOf(cust, type);
            if (custIndex < 0)
            {
                if (enabled)
                    AddLocal(prefab, type);
                return;
            }

            LocalModifierCustomization e = cust[custIndex];
            e.IsEnabled = enabled;
            cust[custIndex] = e;
            ApplyLocal(prefab, type);
            PersistLocal(prefab, type);
            Refresh(prefab);
        }

        public void RevertLocal(Entity prefab, LocalModifierType type, bool refresh = true)
        {
            if (!EntityManager.HasBuffer<LocalModifierCustomization>(prefab))
                return;

            DynamicBuffer<LocalModifierCustomization> cust =
                EntityManager.GetBuffer<LocalModifierCustomization>(prefab);
            int custIndex = IndexOf(cust, type);
            if (custIndex < 0)
                return;

            if (cust[custIndex].HasOriginal)
            {
                LocalModifierCustomization e = cust[custIndex];
                e.Current = e.Original;
                e.IsEnabled = true;
                cust[custIndex] = e;
            }
            else
                cust.RemoveAt(custIndex);

            ApplyLocal(prefab, type);
            PersistLocal(prefab, type);
            if (refresh)
                Refresh(prefab);
        }

        public void RevertAllLocal(Entity prefab, bool refresh = true)
        {
            if (!EntityManager.HasBuffer<LocalModifierCustomization>(prefab))
                return;

            List<LocalModifierType> types = new();

            foreach (
                LocalModifierCustomization c in EntityManager.GetBuffer<LocalModifierCustomization>(
                    prefab,
                    true
                )
            )
                types.Add(c.Current.m_Type);

            foreach (LocalModifierType t in types)
                RevertLocal(prefab, t, false);

            if (refresh)
                Refresh(prefab);
        }

        public List<LocalModifierCustomization> LocalEntries(Entity prefab)
        {
            List<LocalModifierCustomization> list = new();
            if (
                EntityManager.TryGetBuffer(
                    prefab,
                    true,
                    out DynamicBuffer<LocalModifierCustomization> cust
                )
            )
                foreach (LocalModifierCustomization c in cust)
                    list.Add(c);
            else if (
                EntityManager.TryGetBuffer(prefab, true, out DynamicBuffer<LocalModifierData> live)
            )
                foreach (LocalModifierData d in live)
                    list.Add(
                        new LocalModifierCustomization
                        {
                            IsEnabled = true,
                            HasOriginal = true,
                            Original = d,
                            Current = d,
                        }
                    );
            return list;
        }

        public bool IsLocalModified(LocalModifierCustomization c) =>
            !c.HasOriginal || !c.IsEnabled || !SameLocal(c.Current, c.Original);

        private void ResetTouchedLocal(HashSet<Entity> affected)
        {
            foreach (Entity prefab in touchedLocal)
            {
                if (
                    !EntityManager.Exists(prefab)
                    || !EntityManager.HasBuffer<LocalModifierCustomization>(prefab)
                )
                    continue;

                NativeList<LocalModifierData> vanilla = new(Allocator.Temp);
                foreach (
                    LocalModifierCustomization c in EntityManager.GetBuffer<LocalModifierCustomization>(
                        prefab,
                        true
                    )
                )
                    if (c.HasOriginal)
                        vanilla.Add(c.Original);

                DynamicBuffer<LocalModifierData> live = EntityManager.GetBuffer<LocalModifierData>(
                    prefab
                );
                live.Clear();
                live.AddRange(vanilla.AsArray());
                EntityManager.RemoveComponent<LocalModifierCustomization>(prefab);
                affected.Add(prefab);
                vanilla.Dispose();
            }
            touchedLocal.Clear();
        }

        private void ApplySavedLocal(HashSet<Entity> affected)
        {
            if (!TryGetSavedLocalEffect(out DynamicBuffer<ModifiedLocalEffect> buf))
                return;
            NativeArray<ModifiedLocalEffect> saved = buf.ToNativeArray(Allocator.Temp);

            for (int i = 0; i < saved.Length; i++)
            {
                ModifiedLocalEffect s = saved[i];
                if (
                    !EntityManager.Exists(s.ModEntity)
                    || !EntityManager.TryGetComponent(s.ModEntity, out PrefabRef pr)
                    || !EntityManager.Exists(pr.m_Prefab)
                )
                    continue;

                Entity prefab = pr.m_Prefab;
                effectEntities[prefab] = s.ModEntity;
                CheckLocalBuffers(prefab);

                LocalModifierType type = s.LocalModifier.m_Type;
                DynamicBuffer<LocalModifierCustomization> cust =
                    EntityManager.GetBuffer<LocalModifierCustomization>(prefab);
                int ci = IndexOf(cust, type);
                if (ci >= 0)
                {
                    LocalModifierCustomization e = cust[ci];
                    e.Current = s.LocalModifier;
                    e.IsEnabled = s.IsEnabled;
                    cust[ci] = e;
                }
                else
                    cust.Add(
                        new LocalModifierCustomization
                        {
                            IsEnabled = s.IsEnabled,
                            HasOriginal = false,
                            Current = s.LocalModifier,
                        }
                    );

                ApplyLocal(prefab, type);
                affected.Add(prefab);
            }
            saved.Dispose();
        }
    }
}
