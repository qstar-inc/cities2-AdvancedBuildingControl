using AdvancedBuildingControl.Components;
using Game;
using Game.Common;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class UpdateNextFrameClearSystem : GameSystemBase
    {
        private EntityQuery ClearUpdateNextFrameQuery;
#nullable disable
        private ModificationEndBarrier barrier;

        public UpdateNextFrameClearSystem() { }

#nullable enable
        protected override void OnCreate()
        {
            barrier = WorldHelper.ModificationEndBarrier;
            ClearUpdateNextFrameQuery = SystemAPI
                .QueryBuilder()
                .WithAny<UpdateNextFrame>()
                .WithNone<Deleted>()
                .Build();

            RequireForUpdate(ClearUpdateNextFrameQuery);
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer buffer = barrier.CreateCommandBuffer();
            NativeArray<Entity> clearUpdateNextFrameEntities =
                ClearUpdateNextFrameQuery.ToEntityArray(Allocator.Temp);
            buffer.RemoveComponent<UpdateNextFrame>(clearUpdateNextFrameEntities);
            buffer.Dispose();
        }
    }
}
