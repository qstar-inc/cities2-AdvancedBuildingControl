using Colossal.UI.Binding;
using Unity.Entities;

namespace AdvancedBuildingControl.Bindings
{
    public class EditorBuilding : IJsonWritable
    {
        public string PrefabName { get; set; } = "UnknownPrefab";
        public Entity Entity { get; set; } = Entity.Null;
        public string UIIcon { get; set; }
        public string Source { get; set; }
        public Entity[] AssetPacks { get; set; }
        public bool IsUpgrade { get; set; }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(PrefabName));
            writer.Write(PrefabName);

            writer.PropertyName(nameof(Entity));
            writer.Write(Entity);

            writer.PropertyName(nameof(UIIcon));
            writer.Write(UIIcon);

            writer.PropertyName(nameof(AssetPacks));

            int length = AssetPacks.Length;
            writer.ArrayBegin(length);
            for (int i = 0; i < length; i++)
            {
                writer.Write(AssetPacks[i]);
            }
            writer.ArrayEnd();

            writer.PropertyName(nameof(IsUpgrade));
            writer.Write(IsUpgrade);

            writer.TypeEnd();
        }
    }
}
