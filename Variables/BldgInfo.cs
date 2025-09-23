using System.Collections.Generic;
using AdvancedBuildingControl.Systems;

namespace AdvancedBuildingControl.Variables
{
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
        public float SpaceMultiplier { get; set; } = 0;
        public float ZoneTypeBase { get; set; } = 0;
        public float TotalRent { get; set; } = 0;
        public float PropertiesCount { get; set; } = 0;
        public float MixedPercent { get; set; } = 0;
        public float LandValueBase { get; set; } = 0;
        public float LandValueModifier { get; set; } = 0;
        public bool IgnoreLandValue { get; set; } = false;
        public int LotSize { get; set; } = 0;
        public bool IsMixed { get; set; } = false;
    }

    public class BldgStorageInfo
    {
        public bool HasStorage { get; set; } = false;
        public ResourceDataInfo[] BuildingResources { get; set; } = new ResourceDataInfo[0];
        public ResourceDataInfo[] BuildingResourcesAll { get; set; } = new ResourceDataInfo[0];
    }
}
