using Colossal.UI.Binding;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Bindings
{
    public class BrandDataInfo : IJsonWritable
    {
        //public string Name { get; set; } = "UnknownName";
        public string PrefabName { get; set; } = "UnknownPrefab";
        public string Color1 { get; set; } = "RGBA(0,0,0)";
        public string Color2 { get; set; } = "RGBA(0,0,0)";
        public string Color3 { get; set; } = "RGBA(0,0,0)";
        public Entity Entity { get; set; } = Entity.Null;
        public string Icon { get; set; } = "";
        public string[] Companies { get; set; } = new string[0];

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            //writer.PropertyName(nameof(Name));
            //writer.Write(Name);

            writer.PropertyName(nameof(PrefabName));
            writer.Write(PrefabName);

            writer.PropertyName(nameof(Color1));
            writer.Write(Color1);

            writer.PropertyName(nameof(Color2));
            writer.Write(Color2);

            writer.PropertyName(nameof(Color3));
            writer.Write(Color3);

            writer.PropertyName(nameof(Entity));
            writer.Write(Entity);

            writer.PropertyName(nameof(Icon));
            writer.Write(Icon);

            writer.PropertyName(nameof(Companies));
            writer.Write(Companies);

            writer.TypeEnd();
        }
    }
}
