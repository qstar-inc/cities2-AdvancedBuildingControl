using Colossal.UI.Binding;
using Unity.Entities;

namespace AdvancedBuildingControl.Bindings
{
    public class EditorBuildingAssetPack : IJsonWritable
    {
        public string PrefabName { get; set; }
        public Entity Entity { get; set; }
        public string UIIcon { get; set; }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(PrefabName));
            writer.Write(PrefabName);

            writer.PropertyName(nameof(Entity));
            writer.Write(Entity);

            writer.PropertyName(nameof(UIIcon));
            writer.Write(UIIcon);

            writer.TypeEnd();
        }
    }
}
