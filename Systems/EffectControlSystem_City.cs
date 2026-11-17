using System.Collections.Generic;
using AdvancedBuildingControl.Components.Effects;
using Colossal.Entities;
using Colossal.Mathematics;
using Game;
using Game.City;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class EffectControlSystem : GameSystemBase
    {
        private readonly HashSet<Entity> touchedCity = new();

        private static int IndexOf(DynamicBuffer<CityModifierCustomization> b, CityModifierType t)
        {
            for (int i = 0; i < b.Length; i++)
                if (b[i].Current.m_Type == t)
                    return i;
            return -1;
        }

        private static int IndexOf(DynamicBuffer<CityModifierData> b, CityModifierType t)
        {
            for (int i = 0; i < b.Length; i++)
                if (b[i].m_Type == t)
                    return i;
            return -1;
        }

        private int IndexOf(
            DynamicBuffer<ModifiedCityEffect> b,
            CityModifierType type,
            Entity prefab
        )
        {
            for (int i = b.Length - 1; i >= 0; i--)
                if (b[i].CityModifier.m_Type == type && Matches(b[i].ModEntity, prefab))
                    return i;
            return -1;
        }

        private static bool SameCity(CityModifierData a, CityModifierData b) =>
            a.m_Type == b.m_Type
            && a.m_Mode == b.m_Mode
            && a.m_Range.min == b.m_Range.min
            && a.m_Range.max == b.m_Range.max;

        private void CheckCityBuffers(Entity prefab)
        {
            if (!EntityManager.HasBuffer<CityModifierData>(prefab))
                EntityManager.AddBuffer<CityModifierData>(prefab);

            CheckPrefabSignature(prefab);

            if (!EntityManager.HasBuffer<CityModifierCustomization>(prefab))
            {
                DynamicBuffer<CityModifierCustomization> cust =
                    EntityManager.AddBuffer<CityModifierCustomization>(prefab);
                DynamicBuffer<CityModifierData> live = EntityManager.GetBuffer<CityModifierData>(
                    prefab,
                    true
                );

                foreach (CityModifierData d in live)
                    cust.Add(
                        new CityModifierCustomization
                        {
                            IsEnabled = true,
                            HasOriginal = true,
                            Original = d,
                            Current = d,
                        }
                    );
            }
            touchedCity.Add(prefab);
        }

        private void ApplyCity(Entity prefab, CityModifierType type)
        {
            DynamicBuffer<CityModifierData> live = EntityManager.GetBuffer<CityModifierData>(
                prefab
            );
            DynamicBuffer<CityModifierCustomization> cust =
                EntityManager.GetBuffer<CityModifierCustomization>(prefab, true);

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

        private bool TryGetSavedCityEffect(out DynamicBuffer<ModifiedCityEffect> buf)
        {
            buf = default;
            if (!TryGetCity(out Entity city))
                return false;

            if (!EntityManager.TryGetBuffer(city, false, out buf))
                buf = EntityManager.AddBuffer<ModifiedCityEffect>(city);
            return true;
        }

        private bool HasSavedCityEffect(Entity prefab)
        {
            if (!TryGetSavedCityEffect(out DynamicBuffer<ModifiedCityEffect> buf))
                return false;
            for (int i = 0; i < buf.Length; i++)
                if (Matches(buf[i].ModEntity, prefab))
                    return true;
            return false;
        }

        private void PersistCity(Entity prefab, CityModifierType type)
        {
            CityModifierCustomization c = default;
            bool found = false;

            if (
                EntityManager.TryGetBuffer(
                    prefab,
                    true,
                    out DynamicBuffer<CityModifierCustomization> cust
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
                found && (!c.HasOriginal || !c.IsEnabled || !SameCity(c.Current, c.Original));

            if (!differs)
            {
                if (TryGetSavedCityEffect(out DynamicBuffer<ModifiedCityEffect> b))
                {
                    int idx = IndexOf(b, type, prefab);
                    if (idx >= 0)
                        b.RemoveAt(idx);
                }
                DeleteEffectEntityIfUnused(prefab);
                return;
            }

            Entity mod = GetOrCreateEffectEntity(prefab);
            if (!TryGetSavedCityEffect(out DynamicBuffer<ModifiedCityEffect> buf))
                return;

            ModifiedCityEffect entry = new()
            {
                ModEntity = mod,
                Enabled = (byte)(c.IsEnabled ? 1 : 0),
                CityModifier = c.Current,
            };

            int existing = IndexOf(buf, type, prefab);
            if (existing >= 0)
                buf[existing] = entry;
            else
                buf.Add(entry);
        }

        public void SetCity(Entity prefab, CityModifierData data)
        {
            CheckCityBuffers(prefab);
            DynamicBuffer<CityModifierCustomization> cust =
                EntityManager.GetBuffer<CityModifierCustomization>(prefab);

            int custIndex = IndexOf(cust, data.m_Type);
            if (custIndex >= 0)
            {
                CityModifierCustomization e = cust[custIndex];
                e.Current = data;
                cust[custIndex] = e;
            }
            else
                cust.Add(
                    new CityModifierCustomization
                    {
                        IsEnabled = true,
                        HasOriginal = false,
                        Current = data,
                    }
                );

            ApplyCity(prefab, data.m_Type);
            PersistCity(prefab, data.m_Type);
            Refresh(prefab);
        }

        public void AddCity(Entity prefab, CityModifierType type) =>
            SetCity(
                prefab,
                new CityModifierData(type, ModifierValueMode.Relative, new Bounds1(0f, 0.0f))
            );

        public void SetCityEnabled(Entity prefab, CityModifierType type, bool enabled)
        {
            CheckCityBuffers(prefab);
            DynamicBuffer<CityModifierCustomization> cust =
                EntityManager.GetBuffer<CityModifierCustomization>(prefab);

            int custIndex = IndexOf(cust, type);
            if (custIndex < 0)
            {
                if (enabled)
                    AddCity(prefab, type);
                return;
            }

            CityModifierCustomization e = cust[custIndex];
            e.IsEnabled = enabled;
            cust[custIndex] = e;
            ApplyCity(prefab, type);
            PersistCity(prefab, type);
            Refresh(prefab);
        }

        public void RevertCity(Entity prefab, CityModifierType type, bool refresh = true)
        {
            if (!EntityManager.HasBuffer<CityModifierCustomization>(prefab))
                return;

            DynamicBuffer<CityModifierCustomization> cust =
                EntityManager.GetBuffer<CityModifierCustomization>(prefab);
            int custIndex = IndexOf(cust, type);
            if (custIndex < 0)
                return;

            if (cust[custIndex].HasOriginal)
            {
                CityModifierCustomization e = cust[custIndex];
                e.Current = e.Original;
                e.IsEnabled = true;
                cust[custIndex] = e;
            }
            else
                cust.RemoveAt(custIndex);

            ApplyCity(prefab, type);
            PersistCity(prefab, type);
            if (refresh)
                Refresh(prefab);
        }

        public void RevertAllCity(Entity prefab, bool refresh = true)
        {
            if (!EntityManager.HasBuffer<CityModifierCustomization>(prefab))
                return;

            List<CityModifierType> types = new();

            foreach (
                CityModifierCustomization c in EntityManager.GetBuffer<CityModifierCustomization>(
                    prefab,
                    true
                )
            )
                types.Add(c.Current.m_Type);

            foreach (CityModifierType t in types)
                RevertCity(prefab, t, false);

            if (refresh)
                Refresh(prefab);
        }

        public List<CityModifierCustomization> CityEntries(Entity prefab)
        {
            List<CityModifierCustomization> list = new();
            if (
                EntityManager.TryGetBuffer(
                    prefab,
                    true,
                    out DynamicBuffer<CityModifierCustomization> cust
                )
            )
                foreach (CityModifierCustomization c in cust)
                    list.Add(c);
            else if (
                EntityManager.TryGetBuffer(prefab, true, out DynamicBuffer<CityModifierData> live)
            )
                foreach (CityModifierData d in live)
                    list.Add(
                        new CityModifierCustomization
                        {
                            IsEnabled = true,
                            HasOriginal = true,
                            Original = d,
                            Current = d,
                        }
                    );
            return list;
        }

        public bool IsCityModified(CityModifierCustomization c) =>
            !c.HasOriginal || !c.IsEnabled || !SameCity(c.Current, c.Original);

        private void ResetTouchedCity(HashSet<Entity> affected)
        {
            foreach (Entity prefab in touchedCity)
            {
                if (
                    !EntityManager.Exists(prefab)
                    || !EntityManager.HasBuffer<CityModifierCustomization>(prefab)
                )
                    continue;

                NativeList<CityModifierData> vanilla = new(Allocator.Temp);
                foreach (
                    CityModifierCustomization c in EntityManager.GetBuffer<CityModifierCustomization>(
                        prefab,
                        true
                    )
                )
                    if (c.HasOriginal)
                        vanilla.Add(c.Original);

                DynamicBuffer<CityModifierData> live = EntityManager.GetBuffer<CityModifierData>(
                    prefab
                );
                live.Clear();
                live.AddRange(vanilla.AsArray());
                EntityManager.RemoveComponent<CityModifierCustomization>(prefab);
                affected.Add(prefab);
                vanilla.Dispose();
            }
            touchedCity.Clear();
        }

        private void ApplySavedCity(HashSet<Entity> affected)
        {
            if (!TryGetSavedCityEffect(out DynamicBuffer<ModifiedCityEffect> buf))
                return;
            NativeArray<ModifiedCityEffect> saved = buf.ToNativeArray(Allocator.Temp);

            for (int i = 0; i < saved.Length; i++)
            {
                ModifiedCityEffect s = saved[i];
                if (
                    !EntityManager.Exists(s.ModEntity)
                    || !EntityManager.TryGetComponent(s.ModEntity, out PrefabRef pr)
                    || !EntityManager.Exists(pr.m_Prefab)
                )
                    continue;

                Entity prefab = pr.m_Prefab;
                effectEntities[prefab] = s.ModEntity;
                CheckCityBuffers(prefab);

                CityModifierType type = s.CityModifier.m_Type;
                DynamicBuffer<CityModifierCustomization> cust =
                    EntityManager.GetBuffer<CityModifierCustomization>(prefab);
                int ci = IndexOf(cust, type);
                if (ci >= 0)
                {
                    CityModifierCustomization e = cust[ci];
                    e.Current = s.CityModifier;
                    e.IsEnabled = s.IsEnabled;
                    cust[ci] = e;
                }
                else
                    cust.Add(
                        new CityModifierCustomization
                        {
                            IsEnabled = s.IsEnabled,
                            HasOriginal = false,
                            Current = s.CityModifier,
                        }
                    );

                ApplyCity(prefab, type);
                affected.Add(prefab);
            }
            saved.Dispose();
        }
    }
}
