using AdvancedBuildingControl.Systems;
using AdvancedBuildingControl.Variables;
using Colossal.UI.Binding;
using Unity.Entities;

namespace AdvancedBuildingControl.Extensions
{
    //public static class ResourceDataInfoJsonWriterExtensions
    //{
    //    public static void Write(this IJsonWriter writer, ResourceDataInfo value)
    //    {
    //        writer.TypeBegin(value.GetType().FullName);

    //        writer.PropertyName(nameof(value.Group));
    //        writer.Write((int)value.Group);

    //        writer.PropertyName(nameof(value.Id));
    //        writer.Write(value.Id);

    //        writer.PropertyName(nameof(value.Name));
    //        writer.Write(value.Name);

    //        writer.TypeEnd();
    //    }

    //    public static void Write(this IJsonWriter writer, ResourceDataInfo[] array)
    //    {
    //        if (array == null)
    //        {
    //            writer.ArrayBegin(0);
    //            writer.ArrayEnd();
    //            return;
    //        }

    //        writer.ArrayBegin(array.Length);
    //        foreach (var item in array)
    //            Write(writer, item);
    //        writer.ArrayEnd();
    //    }
    //}

    public static class BldgBrandInfoWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BrandDataInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            writer.PropertyName(nameof(value.Name));
            writer.Write(value.Name);

            writer.PropertyName(nameof(value.PrefabName));
            writer.Write(value.PrefabName);

            writer.PropertyName(nameof(value.Color1));
            writer.Write(value.Color1);

            writer.PropertyName(nameof(value.Color2));
            writer.Write(value.Color2);

            writer.PropertyName(nameof(value.Color3));
            writer.Write(value.Color3);

            writer.PropertyName(nameof(value.Entity));
            writer.Write(value.Entity);

            writer.PropertyName(nameof(value.Icon));
            writer.Write(value.Icon);

            writer.PropertyName(nameof(value.Companies));
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

        public static void Write(this IJsonWriter writer, BldgBrandInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            writer.PropertyName(nameof(value.HasBrand));
            writer.Write(value.HasBrand);

            writer.PropertyName(nameof(value.BrandName));
            writer.Write(value.BrandName);

            writer.PropertyName(nameof(value.BrandIcon));
            writer.Write(value.BrandIcon);

            writer.PropertyName(nameof(value.CompanyName));
            writer.Write(value.CompanyName);

            writer.PropertyName(nameof(value.BrandList));
            writer.Write(value.BrandList);

            writer.TypeEnd();
        }
    }

    public static class BldgCleanupInfoWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BldgCleanupInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            writer.PropertyName(nameof(value.Enabled));
            writer.Write(value.Enabled);

            writer.PropertyName(nameof(value.Array));
            writer.Write(value.Array);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgCleanupTypeInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }

        public static void Write(this IJsonWriter writer, BldgCleanupTypeInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            writer.PropertyName(nameof(value.Enabled));
            writer.Write(value.Enabled);

            writer.PropertyName(nameof(value.CurrentValueNumber));
            writer.Write(value.CurrentValueNumber);

            writer.PropertyName(nameof(value.CleanupType));
            writer.Write((long)value.CleanupType);

            writer.TypeEnd();
        }
    }

    public static class BldgModifiedInfoWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BldgModifiedInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            writer.PropertyName(nameof(value.OriginalText));
            writer.Write(value.OriginalText);

            writer.PropertyName(nameof(value.ValueType));
            writer.Write((long)value.ValueType);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgModifiedInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }
    }
}
