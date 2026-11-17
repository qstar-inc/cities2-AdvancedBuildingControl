using Game.Prefabs;
using Unity.Entities;

namespace AdvancedBuildingControl.Components.Effects
{
    [InternalBufferCapacity(0)]
    public struct CityModifierCustomization : IBufferElementData
    {
        public bool IsEnabled { get; set; }
        public bool HasOriginal { get; set; }
        public CityModifierData Original { get; set; }
        public CityModifierData Current { get; set; }
    }
}
