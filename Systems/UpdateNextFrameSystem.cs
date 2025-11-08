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

        //private EntityQuery updateNextFrameQuery2;

#nullable disable
        private ModificationBarrier1 barrier;

        public UpdateNextFrameSystem() { }

#nullable enable
        protected override void OnCreate()
        {
            updateNextFrameQuery = SystemAPI
                .QueryBuilder()
                .WithAll<UpdateNextFrame>()
                .WithNone<Deleted, Updated>()
                .Build();
            //updateNextFrameQuery2 = SystemAPI
            //    .QueryBuilder()
            //    .WithAll<UpdateNextFrame>()
            //    .WithNone<Deleted, Updated, ClearUpdateNextFrame>()
            //    .Build();
            barrier = WorldHelper.ModificationBarrier1;
            RequireForUpdate(updateNextFrameQuery);
            //RequireAnyForUpdate(updateNextFrameQuery, updateNextFrameQuery2);
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer buffer = barrier.CreateCommandBuffer();
            NativeArray<Entity> updateNextFrameEntities = updateNextFrameQuery.ToEntityArray(
                Allocator.Temp
            );
            buffer.AddComponent<Updated>(updateNextFrameEntities);
            //NativeArray<Entity> updateNextFrame2Entities = updateNextFrameQuery2.ToEntityArray(
            //    Allocator.Temp
            //);
            //buffer.AddComponent<ClearUpdateNextFrame>(updateNextFrame2Entities);
            //buffer.AddComponent<Updated>(updateNextFrame2Entities);
            buffer.Dispose();
        }
    }
}
