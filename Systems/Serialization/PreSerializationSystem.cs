using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Systems.Changers;
using AdvancedBuildingControl.Variables;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class PreSerializationSystem : GameSystemBase
    {
#nullable disable
        private RefChangerSystem refChangerSystem;
        public PrefabSystem prefabSystem;
#nullable enable
        public EntityQuery alteredComps;

        protected override void OnCreate()
        {
            refChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RefChangerSystem>();

            alteredComps = SystemAPI.QueryBuilder().WithAny<OriginalEntity>().Build();

            prefabSystem = WorldHelper.PrefabSystem;
            RequireForUpdate(alteredComps);
        }

        protected override void OnUpdate()
        {
            LogHelper.SendLog("Starting saving", LogLevel.DEV);

            var entities = alteredComps.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
                refChangerSystem.ReplaceEntity(entity, string.Empty, ProcessType.Saving);

            LogHelper.SendLog("Ending saving", LogLevel.DEV);
        }
    }
}
