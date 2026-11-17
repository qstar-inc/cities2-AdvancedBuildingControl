using AdvancedBuildingControl.Enums;
using Colossal.UI.Binding;
using StarQ.Shared.Extensions;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgCleanupTypeInfo : IJsonWritable
    {
        public bool Enabled = false;
        public BldgCleanupType CleanupType = BldgCleanupType._None;
        public float CurrentValueNumber = 0;

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(Enabled));
            writer.Write(Enabled);

            writer.PropertyName(nameof(CleanupType));
            writer.Write((long)CleanupType);

            writer.PropertyName(nameof(CurrentValueNumber));
            writer.Write(MathHelper.Float2Double(CurrentValueNumber));

            writer.TypeEnd();
        }
    }
}
