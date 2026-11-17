using Colossal.UI.Binding;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgEffectInfo : IJsonWritable
    {
        public BldgEffectTypeInfoLocal[] LocalModifiers = new BldgEffectTypeInfoLocal[0];
        public BldgEffectTypeInfoCity[] CityModifiers = new BldgEffectTypeInfoCity[0];
        public Color Color = new(
            Mod.m_Setting.EffectOverlayR,
            Mod.m_Setting.EffectOverlayB,
            Mod.m_Setting.EffectOverlayG,
            Mod.m_Setting.EffectOverlayA
        );

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(LocalModifiers));
            writer.Write(LocalModifiers);

            writer.PropertyName(nameof(CityModifiers));
            writer.Write(CityModifiers);

            writer.PropertyName(nameof(Color));
            writer.Write(Color);

            writer.TypeEnd();
        }
    }
}
