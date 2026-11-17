using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgEffectTypeInfoCity : IJsonWritable
    {
        public bool Enabled = false;
        public bool Customized = false;
        public CityModifierType Type;
        public ModifierValueMode Mode;
        public float Range;

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

            writer.PropertyName(nameof(Range));
            writer.Write(Range);

            writer.TypeEnd();
        }
    }
}
