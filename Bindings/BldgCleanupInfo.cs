using Colossal.UI.Binding;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgCleanupInfo : IJsonWritable
    {
        public bool Enabled = false;
        public BldgCleanupTypeInfo[] Array = new BldgCleanupTypeInfo[0];

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(Enabled));
            writer.Write(Enabled);

            writer.PropertyName(nameof(Array));
            writer.Write(Array);

            writer.TypeEnd();
        }
    }
}
