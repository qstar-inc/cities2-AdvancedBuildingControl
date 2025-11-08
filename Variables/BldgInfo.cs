using AdvancedBuildingControl.Systems;

namespace AdvancedBuildingControl.Variables
{
    public class BldgGeneralInfo
    {
        public int Efficiency { get; set; } = 0;
        public bool HasHeli { get; set; } = false;
    }

    public class BldgBrandInfo
    {
        public bool HasBrand { get; set; } = false;
        public string BrandName { get; set; } = string.Empty;
        public string BrandIcon { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public BrandDataInfo[] BrandList { get; set; } = new BrandDataInfo[0];
    }

    public class BldgZoningInfo
    {
        public bool HasLevel { get; set; } = false;
        public int Level { get; set; } = 0;
        public int Upkeep { get; set; } = 0;
        public bool HasHousehold { get; set; } = false;
        public int Household { get; set; } = 0;
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
        public bool HasWorkplace { get; set; } = false;
        public int CurrentMaxWorkplaceCount { get; set; } = 0;
        public int OriginalMaxWorkplaceCount { get; set; } = 0;
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

    public class VehicleInfo
    {
        public int Current { get; set; } = 0;
        public int Original { get; set; } = 0;
        public int Combined { get; set; } = 0;
    }

    public class BldgVehicleInfo
    {
        public bool IsDepot { get; set; } = false;
        public string TransportType { get; set; } = string.Empty;
        public VehicleInfo DepotVehicle { get; set; } = new VehicleInfo();

        //public int CurrentDepotCap { get; set; } = 0;
        //public int OriginalDepotCap { get; set; } = 0;
        //public int CombinedDepotCap { get; set; } = 0;
        public bool IsGarbageFacility { get; set; } = false;
        public VehicleInfo GarbageTruck { get; set; } = new VehicleInfo();

        //public int CurrentGarbageTruckCap { get; set; } = 0;
        //public int OriginalGarbageTruckCap { get; set; } = 0;
        //public int CombinedGarbageTruckCap { get; set; } = 0;
        public bool IsHospital { get; set; } = false;
        public VehicleInfo Ambulance { get; set; } = new VehicleInfo();
        public VehicleInfo MediHeli { get; set; } = new VehicleInfo();

        //public int CurrentAmbulanceCap { get; set; } = 0;
        //public int OriginalAmbulanceCap { get; set; } = 0;
        //public int CombinedAmbulanceCap { get; set; } = 0;
        //public int CurrentMediHeliCap { get; set; } = 0;
        //public int OriginalMediHeliCap { get; set; } = 0;
        //public int CombinedMediHeliCap { get; set; } = 0;
        public bool IsDeathcare { get; set; } = false;
        public VehicleInfo Hearse { get; set; } = new VehicleInfo();
        //public int CurrentHearseCap { get; set; } = 0;
        //public int OriginalHearseCap { get; set; } = 0;
        //public int CombinedHearseCap { get; set; } = 0;
    }
}
