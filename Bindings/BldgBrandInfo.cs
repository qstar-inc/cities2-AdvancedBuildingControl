using Colossal.UI.Binding;

namespace AdvancedBuildingControl.Bindings
{
    public class BldgBrandInfo : IJsonWritable
    {
        public bool HasBrand { get; set; } = false;
        public string BrandName { get; set; } = string.Empty;
        public string BrandIcon { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public BrandDataInfo[] BrandList { get; set; } = new BrandDataInfo[0];

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName(nameof(HasBrand));
            writer.Write(HasBrand);

            writer.PropertyName(nameof(BrandName));
            writer.Write(BrandName);

            writer.PropertyName(nameof(BrandIcon));
            writer.Write(BrandIcon);

            writer.PropertyName(nameof(CompanyName));
            writer.Write(CompanyName);

            writer.PropertyName(nameof(BrandList));
            writer.Write(BrandList);

            writer.TypeEnd();
        }
    }
}
