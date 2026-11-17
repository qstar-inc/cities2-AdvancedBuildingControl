using Colossal.UI.Binding;
using Unity.Entities;

namespace AdvancedBuildingControl.Bindings
{
    public class EditorBuildingData : IJsonWritable
    {
        public string PrefabName { get; set; } = "UnknownPrefab";
        public Entity Entity { get; set; } = Entity.Null;
        public BldgEffectInfo BldgEffectInfo { get; set; }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(PrefabName));
            writer.Write(PrefabName);

            writer.PropertyName(nameof(Entity));
            writer.Write(Entity);

            writer.PropertyName(nameof(BldgEffectInfo));
            writer.Write(BldgEffectInfo);

            writer.TypeEnd();
        }
    }
}
