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

    //public static class ZoneDataInfoJsonWriterExtensions
    //{
    //    public static void Write(this IJsonWriter writer, ZoneDataInfo value)
    //    {
    //        writer.TypeBegin(typeof(ZoneDataInfo).FullName);

    //        writer.PropertyName("Name");
    //        writer.Write(value.Name);

    //        writer.PropertyName("PrefabName");
    //        writer.Write(value.PrefabName);

    //        writer.PropertyName("Color1");
    //        writer.Write(value.Color1);

    //        writer.PropertyName("Color2");
    //        writer.Write(value.Color2);

    //        writer.PropertyName("Entity");
    //        writer.Write(value.Entity);

    //        writer.PropertyName("Icon");
    //        writer.Write(value.Icon);

    //        writer.PropertyName("AreaTypeString");
    //        writer.Write(value.AreaTypeString);

    //        writer.TypeEnd();
    //    }

    //    public static void Write(this IJsonWriter writer, ZoneDataInfo[] array)
    //    {
    //        writer.ArrayBegin(array.Length);
    //        foreach (var item in array)
    //            Write(writer, item);
    //        writer.ArrayEnd();
    //    }
    //}

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
        public static void Write(this IJsonWriter writer, BldgGeneralInfo value)
        {
            writer.TypeBegin(typeof(BldgGeneralInfo).FullName);

            writer.PropertyName("Efficiency");
            writer.Write(value.Efficiency);

            writer.PropertyName("HasHeli");
            writer.Write(value.HasHeli);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgBrandInfo value)
        {
            writer.TypeBegin(typeof(BldgBrandInfo).FullName);

            writer.PropertyName("HasBrand");
            writer.Write(value.HasBrand);

            writer.PropertyName("BrandName");
            writer.Write(value.BrandName);

            writer.PropertyName("BrandIcon");
            writer.Write(value.BrandIcon);

            writer.PropertyName("CompanyName");
            writer.Write(value.CompanyName);

            writer.PropertyName("BrandList");
            writer.Write(value.BrandList);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgZoningInfo value)
        {
            writer.TypeBegin(typeof(BldgZoningInfo).FullName);

            writer.PropertyName("HasLevel");
            writer.Write(value.HasLevel);

            writer.PropertyName("Level");
            writer.Write(value.Level);

            writer.PropertyName("Upkeep");
            writer.Write(value.Upkeep);

            writer.PropertyName("HasHousehold");
            writer.Write(value.HasHousehold);

            writer.PropertyName("Household");
            writer.Write(value.Household);

            writer.PropertyName("MaxHousehold");
            writer.Write(value.MaxHousehold);

            writer.PropertyName("Rent");
            writer.Write(value.Rent);

            writer.PropertyName("AreaType");
            writer.Write(value.AreaType);

            //writer.PropertyName("SpaceMultiplier");
            //writer.Write(value.SpaceMultiplier);

            //writer.PropertyName("ZoneTypeBase");
            //writer.Write(value.ZoneTypeBase);

            //writer.PropertyName("TotalRent");
            //writer.Write(value.TotalRent);

            //writer.PropertyName("PropertiesCount");
            //writer.Write(value.PropertiesCount);

            //writer.PropertyName("MixedPercent");
            //writer.Write(value.MixedPercent);

            //writer.PropertyName("LandValueBase");
            //writer.Write(value.LandValueBase);

            //writer.PropertyName("LandValueModifier");
            //writer.Write(value.LandValueModifier);

            //writer.PropertyName("IgnoreLandValue");
            //writer.Write(value.IgnoreLandValue);

            //writer.PropertyName("LotSize");
            //writer.Write(value.LotSize);

            //writer.PropertyName("IsMixed");
            //writer.Write(value.IsMixed);

            writer.PropertyName("HasWorkplace");
            writer.Write(value.HasWorkplace);

            writer.PropertyName("CurrentMaxWorkplaceCount");
            writer.Write(value.CurrentMaxWorkplaceCount);

            writer.PropertyName("OriginalMaxWorkplaceCount");
            writer.Write(value.OriginalMaxWorkplaceCount);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgStorageInfo value)
        {
            writer.TypeBegin(typeof(BldgStorageInfo).FullName);

            writer.PropertyName("HasStorage");
            writer.Write(value.HasStorage);

            writer.PropertyName("BuildingResources");
            writer.Write(value.BuildingResources);

            writer.PropertyName("BuildingResourcesAll");
            writer.Write(value.BuildingResourcesAll);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgUtilityInfo value)
        {
            writer.TypeBegin(typeof(BldgUtilityInfo).FullName);

            writer.PropertyName("IsWaterPump");
            writer.Write(value.IsWaterPump);

            writer.PropertyName("CurrentWaterPumpCap");
            writer.Write(value.CurrentWaterPumpCap);

            writer.PropertyName("OriginalWaterPumpCap");
            writer.Write(value.OriginalWaterPumpCap);

            writer.PropertyName("IsSewageOutlet");
            writer.Write(value.IsSewageOutlet);

            writer.PropertyName("CurrentSewageDumpCap");
            writer.Write(value.CurrentSewageDumpCap);

            writer.PropertyName("OriginalSewageDumpCap");
            writer.Write(value.OriginalSewageDumpCap);

            writer.PropertyName("CurrentSewagePurification");
            writer.Write(value.CurrentSewagePurification);

            writer.PropertyName("OriginalSewagePurification");
            writer.Write(value.OriginalSewagePurification);

            writer.PropertyName("IsPowerPlant");
            writer.Write(value.IsPowerPlant);

            writer.PropertyName("CurrentPowerProdCap");
            writer.Write(value.CurrentPowerProdCap);

            writer.PropertyName("OriginalPowerProdCap");
            writer.Write(value.OriginalPowerProdCap);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, VehicleInfo value)
        {
            writer.TypeBegin(typeof(VehicleInfo).FullName);

            writer.PropertyName("Current");
            writer.Write(value.Current);

            writer.PropertyName("Original");
            writer.Write(value.Original);

            writer.PropertyName("Combined");
            writer.Write(value.Combined);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BldgVehicleInfo value)
        {
            writer.TypeBegin(typeof(BldgVehicleInfo).FullName);

            writer.PropertyName("IsDepot");
            writer.Write(value.IsDepot);

            writer.PropertyName("TransportType");
            writer.Write(value.TransportType);

            writer.PropertyName("DepotVehicle");
            writer.Write(value.DepotVehicle);

            //writer.PropertyName("CurrentDepotCap");
            //writer.Write(value.CurrentDepotCap);

            //writer.PropertyName("OriginalDepotCap");
            //writer.Write(value.OriginalDepotCap);

            //writer.PropertyName("CombinedDepotCap");
            //writer.Write(value.CombinedDepotCap);

            writer.PropertyName("IsGarbageFacility");
            writer.Write(value.IsGarbageFacility);

            writer.PropertyName("GarbageTruck");
            writer.Write(value.GarbageTruck);

            //writer.PropertyName("CurrentGarbageTruckCap");
            //writer.Write(value.CurrentGarbageTruckCap);

            //writer.PropertyName("OriginalGarbageTruckCap");
            //writer.Write(value.OriginalGarbageTruckCap);

            //writer.PropertyName("CombinedGarbageTruckCap");
            //writer.Write(value.CombinedGarbageTruckCap);

            writer.PropertyName("IsHospital");
            writer.Write(value.IsHospital);

            writer.PropertyName("Ambulance");
            writer.Write(value.Ambulance);

            writer.PropertyName("MediHeli");
            writer.Write(value.MediHeli);

            //writer.PropertyName("CurrentAmbulanceCap");
            //writer.Write(value.CurrentAmbulanceCap);

            //writer.PropertyName("OriginalAmbulanceCap");
            //writer.Write(value.OriginalAmbulanceCap);

            //writer.PropertyName("CombinedAmbulanceCap");
            //writer.Write(value.CombinedAmbulanceCap);

            //writer.PropertyName("CurrentMediHeliCap");
            //writer.Write(value.CurrentMediHeliCap);

            //writer.PropertyName("OriginalMediHeliCap");
            //writer.Write(value.OriginalMediHeliCap);

            //writer.PropertyName("CombinedMediHeliCap");
            //writer.Write(value.CombinedMediHeliCap);

            writer.PropertyName(name: "IsDeathcare");
            writer.Write(value.IsDeathcare);

            writer.PropertyName(name: "Hearse");
            writer.Write(value.Hearse);

            writer.TypeEnd();
        }
    }
}
