using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Prefabs;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;
using static AdvancedBuildingControl.Systems.StorageChangerSystem;

namespace AdvancedBuildingControl.Systems
{
    public partial class PreSerializationSystem : GameSystemBase
    {
#nullable disable
        private StorageChangerSystem storageChangerSystem;

        public PrefabSystem prefabSystem;
#nullable enable
        public EntityQuery alteredStorageComps;

        protected override void OnCreate()
        {
            storageChangerSystem = World.GetOrCreateSystemManaged<StorageChangerSystem>();
            alteredStorageComps = SystemAPI.QueryBuilder().WithAll<AlteredStorage>().Build();
            prefabSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
            RequireForUpdate(alteredStorageComps);
        }

        protected override void OnUpdate()
        {
            var eq2 = alteredStorageComps.ToEntityArray(Allocator.Temp);
            foreach (var entity in eq2)
            {
                storageChangerSystem.ReplaceEntity(entity, ProcessMode.Saving);
            }
        }
    }
}
