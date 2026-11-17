using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgEffectTypeInfoLocal : IJsonWritable
    {
        public bool Enabled = false;
        public bool Customized = false;
        public LocalModifierType Type;
        public ModifierValueMode Mode;
        public ModifierRadiusCombineMode Combine;
        public float Delta;
        public float Radius;

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(Enabled));
            writer.Write(Enabled);

            writer.PropertyName(nameof(Customized));
            writer.Write(Customized);

            writer.PropertyName(nameof(Type));
            writer.Write((int)Type);

            writer.PropertyName(nameof(Mode));
            writer.Write((int)Mode);

            writer.PropertyName(nameof(Combine));
            writer.Write((int)Combine);

            writer.PropertyName(nameof(Delta));
            writer.Write(Delta);

            writer.PropertyName(nameof(Radius));
            writer.Write(Radius);

            writer.TypeEnd();
        }
    }
}
