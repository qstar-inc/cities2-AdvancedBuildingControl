using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Game;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class PreSerializationSystem : GameSystemBase
    {
#nullable disable
        //private StorageChangerSystem storageChangerSystem;
        //private LevelChangerSystem levelChangerSystem;
        //private HouseholdChangerSystem householdChangerSystem;
        private RefChangerSystem refChangerSystem;

        public PrefabSystem prefabSystem;
#nullable enable
        public EntityQuery alteredComps;

        //public EntityQuery alteredLevelComps;
        //public EntityQuery alteredHouseholdComps;

        protected override void OnCreate()
        {
            //storageChangerSystem = Mod.world.GetOrCreateSystemManaged<StorageChangerSystem>();
            //levelChangerSystem = Mod.world.GetOrCreateSystemManaged<LevelChangerSystem>();
            //householdChangerSystem = Mod.world.GetOrCreateSystemManaged<HouseholdChangerSystem>();
            refChangerSystem = Mod.world.GetOrCreateSystemManaged<RefChangerSystem>();

            alteredComps = SystemAPI
                .QueryBuilder()
                .WithAny<AltStorage, AltLevel, AltHousehold>()
                .Build();

            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
            RequireAnyForUpdate(alteredComps);
        }

        protected override void OnUpdate()
        {
            var entities = alteredComps.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                refChangerSystem.ReplaceEntity(entity, string.Empty, ProcessMode.Saving);
            }
            //var eq_level = alteredLevelComps.ToEntityArray(Allocator.Temp);
            //foreach (var entity in eq_level)
            //{
            //    levelChangerSystem.ReplaceEntity(entity, ProcessMode.Saving);
            //}
            //var eq_household = alteredHouseholdComps.ToEntityArray(Allocator.Temp);
            //foreach (var entity in eq_household)
            //{
            //    householdChangerSystem.ReplaceEntity(entity, ProcessMode.Saving);
            //}
        }
    }
}
