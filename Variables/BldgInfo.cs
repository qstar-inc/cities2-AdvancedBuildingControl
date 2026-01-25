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

    public class BldgModifiedInfo
    {
        //public long Modified { get; set; }
        //public long Original { get; set; }
        public string OriginalText { get; set; }
        public UpdateValueType ValueType { get; set; } = UpdateValueType._None;
    }
}
