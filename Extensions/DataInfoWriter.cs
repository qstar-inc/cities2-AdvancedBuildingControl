using AdvancedBuildingControl.Systems;
using AdvancedBuildingControl.Variables;
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

    public static class BldgInfoJsonWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BldgZoningInfo value)
        {
            writer.TypeBegin(typeof(BldgZoningInfo).FullName);

            writer.PropertyName("HasLevel");
            writer.Write(value.HasLevel);

            //if (value.HasLevel)
            //{
            writer.PropertyName("Level");
            writer.Write(value.Level);

            writer.PropertyName("Upkeep");
            writer.Write(value.Upkeep);

            writer.PropertyName("HasHousehold");
            writer.Write(value.HasHousehold);

            //if (value.HasHousehold)
            //{
            writer.PropertyName("Household");
            writer.Write(value.Household);

            writer.PropertyName("MaxHousehold");
            writer.Write(value.MaxHousehold);

            writer.PropertyName("Rent");
            writer.Write(value.Rent);

            writer.PropertyName("AreaType");
            writer.Write(value.AreaType);

            writer.PropertyName("SpaceMultiplier");
            writer.Write(value.SpaceMultiplier);

            writer.PropertyName("ZoneTypeBase");
            writer.Write(value.ZoneTypeBase);

            writer.PropertyName("TotalRent");
            writer.Write(value.TotalRent);

            writer.PropertyName("PropertiesCount");
            writer.Write(value.PropertiesCount);

            writer.PropertyName("MixedPercent");
            writer.Write(value.MixedPercent);

            writer.PropertyName("LandValueBase");
            writer.Write(value.LandValueBase);

            writer.PropertyName("LandValueModifier");
            writer.Write(value.LandValueModifier);

            writer.PropertyName("IgnoreLandValue");
            writer.Write(value.IgnoreLandValue);

            writer.PropertyName("LotSize");
            writer.Write(value.LotSize);

            writer.PropertyName("IsMixed");
            writer.Write(value.IsMixed);

            //    }
            //}
            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgStorageInfo value)
        {
            writer.TypeBegin(typeof(BldgStorageInfo).FullName);

            writer.PropertyName("HasStorage");
            writer.Write(value.HasStorage);

            //if (value.HasStorage)
            //{
            writer.PropertyName("BuildingResources");
            writer.Write(value.BuildingResources);

            writer.PropertyName("BuildingResourcesAll");
            writer.Write(value.BuildingResourcesAll);
            //}
            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgBrandInfo value)
        {
            writer.TypeBegin(typeof(BldgBrandInfo).FullName);

            writer.PropertyName("HasBrand");
            writer.Write(value.HasBrand);

            //if (value.HasBrand)
            //{
            writer.PropertyName("BrandName");
            writer.Write(value.BrandName);

            writer.PropertyName("BrandIcon");
            writer.Write(value.BrandIcon);

            writer.PropertyName("CompanyName");
            writer.Write(value.CompanyName);
            //}
            writer.TypeEnd();
        }
    }
}
