using AdvancedBuildingControl.Enums;
using Colossal.UI.Binding;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgModifiedInfo : IJsonWritable
    {
        public string OriginalText { get; set; }
        public UpdateValueType ValueType { get; set; } = UpdateValueType._None;

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(OriginalText));
            writer.Write(OriginalText);

            writer.PropertyName(nameof(ValueType));
            writer.Write((long)ValueType);

            writer.TypeEnd();
        }
    }
}
