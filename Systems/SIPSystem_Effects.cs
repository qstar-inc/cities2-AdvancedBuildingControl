using System;
using AdvancedBuildingControl.Bindings;
using AdvancedBuildingControl.Models;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Prefabs;
using StarQ.Shared.Extensions.UI;
using UnityEngine;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIPSystem : ExtendedInfoSectionBase
    {
        public static BldgEffectInfo bldgEffectInfo = new();
        private EffectControlSystem effectControlSystem;
        private EffectOverlaySystem effectOverlaySystem;

        public void ShowEffectOverlay(EffectOptions options)
        {
            if (options.Enabled)
            {
                effectOverlaySystem.Show(
                    selectedEntity,
                    options.Local,
                    options.EffectType,
                    options.Radius
                );
            }
            else
                effectOverlaySystem.Hide();
        }

        public void SetEffectOverlayColor(Color color)
        {
            effectOverlaySystem.SetColor(color);
        }

        public void SetRadiusCommon(bool radiusCommon)
        {
            effectOverlaySystem.SetRadiusCommon(radiusCommon);
        }

        public void RevertAllEffects()
        {
            effectControlSystem.RevertEverything(selectedPrefab);
        }

        public void SetLocalEffect(BldgEffectTypeInfoLocal type)
        {
            LocalModifierData d = new(
                type.Type,
                type.Mode,
                type.Combine,
                new Bounds1(0, type.Delta),
                new Bounds1(0, type.Radius)
            );
            effectControlSystem.SetLocal(selectedPrefab, d);
        }

        public void SetLocalEnabled(LocalModifierType t, bool on)
        {
            if (!Enum.IsDefined(typeof(LocalModifierType), t))
                return;

            effectControlSystem.SetLocalEnabled(selectedPrefab, t, on);
        }

        public void AddLocalEffect(LocalModifierType t)
        {
            if (!Enum.IsDefined(typeof(LocalModifierType), t))
                return;
            effectControlSystem.AddLocal(selectedPrefab, t);
        }

        public void RevertLocalEffect(LocalModifierType t)
        {
            if (!Enum.IsDefined(typeof(LocalModifierType), t))
                return;
            effectControlSystem.RevertLocal(selectedPrefab, t);
        }

        public void RevertAllLocalEffects()
        {
            effectControlSystem.RevertAllLocal(selectedPrefab);
        }

        public void SetCityEffect(BldgEffectTypeInfoCity ui)
        {
            CityModifierData d = new(ui.Type, ui.Mode, new Bounds1(0, ui.Range));
            effectControlSystem.SetCity(selectedPrefab, d);
        }

        public void SetCityEnabled(int t, bool on)
        {
            if (!Enum.IsDefined(typeof(CityModifierType), t))
                return;

            effectControlSystem.SetCityEnabled(selectedPrefab, (CityModifierType)t, on);
        }

        public void AddCityEffect(int t)
        {
            if (!Enum.IsDefined(typeof(CityModifierType), t))
                return;
            effectControlSystem.AddCity(selectedPrefab, (CityModifierType)t);
        }

        public void RevertCityEffect(int t)
        {
            if (!Enum.IsDefined(typeof(CityModifierType), t))
                return;
            effectControlSystem.RevertCity(selectedPrefab, (CityModifierType)t);
        }

        public void RevertAllCityEffects()
        {
            effectControlSystem.RevertAllCity(selectedPrefab);
        }
    }
}
