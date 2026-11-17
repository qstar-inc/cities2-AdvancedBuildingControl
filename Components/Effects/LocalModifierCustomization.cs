using Game.Prefabs;
using Unity.Entities;

namespace AdvancedBuildingControl.Components.Effects
{
    [InternalBufferCapacity(0)]
    public struct LocalModifierCustomization : IBufferElementData
    {
        public bool IsEnabled { get; set; }
        public bool HasOriginal { get; set; }
        public LocalModifierData Original { get; set; }
        public LocalModifierData Current { get; set; }
    }
}
