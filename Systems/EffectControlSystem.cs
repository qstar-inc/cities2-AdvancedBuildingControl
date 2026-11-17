using System.Collections.Generic;
using AdvancedBuildingControl.Bindings;
using AdvancedBuildingControl.Components.Effects;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class EffectControlSystem : GameSystemBase
    {
        private readonly Dictionary<Entity, Entity> effectEntities = new();
        private readonly HashSet<Entity> fakeSignaturePrefabs = new();
        private EffectOverlaySystem effectOverlaySystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            effectOverlaySystem = WorldHelper.GetSystem<EffectOverlaySystem>();
        }

        protected override void OnUpdate() { }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (!WorldHelper.IsGame)
                return;

            LoadAll();
            LogHelper.SendLog("OnLoad EffectControl done", LogLevel.DEVD);
        }

        public void LoadAll()
        {
            HashSet<Entity> affected = new();
            ResetTouchedLocal(affected);
            ResetTouchedCity(affected);
            ResetFakeSignatures();
            effectEntities.Clear();

            ApplySavedLocal(affected);
            ApplySavedCity(affected);

            foreach (Entity prefab in affected)
                if (EntityManager.Exists(prefab))
                    Refresh(prefab);
        }

        public bool TryGetCity(out Entity city) =>
            SystemAPI.TryGetSingletonEntity<Game.City.City>(out city);

        public Entity GetOrCreateEffectEntity(Entity prefab)
        {
            if (
                effectEntities.TryGetValue(prefab, out Entity effectEntity)
                && effectEntity != Entity.Null
                && EntityManager.Exists(effectEntity)
            )
                return effectEntity;

            effectEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<EffectEntities>(effectEntity);
            EntityManager.AddComponentData(effectEntity, new PrefabRef(prefab));
            effectEntities[prefab] = effectEntity;
            return effectEntity;
        }

        private bool HasAnySaved(Entity prefab) =>
            HasSavedLocalEffect(prefab) || HasSavedCityEffect(prefab);

        public void DeleteEffectEntityIfUnused(Entity prefab)
        {
            if (HasAnySaved(prefab))
                return;

            if (
                effectEntities.TryGetValue(prefab, out Entity effectEntity)
                && EntityManager.Exists(effectEntity)
            )
                EntityManager.DestroyEntity(effectEntity);
            effectEntities.Remove(prefab);
            RemoveFakeSignature(prefab);
        }

        public void Refresh(Entity prefab) => EntityManager.UpdateAllPlacedInstance(prefab);

        public void RevertEverything(Entity prefab)
        {
            RevertAllLocal(prefab, false);
            RevertAllCity(prefab, false);
            Refresh(prefab);
        }

        private bool Matches(Entity mod, Entity prefab) =>
            EntityManager.Exists(mod)
            && EntityManager.TryGetComponent(mod, out PrefabRef pr)
            && pr.m_Prefab == prefab;

        public void CheckEffects(Entity selectedPrefab, out BldgEffectInfo bldgEffectInfo)
        {
            bldgEffectInfo = new() { Color = effectOverlaySystem.GetColor() };

            List<LocalModifierCustomization> localEntries = LocalEntries(selectedPrefab);
            List<CityModifierCustomization> cityEntries = CityEntries(selectedPrefab);
            if (localEntries.Count == 0 && cityEntries.Count == 0)
                return;

            List<BldgEffectTypeInfoLocal> localEff = new(localEntries.Count);

            foreach (LocalModifierCustomization c in localEntries)
            {
                LocalModifierData d = c.Current;
                localEff.Add(
                    new BldgEffectTypeInfoLocal()
                    {
                        Enabled = c.IsEnabled,
                        Customized = IsLocalModified(c),
                        Type = d.m_Type,
                        Mode = d.m_Mode,
                        Combine = d.m_RadiusCombineMode,
                        Delta = d.m_Delta.max,
                        Radius = d.m_Radius.max,
                    }
                );
            }
            bldgEffectInfo.LocalModifiers = localEff.ToArray();

            List<BldgEffectTypeInfoCity> cityEff = new(cityEntries.Count);

            foreach (CityModifierCustomization c in cityEntries)
            {
                CityModifierData d = c.Current;
                cityEff.Add(
                    new BldgEffectTypeInfoCity()
                    {
                        Enabled = c.IsEnabled,
                        Customized = IsCityModified(c),
                        Type = d.m_Type,
                        Mode = d.m_Mode,
                        Range = d.m_Range.max,
                    }
                );
            }
            bldgEffectInfo.CityModifiers = cityEff.ToArray();
        }

        public void CheckPrefabSignature(Entity prefab)
        {
            if (EntityManager.HasComponent<FakeSignature>(prefab))
            {
                fakeSignaturePrefabs.Add(prefab);
                return;
            }

            if (
                EntityManager.HasComponent<SignatureBuildingData>(prefab)
                || EntityManager.HasComponent<ServiceObjectData>(prefab)
            )
                return;

            EntityManager.AddComponent<SignatureBuildingData>(prefab);
            EntityManager.AddComponent<FakeSignature>(prefab);
            fakeSignaturePrefabs.Add(prefab);
        }

        private void RemoveFakeSignature(Entity prefab)
        {
            fakeSignaturePrefabs.Remove(prefab);
            if (!EntityManager.Exists(prefab) || !EntityManager.HasComponent<FakeSignature>(prefab))
                return;

            EntityManager.RemoveComponent<FakeSignature>(prefab);
            EntityManager.RemoveComponent<SignatureBuildingData>(prefab);
        }

        private void ResetFakeSignatures()
        {
            foreach (Entity prefab in new List<Entity>(fakeSignaturePrefabs))
                RemoveFakeSignature(prefab);
            fakeSignaturePrefabs.Clear();
        }
    }
}
