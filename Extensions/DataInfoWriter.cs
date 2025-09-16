using AdvancedBuildingControl.Systems;
using Colossal.UI.Binding;
using Unity.Entities;

namespace AdvancedBuildingControl.Extensions
{
    public static class BrandDataInfoJsonWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BrandDataInfo value)
        {
            writer.TypeBegin(typeof(BrandDataInfo).FullName);

            writer.PropertyName("Name");
            writer.Write(value.Name);

            writer.PropertyName("PrefabName");
            writer.Write(value.PrefabName);

            writer.PropertyName("Color1");
            writer.Write(value.Color1);

            writer.PropertyName("Color2");
            writer.Write(value.Color2);

            writer.PropertyName("Color3");
            writer.Write(value.Color3);

            writer.PropertyName("Entity");
            writer.Write(value.Entity);

            writer.PropertyName("Icon");
            writer.Write(value.Icon);

            writer.PropertyName("Companies");
            writer.Write(value.Companies);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BrandDataInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }
    }

    public static class ZoneDataInfoJsonWriterExtensions
    {
        public static void Write(this IJsonWriter writer, ZoneDataInfo value)
        {
            writer.TypeBegin(typeof(ZoneDataInfo).FullName);

            writer.PropertyName("Name");
            writer.Write(value.Name);

            writer.PropertyName("PrefabName");
            writer.Write(value.PrefabName);

            writer.PropertyName("Color1");
            writer.Write(value.Color1);

            writer.PropertyName("Color2");
            writer.Write(value.Color2);

            writer.PropertyName("Entity");
            writer.Write(value.Entity);

            writer.PropertyName("Icon");
            writer.Write(value.Icon);

            writer.PropertyName("AreaTypeString");
            writer.Write(value.AreaTypeString);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, ZoneDataInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }
    }

    public static class ResourceDataInfoJsonWriterExtensions
    {
        public static void Write(this IJsonWriter writer, ResourceDataInfo value)
        {
            writer.TypeBegin(typeof(ResourceDataInfo).FullName);

            writer.PropertyName("Group");
            writer.Write((int)value.Group);

            writer.PropertyName("Id");
            writer.Write(value.Id);

            writer.PropertyName("Name");
            writer.Write(value.Name);

            //writer.PropertyName("PrefabName");
            //writer.Write(value.PrefabName);

            //writer.PropertyName("Color1");
            //writer.Write(value.Color1);

            //writer.PropertyName("Color2");
            //writer.Write(value.Color2);

            //writer.PropertyName("Entity");
            //writer.Write(value.Entity);

            //writer.PropertyName("Icon");
            //writer.Write(value.Icon);

            //writer.PropertyName("DisplayName");
            //writer.Write(value.DisplayName);

            //writer.PropertyName("AreaTypeString");
            //writer.Write(value.AreaTypeString);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, ResourceDataInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }
    }
}
