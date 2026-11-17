using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Enums;

namespace AdvancedBuildingControl.Models
{
    public class PrefabChange
    {
        public UpdateValueType ValueType { get; set; }
        public ModifiedPrefab Modifications { get; set; }
    }
}
