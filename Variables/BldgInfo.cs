using System;
using AdvancedBuildingControl.Systems;

namespace AdvancedBuildingControl.Variables
{
    public class BldgGeneralInfo
    {
        public int Efficiency { get; set; } = 0;
        public bool HasHeli { get; set; } = false;
        public bool HasPostVan { get; set; } = false;
        public bool HasPostTruck { get; set; } = false;
    }

    public class BldgBrandInfo
    {
        public bool HasBrand { get; set; } = false;
        public string BrandName { get; set; } = string.Empty;
        public string BrandIcon { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public BrandDataInfo[] BrandList { get; set; } = new BrandDataInfo[0];
    }

    public class BldgPropertiesInfo
    {
        public bool HasLevel { get; set; } = false;
        public InfoTypeCOCE Level { get; set; } = new InfoTypeCOCE();
        public int Upkeep { get; set; } = 0;
        public bool HasHousehold { get; set; } = false;
        public InfoTypeCOCE Household { get; set; } = new InfoTypeCOCE();
        public int MaxHousehold { get; set; } = 0;
        public int Rent { get; set; } = 0;
        public string AreaType { get; set; } = string.Empty;

        //public float SpaceMultiplier { get; set; } = 0;
        //public float ZoneTypeBase { get; set; } = 0;
        //public float TotalRent { get; set; } = 0;
        //public float PropertiesCount { get; set; } = 0;
        //public float MixedPercent { get; set; } = 0;
        //public float LandValueBase { get; set; } = 0;
        //public float LandValueModifier { get; set; } = 0;
        //public bool IgnoreLandValue { get; set; } = false;
        //public int LotSize { get; set; } = 0;
        //public bool IsMixed { get; set; } = false;
        public bool IsWorkplace { get; set; } = false;
        public InfoTypeCOCE Workplace { get; set; } = new InfoTypeCOCE();
    }

    public class BldgStorageInfo
    {
        public bool HasStorage { get; set; } = false;
        public ResourceDataInfo[] BuildingResources { get; set; } = new ResourceDataInfo[0];
        public ResourceDataInfo[] BuildingResourcesAll { get; set; } = new ResourceDataInfo[0];
    }

    public class BldgUtilityInfo
    {
        public bool IsWaterPump { get; set; } = false;
        public int CurrentWaterPumpCap { get; set; } = 0;
        public int OriginalWaterPumpCap { get; set; } = 0;
        public bool IsSewageOutlet { get; set; } = false;
        public int CurrentSewageDumpCap { get; set; } = 0;
        public int OriginalSewageDumpCap { get; set; } = 0;
        public float CurrentSewagePurification { get; set; } = 0;
        public float OriginalSewagePurification { get; set; } = 0;
        public bool IsPowerPlant { get; set; } = false;
        public int CurrentPowerProdCap { get; set; } = 0;
        public int OriginalPowerProdCap { get; set; } = 0;
    }

    public class InfoTypeCOCE
    {
        public int Current { get; set; } = 0;
        public int Original { get; set; } = 0;
        public int Combined { get; set; } = 0;
        public bool Enabled { get; set; } = false;
    }

    public class BldgVehicleInfo
    {
        public bool IsDepot { get; set; } = false;
        public string TransportType { get; set; } = string.Empty;
        public InfoTypeCOCE DepotVehicle { get; set; } = new InfoTypeCOCE();
        public bool IsGarbageFacility { get; set; } = false;
        public InfoTypeCOCE GarbageTruck { get; set; } = new InfoTypeCOCE();
        public bool IsHospital { get; set; } = false;
        public InfoTypeCOCE Ambulance { get; set; } = new InfoTypeCOCE();
        public InfoTypeCOCE MediHeli { get; set; } = new InfoTypeCOCE();
        public bool IsDeathcare { get; set; } = false;
        public InfoTypeCOCE Hearse { get; set; } = new InfoTypeCOCE();
        public bool IsPoliceStation { get; set; } = false;
        public InfoTypeCOCE PatrolCar { get; set; } = new InfoTypeCOCE();
        public InfoTypeCOCE PoliceHeli { get; set; } = new InfoTypeCOCE();
        public bool IsPrison { get; set; } = false;
        public InfoTypeCOCE PrisonVan { get; set; } = new InfoTypeCOCE();
        public bool IsFireStation { get; set; } = false;
        public InfoTypeCOCE FireTruck { get; set; } = new InfoTypeCOCE();
        public InfoTypeCOCE FireHeli { get; set; } = new InfoTypeCOCE();
        public bool IsEmergencyShelter { get; set; } = false;
        public InfoTypeCOCE EvacBus { get; set; } = new InfoTypeCOCE();
        public bool IsPostFacility { get; set; } = false;
        public InfoTypeCOCE PostVan { get; set; } = new InfoTypeCOCE();
        public InfoTypeCOCE PostTruck { get; set; } = new InfoTypeCOCE();
        public bool IsMaintenanceDepot { get; set; } = false;
        public InfoTypeCOCE MaintenanceVehicle { get; set; } = new InfoTypeCOCE();
    }
}
