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

            writer.PropertyName("HasPostVan");
            writer.Write(value.HasPostVan);

            writer.PropertyName("HasPostTruck");
            writer.Write(value.HasPostTruck);

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

        public static void Write(this IJsonWriter writer, BldgPropertiesInfo value)
        {
            writer.TypeBegin(typeof(BldgPropertiesInfo).FullName);

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

            writer.PropertyName("IsWorkplace");
            writer.Write(value.IsWorkplace);

            writer.PropertyName("Workplace");
            writer.Write(value.Workplace);

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

        public static void Write(this IJsonWriter writer, InfoTypeCOCE value)
        {
            writer.TypeBegin(typeof(InfoTypeCOCE).FullName);

            writer.PropertyName("Current");
            writer.Write(value.Current);

            writer.PropertyName("Original");
            writer.Write(value.Original);

            writer.PropertyName("Combined");
            writer.Write(value.Combined);

            writer.PropertyName("Enabled");
            writer.Write(value.Enabled);

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

            writer.PropertyName("IsGarbageFacility");
            writer.Write(value.IsGarbageFacility);

            writer.PropertyName("GarbageTruck");
            writer.Write(value.GarbageTruck);

            writer.PropertyName("IsHospital");
            writer.Write(value.IsHospital);

            writer.PropertyName("Ambulance");
            writer.Write(value.Ambulance);

            writer.PropertyName("MediHeli");
            writer.Write(value.MediHeli);

            writer.PropertyName(name: "IsDeathcare");
            writer.Write(value.IsDeathcare);

            writer.PropertyName(name: "Hearse");
            writer.Write(value.Hearse);

            writer.PropertyName("IsPoliceStation");
            writer.Write(value.IsPoliceStation);

            writer.PropertyName("PatrolCar");
            writer.Write(value.PatrolCar);

            writer.PropertyName("PoliceHeli");
            writer.Write(value.PoliceHeli);

            writer.PropertyName("IsPrison");
            writer.Write(value.IsPrison);

            writer.PropertyName("PrisonVan");
            writer.Write(value.PrisonVan);

            writer.PropertyName("IsFireStation");
            writer.Write(value.IsFireStation);

            writer.PropertyName("FireTruck");
            writer.Write(value.FireTruck);

            writer.PropertyName("FireHeli");
            writer.Write(value.FireHeli);

            writer.PropertyName("IsEmergencyShelter");
            writer.Write(value.IsEmergencyShelter);

            writer.PropertyName("EvacBus");
            writer.Write(value.EvacBus);

            writer.PropertyName("IsPostFacility");
            writer.Write(value.IsPostFacility);

            writer.PropertyName("PostVan");
            writer.Write(value.PostVan);

            writer.PropertyName("PostTruck");
            writer.Write(value.PostTruck);

            writer.PropertyName("IsMaintenanceDepot");
            writer.Write(value.IsMaintenanceDepot);

            writer.PropertyName("MaintenanceVehicle");
            writer.Write(value.MaintenanceVehicle);

            writer.TypeEnd();
        }
    }
}
