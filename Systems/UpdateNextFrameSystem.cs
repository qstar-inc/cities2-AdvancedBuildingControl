using AdvancedBuildingControl.Components;
using Game;
using Game.Common;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class UpdateNextFrameSystem : GameSystemBase
    {
        private EntityQuery updateNextFrameQuery;

        private ModificationBarrier1 barrier;

        public UpdateNextFrameSystem() { }

        protected override void OnCreate()
        {
            updateNextFrameQuery = SystemAPI
                .QueryBuilder()
                .WithAll<UpdateNextFrame>()
                .WithNone<Deleted, Updated>()
                .Build();
            barrier = World.GetOrCreateSystemManaged<ModificationBarrier1>();
            RequireAnyForUpdate(updateNextFrameQuery);
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            //LogHelper.SendLog("UpdateNextFrameSystem OnUpdate", LogLevel.DEV);
            EntityCommandBuffer buffer = barrier.CreateCommandBuffer();
            NativeArray<Entity> updateNextFrameEntities = updateNextFrameQuery.ToEntityArray(
                Allocator.Temp
            );
            buffer.AddComponent<Updated>(updateNextFrameEntities);
        }
    }
}
