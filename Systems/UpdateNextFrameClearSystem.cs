using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedBuildingControl.Components;
using Colossal.Logging;
using Game;
using Game.Common;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class UpdateNextFrameClearSystem : GameSystemBase
    {
        private EntityQuery ClearUpdateNextFrameQuery;
        private ModificationEndBarrier Barrier;

        public UpdateNextFrameClearSystem() { }

        protected override void OnCreate()
        {
            Barrier = World.GetOrCreateSystemManaged<ModificationEndBarrier>();
            ClearUpdateNextFrameQuery = SystemAPI
                .QueryBuilder()
                .WithAll<UpdateNextFrame, ClearUpdateNextFrame>()
                .WithNone<Deleted, Updated>()
                .Build();

            RequireForUpdate(ClearUpdateNextFrameQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            EntityCommandBuffer buffer = Barrier.CreateCommandBuffer();
            NativeArray<Entity> clearUpdateNextFrameEntities =
                ClearUpdateNextFrameQuery.ToEntityArray(Allocator.Temp);
            buffer.RemoveComponent<ClearUpdateNextFrame>(clearUpdateNextFrameEntities);
            buffer.RemoveComponent<UpdateNextFrame>(clearUpdateNextFrameEntities);
        }
    }
}
